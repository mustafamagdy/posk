using System;
using NUnit.Framework;
using System.Data;
using Geeky.POSK.Infrastructore.Extensions;
using CommonServiceLocator;
using Geeky.POSK.ServiceContracts;
using Geeky.POSK.ORM.Context.EF;
using Geeky.POSK.Models;
using System.Linq;
using Geeky.POSK.Repository.Interfface;
using Geeky.POSK.Infrastructore.Core;
using Geeky.POSK.Infrastructore.Core.Extensions;
using System.Data.SqlClient;

namespace UnitTesting
{
  [TestFixture]
  public class BulkInsertProducts : BaseUnitTest
  {
    private ITerminalRepository _terminalRepo;
    private IVendorRepository _vendorRepo;
    private IProductRepository _productRepo;


    public override void SetupTest()
    {
      base.SetupTest();

      InitDependancies();

      _vendorRepo = ServiceLocator.Current.GetInstance<IVendorRepository>();
      _terminalRepo = ServiceLocator.Current.GetInstance<ITerminalRepository>();
      _productRepo = ServiceLocator.Current.GetInstance<IProductRepository>();

    }

    private void InitDependancies()
    {
    }

    [Test]
    public void BulkInsertWithVendors()
    {
      var db = new AppDbContext();
      //var db = new POSKDbContext<Vendor, Guid>(dbContext);
      var vendors = db.Set<Vendor>().ToList();

      var terminalRepo = ServiceLocator.Current.GetInstance<ITerminalRepository>();
      var serverTerminal = terminalRepo.CreateSererTerminal();

      var trx = (SqlTransaction)db.Database.BeginTransaction().UnderlyingTransaction;

      var dt = new DataTable();
      dt.Columns.Add("PIN", typeof(string));
      dt.Columns.Add("Serial", typeof(string));
      dt.Columns.Add("Expire", typeof(DateTime));
      dt.Columns.Add("Vendor", typeof(string));
      dt.Columns.Add("Product", typeof(string));
      dt.Columns.Add("Price", typeof(decimal));
      dt.Columns.Add("ProductType", typeof(ProductTypeEnum));

      Random r = new Random();

      r.Next(10, 50).Loop(i =>
      {
        dt.Rows.Add(Randomz.GetRandomNumber(8),
                    Randomz.GetRandomNumber(14),
                    Randomz.RandomFutureDate(),
                    "STC",
                    "STC_10",
                    10.0d, false, ProductTypeEnum.Voice);
      });

      r.Next(10, 50).Loop(i =>
      {
        dt.Rows.Add(Randomz.GetRandomNumber(8),
                    Randomz.GetRandomNumber(14),
                    Randomz.RandomFutureDate(),
                    "MOBILY",
                    "MOBILY_20",
                    20.0d, false, ProductTypeEnum.Data);
      });

      dt.TableName = "Products";

      var svc = ServiceLocator.Current.GetInstance<IProductManagementService>();
      svc.ImportProductList(dt, serverTerminal.Id, trx);

      trx.Commit();
    }

  }
}
