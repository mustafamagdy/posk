using Geeky.POSK.DataContracts;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace POSK.Client.ViewModels
{
  class ReceiptGenerator
  {
    PrintDocument doc = null;
    public ReceiptGenerator()
    {

      doc = new PrintDocument();
      doc.PrintPage += Doc_PrintPage;
      doc.EndPrint += Doc_EndPrint;
    }

    private void Doc_EndPrint(object sender, PrintEventArgs e)
    {
      
    }

    private void Doc_PrintPage(object sender, PrintPageEventArgs e)
    {
      e.Graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
      
    }

    private Font MainFont = new Font("Verdana", 18);
    SizeF SizeString(Graphics g, string text, Font font)
    {
      return g.MeasureString(text, font);
    }

    PointF CenterItem(SizeF textSize, SizeF papgerSize)
    {
      int nLeft = Convert.ToInt32((papgerSize.Width / 2) - (textSize.Width / 2));
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
        return WhiteBitmap(200, 90);
      }

      using (var ms = new MemoryStream(image))
      {
        return Image.FromStream(ms);
      }
    }
    internal string GenerateReceipt(DecryptedPinDto pin, Guid sessionId, int width)
    {
      PrivateFontCollection barcodeeFonts = new PrivateFontCollection();
      //barcodeeFonts.AddFontFile(@"d:\barcode.ttf");
      int fontLength = Properties.Resources.barcode.Length;
      byte[] fontdata = Properties.Resources.barcode;
      System.IntPtr data = Marshal.AllocCoTaskMem(fontLength);
      Marshal.Copy(fontdata, 0, data, fontLength);
      barcodeeFonts.AddMemoryFont(data, fontLength);
      var families = barcodeeFonts.Families;

      Font barcodeFont = new Font(families[0], 26);

      Size paperSize = new Size(width, 2000);
      Brush white = new SolidBrush(Color.White);

      var printedLogo = pin.PrintedLogo;
      var filename = Path.Combine(Path.GetTempPath(), string.Format("{0}_{1}.jpeg", sessionId, pin.ProductCode));

      Bitmap bmp = new Bitmap(paperSize.Width, paperSize.Height);
      Image logo = GetImageFromByteArray(printedLogo);
      float yStart = 2F;
      using (Graphics graph = Graphics.FromImage(bmp))
      {
        graph.FillRectangle(white, 0, 0, paperSize.Width, paperSize.Height);
        graph.SmoothingMode = SmoothingMode.AntiAlias;
        graph.TextRenderingHint = TextRenderingHint.AntiAlias;

        yStart += DrawImage(graph, paperSize, logo, yStart);
        yStart += WriteText(graph, paperSize, $"{pin.ProductCode} SAR", 24, yStart);
        yStart += WriteText(graph, paperSize, $"{pin.PriceAfterTax} SAR", 24, yStart);
        yStart += WriteText(graph, paperSize, "5% شامل ضريبة القيمة المضافة", 24, yStart);
        yStart += WriteText(graph, paperSize, "5% VAT Included", 24, yStart);
        yStart += WriteText(graph, paperSize, "****************************", 24, yStart);
        yStart += WriteText(graph, paperSize, "Activation Number   رقم التفعيل", 24, yStart);
        yStart += WriteText(graph, paperSize, $"{pin.Pin}", 40, yStart);
        yStart += WriteText(graph, paperSize, "****************************", 24, yStart);
        yStart += WriteText(graph, paperSize, $"Expiry Date: {pin.ExpiryDate.ToString("dd-MM-yyyy")}", 24, yStart);
        yStart += WriteText(graph, paperSize, $"{pin.Pin}", 0, yStart, barcodeFont);
        yStart += WriteText(graph, paperSize, "Serial Number   الرقم التسلسلي", 18, yStart);
        yStart += WriteText(graph, paperSize, $"{pin.SerialNumber}", 32, yStart);
        yStart += WriteText(graph, paperSize, "-----------------------------------------", 24, yStart);
        if (pin.ProductInstructions != null && pin.ProductInstructions.Trim() != "")
        {
          yStart += WriteText(graph, paperSize, $@"{pin.ProductInstructions}", 18, yStart);
          yStart += WriteText(graph, paperSize, "-----------------------------------------", 24, yStart);
        }
        yStart += WriteText(graph, paperSize, $"Terminal ID: {pin.TerminalCode}", 18, yStart);
        yStart += WriteText(graph, paperSize, "Date: " + DateTime.Now.ToShortDateString(), 18, yStart);
        yStart += WriteText(graph, paperSize, "Time: " + DateTime.Now.ToShortTimeString(), 18, yStart);

        Rectangle cropRect = new Rectangle(0, 0, width, (int)yStart);
        bmp = bmp.Clone(cropRect, bmp.PixelFormat);
        bmp.Save(filename, ImageFormat.Jpeg);
      }
      bmp.Dispose();

      return filename;
    }
    /*
     Recharge via MySTC للشحن عبر تطبيق
Tap on recharge    اضغط على شحن
Tap on Camera icon اضغط على الكاميرا
Scan the Barcode امسح الباركود
Tap on recharge اضغط على شحن
Or enter او أدخل
From left to right  من اليسار الى اليمين
*155*Activation No# *155*رقم التفعيل#
For other ways لاعادة الشحن
Of recharge send   بطرق مختلفة أرسل
Recharge to   900  كلمة شحن الى
     
     */

  }
}
