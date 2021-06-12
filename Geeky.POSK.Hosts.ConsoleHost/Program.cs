using Autofac;
using Autofac.Extras.CommonServiceLocator;
using Autofac.Integration.Wcf;
using CommonServiceLocator;
using Geeky.POSK.Infrastructore.Core;
using Geeky.POSK.Infrastructore.Extensions;
using Geeky.POSK.ServiceContracts;
using Geeky.POSK.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Geeky.POSK.Hosts.ConsoleHost
{
  class Program
  {
    static void Main(string[] args)
    {
      AppStart.AppInitialize();
      ServiceHost systemHost = new ServiceHost(typeof(SystemService));
      systemHost.Open();

      ServiceHost productHost = new ServiceHost(typeof(ProductService));
      productHost.Open();

      Console.WriteLine($"Server is running now @ {systemHost.Description.Endpoints[0].Address}");
      60.Loop(() => Console.Write("*"));
      Console.WriteLine();
      Console.ReadLine();

      systemHost.Close();
      productHost.Close();
    }
  }
}
