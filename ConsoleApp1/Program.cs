using POSK.Printers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
  class Program
  {
    static void Main(string[] args)
    {
      //var printer = new ReceiptPrinter();
      //printer.Print();
      //Console.ReadLine();

      PrintDocument doc = new PrintDocument();
      doc.PrintPage += Doc_PrintPage;

      doc.Print();
    }

    private static void Doc_PrintPage(object sender, PrintPageEventArgs e)
    {
      Font f = new Font("Tahoma", 24, FontStyle.Regular);
      var black = new SolidBrush(Color.Black);
      var pen = new Pen(black);
      e.Graphics.DrawRectangle(pen, 0, 0, 600, 800);
      //e.Graphics.DrawString("مرحبا بك", f, new SolidBrush(Color.Black), 2, 2);
    }
  }
}
