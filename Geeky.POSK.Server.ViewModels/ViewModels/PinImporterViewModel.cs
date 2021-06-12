using AutoMapper;
using CommonServiceLocator;
using Geeky.POSK.DataContracts;
using Geeky.POSK.Models;
using Geeky.POSK.Repository.Interfface;
using Geeky.POSK.ServiceContracts;
using Geeky.POSK.WPF.Core.Base;
using Microsoft.Practices.Prism.Commands;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Geeky.POSK.Server.ViewModels
{
  public class PinImporterViewModel : BindableBaseViewModel
  {
    private bool _loadingFile;
    public bool LoadingFile { get { return _loadingFile; } set { SetProperty(ref _loadingFile, value); } }

    private bool _columnHeaders;
    public bool ColumnHeaders
    {
      get { return _columnHeaders; }
      set
      {
        SetProperty(ref _columnHeaders, value);
        GetExcelColumns(_excelFilePath);
      }
    }

    private ColumnMapping _mapping;
    public ColumnMapping Mapping { get { return _mapping; } set { SetProperty(ref _mapping, value); } }

    private ImportResult _result;
    public ImportResult Result { get { return _result; } set { SetProperty(ref _result, value); } }

    private ObservableCollection<ExcelColumnDto> _columns;
    public ObservableCollection<ExcelColumnDto> Columns { get { return _columns; } set { SetProperty(ref _columns, value); } }

    private string _excelFilePath = "";

    public DelegateCommand ChooseFile { get; private set; }
    public DelegateCommand ImportFile { get; private set; }

    public event Action SelectImportingFile = delegate { };

    public PinImporterViewModel()
    {
      ChooseFile = new DelegateCommand(OnChooseFile);
      ImportFile = new DelegateCommand(OnImportFile);
      Columns = new ObservableCollection<ExcelColumnDto>();
      Mapping = new ColumnMapping();
      Result = new ImportResult();
      LoadingFile = false;
    }

    private void OnChooseFile()
    {
      SelectImportingFile();
    }
    private void OnImportFile()
    {
      ImportFromExcelFile();
    }

    public void GetExcelColumns(string filePath)
    {
      if (string.IsNullOrEmpty(filePath))
        return;

      _excelFilePath = filePath;
      Columns = new ObservableCollection<ExcelColumnDto>();
      var fi = new FileInfo(filePath);
      bool hasHeaderRow = ColumnHeaders;
      using (var p = new ExcelPackage(fi))
      {
        var ws = p.Workbook.Worksheets["Sheet1"];
        foreach (var firstRowCell in ws.Cells[1, 1, 1, ws.Dimension.End.Column])
        {
          Columns.Add(new ExcelColumnDto
          {
            ColumnIndex = firstRowCell.Start.Column,
            ColumnName = hasHeaderRow ?
                            firstRowCell.Text : string.Format("Column {0}", firstRowCell.Start.Column)
          });
        }
      }
      LoadingFile = true;
    }
    public void ImportFromExcelFile()
    {
      var _terminalRepo = ServiceLocator.Current.GetInstance<ITerminalRepository>();
      var serverTerminal = _terminalRepo.GetServerTerminal();

      var fi = new FileInfo(_excelFilePath);
      var tbl = new DataTable();
      tbl.Columns.Add("PIN", typeof(string));
      tbl.Columns.Add("Serial", typeof(string));
      tbl.Columns.Add("Expire", typeof(DateTime));
      tbl.Columns.Add("Vendor", typeof(string));
      tbl.Columns.Add("Product", typeof(string));
      tbl.Columns.Add("Price", typeof(decimal));
      tbl.Columns.Add("PriceAfterTax", typeof(decimal));
      tbl.Columns.Add("ProductType", typeof(string));

      var colIndexes = new int[] { Mapping.PinColumn, Mapping.SerialNumberColumn,
                                  Mapping.ExpiryDateColumn, Mapping.PriceColumn,
                                  Mapping.ProductColumn, Mapping.VendorColumn,
                                  Mapping.PriceAfterTaxColumn
                                 };
      using (var p = new ExcelPackage(fi))
      {
        var ws = p.Workbook.Worksheets["Sheet1"];
        var startRow = ColumnHeaders ? 2 : 1;
        for (var rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
        {
          var row = tbl.NewRow();

          var wsRow = ws.Cells[rowNum, 1, rowNum, ws.Dimension.End.Column];
          foreach (int colIndex in colIndexes)
          {
            string colName = "";
            if (colIndex == Mapping.PinColumn) colName = "PIN";
            else if (colIndex == Mapping.SerialNumberColumn) colName = "Serial";
            else if (colIndex == Mapping.ExpiryDateColumn) colName = "Expire";
            else if (colIndex == Mapping.VendorColumn) colName = "Vendor";
            else if (colIndex == Mapping.ProductColumn) colName = "Product";
            else if (colIndex == Mapping.PriceColumn) colName = "Price";
            else if (colIndex == Mapping.PriceAfterTaxColumn) colName = "PriceAfterTax";
            else if (colIndex == Mapping.ProductTypeColumn) colName = "ProductType";

            if (!string.IsNullOrEmpty(colName))
            {
              try
              {
                row[colName] = wsRow[rowNum, colIndex].Text;
              }
              catch (Exception ex)
              {
                //data type error
              }
            }
          }
          tbl.Rows.Add(row);
        }
      }

      Result.TotalRecords = tbl.Rows.Count;
      Result.Importing = true;
      var svc = ServiceLocator.Current.GetInstance<IProductManagementService>();
      try
      {
        svc.ImportProductList(tbl, serverTerminal.Id);
        MessageBox.Show("Import completed");
      }
      catch (Exception ex)
      {
        MessageBox.Show($"Failed to import\n{ex.Message}");
      }
    }
  }

  public class ExcelColumnDto : BindableBaseViewModel
  {
    private int _columnIndex;
    public int ColumnIndex { get { return _columnIndex; } set { SetProperty(ref _columnIndex, value); } }
    private string _columnName;
    public string ColumnName { get { return _columnName; } set { SetProperty(ref _columnName, value); } }
  }

  public class ColumnMapping : BindableBaseViewModel
  {
    private int _pinColumn;
    public int PinColumn { get { return _pinColumn; } set { SetProperty(ref _pinColumn, value); } }
    private int _serialNumberColumn;
    public int SerialNumberColumn { get { return _serialNumberColumn; } set { SetProperty(ref _serialNumberColumn, value); } }
    private int _expiryDateColumn;
    public int ExpiryDateColumn { get { return _expiryDateColumn; } set { SetProperty(ref _expiryDateColumn, value); } }
    private int _priceColumn;
    public int PriceColumn { get { return _priceColumn; } set { SetProperty(ref _priceColumn, value); } }
    private int _priceAfterTaxColumn;
    public int PriceAfterTaxColumn { get { return _priceAfterTaxColumn; } set { SetProperty(ref _priceAfterTaxColumn, value); } }
    private int _productColumn;
    public int ProductColumn { get { return _productColumn; } set { SetProperty(ref _productColumn, value); } }
    private int _vendorColumn;
    public int VendorColumn { get { return _vendorColumn; } set { SetProperty(ref _vendorColumn, value); } }
    private int _productTypeColumn;
    public int ProductTypeColumn { get { return _productTypeColumn; } set { SetProperty(ref _productTypeColumn, value); } }
  }

  public class ImportResult : BindableBaseViewModel
  {
    private int _totalRecords;
    public int TotalRecords { get { return _totalRecords; } set { SetProperty(ref _totalRecords, value); } }

    private int _imported;
    public int Imported { get { return _imported; } set { SetProperty(ref _imported, value); } }

    private int _failed;
    public int Failed { get { return _failed; } set { SetProperty(ref _failed, value); } }

    private bool _importing;
    public bool Importing { get { return _importing; } set { SetProperty(ref _importing, value); } }

  }
}
