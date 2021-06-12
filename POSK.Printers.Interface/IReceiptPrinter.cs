using Geeky.POSK.DataContracts;
using System;

namespace POSK.Printers.Interface
{
  public interface IReceiptPrinter : IDisposable
  {
    void Print(string receiptImageFileName, int width);
    void Print(DecryptedPinDto pin, Guid sessionId, int width);
    void Print(params PrinterLine[] lines);

    /// <summary>
    /// This will be called to check if this printer is working 
    /// to display a notification to the user to tell him it is not
    /// working if it is not
    /// </summary>
    /// <returns>true if working, false if else</returns>
    bool IsWorking();
  }

  public class PrinterLine
  {
    public PrinterLine(string text, int fontSize = 10)
    {
      this.Text = text;
      this.FontSize = fontSize;
    }
    public string Text { get; set; }
    public int FontSize { get; set; }
  }
}
