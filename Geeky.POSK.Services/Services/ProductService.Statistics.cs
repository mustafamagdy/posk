using CommonServiceLocator;
using Geeky.POSK.Filters;
using Geeky.POSK.Models;
using Geeky.POSK.ORM.Contect.Core;
using Geeky.POSK.ORM.Context.EF;
using Geeky.POSK.Views;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Geeky.POSK.Services
{
  public partial class ProductService
  {
    public IEnumerable<SalesByProduct> SalesByProduct(FilterCriteria filter)
    {
      filter = filter ?? new FilterCriteria();
      IEnumerable<SalesByProduct> result = null;
      var db = ServiceLocator.Current.GetInstance<AppDbContext>();
      var filterDateFrom = filter.FromDate.HasValue;
      var filterDateTo = filter.ToDate.HasValue;
      DateTime filterDateFromValue = filterDateFrom ? filter.FromDate.Value : DateTime.MinValue;
      DateTime filterDateToValue = filterDateTo ? filter.ToDate.Value : DateTime.MinValue;

      result = from p in db.Set<Pin>()
               group p by new { p.Product.Code, p.Product.SellingPrice } into grp
               orderby grp.Key.SellingPrice
               select new SalesByProduct
               {
                 ProductCode = grp.Key.Code,
                 SoldCount = grp.Where(x => x.Sold == true &&
                   (!filterDateFrom || x.SoldDate.Value >= filterDateFromValue) &&
                   (!filterDateTo || x.SoldDate.Value <= filterDateToValue)
                   ).Count(),
                 Remaining = grp.Where(x => x.Sold == false).Count(),
               };
      return result;
    }

    public IEnumerable<SalesByTerminal> SalesByTerminal(FilterCriteria filter)
    {
      filter = filter ?? new FilterCriteria();
      IEnumerable<SalesByTerminal> result = null;
      var db = ServiceLocator.Current.GetInstance<AppDbContext>();
      var filterDateFrom = filter.FromDate.HasValue;
      var filterDateTo = filter.ToDate.HasValue;
      DateTime filterDateFromValue = filterDateFrom ? filter.FromDate.Value : DateTime.MinValue;
      DateTime filterDateToValue = filterDateTo ? filter.ToDate.Value : DateTime.MinValue;

      result = from p in db.Set<Pin>()
               group p by p.Terminal.TerminalKey into grp
               orderby grp.Key
               select new SalesByTerminal
               {
                 TerminalCode = grp.Key,
                 SoldCount = grp.Where(x => x.Sold == true &&
                   (!filterDateFrom || x.SoldDate.Value >= filterDateFromValue) &&
                   (!filterDateTo || x.SoldDate.Value <= filterDateToValue)
                   ).Count(),
                 Remaining = grp.Where(x => x.Sold == false).Count(),
               };
      return result;
    }

    public IEnumerable<SalesByVendor> SalesByVendor(FilterCriteria filter)
    {
      filter = filter ?? new FilterCriteria();
      IEnumerable<SalesByVendor> result = null;
      var db = ServiceLocator.Current.GetInstance<AppDbContext>();
      var filterDateFrom = filter.FromDate.HasValue;
      var filterDateTo = filter.ToDate.HasValue;
      DateTime filterDateFromValue = filterDateFrom ? filter.FromDate.Value : DateTime.MinValue;
      DateTime filterDateToValue = filterDateTo ? filter.ToDate.Value : DateTime.MinValue;

      result = from p in db.Set<Pin>()
               group p by p.Product.Vendor.Code into grp
               orderby grp.Key
               select new SalesByVendor
               {
                 VendorCode = grp.Key,
                 SoldCount = grp.Where(x => x.Sold == true &&
                   (!filterDateFrom || x.SoldDate.Value >= filterDateFromValue) &&
                   (!filterDateTo || x.SoldDate.Value <= filterDateToValue)
                   ).Count(),
                 Remaining = grp.Where(x => x.Sold == false).Count(),
               };
      return result;
    }

    public IEnumerable<SalesReportRow> SalesReport(SalesReportFilterCriteria filter)
    {
      filter = filter ?? new SalesReportFilterCriteria();
      IEnumerable<SalesReportRow> result = null;
      var db = ServiceLocator.Current.GetInstance<AppDbContext>();
      var filterDateFrom = filter.FromDate.HasValue;
      var filterDateTo = filter.ToDate.HasValue;
      DateTime filterDateFromValue = filterDateFrom ? filter.FromDate.Value : DateTime.MinValue;
      DateTime filterDateToValue = filterDateTo ? filter.ToDate.Value : DateTime.MinValue;

      result = from p in db.Set<Pin>()
               where p.Product.Vendor.Id  == filter.Vendor.Id
               group p by new
               {
                 MachineName = p.Terminal.MachineName,
                 ProductCode = p.Product.Code,
                 Price = p.Product.SellingPrice,
                 VendorName = p.Product.Vendor.Code
               } into grp
               select new SalesReportRow
               {
                 TerminalCode = grp.Key.MachineName,
                 VendorCode = grp.Key.VendorName,
                 ProductCode = grp.Key.ProductCode,
                 SoldCount = grp.Where(x => x.Sold == true &&
                   (!filterDateFrom || x.SoldDate.Value >= filterDateFromValue) &&
                   (!filterDateTo || x.SoldDate.Value <= filterDateToValue)
                   ).Count(),
                 Remaining = grp.Where(x => x.Sold == false).Count(),
                 Price = grp.Key.Price
               };

      return result;
    }

  }
}
