using CommonServiceLocator;
using Geeky.POSK.Client.Proxy;
using Geeky.POSK.DataContracts;
using POSK.Client.ApplicationService;
using POSK.Client.ApplicationService.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace POSK.ClientApp
{
  /// <summary>
  /// Interaction logic for SetupTerminal.xaml
  /// </summary>
  public partial class SetupTerminal : Window
  {
    public RegistrationRespopnseDto Response { get; internal set; }
    SynchronizationContext _sync;

    public SetupTerminal()
    {
      InitializeComponent();
      _sync = SynchronizationContext.Current;
    }

    private void btnInitialize_Click(object sender, RoutedEventArgs e)
    {
      btnInitialize.IsEnabled = false;
      txtTerminalKey.IsEnabled = false;
      btnInitialize.Content = "Please wait ...";

      var terminalKey = txtTerminalKey.Text;

      Task<RegistrationRespopnseDto> t = Task.Run(() =>
      {
        try
        {
          var proxyToRegister = new ProductsClient();
          proxyToRegister.SetCredentials();

          var ip = GetLocalIPAddress();
          //MessageBox.Show(ip);

          var result = proxyToRegister.RegisterTerminal(terminalKey, ip, Environment.MachineName);
          //MessageBox.Show(result.Id.ToString());

          proxyToRegister.Close();
          if (!result.Approved)
            throw new Exception($"Terminal {Environment.MachineName} not registered/approved for key {terminalKey}");

          var terminalService = ServiceLocator.Current.GetInstance<ITerminalService>();
          terminalService.CreateTerminal(result, ip, Environment.MachineName);

          return result;
        }
        catch (Exception ex)
        {
          MessageBox.Show(ex.Message);
          Application.Current.Shutdown();
          return null;
        }
      });

      t.ContinueWith(r =>
      {
        _sync.Post(o =>
        {
          this.Response = (RegistrationRespopnseDto)o;
          this.DialogResult = true;
          this.Close();
        }, r.Result);
      });
    }

    public static string GetLocalIPAddress()
    {
      var host = Dns.GetHostEntry(Dns.GetHostName());
      foreach (var ip in host.AddressList)
      {
        if (ip.AddressFamily == AddressFamily.InterNetwork)
        {
          return ip.ToString();
        }
      }
      throw new Exception("No network adapters with an IPv4 address in the system!");
    }

  }
}
