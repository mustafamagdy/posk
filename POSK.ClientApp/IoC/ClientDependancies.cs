using System.Linq;
using Autofac;
using Geeky.POSK.Repository.Core.Base;
using Geeky.POSK.Repository;
using Geeky.POSK.Repository.Core.Interface.Base;
using Geeky.POSK.ORM.Context.EF;
using Geeky.POSK.ORM.Contect.Core;
//using Geeky.POSK.ORM.Context.Core.Extensions;
//using Geeky.POSK.ORM.Context.Core.Interface;
using Geeky.POSK.Infrastructore.Core.Exceptions;
using Geeky.POSK.Infrastructore.Core.Logging;
using System.Data.Entity;
using POSK.Client.ApplicationService.Interface;
using POSK.Client.ApplicationService;
using POSK.Client.CashCode.Interface;
using POSK.Client.CashCode;
using POSK.Printers;
using POSK.Printers.Interface;
using POSK.ClientApp;
using POSK.Payment.Interface;
using POSK.Client.CRT.Interface;

namespace Geeky.POSK.Services
{
  public class ClientDependancies : Autofac.Module
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
      builder.RegisterType<ClientService>().As<IClientService>().InstancePerLifetimeScope();
      builder.RegisterType<TerminalService>().As<ITerminalService>().InstancePerLifetimeScope();


#if DEBUG
      if (AppStartup.SIMULATED_MODE)
      {
        builder.RegisterType<CashCodeSimulator>().As<IPaymentMethod>().InstancePerLifetimeScope();
        builder.RegisterType<CRTCardReader>().As<IPaymentMethod>().InstancePerLifetimeScope();

        builder.RegisterType<CashCodeSimulator>().As<ICashCodeBillValidator>().InstancePerLifetimeScope();

        builder.RegisterType<FakeReceiptPrinter>().As<IReceiptPrinter>().InstancePerLifetimeScope();
        builder.RegisterType<FakeReceiptPrinter>().Named<IReceiptPrinter>("session_end").InstancePerLifetimeScope();
      }
      else
      {
        builder.RegisterType<CashCodeBillValidator>().As<IPaymentMethod>().InstancePerLifetimeScope();
        builder.RegisterType<CRTCardReader>().As<IPaymentMethod>().InstancePerLifetimeScope();

        builder.RegisterType<CashCodeBillValidator>().As<ICashCodeBillValidator>().InstancePerLifetimeScope();
        builder.RegisterType<ReceiptDocumentPrinter>().As<IReceiptPrinter>().InstancePerLifetimeScope();
        builder.RegisterType<SessionDocumentPrinter>().Named<IReceiptPrinter>("session_end").InstancePerLifetimeScope();
      }
      //builder.RegisterType<ReceiptPrinter>().As<IReceiptPrinter>().InstancePerLifetimeScope();
#else
      builder.RegisterType<CashCodeBillValidator>().As<IPaymentMethod>().InstancePerLifetimeScope();
      builder.RegisterType<CRTCardReader>().As<IPaymentMethod>().InstancePerLifetimeScope();

      builder.RegisterType<CashCodeBillValidator>().As<ICashCodeBillValidator>().InstancePerLifetimeScope();
      builder.RegisterType<ReceiptDocumentPrinter>().As<IReceiptPrinter>().InstancePerLifetimeScope();
      builder.RegisterType<SessionDocumentPrinter>().Named<IReceiptPrinter>("session_end").InstancePerLifetimeScope();
#endif

    }
  }
}
