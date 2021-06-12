using Geeky.POSK.DataContracts;
using Microsoft.PointOfService;
using POSK.Printers.Interface;
using System;
using System.Configuration;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;

namespace POSK.Printers
{
  public class SessionDocumentPrinter : IReceiptPrinter
  {
    private int width = 0;
    PrintDocument doc = null;
    PrinterLine[] lines;
    public SessionDocumentPrinter()
    {
      PrintController printController = new StandardPrintController();
      doc = new PrintDocument();
      doc.PrintController = printController;
      doc.PrintPage += Doc_PrintPage;
      doc.EndPrint += Doc_EndPrint;
    }

    private void Doc_EndPrint(object sender, PrintEventArgs e)
    {
    }

    public void Dispose()
    {
      doc.Dispose();
    }

    public void Print(DecryptedPinDto pin, Guid sessionId, int width)
    {
      this.width = width;
      doc.Print();
    }

    private Font MainFont = new Font("Verdana", 18);
    SizeF SizeString(Graphics g, string text, Font font)
    {
      return g.MeasureString(text, font);
    }

    PointF CenterItem(SizeF textSize, SizeF papgerSize)
    {
      int fixToOriginalMargin = 30;
      int nLeft = Convert.ToInt32((papgerSize.Width / 2) - (textSize.Width / 2)) + fixToOriginalMargin;
      int nTop = Convert.ToInt32((papgerSize.Height / 2) - (textSize.Height / 2));
      return new PointF(nLeft, nTop);
    }

    float WriteText(Graphics g, SizeF papgerSize, string text, float fontSize, float yAxis, Font font = null)
    {
      var _font = font == null ? new Font(MainFont.Name, fontSize) : font;
      var textSize = SizeString(g, text, _font);
      var position = CenterItem(textSize, papgerSize);
      g.DrawString(text, _font, SystemBrushes.WindowText, position.X, yAxis);
      return textSize.Height;
    }

    float DrawImage(Graphics g, SizeF papgerSize, Image image, float yAxis)
    {
      var position = CenterItem(image.Size, papgerSize);
      g.DrawImage(image, position.X, yAxis);
      return image.Height;
    }

    private Bitmap WhiteBitmap(int width, int height)
    {
      Brush white = new SolidBrush(Color.White);
      Bitmap bmp = new Bitmap(width, height);
      using (Graphics graph = Graphics.FromImage(bmp))
      {
        graph.FillRectangle(white, 0, 0, width, height);
      }
      return bmp;
    }

    private Image GetImageFromByteArray(byte[] image)
    {
      if (image.Length == 0)
      {
        return WhiteBitmap(100, 50);
      }

      using (var ms = new MemoryStream(image))
      {
        var img = Image.FromStream(ms) as Bitmap;
        img = new Bitmap(img, new Size(100, 50));
        return img;
      }
    }

    private void Doc_PrintPage(object sender, PrintPageEventArgs e)
    {
      Margins margins = new Margins(0, 2, 2, 2);
      doc.DefaultPageSettings.Margins = margins;
      var graph = e.Graphics;

      Size paperSize = new Size(200, 300);
      Brush white = new SolidBrush(Color.White);
      Pen blackPen = new Pen(new SolidBrush(Color.Black));

      float yStart = 2F;
      //for test
      //graph.DrawRectangle(blackPen, 0, 0, width, 500);
      graph.SmoothingMode = SmoothingMode.AntiAlias;
      graph.TextRenderingHint = TextRenderingHint.AntiAlias;
      foreach (var line in lines)
      {
        yStart += WriteText(graph, paperSize, line.Text, line.FontSize, yStart);
      }

      yStart += WriteText(graph, paperSize, "Date: " + DateTime.Now.ToShortDateString(), 8, yStart);
      yStart += WriteText(graph, paperSize, "Time: " + DateTime.Now.ToShortTimeString(), 8, yStart);
    }

    public void Print(string receiptImageFileName, int width)
    {
      throw new NotImplementedException();
    }

    public void Print(params PrinterLine[] lines)
    {
      this.lines = lines;
      this.doc.Print();
    }

    public bool IsWorking()
    {
      //TODO
      return true;
    }
  }
}
