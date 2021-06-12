using CommonServiceLocator;
using Geeky.POSK.Infrastructore.Core.Uow;
using Geeky.POSK.Repository.Interfface;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace UnitTesting
{
  [TestFixture]
  class UOWTests : BaseUnitTest
  {

    [Test]
    public void UOW_Test()
    {
      //var terminalRepo = ServiceLocator.Current.GetInstance<ITerminalRepository>();
      //var terminals = terminalRepo.FindAll().ToList();

      //var terminal = terminals.First();
      //terminal.Address = "";

      //terminalRepo.Update(terminal);


      //Task t1 = CreateATask(1);
      //Task t2 = CreateATask(0);
      //Task t3 = CreateATask(2);

      //t1.Start();
      //t1.Wait();
      //t2.Start();
      //t2.Wait();
      //t3.Start();
      //t3.Wait();
      var t = Task.Run(async () =>
      {
        await CreateATask(1);
        await CreateATask(0);
        await CreateATask(2);
      });

      //Task.WaitAll(t1, t2, t3);
      t.Wait();
    }

    [Test]
    public void NO_UOW_Test()
    {
      var t = Task.Run( () =>
      {
        CreateATaskWithoutUow(1);
        CreateATaskWithoutUow(0);
        CreateATaskWithoutUow(2);
      });

      t.Wait();
    }

    private async Task CreateATask(int terminalIndex)
    {
      await Task.Factory.StartNew(() =>
      {
        using (var uow = ServiceLocator.Current.GetInstance<IUnitOfWork>())
        {
          //var ctx = ServiceLocator.Current.GetInstance<DbContext>();
          //using (var trx = ctx.Database.BeginTransaction())
          //{
          using (var scope = new TransactionScope(TransactionScopeOption.Required))
          {
            var terminalRepo = ServiceLocator.Current.GetInstance<ITerminalRepository>();
            var terminals = terminalRepo.FindAll().ToList();

            var terminal = terminals[terminalIndex];
            terminal.Address = "Some text";

            terminalRepo.Update(terminal);

            scope.Complete();
          }
          //  trx.Commit();
          //}
        }
      });
    }

    private async Task CreateATaskWithoutUow(int terminalIndex)
    {
      await Task.Factory.StartNew(() =>
      {
        //using (var uow = ServiceLocator.Current.GetInstance<IUnitOfWork>())
        //{
        //var ctx = ServiceLocator.Current.GetInstance<DbContext>();
        //using (var trx = ctx.Database.BeginTransaction())
        //{
        using (var scope = new TransactionScope(TransactionScopeOption.Required))
        {
          var terminalRepo = ServiceLocator.Current.GetInstance<ITerminalRepository>();
          var terminals = terminalRepo.FindAll().ToList();

          var terminal = terminals[terminalIndex];
          terminal.Address = "Some text";

          terminalRepo.Update(terminal);

          scope.Complete();
        }
        //  trx.Commit();
        //}
        //}
      });
    }


    public override void SetupTest()
    {
      base.SetupTest();
      InitDependancies();
    }


    private void InitDependancies()
    {


    }
  }
}
