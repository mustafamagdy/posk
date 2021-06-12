using System;
using Geeky.POSK.ORM.Context.EF;
using Geeky.POSK.ORM.Mapping;
using Geeky.POSK.ORM.Contect.Core.DbInitializers;
using Geeky.POSK.Models;
using System.Linq;
using NUnit.Framework;
using Geeky.POSK.Client.Proxy;
using Geeky.POSK.Repository.Core.Base;
using Geeky.POSK.Repository;
using Geeky.POSK.Infrastructore.Extensions;
using Geeky.POSK.ORM.Contect.Core;
using Autofac;
using Geeky.POSK.Infrastructore.Core;
using Autofac.Extras.CommonServiceLocator;
using CommonServiceLocator;
using Geeky.POSK.Services;
using Geeky.POSK.Repository.Interfface;
using System.Transactions;

namespace UnitTesting
{
  [TestFixture]
  public class DataAccessTests : BaseUnitTest
  {
    public override void SetupTest()
    {
      base.SetupTest();
      InitDependancies();
    }

    [Test]
    public void TestContextAddAndRetrive()
    {
      var db = ServiceLocator.Current.GetInstance<AppDbContext>();
      int currentCount = db.Set<Vendor>().Count();
      db.Set<Vendor>().Add(new Vendor { Code = "My V",Language1Name="a", Language2Name = "a", Language3Name = "a", Language4Name = "a" });
      var all = db.Set<Vendor>().ToList();
      Assert.AreEqual(currentCount, all.Count);
      db.SaveChanges();
      all = db.Set<Vendor>().ToList();
      Assert.AreEqual(currentCount + 1, all.Count);
    }

        [Test]
        public void TestBasicQuery()
        {
            var db = ServiceLocator.Current.GetInstance<AppDbContext>();
            var terminalRepo = ServiceLocator.Current.GetInstance<ITerminalRepository>();
            var terminals = terminalRepo.FindAll().ToList();
            Assert.AreNotEqual(null, terminals);
            Assert.AreNotEqual(0, terminals.Count());

        }


        [Test]
    public void GenerateCleanDb()
    {
      var db = new AppDbContext();    
      var vendors = db.Set<Vendor>().ToList();
    }


    [Test]
    public void BaseRepositoryTest()
    {
      var repo = ServiceLocator.Current.GetInstance<IVendorRepository>();

      var c = new Vendor
      {
        Code = "STC",
      };

      var v = repo.Add(c);
      Assert.IsNotNull(v);

      var v1 = repo.Get(v.Id);
      Assert.AreEqual(v1.Id, v.Id);

    }

    [Test]
    public void BaseRepositoryTestWithTransaction()
    {
      using (var scope = new TransactionScope())
      {
        var repo = ServiceLocator.Current.GetInstance<IVendorRepository>();

        var c = new Vendor
        {
          Code = "STC",
          Language1Name = "سوا",
          Language2Name = "SAWA",
          Language3Name = "SAWA",
          Language4Name = "SAWA",
          IsActive = true,
        };

        var v = repo.Add(c);
        Assert.IsNotNull(v);

        var v1 = repo.Get(v.Id);
        Assert.AreEqual(v1.Id, v.Id);

        scope.Complete();
      }
    }

    [Test]
    public void TestingTransaction()
    {

      //var dbContext = new BaseContext();
      var _context = new AppDbContext();
      var repo = new VendorRepository(_context);

      var trx = _context.Database.BeginTransaction();

      var c = new Vendor
      {
        Code = "STC",
      };

      repo.Add(c);
      trx.Commit();
    }

    [Test]
    public void TestingFaildTransaction()
    {

      TestDelegate throwable = new TestDelegate(() =>
     {
       //var dbContext = new BaseContext();
       var _context = new AppDbContext();
       var repo = new VendorRepository(_context);

       using (var trx = _context.Database.BeginTransaction())
       {
         var c = new Vendor
         {
           Code = "Another STC",
         };
         repo.Add(c);

         throw new Exception("Error");
         trx.Commit();
       }
     });

      Assert.Throws<Exception>(throwable);
    }

    [Test]
    public void TestingSuccessTransactionAndExceptionAfterIt()
    {

      TestDelegate throwable = new TestDelegate(() =>
      {
        //var dbContext = new BaseContext();
        var _context = new AppDbContext();
        var repo = new VendorRepository(_context);

        using (var trx = _context.Database.BeginTransaction())
        {
          var c = new Vendor
          {
            Code = "Another STC",
          };
          repo.Add(c);

          trx.Commit();
          throw new Exception("Error");

        }
      });

      Assert.Throws<Exception>(throwable);
    }

    [Test]
    public void AddIEntitiesWithGraph()
    {
      var sessionRepo = ServiceLocator.Current.GetInstance<IClientSessionRepository>();
      var sessionPaymentRepo = ServiceLocator.Current.GetInstance<ISessionPaymentRepository>();

      var session1 = sessionRepo.FindAll().FirstOrDefault();
      var payment = new SessionPayment {
        CashAmount = 100,
        PaymentRefNumber = "123",
        StackedAmount = 100,
        Session = session1
      };

      sessionPaymentRepo.Add(payment);

    }




    private void InitDependancies()
    {


    }
  }


}
