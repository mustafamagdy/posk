using Geeky.POSK.DataContracts;
using Microsoft.PointOfService;
using POSK.Printers.Interface;
using System;
using System.Threading;

namespace POSK.Printers
{
  public class FakeReceiptPrinter : IReceiptPrinter
  {
    Random r = new Random();
    public FakeReceiptPrinter()
    {
     
    }

    public void Dispose()
    {
     
    }

    public void Print(string receiptImageFileName, int width)
    {
      Thread.Sleep(r.Next(1000, 5000));
    }

    public void Print(DecryptedPinDto pin, Guid sessionId, int width)
    {
      //throw new NotImplementedException();
    }

    public void Print(params PrinterLine[] lines)
    {
      //throw new NotImplementedException();
    }

    public bool IsWorking()
    {
      //TODO
      return true;
    }
  }
}
