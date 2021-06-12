using Geeky.POSK.Infrastructore.Extensions;
using Geeky.POSK.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Geeky.POSK.Hosts.WinSvc
{
  public partial class KioskService : ServiceBase
  {
    ServiceHost systemHost;
    ServiceHost productHost;
    public KioskService()
    {
      InitializeComponent();
    }

    protected override void OnStart(string[] args)
    {
      AppStart.AppInitialize();
      systemHost = new ServiceHost(typeof(SystemService));
      systemHost.Open();

      productHost = new ServiceHost(typeof(ProductService));
      productHost.Open();

      Console.WriteLine($"Server is running now @ {systemHost.Description.Endpoints[0].Address}");
      60.Loop(() => Console.Write("*"));
      //Console.WriteLine();
      //Console.ReadLine();
    }

    protected override void OnStop()
    {
      systemHost.Close();
      productHost.Close();
    }
  }
}
