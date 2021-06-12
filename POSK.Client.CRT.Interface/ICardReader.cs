using POSK.Payment.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSK.Client.CRT.Interface
{
  public interface ICardReader : IPaymentMethod
  {
    CRTConnectionType ConnectionType { get; set; }
  }

  public class CRTCardReader : ICardReader
  {
    public bool IsConnected => _portHandler != 0;

    public string Code => "CARD_READER";
    public string Name => "ATM";
    public string ButtonImageName => "../Style/pAtm.png";
    public string DescriptionImageName => "../Style/dAtm.png";

    public CRTConnectionType ConnectionType { get; set; }
#if DEBUG
    public decimal _required { get; set; }
#endif
    public event AmountReceivedHandler AmountReceived;
    public event AmountReceivingHandler AmountReceiving;

    private string _commPortName;
    private UInt32 _portHandler;

    public CRTCardReader(CRTConnectionType connectionType, string commPortName = "COM1")
      : this()
    {
      ConnectionType = connectionType;
      _commPortName = commPortName;
    }
    public CRTCardReader()
    {
      //default to USB
      ConnectionType = CRTConnectionType.USB;

    }


    public int Disable()
    {
      throw new NotImplementedException();
    }

    public void Dispose()
    {
      throw new NotImplementedException();
    }

    public int Enable()
    {
      return 0;//temp
      UInt16 TxDataLen, RxDataLen;
      byte[] TxData = new byte[1024];
      byte[] RxData = new byte[1024];
      byte ReType = 0;
      byte St0, St1;

      //Cm = 0x51;
      //Pm = 0x31;
      //St0 = St1 = 0;
      //TxDataLen = 0;
      //RxDataLen = 0;

      //ExecuteCommand(CRTCommands.CPU_CARD_OP, )
    }

    public void Start()
    {
      try
      {
        if (ConnectionType == CRTConnectionType.USB)
        {
          _portHandler = CRTDLL.CRT288KUOpen();
        }
        else
        {
          if (string.IsNullOrEmpty(_commPortName))
            throw new ArgumentException("Comm port name not specified");

          _portHandler = CRTDLL.CRT288KROpen(_commPortName);
        }
      }
      catch (ArgumentException ex)
      {
        throw;
      }
      catch (Exception ex)
      {

      }
    }

    public void Stop()
    {
      if (!IsConnected) return;
      if (ConnectionType == CRTConnectionType.USB)
        CRTDLL.CRT288KUClose(_portHandler);
      else
        CRTDLL.CRT288KRClose(_portHandler);
    }

    private int ExecuteCommand(CRTCommands command, byte Pm, UInt16 TxDataLen, byte[] TxData,
                              ref byte ReType, ref byte St1, ref byte St0, ref UInt16 RxDataLen,
                              byte[] RxData)
    {
      var result = 0;

      if (ConnectionType == CRTConnectionType.USB)
        result = CRTDLL.USB_ExeCommand(_portHandler, (byte)command, Pm, TxDataLen, TxData, 
                                      ref ReType, ref St1, ref St0, ref RxDataLen, RxData);
      else
        result = CRTDLL.RS232_ExeCommand(_portHandler, (byte)command, Pm, TxDataLen, TxData, 
                                        ref ReType, ref St1, ref St0, ref RxDataLen, RxData);

      return result;
    }

    public int PowerUp()
    {
      return 0;
    }

    public int Connect()
    {
      return 0;
    }

    public bool IsWorking()
    {
      //TODO
      return false;
    }

    public void StopListening()
    {
      Stop();
    }
  }
}
