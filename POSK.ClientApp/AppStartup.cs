using Autofac;
using Autofac.Extras.CommonServiceLocator;
using CommonServiceLocator;
using Geeky.POSK.Client.Proxy;
using Geeky.POSK.DataContracts;
using Geeky.POSK.DataContracts.MappingProfile;
using Geeky.POSK.Infrastructore.Core;
using Geeky.POSK.ServiceContracts;
using Geeky.POSK.Services;
using Madaa.Lib.Win.Services.Msdtc;
using Microsoft.Win32;
using NLog;
using POSK.Client.ApplicationService;
using POSK.Client.ApplicationService.Interface;
using POSK.Client.ViewModels.MappingProfile;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.ServiceModel;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Xml.Linq;

namespace POSK.ClientApp
{

  //todo convert this to startup task
  public class AppStartup
  {
    private static MsdtcManager msdtcManager;

    private static Logger logger = NLog.LogManager.GetCurrentClassLogger();
    private static Action<string> Info = (string s) => { logger.Info(s); };

    private static readonly string CONFIG_FILE_NAME = "terminal.xml";
    private static readonly string ID = "id";
    private static readonly string TERMINAL = "terminal";
    private static readonly string KEY = "key";
    private static readonly string SYNC_INTERVAL = "sync-interval";
    private static readonly string DISABLE_TERMINAL = "disbale-terminal";
    private static readonly string DISABLE_SHOPPINGCART = "disable-cart";

    private static bool _isSyncingNow = false;
    private static object _locker = new object();

    public static Guid TerminalId { get { return _terminalId; } }
    public static int SyncInterval { get { return _syncInterval; } }
    public static TerminalSettingDto TerminalSettings { get; private set; }

    private static Guid _terminalId = Guid.Empty;
    private static string _terminalKey = "";
    private static int _syncInterval = 0;//in minutes
    private static Timer _tmrSync;
    private static Timer _tmrPing;
    private static string terminalSettingFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, CONFIG_FILE_NAME);

    private static bool? _debugMode;
    public static bool DEBUG_MODE
    {
      get
      {
        if (!_debugMode.HasValue)
          _debugMode = ConfigurationManager.AppSettings.AllKeys.Contains("debug")
            ? Convert.ToBoolean(ConfigurationManager.AppSettings["debug"]) : false;
        return _debugMode.Value;
      }
    }
    private static bool? _simulatedMode;
    public static bool SIMULATED_MODE
    {
      get
      {
        if (!_simulatedMode.HasValue)
          _simulatedMode = ConfigurationManager.AppSettings.AllKeys.Contains("simulated")
            ? Convert.ToBoolean(ConfigurationManager.AppSettings["simulated"]) : false;
        return _simulatedMode.Value;
      }
    }
    private static bool? _custom_ceiling_mode;
    public static bool CUSTOM_CEILING_MODE//default is true
    {
      get
      {
        if (!_custom_ceiling_mode.HasValue)
          _custom_ceiling_mode = ConfigurationManager.AppSettings.AllKeys.Contains("custom_ceiling") ?
            Convert.ToBoolean(ConfigurationManager.AppSettings["custom_ceiling"]) : true;
        return _custom_ceiling_mode.Value;
      }
    }
    private static bool? _lockDown;
    public static bool LOCKDOWN
    {
      get
      {
        if (!_lockDown.HasValue)
          _lockDown = ConfigurationManager.AppSettings.AllKeys.Contains("lockdown") ? Convert.ToBoolean(ConfigurationManager.AppSettings["lockdown"]) : false;
        return _lockDown.Value;
      }
    }

    public static async Task ExecuteAsync()
    {
      SetupDependancies();

      SetupAutoMappingProfiles();

      RestartMSDTC();

      if (DEBUG_MODE)
      {
        var debugWindow = new Debug();
        Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
        Application.Current.MainWindow = debugWindow;
        debugWindow.Show();
        return;
      }

      if (ReadTerminalSettings())
      {
        //init data for first time
        await StartupSync();

        //setup ping timer
        SetupPingTimers();

        //setup sync timer
        SetupSyncTimers();

        //set main window and start the app
        var mainWindow = new MainWindow();
        if (LOCKDOWN)
        {
          mainWindow.Topmost = true;
        }

        Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
        Application.Current.MainWindow = mainWindow;
        mainWindow.Show();

        //Add the app to windows startup
        SetStartup();
      }
    }

    private static void RestartMSDTC()
    {
      try
      {
        msdtcManager = new MsdtcManager(false, 1000);
        if (msdtcManager.GetServiceStatus() == ServiceControllerStatus.Stopped)
          msdtcManager.RestartService();
      }
      catch (Exception ex) { }
    }

    private static void SetStartup()
    {
      try
      {
        if (SIMULATED_MODE) return;

        RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
        Assembly curAssembly = Assembly.GetExecutingAssembly();
        key.SetValue(curAssembly.GetName().Name, curAssembly.Location);
      }
      catch { }
    }

    private static void SetupDependancies()
    {
      ContainerBuilder builder = new ContainerBuilder();
      builder.RegisterModule<CoreDependancies>();
      builder.RegisterModule<ClientDependancies>();
      var container = builder.Build();

      //var slocator = new AutofacServiceLocator(container);
      //ServiceLocator.SetLocatorProvider(() => slocator);

      var csl = AutofacHybridServiceLocator.Initialize(container);
      ServiceLocator.SetLocatorProvider(() => csl);
    }
    private static void SetupAutoMappingProfiles()
    {
      AutoMapper.Mapper.Initialize((cfg) =>
      {
        cfg.AddProfile<ContractProfile>();
        cfg.AddProfile<ClientProfile>();
      });
    }
    private static bool ReadTerminalSettings()
    {
      XDocument doc = null;
      if (!File.Exists(terminalSettingFilePath))
      {
        XElement terminal = new XElement(TERMINAL,
                          new XElement(ID),
                          new XElement(KEY),
                          new XElement(SYNC_INTERVAL),
                          new XElement(DISABLE_TERMINAL),
                          new XElement(DISABLE_SHOPPINGCART)
                          );
        doc = new XDocument();
        doc.Add(terminal);
        doc.Save(terminalSettingFilePath);
      }
      else
      {
        doc = XDocument.Load(terminalSettingFilePath);
      }

      bool startApp = true;
      try
      {
        TerminalSettings = new TerminalSettingDto();
        bool disableTerminal = false, disableShoppingCart = false;

        var root = doc.Element(XName.Get(TERMINAL));
        var tmpterminalId = root.Element(XName.Get(ID));
        if (tmpterminalId != null && tmpterminalId.Value.Trim() != "" && !string.IsNullOrEmpty(tmpterminalId.Value))
        {
          Guid.TryParse(tmpterminalId.Value, out _terminalId);
        }

        _terminalKey = root.Element(XName.Get(KEY))?.Value;
        int.TryParse(root.Element(XName.Get(SYNC_INTERVAL))?.Value, out _syncInterval);
        bool.TryParse(root.Element(XName.Get(DISABLE_TERMINAL))?.Value, out disableTerminal);
        bool.TryParse(root.Element(XName.Get(DISABLE_SHOPPINGCART))?.Value, out disableShoppingCart);

        TerminalSettings.DisableTerminal = disableTerminal;
        TerminalSettings.DisableShoppingCart = disableShoppingCart;

        if (string.IsNullOrEmpty(_terminalKey))
        {
          var dlgSetupWindow = new SetupTerminal();
          var result = dlgSetupWindow.ShowDialog();
          if (result == null || result == false)
          {
            MessageBox.Show("FAILED TO REGISTER TERMINAL !!", "FAILED", MessageBoxButton.OK, MessageBoxImage.Error);
            startApp = false;
            Application.Current.Shutdown(-1);
          }
          else if (result == true)
          {
            var terminalResponse = dlgSetupWindow.Response;
            var keyElement = root.Element(KEY);
            if (keyElement == null)
            {
              keyElement = new XElement(KEY);
              root.Add(keyElement);
            }
            keyElement.Value = terminalResponse.TerminalKey;

            var idElement = root.Element(ID);
            if (idElement == null)
            {
              idElement = new XElement(ID);
              root.Add(idElement);
            }
            idElement.Value = terminalResponse.TerminalId.ToString();

            var syncIntervalElement = root.Element(SYNC_INTERVAL);
            if (syncIntervalElement == null)
            {
              syncIntervalElement = new XElement(SYNC_INTERVAL);
              root.Add(syncIntervalElement);
            }
            var shoppingCartDisabled = root.Element(DISABLE_SHOPPINGCART);
            if (shoppingCartDisabled == null)
            {
              shoppingCartDisabled = new XElement(DISABLE_SHOPPINGCART);
              root.Add(shoppingCartDisabled);
            }
            var terminalDisabled = root.Element(DISABLE_TERMINAL);
            if (terminalDisabled == null)
            {
              terminalDisabled = new XElement(DISABLE_TERMINAL);
              root.Add(terminalDisabled);
            }

            var tmpInterval = 0;
            int.TryParse(syncIntervalElement.Value, out tmpInterval);
            tmpInterval = tmpInterval <= 0 ? 3 : tmpInterval;
            _syncInterval = tmpInterval;
            syncIntervalElement.Value = _syncInterval.ToString();

            TerminalSettings.DisableShoppingCart = terminalResponse.Settings.DisableShoppingCart;
            shoppingCartDisabled.Value = TerminalSettings.DisableShoppingCart.ToString();

            TerminalSettings.DisableTerminal = terminalResponse.Settings.DisableTerminal;
            terminalDisabled.Value = TerminalSettings.DisableTerminal.ToString();

            //set local terminalid
            _terminalId = terminalResponse.TerminalId;
            doc.Save(terminalSettingFilePath);
          }
        }
      }
      catch (Exception ex)
      {
        //Faild to register terminal
        startApp = false;
      }

      return startApp;
    }

    private async static Task StartupSync()
    {
      ClearUnEndedSessions();
      await Task.Run(async () => await SyncData());
    }

    private static void SetupSyncTimers()
    {
      _tmrSync = new Timer();
      _tmrSync.Interval = SyncInterval * 60 * 1000;
      _tmrSync.Elapsed += _tmrSync_ElapsedAsync;
      _tmrSync.Start();
    }
    private static void SetupPingTimers()
    {
      _tmrPing = new Timer();
      _tmrPing.Interval = 1 * 60 * 1000;
      _tmrPing.Elapsed += _tmrPing_Elapsed;
      _tmrPing.Start();
    }
    private static void ClearUnEndedSessions()
    {
      var _clientSvc = ServiceLocator.Current.GetInstance<IClientService>();
      _clientSvc.ReleaseUnEndedSessionItems();
    }

    private static async Task SyncData()
    {
      try
      {
        Info("[Sync] -> Starting Sync now");

        var _clientSvc = ServiceLocator.Current.GetInstance<IClientService>();
        var _terminalSvc = ServiceLocator.Current.GetInstance<ITerminalService>();
        _isSyncingNow = true;

        //await _clientSvc.SyncMySales(TerminalId);
        //await _clientSvc.ApproveAnyHoldTransfers(TerminalId);
        //await _clientSvc.SyncMyLogs(TerminalId);

        //Stop ping (don't call server while we are syncing already with it)
        if (_tmrPing != null) _tmrPing.Stop();

        await _clientSvc.SyncWithServer(TerminalId);

        //already done in the sync call
        //_terminalSvc.CheckNewPins(TerminalId);
      }
      catch (Exception ex)
      {
        Console.WriteLine("Sync process failed\r\n" + ex.Message);
        Info("[Sync] -> Faild to sync :" + ex.Message + "\n\r" + ex.StackTrace);
      }
      finally
      {
        //Start ping
        if (_tmrPing != null) _tmrPing.Start();
        _isSyncingNow = false;
        Info("[Sync] -> Sync complete");
      }
    }
    private static void _tmrPing_Elapsed(object sender, ElapsedEventArgs e)
    {
      try
      {
        //don't ping if we are already syncing with server
        if (_isSyncingNow) return;

        //stop timer
        _tmrPing.Stop();
        var proxy = new SystemClient();
        proxy.SetCredentials();
        ExtraInfo info = PingExtraInfo;
        proxy.Ping(_terminalId, info);
        proxy.Close();
        //start timer again
        _tmrPing.Start();
      }
      catch (Exception exTimer)
      {
        Console.WriteLine("Error in ping timer : \n\r" + exTimer.Message);
      }
    }

    private static ExtraInfo _info = new ExtraInfo();
    private static ExtraInfo PingExtraInfo { get { return _info; } }
    public static void SetExtraInfo(ExtraInfo info)
    {
      lock (_locker)
      {
        _info = info;
      }
    }


    private static async void _tmrSync_ElapsedAsync(object sender, ElapsedEventArgs e)
    {
      try
      {
        //stop timer
        _tmrSync.Stop();
        //wait for sync task to complete
        await SyncData();
        //start timer again
        _tmrSync.Start();
      }
      catch (Exception exTimer)
      {
        Console.WriteLine("Error in sync timer : \n\r" + exTimer.Message);
      }
    }
  }
}

