using Autofac;
using Autofac.Extras.CommonServiceLocator;
using CommonServiceLocator;
using Geeky.POSK.Infrastructore.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;

namespace Geeky.POSK.Services
{
  //public class DefaultServiceHostFactory : ServiceHostFactory
  //{

  //  public DefaultServiceHostFactory()
  //  {
  //    Console.WriteLine("Starting up");
  //    ConfigureServiceLocator();

  //  }

  //  private void ConfigureServiceLocator()
  //  {
  //    ContainerBuilder builder = new ContainerBuilder();
  //    builder.RegisterAssemblyModules<CoreDependancies>();
  //    var container = builder.Build();

  //    var slocator = new AutofacServiceLocator(container);
  //    ServiceLocator.SetLocatorProvider(() => slocator);
  //  }

  //  protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
  //  {
  //    return base.CreateServiceHost(serviceType, baseAddresses);
  //  }
  //}
}
