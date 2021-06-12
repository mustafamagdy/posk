using Autofac;
using Autofac.Extras.CommonServiceLocator;
using CommonServiceLocator;
using Geeky.POSK.Infrastructore.Core;
using Geeky.POSK.Services;
using NUnit.Framework;

namespace UnitTesting
{
  public class BaseUnitTest
  {

    [SetUp]
    public virtual void SetupTest()
    {
      ContainerBuilder builder = new ContainerBuilder();
      builder.RegisterModule<CoreDependancies>();
      builder.RegisterModule<ServiceDependancies>();
      var container = builder.Build();
      container.BeginLifetimeScope();

      //var slocator = new AutofacServiceLocator(container);
      //ServiceLocator.SetLocatorProvider(() => slocator);


      var csl = AutofacHybridServiceLocator.Initialize(container);
      ServiceLocator.SetLocatorProvider(() => csl);
    }
  }
}