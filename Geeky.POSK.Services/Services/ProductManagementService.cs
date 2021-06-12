using System;
using System.Collections.Generic;
using System.Linq;
using Geeky.POSK.Models;
using Geeky.POSK.Infrastructore.Extensions;
using System.Data.Entity;
using System.Data;
using CommonServiceLocator;
using Geeky.POSK.ORM.Contect.Core.Context.Bulk;
using Geeky.POSK.Services.Internal;
using Geeky.POSK.Repository.Interfface;
using Geeky.POSK.ORM.Contect.Core.Context;
using Geeky.POSK.Infrastructore.Core;
//using Geeky.POSK.ORM.Contect.Core.IdGenerator;
using System.Data.SqlClient;
using System.Transactions;
using Geeky.POSK.ServiceContracts;

namespace Geeky.POSK.Services
{
  public class ProductManagementService : IProductManagementService
  {

    public void ImportProductList(DataTable dtPins, Guid terminalId, SqlTransaction trx = null)
    {
      var products = from p in dtPins.AsEnumerable()
                     select new PinDataContract
                     {
                       PIN = p.SafeField<string>("PIN"),
                       SerialNumber = p.SafeField<string>("Serial"),
                       ExpiryDate = p.SafeField<DateTime>("Expire"),
                       VendorCode = p.SafeField<string>("Vendor"),
                       ProductCode = p.SafeField<string>("Product"),
                       Price = p.SafeField<decimal>("Price", 0),
                       ProductType = p.SafeField<string>("ProductType"),
                       PriceAfterTax = p.SafeField<decimal>("PriceAfterTax", 0),
                     };
      BulkImportPins(products, terminalId, trx);
    }

    public void ImportProductListForVendor(DataTable dtPins, Guid vendorId, Guid terminalId, SqlTransaction trx = null)
    {
      var _vendorRepo = ServiceLocator.Current.GetInstance<IVendorRepository>();
      var vendor = _vendorRepo.Get(vendorId);
      var products = from p in dtPins.AsEnumerable()
                     select new PinDataContract
                     {
                       PIN = p.SafeField<string>("PIN"),
                       SerialNumber = p.SafeField<string>("Serial"),
                       ExpiryDate = p.SafeField<DateTime>("Expire"),
                       VendorCode = vendor.Code,
                       ProductCode = p.SafeField<string>("Product"),
                       Price = p.SafeField<decimal>("Price", 0),
                       ProductType = p.SafeField<string>("ProductType"),
                       PriceAfterTax = p.SafeField<decimal>("PriceAfterTax", 0),
                     };

      BulkImportPins(products, terminalId, trx);
    }

    private void BulkImportPins(IEnumerable<PinDataContract> pins, Guid terminalId, SqlTransaction trx = null)
    {
      var _vendorRepo = ServiceLocator.Current.GetInstance<IVendorRepository>();
      var _productRepo = ServiceLocator.Current.GetInstance<IProductRepository>();

      var vendors = pins.Select(x => x.VendorCode).Distinct().ToArray();
      var notFoundVendors = _vendorRepo.GetNotFoundVendorsByName(vendors);
      var allVendors = _vendorRepo.InsertAndGetAll(notFoundVendors);

      var allProducts = _productRepo.FindAll().ToList();

      var _context = ServiceLocator.Current.GetInstance<DbContext>();
      var dtPins = MetaData.CreateDataTableStructure<Pin>((DbContext)_context);
      //var idGen = new SeqHiloGenerator<Pin, Guid>(_context);
      string[] columnToEncrypt = new string[] { "PIN" };

      Func<string, object, object> getValue = (name, obj) =>
      {
        return obj.GetType().GetProperty(name)?
                                .GetValue(obj);
      };

      Func<decimal, decimal, string, ProductTypeEnum, Vendor, Product> getOrSaveProduct = (price, priceAftertax, code, productType, vendor) =>
         {
           var product = allProducts.FirstOrDefault(x => x.Code.ToLower() == code.ToLower());
           if (product == null)
             product = _productRepo.Add(new Product
             {
               Vendor = vendor,
               Code = code,
               SellingPrice = price,
               PriceAfterTax = priceAftertax,
               Language1Name = code,
               Language2Name = code,
               Language3Name = code,
               Language4Name = code,
               IsActive = true,
               ProductType = productType
             });
           allProducts.Add(product);
           return product;
         };

      Func<Type, object> GetDefault = (type) =>
       {
         if (type.IsValueType)
         {
           return Activator.CreateInstance(type);
         }
         else if (type == typeof(string))
         {
           return "";
         }

         return DBNull.Value;
       };

      foreach (var p in pins)
      {
        DataRow row = dtPins.NewRow();
        var vendor = allVendors.FirstOrDefault(x => x.Code.ToLower() == p.VendorCode.ToLower());
        var productType = ProductTypeEnum.Voice;
        Enum.TryParse(p.ProductType, out productType);
        var product = getOrSaveProduct(p.Price, p.PriceAfterTax, p.ProductCode, productType, vendor);
        foreach (DataColumn col in dtPins.Columns)
        {
          object value = null;
          if (col.ColumnName == "Vendor_Id")
          {
            value = vendor.Id;
          }
          else if (col.ColumnName == "Terminal_Id")
          {
            value = terminalId;
          }
          else if (col.ColumnName == "Product_Id")
          {
            value = product.Id;
          }
          else if (col.ColumnName == "CreateDate")
          {
            value = DateTime.Now;
          }
          else if (col.ColumnName == "Id")
          {
            //value = idGen.Generate();
            value = Guid.NewGuid();
          }
          else if (columnToEncrypt.Contains(col.ColumnName))
          {
            var _normalValue = getValue(col.ColumnName, p).ToString();
            value = AesEncyHelper.Encyrpt(_normalValue, terminalId.ToByteArray(), terminalId.ToByteArray());
          }
          else
            value = getValue(col.ColumnName, p);

          row[col.ColumnName] = value ?? GetDefault(col.DataType);
        }
        dtPins.Rows.Add(row);
      }

     (_context as DbContext).BulkInsert<Pin>(dtPins, trx);
    }

    public void RedeemProductsFromTerminal(Guid terminalId, IEnumerable<Pin> pins)
    {
      var _terminalRepo = ServiceLocator.Current.GetInstance<ITerminalRepository>();
      var _pinRepo = ServiceLocator.Current.GetInstance<IPinRepository>();
      var serverAsTerminal = _terminalRepo.GetServerTerminal();

      using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
      {
        pins.ForEach(p =>
          {
            p.PIN = AesEncyHelper.ReEncrypt(p.PIN, p.Terminal.Id, serverAsTerminal.Id);
            p.Terminal = serverAsTerminal;

            _pinRepo.Update(p);
          });
        scope.Complete();
      }
    }

    public void SendProductsToTerminal(Guid terminalId, IEnumerable<Pin> products)
    {
      var _terminalRepo = ServiceLocator.Current.GetInstance<ITerminalRepository>();
      var _pinRepo = ServiceLocator.Current.GetInstance<IPinRepository>();
      var terminal = _terminalRepo.Get(terminalId);

      using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
      {
        products.ForEach(p =>
          {
            p.PIN = AesEncyHelper.ReEncrypt(p.PIN, p.Terminal.Id, terminal.Id);
            p.Terminal = terminal;

            _pinRepo.Update(p);
          });
        scope.Complete();
      }
    }

    public void TransferProductsBetweenTerminals(Guid fromTerminalId, Guid toTerminalId, Guid productId, int count)
    {
      var _terminalRepo = ServiceLocator.Current.GetInstance<ITerminalRepository>();
      var _pinRepo = ServiceLocator.Current.GetInstance<IPinRepository>();
      var _productRepo = ServiceLocator.Current.GetInstance<IProductRepository>();
      var _transferTrxRepo = ServiceLocator.Current.GetInstance<ITransferTrxRepository>();

      var sourceTerminal = _terminalRepo.Get(fromTerminalId);
      var destTerminal = _terminalRepo.Get(toTerminalId);
      var product = _productRepo.Get(productId);

      var availablePins = _pinRepo.FindAll(x => x.Terminal.Id == fromTerminalId && x.Sold == false && x.Product.Id == productId);
      var availableCount = availablePins.Count();
      if (availableCount < count) throw new InvalidOperationException($"Available count to be transfered is {availableCount} which is less than required count {count}");
      var notSoldPinsToTransfer = availablePins.Take(count).ToList();

      using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
      {
        var transferTrx = new TransferTrx
        {
          SourceTerminal = sourceTerminal,
          DestTerminal = destTerminal,
          CreateDate = DateTime.Now,
          Status = TransferTrxStatusEnum.Hold,
          Product = product,
          RequestedCount = count
        };

        transferTrx = _transferTrxRepo.Add(transferTrx);

        if (sourceTerminal.ServerTerminal)
        {
          transferTrx.Status = TransferTrxStatusEnum.Completed;
          transferTrx.TransferredCount = transferTrx.RequestedCount;
          _transferTrxRepo.Update(transferTrx);

          notSoldPinsToTransfer.ForEach(p =>
          {
            p.Hold = false;//source terminal is the server, and it approved automatically
            p.TransferTrx = transferTrx;
            p.PIN = AesEncyHelper.ReEncrypt(p.PIN, p.Terminal.Id, transferTrx.DestTerminal.Id);
            p.Terminal = transferTrx.DestTerminal;
            _pinRepo.Update(p);
          });
        }
        else
        {
          notSoldPinsToTransfer.ForEach(p =>
          {
            p.Hold = true;//mark it as hold untill source terminal approve that
            p.TransferTrx = transferTrx;
            _pinRepo.Update(p);
          });
        }

        scope.Complete();
      }

    }

  }
}
