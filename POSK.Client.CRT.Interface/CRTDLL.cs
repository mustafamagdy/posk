using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace POSK.Client.CRT.Interface
{
  public class CRTDLL
  {
    /// <summary>
    /// Open serial port
    /// </summary>
    /// <param name="port">Com port name</param>
    /// <returns></returns>
    [DllImport("CRT_288_K001.dll")]
    public static extern UInt32 CRT288KROpen(string port);

    /// <summary>
    /// Open the serial port at the specified baud rate
    /// </summary>
    /// <param name="port">Comm port name</param>
    /// <param name="Baudrate">Baud ratte</param>
    /// <returns></returns>
    [DllImport("CRT_288_K001.dll")]
    public static extern long CRT288KROpenWithBaut(string port, UInt32 Baudrate);
    /// <summary>
    /// Close serial port
    /// </summary>
    /// <param name="ComHandle">Port handle got from <see cref="CRT288KROpen(string)"/></param>
    /// <returns></returns>
    [DllImport("CRT_288_K001.dll")]
    public static extern int CRT288KRClose(UInt32 ComHandle);

    /// <summary>
    /// Execute command on serial portss
    /// </summary>
    /// <param name="ComHandle">Port handle got from <see cref="CRT288KROpen(string)"/></param>
    /// <param name="TxCmCode"></param>
    /// <param name="TxPmCode"></param>
    /// <param name="TxDataLen"></param>
    /// <param name="TxData"></param>
    /// <param name="RxReplyType"></param>
    /// <param name="RxStCode1"></param>
    /// <param name="RxStCode0"></param>
    /// <param name="RxDataLen"></param>
    /// <param name="RxData"></param>
    /// <returns></returns>
    [DllImport("CRT_288_K001.dll")]
    public static extern int RS232_ExeCommand(UInt32 ComHandle, byte TxCmCode, byte TxPmCode, UInt16 TxDataLen, byte[] TxData, ref byte RxReplyType, ref byte RxStCode1, ref byte RxStCode0, ref UInt16 RxDataLen, byte[] RxData);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ComHandle">Port handle got from <see cref="CRT288KROpen(string)"/></param>
    /// <returns></returns>
    [DllImport("CRT_288_K001.dll")]
    public static extern int RS232_Cancel_UpTrackData(UInt32 ComHandle);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ComHandle">Port handle got from <see cref="CRT288KROpen(string)"/></param>
    /// <param name="tracks"></param>
    /// <param name="ReadMode"></param>
    /// <param name="_WaitTime"></param>
    /// <param name="RxReplyType"></param>
    /// <param name="RxDataLen"></param>
    /// <param name="RxData"></param>
    /// <returns></returns>
    [DllImport("CRT_288_K001.dll")]
    public static extern int RS232_UpTrackData(UInt32 ComHandle, byte tracks, byte ReadMode, byte _WaitTime, ref byte RxReplyType, ref UInt16 RxDataLen, byte[] RxData);


    /// <summary>
    /// Open connection using USB
    /// </summary>
    /// <returns></returns>
    [DllImport("CRT_288_K001.dll")]
    public static extern UInt32 CRT288KUOpen();
    
    /// <summary>
    /// Close connection to USB
    /// </summary>
    /// <param name="ComHandle">Port handle you got from <see cref="CRT288KROpen(string)"/></param>
    /// <returns></returns>
    [DllImport("CRT_288_K001.dll")]
    public static extern int CRT288KUClose(UInt32 ComHandle);

    /// <summary>
    /// Execute command on USB port
    /// </summary>
    /// <param name="ComHandle">Port handle you got from <see cref="CRT288KROpen(string)"/></param>
    /// <param name="TxCmCode"></param>
    /// <param name="TxPmCode"></param>
    /// <param name="TxDataLen"></param>
    /// <param name="TxData"></param>
    /// <param name="RxReplyType"></param>
    /// <param name="RxStCode1"></param>
    /// <param name="RxStCode0"></param>
    /// <param name="RxDataLen"></param>
    /// <param name="RxData"></param>
    /// <returns></returns>
    [DllImport("CRT_288_K001.dll")]
    public static extern int USB_ExeCommand(UInt32 ComHandle, byte TxCmCode, byte TxPmCode, UInt16 TxDataLen, byte[] TxData, ref byte RxReplyType, ref byte RxStCode1, ref byte RxStCode0, ref UInt16 RxDataLen, byte[] RxData);

    /// <summary>
    /// Cancel UpTrack Data
    /// </summary>
    /// <param name="ComHandle">Port handle you got from <see cref="CRT288KROpen(string)"/></param>
    /// <returns></returns>
    [DllImport("CRT_288_K001.dll")]
    public static extern int USB_Cancel_UpTrackData(UInt32 ComHandle);

    /// <summary>
    /// UpTrackData 
    /// </summary>
    /// <param name="ComHandle">Port handle you got from <see cref="CRT288KROpen(string)"/></param>
    /// <param name="tracks"></param>
    /// <param name="ReadMode"></param>
    /// <param name="_WaitTime"></param>
    /// <param name="RxReplyType"></param>
    /// <param name="RxDataLen"></param>
    /// <param name="RxData"></param>
    /// <returns></returns>
    [DllImport("CRT_288_K001.dll")]
    public static extern int USB_UpTrackData(UInt32 ComHandle, byte tracks, byte ReadMode, byte _WaitTime, ref byte RxReplyType, ref UInt16 RxDataLen, byte[] RxData);

  }
}
