using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Geeky.POSK.Infrastructore.Core.Uow;
using Geeky.POSK.Infrastructore.Core.Startup;

namespace Geeky.POSK.Infrastructore.Core
{
  public class CoreDependancies : Autofac.Module
  {
    protected override void Load(ContainerBuilder builder)
    {

      //Unit of Work
      builder.Register<IUnitOfWork>(x =>
      new IocChildScopUnitOfWork(
        AutofacHybridServiceLocator.Instance.CreateChildScope(), () =>
        {
          AutofacHybridServiceLocator.Instance.DisposeCurrentChildScope();
        }))
       .InstancePerDependency();

      builder.RegisterAssemblyTypes(typeof(IStartupTask).Assembly)
        .Where(t => !t.IsAbstract && t.GetInterfaces().Any(i => i == typeof(IStartupTask)))
        .As<IStartupTask>().InstancePerLifetimeScope();

    }
  }
}
