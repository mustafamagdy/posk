using System.Linq;
using Autofac;
using Geeky.POSK.Repository.Core.Base;
using Geeky.POSK.Repository;
using Geeky.POSK.Repository.Core.Interface.Base;
using Geeky.POSK.ORM.Context.EF;
using Geeky.POSK.ORM.Contect.Core;
//using Geeky.POSK.ORM.Context.Core.Extensions;
//using Geeky.POSK.ORM.Context.Core.Interface;
using Geeky.POSK.ORM.Mapping;
using Geeky.POSK.Services.Exceptions;
using Geeky.POSK.Infrastructore.Core.Exceptions;
using Geeky.POSK.Infrastructore.Core.Logging;
using Geeky.POSK.ServiceContracts;
using System.Data.Entity;

namespace Geeky.POSK.Services
{
  public class ServiceDependancies : Autofac.Module
  {
    protected override void Load(ContainerBuilder builder)
    {
      //builder.RegisterType<BaseContext>().AsSelf().As<DbContext>().As<IDataContext>().InstancePerLifetimeScope();
      //builder.RegisterGeneric(typeof(POSKDbContext<,>)).As(typeof(IDataContext<,>)).InstancePerLifetimeScope();
      //builder.RegisterType<EFEagerFetch>().As<IEagerFetching>().InstancePerLifetimeScope();
      builder.RegisterType<AppDbContext>().AsSelf().As<DbContext>().As<IAppDbContext>().InstancePerLifetimeScope();



      builder.RegisterAssemblyTypes(typeof(TerminalRepository).Assembly)
       .Where(t => !t.IsAbstract && t.GetInterfaces().Any(i => i.Name != "IBaseRepository"))
       .As(i => i.GetInterfaces().FirstOrDefault(x => x.Name != "IBaseRepository" && x.Name.StartsWith("I")))
       .AsClosedTypesOf(typeof(IBaseRepository<,>))
       .InstancePerLifetimeScope();

      builder.RegisterType<Logger>().As<ILogger>().InstancePerLifetimeScope();
      builder.RegisterType<ServerFaultsFactory>().As<IServerFaultsFactory>().InstancePerLifetimeScope();

      builder.RegisterType<ProductService>().As<IProductService>()
                                            .As<IStatisticsService>()
                                            .InstancePerDependency();

      builder.RegisterType<ProductManagementService>().As<IProductManagementService>();


    }
  }
}
