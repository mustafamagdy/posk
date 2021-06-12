using NUnit.Framework;
using POSK.Printers;
using POSK.Printers.Interface;
using System;
using System.Collections.Generic;

namespace Geeky.POSK.ClientTesting
{
  [TestFixture]
  public class ClientTests
  {
    [Test]
    public void test_custom_ceiling()
    {
      decimal val1 = 52.5m;
      decimal r = Math.Ceiling(val1 / 5.0m) * 5.0m;
      Assert.AreEqual(r, 55.0m);

      val1 = 105.0m;
      r = Math.Ceiling(val1 / 5.0m) * 5.0m;
      Assert.AreEqual(r, 105m);


      val1 = 73.5m;
      r = Math.Ceiling(val1 / 5.0m) * 5.0m;
      Assert.AreEqual(r, 75m);


      val1 = 31.5m;
      r = Math.Ceiling(val1 / 5.0m) * 5.0m;
      Assert.AreEqual(r, 35m);

    }

    [Test]
    public void test_printer()
    {
      IReceiptPrinter SessionEndPrinter = new SessionDocumentPrinter();

      var lines = new List<PrinterLine>();
      lines.Add(new PrinterLine($"Amount: {100} SAR"));
      lines.Add(new PrinterLine("********************************************"));
      lines.Add(new PrinterLine($"Ref Number: {12345678}"));
      lines.Add(new PrinterLine($"Trx Date: {DateTime.Now}"));
      SessionEndPrinter.Print(lines.ToArray());

    }
  }
}
