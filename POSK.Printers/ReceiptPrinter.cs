using Geeky.POSK.DataContracts;
using Microsoft.PointOfService;
using POSK.Printers.Interface;
using System;
using System.Configuration;
using System.Linq;
using System.Windows;

namespace POSK.Printers
{
  public class ReceiptPrinter : IReceiptPrinter
  {
    private PosPrinter _printer;
    private PosExplorer _posExplorer;

    private bool _isOpened;
    private bool _isConfigured = false;
    public ReceiptPrinter()
    {
      _posExplorer = new PosExplorer();
      var printerinfo = _posExplorer.GetDevices(DeviceType.PosPrinter);
      if (printerinfo == null || printerinfo.Count == 0)
      {
        _isConfigured = false;
        return;
        //throw new Exception("Faild to get printers");
      }

      if (_printer == null)
      {
        var _device = GetFirstValidPrinter(printerinfo);
        if (_device == null) { _isConfigured = false; return; }
        _printer = _posExplorer.CreateInstance(_device) as PosPrinter;
        _isConfigured = true;
      }


    }

    private DeviceInfo GetFirstValidPrinter(DeviceCollection printers)
    {
      foreach (DeviceInfo device in printers)
      {
        if (device.HardwarePath != "" && !device.ServiceObjectName.Contains("Simulator"))
          return device;
      }

      return null;
    }

    public void Dispose()
    {
      _printer.Close();
    }
    private void Open()
    {
      if (!_isConfigured) return;
      try
      {
        _printer.Open();
        _isOpened = true;
      }
      catch (Exception ex)
      {
        throw new Exception("Faild to open printer, see inner exception", ex);
      }
    }

    public void Print(string receiptImageFileName, int width)
    {
      if (!_isConfigured) return;

      if (!_isOpened) Open();

      _printer.Claim(100);
      _printer.DeviceEnabled = true;
      _printer.CharacterSet = 857;
      _printer.MapMode = MapMode.Dots;

      _printer.PrintBitmap(PrinterStation.None, receiptImageFileName, width, PosPrinter.PrinterBitmapAsIs);

      _printer.CutPaper(100);
      _printer.Release();
      _printer.Close();
      _isOpened = false;
    }

    public void Print(DecryptedPinDto pin, Guid sessionId, int width)
    {
      throw new NotImplementedException();
    }

    public void Print(params PrinterLine[] lines)
    {
      throw new NotImplementedException();
    }

    public bool IsWorking()
    {
      //TODO
      return true;
    }
  }
}
