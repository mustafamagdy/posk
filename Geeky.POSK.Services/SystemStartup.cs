using Autofac;
using Autofac.Extras.CommonServiceLocator;
using CommonServiceLocator;
using Geeky.POSK.DataContracts.MappingProfile;
using Geeky.POSK.Infrastructore.Core;
using Geeky.POSK.Services.MappingProfile;

namespace Geeky.POSK.Services
{
  public static  class AppStart
  {
    public static void AppInitialize()
    {
      ContainerBuilder builder = new ContainerBuilder();
      builder.RegisterModule<CoreDependancies>();
      builder.RegisterModule<ServiceDependancies>();
      var container = builder.Build();

      //var slocator = new AutofacServiceLocator(container);
      //ServiceLocator.SetLocatorProvider(() => slocator);

      var csl = AutofacHybridServiceLocator.Initialize(container);
      ServiceLocator.SetLocatorProvider(() => csl);

      AutoMapper.Mapper.Initialize((cfg) =>
      {
        cfg.AddProfile<ContractProfile>();
        cfg.AddProfile<ServiceProfile>();
      });

    }
  }
}
