using AutoMapper;
using CommonServiceLocator;
using Geeky.POSK.DataContracts;
using Geeky.POSK.Filters;
using Geeky.POSK.Infrastructore.Core;
using Geeky.POSK.Repository.Interfface;
using Geeky.POSK.ServiceContracts;
using Geeky.POSK.Views;
using Geeky.POSK.WPF.Core.Base;
using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.Practices.Prism.Commands;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using PdfSharp.Drawing;
using PdfSharp.Forms;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using VAlign = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment;

namespace Geeky.POSK.Server.ViewModels
{
  public class SalesReportViewModel : BindableBaseViewModel
  {
    private SalesReportFilterCriteria _filter;
    public SalesReportFilterCriteria Filter { get { return _filter; } set { SetProperty(ref _filter, value); } }

    private ObservableCollection<VendorDto> _vendors;
    public ObservableCollection<VendorDto> Vendors { get { return _vendors; } set { SetProperty(ref _vendors, value); } }

    public DelegateCommand ShowReport { get; private set; }

    public SalesReportViewModel()
    {
      ShowReport = new DelegateCommand(OnShowReport);
    }
    public void LoadData()
    {
      var vendorRepo = ServiceLocator.Current.GetInstance<IVendorRepository>();
      var listVendors = vendorRepo.FindAll().AsQueryable().ProjectToArray<VendorDto>();
      Vendors = new ObservableCollection<VendorDto>(listVendors);
      Filter = new SalesReportFilterCriteria();
    }

    private void OnShowReport()
    {
      UpdateSalesReport();
    }

    public void UpdateSalesReport()
    {
      var service = ServiceLocator.Current.GetInstance<IStatisticsService>();
      var result = service.SalesReport(Filter);

      var data = result.ToList();
      var products = data.Select(x => x.ProductCode).Distinct().ToList();
      var grpTerminal = data.GroupBy(x => x.TerminalCode);

      var vendorCode = Filter.Vendor.Code;
      var reportTitle = $"Sales report (for {vendorCode})";
      var filterText = $"From {Filter.FromDate?.ToShortDateString()} to {Filter.ToDate?.ToShortDateString()}";

      var document = CreateDocument(reportTitle, filterText);

      Paragraph paragraph = document.LastSection.AddParagraph("Terminal Sales", "Heading1");
      Table table = document.LastSection.AddTable();
      table.Borders.Visible = true;
      table.TopPadding = 5;
      table.BottomPadding = 7;

      Column colTerminal = table.AddColumn();
      colTerminal.Width = 80;
      colTerminal.Format.Alignment = ParagraphAlignment.Center;

      foreach (var product in products)
      {
        Column colSold = table.AddColumn();
        Column colRemaining = table.AddColumn();

        colSold.Width = 50;
        colRemaining.Width = 50;

        colSold.Format.Alignment = ParagraphAlignment.Center;
        colRemaining.Format.Alignment = ParagraphAlignment.Center;
      }

      Column colTerminalTotal = table.AddColumn();
      colTerminalTotal.Width = 70;
      colTerminalTotal.Format.Alignment = ParagraphAlignment.Center;

      table.Rows.Height = 25;

      Row row = table.AddRow();
      row.Cells[0].AddParagraph("Terminal");
      var colIndex = 1;
      foreach (var product in products)
      {
        row.Cells[colIndex].AddParagraph(product);
        row.Cells[colIndex + 1].AddParagraph("");
        row.Cells[colIndex].MergeRight = 1;
        row.Cells[colIndex].Format.Alignment = ParagraphAlignment.Center;

        colIndex += 2;
      }

      row.Cells[colIndex].AddParagraph("Total");

      row = table.AddRow();
      row.Cells[0].AddParagraph("");
      colIndex = 1;
      foreach (var product in products)
      {
        row.Cells[colIndex].AddParagraph("Sold");
        row.Cells[colIndex + 1].AddParagraph("Remain.");
        row.Cells[colIndex].Format.Alignment = ParagraphAlignment.Center;
        row.Cells[colIndex + 1].Format.Alignment = ParagraphAlignment.Center;

        colIndex += 2;
      }

      row.Cells[colIndex].AddParagraph("");//total

      table.Rows[0].Cells[0].MergeDown = 1;//terminal
      table.Rows[0].Cells[0].VerticalAlignment = VAlign.Center;

      table.Rows[0].Cells[colIndex].MergeDown = 1;//total
      table.Rows[0].Cells[colIndex].VerticalAlignment = VAlign.Center;

      //data
      var grandTotalSold = 0.0M;
      foreach (var tGroup in grpTerminal)
      {
        row = table.AddRow();
        row.Cells[0].AddParagraph(tGroup.Key);

        colIndex = 1;
        var totalSold = 0.0M;
        foreach (var product in products)
        {
          var terminalSales = tGroup.Where(x => x.ProductCode == product).FirstOrDefault();
          int sold = 0, reminaing = 0;
          if (terminalSales != null)
          {
            sold = terminalSales.SoldCount;
            reminaing = terminalSales.Remaining;
            totalSold += terminalSales.SoldCount * terminalSales.Price;
          }

          row.Cells[colIndex].AddParagraph(sold.ToString());
          row.Cells[colIndex + 1].AddParagraph(reminaing.ToString());
          row.Cells[colIndex].Format.Alignment = ParagraphAlignment.Right;
          row.Cells[colIndex + 1].Format.Alignment = ParagraphAlignment.Right;

          colIndex += 2;
        }

        grandTotalSold += totalSold;
        row.Cells[colIndex].AddParagraph(totalSold.ToString());//total
      }

      row = table.AddRow();
      row.Cells[0].AddParagraph("Total");
      var columns = products.Count() * 2;//each one has sold, and rermiaing col;     
      row.Cells[0].Format.Alignment = ParagraphAlignment.Center;
      row.Cells[0].VerticalAlignment = VAlign.Center;

      for (int i = 1; i < columns; i++)
      {
        row.Cells[i].AddParagraph("");
      }
      row.Cells[0].MergeRight = columns;
      var totalCell = table.Rows[table.Rows.Count - 1]
                           .Cells[table.Rows[0].Cells.Count - 1]
                           .AddParagraph(grandTotalSold.ToString());

      MigraDoc.DocumentObjectModel.IO.DdlWriter.WriteToFile(document, "Sales_report.pdf");
      PdfDocumentRenderer renderer = new PdfDocumentRenderer(true, PdfFontEmbedding.Always);
      renderer.Document = document;
      renderer.RenderDocument();

      // Save the document...
      string filename = "HelloMigraDoc.pdf";
      renderer.PdfDocument.Save(filename);
      // ...and start a viewer.
      Process.Start(filename);
    }

    public static Document CreateDocument(string title, string filter)
    {
      // Create a new MigraDoc document
      Document document = new Document();
      document.DefaultPageSetup.Orientation = MigraDoc.DocumentObjectModel.Orientation.Landscape;

      DefineStyles(document);
      SetupDocument(document, title);

      Paragraph paragraph = document.LastSection.AddParagraph(title, "Heading1");
      paragraph.Format.Borders.Width = 2.5;
      paragraph.Format.Borders.Color = Colors.Navy;
      paragraph.Format.Borders.Distance = 3;
      

      paragraph = document.LastSection.AddParagraph(filter, "Heading1");
      paragraph.Format.Borders.Width = 2.5;
      paragraph.Format.Borders.Color = Colors.Navy;
      paragraph.Format.Borders.Distance = 3;


      return document;
    }

    static void DefineStyles(Document document)
    {
      XPdfFontOptions options = new XPdfFontOptions(PdfFontEncoding.Unicode, PdfFontEmbedding.Always);

      MigraDoc.DocumentObjectModel.Style style = document.Styles["Normal"];
      style.Font.Name = "Tahoma";


      style = document.Styles["Heading1"];
      style.Font.Name = "Tahoma";
      style.Font.Size = 14;
      style.Font.Bold = true;
      style.Font.Color = Colors.Black;
      style.ParagraphFormat.SpaceAfter = 6;

      style = document.Styles["Heading2"];
      style.Font.Size = 12;
      style.Font.Bold = true;
      style.ParagraphFormat.SpaceAfter = 6;

      style = document.Styles["Heading3"];
      style.Font.Size = 10;
      style.Font.Bold = false;
      style.Font.Italic = false;
      style.ParagraphFormat.SpaceAfter = 3;

      style = document.Styles[StyleNames.Header];
      style.ParagraphFormat.AddTabStop("16cm", TabAlignment.Right);

      style = document.Styles[StyleNames.Footer];
      style.ParagraphFormat.AddTabStop("8cm", TabAlignment.Center);
    }

    static void SetupDocument(Document document, string pageHeaderText)
    {
      Section section = document.AddSection();
      section.PageSetup.OddAndEvenPagesHeaderFooter = true;
      section.PageSetup.StartingNumber = 1;

      HeaderFooter header = section.Headers.Primary;
      header.AddParagraph($"\t{pageHeaderText}");

      header = section.Headers.EvenPage;
      header.AddParagraph($"{pageHeaderText}");

      Paragraph paragraph = new Paragraph();
      paragraph.AddTab();
      paragraph.AddPageField();

      section.Footers.Primary.Add(paragraph);
      section.Footers.EvenPage.Add(paragraph.Clone());
    }
  }
}