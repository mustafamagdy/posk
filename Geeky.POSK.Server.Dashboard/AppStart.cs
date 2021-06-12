using Autofac;
using Autofac.Extras.CommonServiceLocator;
using CommonServiceLocator;
using Geeky.POSK.DataContracts.MappingProfile;
using Geeky.POSK.Infrastructore.Core;
using Geeky.POSK.Models;
using Geeky.POSK.ORM.Contect.Core;
using Geeky.POSK.ORM.Context.EF;
using Geeky.POSK.Services;
using Geeky.POSK.Services.MappingProfile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geeky.POSK.Server.Dashboard
{
  public static class AppStart
  {
    private static void SetupDependancies()
    {
      ContainerBuilder builder = new ContainerBuilder();
      builder.RegisterModule<CoreDependancies>();
      builder.RegisterModule<ServiceDependancies>();
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
        cfg.AddProfile<ServiceProfile>();
        cfg.AddProfile<ServerProfile>();
      });
    }

    public static void Execute()
    {
      SetupDependancies();
      SetupAutoMappingProfiles();

      //Db hit to create db if not exist
      var db = ServiceLocator.Current.GetInstance<AppDbContext>();
      db.Set<Vendor>().ToList();//cause db to be created
    }
  }
}
