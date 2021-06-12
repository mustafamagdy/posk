using CommonServiceLocator;
using Geeky.POSK.Client.Proxy;
using Geeky.POSK.DataContracts;
using Geeky.POSK.Infrastructore.Extensions;
using Geeky.POSK.Repository;
using POSK.Client.ApplicationService;
using POSK.Client.ApplicationService.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
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


  public partial class Debug : Window
  {
    Guid TerminalId = Guid.Empty;

    private readonly SynchronizationContext syncContext;
    public Debug()
    {
      InitializeComponent();

      syncContext = SynchronizationContext.Current;
    }

    private void RegisterTerminal_Click(object sender, RoutedEventArgs e)
    {
      ParseTerminalId();

      var proxy = new ProductsClient();
      proxy.SetCredentials();

      lstActions.Items.Clear();
      lstActions.Items.Add("Calling RegisterTerminal on server ");
      var result = proxy.RegisterTerminal("", "1.1.1.1", Environment.MachineName);
      lstActions.Items.Add("RegisterTerminal Finished");
      proxy.Close();
      lstActions.Items.Add("Proxy closed");

    }

    private void ParseTerminalId()
    {
      try
      {
        if (txtTerminalId.Text.Trim() != "")
          TerminalId = Guid.Parse(txtTerminalId.Text);
        else
        {
          TerminalRepository _terminalRepo = ServiceLocator.Current.GetInstance<TerminalRepository>();
          var terminal = _terminalRepo.FindAll().First();
          TerminalId = terminal.Id;
          txtTerminalId.Text = TerminalId.ToString();
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show("Error parsing terminal Id");
      }
    }

    private void PingServer_Click(object sender, RoutedEventArgs e)
    {
      ParseTerminalId();
      1.Loop(() =>
      {
        Thread tPing = new Thread(() =>
        {

          syncContext.Post(o =>
          {
            lstActions.Items.Clear();
            lstActions.Items.Add("Calling GetMyPins on server ");
          }, null);
          var proxy = new SystemClient();
          proxy.SetCredentials();
          var info = new ExtraInfo { };
          proxy.Ping(TerminalId, info);
          syncContext.Post(o =>
          {
            lstActions.Items.Add("GetMyPins Finished");
          }, null);

          proxy.Close();
          syncContext.Post(o =>
          {
            lstActions.Items.Add("Proxy closed ");
          }, null);

        });

        tPing.IsBackground = true;
        tPing.Start();
      });
    }

    private void GetMyPins_Click(object sender, RoutedEventArgs e)
    {
      ParseTerminalId();
      var proxy = new ProductsClient();
      proxy.SetCredentials();

      lstActions.Items.Clear();
      lstActions.Items.Add("Calling GetMyPins on server ");
      var result = proxy.GetMyPins(TerminalId);
      lstActions.Items.Add("GetMyPins Finished");
      proxy.Close();
      lstActions.Items.Add("Proxy closed");
    }

    //private void SyncMySales_Click(object sender, RoutedEventArgs e)
    //{
    //  ParseTerminalId();
    //  var proxy = new ProductsClient();
    //  proxy.SetCredentials();

    //  lstActions.Items.Clear();
    //  lstActions.Items.Add("Calling SyncMySales on server ");
    //  proxy.SyncMySales(TerminalId, null);
    //  lstActions.Items.Add("SyncMySales Finished");
    //  proxy.Close();
    //  lstActions.Items.Add("Proxy closed");
    //}

    //private void SyncTransferTrx_Click(object sender, RoutedEventArgs e)
    //{
    //  ParseTerminalId();
    //  var clientService = ServiceLocator.Current.GetInstance<IClientService>();
    //  lstActions.Items.Clear();
    //  lstActions.Items.Add("Calling ApproveAnyHoldTransfers on server ");
    //  clientService.ApproveAnyHoldTransfers(TerminalId);
    //  lstActions.Items.Add("ApproveAnyHoldTransfers Finished");
    //}

    private void AnyOrders_Click(object sender, RoutedEventArgs e)
    {
      ParseTerminalId();
      var proxy = new SystemClient();
      proxy.SetCredentials();
      var info = new ExtraInfo { ErrorCode = 1, ErrorMessage = "TESTING" };
      lstActions.Items.Clear();
      lstActions.Items.Add("Calling AnyOrders on server ");
      proxy.AnyOrders(TerminalId, info);
      lstActions.Items.Add("AnyOrders Finished");
      proxy.Close();
      lstActions.Items.Add("Proxy closed");
    }

    private void TestDb_Click(object sender, RoutedEventArgs e)
    {
      ParseTerminalId();
      var appservice = new ClientService();
      appservice.SellItem(Guid.NewGuid(), Transaction.Current, null, Guid.NewGuid());
    }

    private async void SyncData_Click(object sender, RoutedEventArgs e)
    {
      ParseTerminalId();
      var clientSvc = ServiceLocator.Current.GetInstance<IClientService>();

      lstActions.Items.Clear();
      lstActions.Items.Add("Calling SyncData on server ");

      await clientSvc.SyncWithServer(TerminalId);

      lstActions.Items.Add("SyncData Finished");
    }
  }
}
