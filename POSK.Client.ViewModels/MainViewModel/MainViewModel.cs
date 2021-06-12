using CommonServiceLocator;
using Geeky.POSK.WPF.Core.Base;
using System;
using System.Diagnostics;
using POSK.Client.CashCode.Interface;
using POSK.Payment.Interface;
using System.Collections.Generic;
using Microsoft.Practices.Prism.Commands;
using NLog;
using Geeky.POSK.Models;
using POSK.Printers;
using POSK.Printers.Interface;
using POSK.Client.ApplicationService.Interface;
using Geeky.POSK.Repository.Interfface;
using Geeky.POSK.Infrastructore.Extensions;
using POSK.Client.CRT.Interface;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Threading;
using Geeky.POSK.DataContracts;

namespace POSK.Client.ViewModels
{
  public sealed partial class MainViewModel : BindableBaseViewModel
  {
    private static Logger logger = NLog.LogManager.GetCurrentClassLogger();
    private Action<ClientSession, string> LogSession = (session, s) => { logger.Trace($"Session: {session?.Id} -> {s}"); };
    private Action<string> Info = (string s) => { logger.Info(s); };
    private Action<string> Error = (string s) => { logger.Error(s); };
    private Action<string> Debug = (string s) => { logger.Debug(s); };
    private Action<string> Warning = (string s) => { logger.Warn(s); };

    public static Guid CurrentTerminalId { get; set; }
    /// <summary>
    /// Used for testing purpose to simulate cash coder
    /// </summary>
    public static bool Simulated { get; set; }

    /// <summary>
    /// Used to determine if we should ceile to upper 5 or not
    /// for example 52.5 -> 55
    /// if false it will be 53
    /// </summary>
    public static bool CustomCeilingMode { get; set; }

    private BindableBaseViewModel _currentVm;
    public BindableBaseViewModel CurrentVm
    {
      get { return _currentVm; }
      set { SetProperty(ref _currentVm, value); }
    }

    private string _currentLanguage;
    public string CurrentLanguage { get { return _currentLanguage; } set { SetProperty(ref _currentLanguage, value); } }

    private SelectLanguageViewModel _selectLanguageVm;
    private ThankYouViewModel _thankYouVm;
    private FaildTrxViewModel _faildTrxVm;
    private FaildTrxViewModel _noStockAvailable;
    private TimeoutWarningViewModel _timeoutWarningVm;
    private SessionExpiredViewModel _sessionExpiredVm;
    private CheckoutViewModel _checkoutVm;
    private TerminalDisabledViewModel _terminalDisabledVm;
    private OnePaymentMethodWarningViewModel _onePaymentMethodWarning;

    private ServiceListViewModel _serviceListVm;
    private ProductTypeViewModel _productTypeVm;
    private ProductListViewModel _productListVm;
    //private ShoppingCartViewModel _shoppingCartVm;
    private SelectPaymentMethodViewModel _selectPaymentMethodVm;
    private OrderPaymentViewModel _orderPaymentVm;
    //private FinishOrderViewModel _finishOrderVm;
    private IClientService _clientSvc;

    private Guid __CURRENT_SESSION_ID = Guid.Empty;
    private UserCart _cart;
    public UserCart Cart { get { return _cart; } set { SetProperty(ref _cart, value); } }

    private List<IPaymentMethod> _paymentMethods;
    public List<IPaymentMethod> PaymentMethods { get { return _paymentMethods; } set { SetProperty(ref _paymentMethods, value); } }

    private List<IReceiptPrinter> _printers;
    public List<IReceiptPrinter> Printers { get { return _printers; } set { SetProperty(ref _printers, value); } }
    private IReceiptPrinter SessionEndPrinter;

    //private bool _showTimeoutWarning;
    //public bool ShowTimeoutWarning { get { return _showTimeoutWarning; } set { SetProperty(ref _showTimeoutWarning, value); } }


    private ICashCodeBillValidator _cashCode;
    private ICardReader _cardReader;

    //private IPaymentMethod _selectedPm = null;

    private Stopwatch _watch;
    //private DispatcherTimer _navigationTimer;
    private System.Timers.Timer _navigationTimer;
    private string _logFilePath;


    /// <summary>
    /// event raise to main window to set current language of screen
    /// </summary>
    public event Action LanguageChanged = delegate { };

    /// <summary>
    /// event raised to notify sync timer to add extra info for ping
    /// </summary>
    public event Action<ExtraInfo> AddExtraInfoForPing = delegate { };

    /// <summary>
    /// Constructor
    /// </summary>
    public MainViewModel()
    {
      SetupNavigationTimer();
      SetupPaymentMethods();
      SetupReceiptPrinter();
      InitializeViewModels();

    }

    /// <summary>
    /// Initialize receipt printer
    /// </summary>
    private void SetupReceiptPrinter()
    {
      //todo
      Printers = new List<IReceiptPrinter>();
      //add printers here
      Printers.Add(ServiceLocator.Current.GetInstance<IReceiptPrinter>());

      SessionEndPrinter = ServiceLocator.Current.GetInstance<IReceiptPrinter>("session_end");
      //we should have a view model that reposent a printer and used in case there is not printer exist

    }

    /// <summary>
    /// Setup navigation timer (with default interval) responsible for navigating from view model to 
    /// another and actions
    /// </summary>
    private void SetupNavigationTimer()
    {
      //_navigationTimer = new DispatcherTimer();
      ////_navigationTimer.Elapsed += _navigationTimer_Elapsed;
      //_navigationTimer.Tick += _navigationTimer_Tick;
      //_navigationTimer.Interval = TimeSpan.FromMinutes(interval_default);

      _navigationTimer = new System.Timers.Timer();
      _navigationTimer.Interval = interval_default;
      _navigationTimer.Elapsed += _navigationTimer_Elapsed;
    }

    /// <summary>
    /// Setup cashcode, and wire its events
    /// </summary>
    private void SetupPaymentMethods()
    {
      _cashCode = ServiceLocator.Current.GetInstance<ICashCodeBillValidator>();
      _cashCode.AmountReceiving += _cashCode_AmountReceiving;
      _cashCode.AmountReceived += _cashCode_AmountReceived;
      _cashCode.BillCassetteStatusEvent += _cashCode_BillCassetteStatusEvent;
      _cashCode.BillFailure += _cashCode_BillFailure;
      _cashCode.EndOfSession += _cashCode_EndOfSession;
      _cashCode.RaiseToClient += _cashCode_RaiseToClient;
      _cashCode.LogToClient += _cashCode_LogToClient;

      //other payment methods goes here
      //....
      _cardReader = ServiceLocator.Current.GetInstance<ICardReader>();



      PaymentMethods = new List<IPaymentMethod>();

      //add any other payment methods
    }


    /// <summary>
    /// Initialize view models and events
    /// </summary>
    private void InitializeViewModels()
    {
      _selectLanguageVm = new SelectLanguageViewModel();
      _selectLanguageVm.LanguageSelected += _selectLanguageVm_LanguageSelected;

      _productListVm = new ProductListViewModel();
      _productListVm.ItemPurchased += _productListVm_ItemPurchased;
      _productListVm.GotoCheckout += _productListVm_GotoCheckout;
      _productListVm.BackToProductTypes += _productListVm_BackToProductTypes;
      _productListVm.ExitSessionNow += _productListVm_ExitSessionNow;

      _serviceListVm = new ServiceListViewModel();
      _serviceListVm.ServiceSelected += _serviceListVm_ServiceSelected;
      _serviceListVm.ExitSessionNow += _serviceListVm_ExitSessionNow;

      _productTypeVm = new ProductTypeViewModel();
      _productTypeVm.ProductTypeSelected += _productTypeVm_ProductTypeSelected;
      _productTypeVm.BackToServices += _productTypeVm_BackToServices;
      _productTypeVm.ExitSessionNow += _productTypeVm_ExitSessionNow;

      //_shoppingCartVm = new ShoppingCartViewModel();
      //_shoppingCartVm.GotoPayment += _shoppingCartVm_GotoOrderPayment;
      //_shoppingCartVm.ContinueShoppingCart += _shoppingCartVm_ContinueShopping;
      //_shoppingCartVm.RemoveItemInCart += _shoppingCartVm_RemoveItemInCart;

      _selectPaymentMethodVm = new SelectPaymentMethodViewModel();
      _selectPaymentMethodVm.PaymentMethodSelected += _selectPaymentMethodVm_PaymentMethodSelected;
      _selectPaymentMethodVm.BackToProducts += _selectPaymentMethodVm_BackToProducts;
      _selectPaymentMethodVm.ExitSessionNow += _selectPaymentMethodVm_ExitSessionNow;

      _orderPaymentVm = new OrderPaymentViewModel();
      //_orderPaymentVm.GoToShoppingCart += _orderPaymentVm_GoToShoppingCart;
      _orderPaymentVm.BackToSelectPaymentMethod += _orderPaymentVm_BackToSelectPaymentMethod;
      _orderPaymentVm.ExitSessionNow += _orderPaymentVm_ExitSessionNow;
      //_finishOrderVm = new FinishOrderViewModel();
      //_finishOrderVm.FinilizeOrder += _finishOrderVm_FinilizeOrder;
      //_finishOrderVm.GoToShoppingCart += _finishOrderVm_GoToShoppingCart;

      _timeoutWarningVm = new TimeoutWarningViewModel();
      _timeoutWarningVm.ExtendSession += _timeoutWarningVm_ExtendSession;
      _timeoutWarningVm.FinishNow += _timeoutWarningVm_FinishNow;

      _checkoutVm = new CheckoutViewModel();
      _checkoutVm.GotoPayment += _checkoutVm_GotoPayment;

      _onePaymentMethodWarning = new OnePaymentMethodWarningViewModel();
      _onePaymentMethodWarning.StartTheSession += _onePaymentMethodWarning_StartTheSession;

      _sessionExpiredVm = new SessionExpiredViewModel();
      _thankYouVm = new ThankYouViewModel();
      _faildTrxVm = new FaildTrxViewModel();
      _noStockAvailable = new FaildTrxViewModel();
      _terminalDisabledVm = new TerminalDisabledViewModel();

      _clientSvc = ServiceLocator.Current.GetInstance<IClientService>();

      //Navigate to welcome
      NavigateToWelcome();
    }

  



    /// <summary>
    /// User want to start new session, this is called either from language 
    /// selection screen, or if terminal has one payment method working, yet 
    /// the user approved to go on
    /// </summary>
    private void StartingSessionNow()
    {
      //load services
      _serviceListVm.LoadAvailableServices();
      //start timeout timer
      StartTimer(interval_services, $"Language Selected");
      //move to service select screen
      CurrentVm = _serviceListVm;

      //Initiate new session for the user
      StartSession();
    }

    /// <summary>
    /// This should be called whenever want to start again
    /// so it checks for devices to decide wether to show welcome
    /// screen or to disable the terminal
    /// </summary>
    private void NavigateToWelcome()
    {
      bool onePaymentMethod = false;
      bool printerWorking = false;
      //if terminal is not working, disable it and do nothing
      if (!CheckTerminalStatus(out onePaymentMethod, out printerWorking))
      {
        Info("Terminal is disabled");
        CurrentVm = _terminalDisabledVm;
        return;
      }

      CurrentVm = _selectLanguageVm;
    }

    /// <summary>
    /// Check if terminal should be disabled, this happened if all 
    /// payment methods are not working  or terminal has no pins available
    /// </summary>
    /// <returns>true if it is ok and working</returns>
    private bool CheckTerminalStatus(out bool onePaymentMethod, out bool printerWorking)
    {
      var _pinRepo = ServiceLocator.Current.GetInstance<IPinRepository>();
      var hasPins = !_pinRepo.IsSoldOut();
      bool cashCodeWorking = CheckCashCodeStatus();
      bool cardReaderWorking = CheckCardReaderStatus();

      PaymentMethods = new List<IPaymentMethod>();
      if (cashCodeWorking)
        PaymentMethods.Add(_cashCode);
      if (cardReaderWorking)
        PaymentMethods.Add(_cardReader);

      _selectPaymentMethodVm.SetPaymentMethods(PaymentMethods);

      _onePaymentMethodWarning.CashCode = cashCodeWorking;
      _onePaymentMethodWarning.CardReader = cardReaderWorking;

      printerWorking = CheckPrinterStatus();
      onePaymentMethod = (cashCodeWorking || cardReaderWorking) && !(cashCodeWorking && cardReaderWorking);
      return hasPins && //it must has something to sell
              (cashCodeWorking || cardReaderWorking) && //at least one payment method is working
              printerWorking && //printer is working
              (TerminalSettings == null || !TerminalSettings.DisableTerminal); //server didn't marked this terminal as disabled explicitly
    }

    /// <summary>
    /// checks if cash code is working
    /// </summary>
    /// <returns>true if it is working, false if else</returns>
    private bool CheckCashCodeStatus() { return _cashCode.IsWorking(); }

    /// <summary>
    /// checks if card reader is working
    /// </summary>
    /// <returns>true if it is working, false if else</returns>
    private bool CheckCardReaderStatus() { return _cardReader?.IsWorking() ?? false; }

    /// <summary>
    /// checks if receipt printer is working
    /// </summary>
    /// <returns>true if it is working, false if else</returns>
    private bool CheckPrinterStatus() { return _printers.Any(x => x.IsWorking()); }


    private void ExitSession()
    {
      _navigationTimer.Stop();

      LogSession(Cart.Session, $"Exit Session ");
      //user asked to exit session 
      EndSession();
    }


    /// <summary>
    /// Timeout timer responsible of navigating back between vms
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void _navigationTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
    //private void _navigationTimer_Tick(object sender, EventArgs e)
    {
      try
      {
        //don't process anything while cash code is working
        if (Cart.IsReceivingMoney) return;

        _navigationTimer.Stop();

        //for only session expired and thank you we don't check for current session 
        //as session in this moment is null as it has been disposed
        if (CurrentVm == _sessionExpiredVm)
        {
          LogSession(Cart.Session, $"Session Expired, Starting new one");
          //we already finished  the order, move to new start screen
          NavigateToWelcome();
          return;
        }
        else if (CurrentVm == _thankYouVm)
        {
          LogSession(Cart.Session, $"Thank you");
          //we already finished  the order, move to new start screen
          NavigateToWelcome();
          return;
        }

        if (Cart.Session.Id != __CURRENT_SESSION_ID) return;

        //LogSession(Cart.Session, $"[Timer] -> Elapsed {_navigationTimer.Interval}");

        bool onePaymentMethod = false;
        bool printerWorking = false;
        //if terminal is not working, disable it and do nothing
        if (!CheckTerminalStatus(out onePaymentMethod, out printerWorking))
        {
          Info("Terminal is disabled");
          CurrentVm = _terminalDisabledVm;
        }

        if (CurrentVm == _selectLanguageVm)
        {
          return;//we don't need to do anything
        }
        //if user didn't take action in timeout screen, we should force end of session
        //and print receipt with the amount
        else if (CurrentVm == _timeoutWarningVm)
        {
          LogSession(Cart.Session, $"Endsession, session expired, force ending now\n\t[Order Total: {CustomCeiling(Cart.TotalValue)}, Total Paid: {Cart.TotalPaid}]");
          EndSession(force: true);
          //Cart.ClearTimeoutWarning();
        }
        //if user walk to the end screen, end the session and start from there
        else if (CurrentVm == _orderPaymentVm ||
                CurrentVm == _selectPaymentMethodVm ||
                CurrentVm == _serviceListVm ||
                CurrentVm == _productListVm ||
                CurrentVm == _productTypeVm)
        {
          //we are on payment scren, and user didn't take action until timer is finished, we shall try ending session normally
          LogSession(Cart.Session, $"User didn't take action, end session starting");
          EndSession();
        }
      }
      catch (Exception ex)
      {
        Error($"Error in main nvaigation timer : {ex.Message}");
      }
    }

    private void StartTimer(TimeSpan interval, string whoStartMe)
    {
      StartTimer((int)interval.TotalMilliseconds, whoStartMe);
    }

    private void StartTimer(double interval, string whoStartMe)
    {
      Info($"[Timer] -> Started @ {interval / 1000} seconds by: {whoStartMe}");

      //_navigationTimer.IsEnabled = false;
      //_navigationTimer.Stop();
      //_navigationTimer = null;
      //_navigationTimer = new DispatcherTimer();
      //_navigationTimer.Interval = TimeSpan.FromMilliseconds(interval);
      //_navigationTimer.Tick += _navigationTimer_Tick;
      //_navigationTimer.Start();



      _navigationTimer.Stop();
      _navigationTimer = null;
      _navigationTimer = new System.Timers.Timer();
      _navigationTimer.Interval = interval;
      _navigationTimer.Elapsed += _navigationTimer_Elapsed;
      _navigationTimer.Start();
    }

   
  }
}
