
using NLog;
using POSK.Client.CashCode.Interface;
using POSK.Payment.Interface;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace POSK.Client.CashCode
{
  public sealed class CashCodeBillValidator : IDisposable, ICashCodeBillValidator
  {

    public string Code => "BILL_VALIDATOR";
    public string Name => "CASH";
    public string ButtonImageName => "../Style/pCash.png";
    public string DescriptionImageName => "../Style/dCash.png";


    private const int POLL_TIMEOUT = 200;    // Timeout for waiting for a response from the reader
    private const int EVENT_WAIT_HANDLER_TIMEOUT = 2000; //10000; // Timeout waiting to unlock

    private byte[] ENABLE_BILL_TYPES_WITH_ESCROW = new byte[6] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };

    private EventWaitHandle _SynchCom;     // Variable synchronization of sending and reading data from the com port
    private List<byte> _ReceivedBytes;  // Received Bytes

    private string _ComPortName;
    private int _BaudRate;

    private int _LastError;
    private bool _Disposed;
    private bool _IsConnected;
    private bool _IsPowerUp;
    private bool _IsListening;
    private bool _IsEnableBills;
    private object _Locker;

    private SerialPort _ComPort;
    private CashCodeErroList _ErrorList;

    private System.Timers.Timer _Listener;  // Banner Receiver Timer

    bool _ReturnBill;
    bool _DisableBillValidator;

    BillCassetteStatus _cassettestatus = BillCassetteStatus.Inplace;

    /// <summary>
    /// Event of receiving a bill
    /// </summary>
    public event AmountReceivedHandler AmountReceived;

    /// <summary>
    /// Event of process of sending a note on the stack (Here it is possible to do a refund)
    /// </summary>
    public event AmountReceivingHandler AmountReceiving;

    /// <summary>
    /// Event of bill cassete
    /// </summary>
    public event BillCassetteHandler BillCassetteStatusEvent;

    /// <summary>
    /// Event of bill validator failure
    /// </summary>
    public event BillValidatorFailureHandler BillFailure;

    /// <summary>
    /// Event for end of session
    /// </summary>
    public event EndOfSessionHandler EndOfSession;

    /// <summary>
    /// Event for raise error to the user
    /// </summary>
    public event RaiseToClienthandler RaiseToClient;

    /// <summary>
    /// Log to client anything
    /// </summary>
    public event LogToClientHandler LogToClient;

    /// <summary>
    /// Get connection status
    /// </summary>
    public bool IsConnected
    {
      get { return _IsConnected; }
    }

#if DEBUG
    public decimal _required { get; set; }
#endif

    public CashCodeBillValidator()
      : this("", 9600)
    {

    }

    public CashCodeBillValidator(string PortName, int BaudRate)
    {
      if (PortName.Trim() == "")
      {
        var appKey = "CashCode_COM";
        PortName = ConfigurationManager.AppSettings.AllKeys.Contains(appKey) ?
        ConfigurationManager.AppSettings[appKey] : "COM1";
      }

      this._ErrorList = new CashCodeErroList();

      this._Disposed = false;
      this._IsEnableBills = false;
      this._ComPortName = "";
      this._Locker = new object();
      this._IsConnected = this._IsPowerUp = this._IsListening = this._ReturnBill = this._DisableBillValidator = false;

      // From the specification:
      // Baud Rate: 9600 bps / 19200 bps (no negotiation, hardware selectable)
      // Start bit: 1
      // Data bit: 8 (bit 0 = LSB, bit 0 sent first)
      // Parity: Parity none
      // Stop bit: 1
      this._ComPort = new SerialPort();
      this._ComPort.PortName = this._ComPortName = PortName;
      this._ComPort.BaudRate = this._BaudRate = BaudRate;
      this._ComPort.DataBits = 8;
      this._ComPort.Parity = Parity.None;
      this._ComPort.StopBits = StopBits.One;
      this._ComPort.DataReceived += new SerialDataReceivedEventHandler(_ComPort_DataReceived);

      this._ReceivedBytes = new List<byte>();
      this._SynchCom = new EventWaitHandle(false, EventResetMode.AutoReset);

      this._Listener = new System.Timers.Timer();
      this._Listener.Interval = POLL_TIMEOUT;
      this._Listener.Enabled = false;
      this._Listener.Elapsed += new System.Timers.ElapsedEventHandler(_Listener_Elapsed);
    }

    /// <summary>
    // Begin listening to the receipt
    /// </ summary>
    public void Start()
    {
      //MessageBox.Show("Starting");
      // If not connected
      if (!this._IsConnected)
      {
        //MessageBox.Show("Not connected");

        this._LastError = 100020;
        throw new System.IO.IOException(this._ErrorList.Errors[this._LastError]);
      }

      // If there is no energy, then we turn on
      if (!this._IsPowerUp) { this.PowerUp(); }

      if (!this._IsListening)
      {
        this._IsListening = true;

        if (!this._IsEnableBills) { this.Enable(); }

        this._Listener.Start();
      }

      //MessageBox.Show("Ready");

    }

    /// <summary>
    /// Stop listening to the bill acceptor
    /// </ summary>
    public void Stop()
    {
      this._IsListening = false;
      this._Listener.Stop();
      this.Disable();

      //I want to check if it is powered or not, to not reset it every time
      if (_IsPowerUp)
        this.Reset();
    }

    /// <summary>
    /// same as Stop but dont reset the cash coder
    /// </ summary>
    public void StopListening()
    {
      this._IsListening = false;
      this._Listener.Stop();
      this.Disable();
    }

    /// <summary>
    /// Opening a COM port for working with a bill acceptor
    /// </summary>
    /// <returns>Error code to be checked in <see cref="CashCodeErroList"/></returns>
    public int Connect()
    {
      try
      {
        this._ComPort.Open();
        this._IsConnected = true;

        OnLogToClient(LogEventArgs.Info($"Com port {_ComPort} opened OK"));
      }
      catch (Exception ex)
      {
        OnLogToClient(LogEventArgs.Error($"Failed to open com port {_ComPort} -> {ex.Message}", ex.StackTrace));

        this._IsConnected = false;
        this._LastError = 100010;
      }

      return this._LastError;
    }

    /// <summary>
    ///  Enabling the bill acceptor
    /// </summary>
    /// <returns></returns>
    public int PowerUp()
    {
      List<byte> ByteResult = null;

      // If the com port is not open
      if (!this._IsConnected)
      {
        this._LastError = 100020;
        throw new System.IO.IOException(this._ErrorList.Errors[this._LastError]);
      }

      // POWER UP
      ByteResult = this.SendCommand(BillValidatorCommands.POLL).ToList();

      // Check the result
      if (CheckPollOnError(ByteResult.ToArray()))
      {
        this.SendCommand(BillValidatorCommands.NAK);
        throw new System.ArgumentException(this._ErrorList.Errors[this._LastError]);
      }

      // Otherwise, send a confirmation tone
      this.SendCommand(BillValidatorCommands.ACK);

      // RESET
      ByteResult = this.SendCommand(BillValidatorCommands.RESET).ToList();

      // If you did not receive an ACK signal from the bill acceptor
      if (ByteResult[3] != 0x00)
      {
        this._LastError = 100050;
        return this._LastError;
      }

      // INITIALIZE
      // Then we again question the bill acceptor
      ByteResult = this.SendCommand(BillValidatorCommands.POLL).ToList();

      if (CheckPollOnError(ByteResult.ToArray()))
      {
        this.SendCommand(BillValidatorCommands.NAK);
        throw new System.ArgumentException(this._ErrorList.Errors[this._LastError]);
      }

      // Otherwise, send a confirmation tone
      this.SendCommand(BillValidatorCommands.ACK);

      // GET STATUS
      ByteResult = this.SendCommand(BillValidatorCommands.GET_STATUS).ToList();

      // The GET STATUS command returns 6 bytes of the response. 
      //If all are equal to 0, then the status is ok and you can work further, otherwise the error
      if (ByteResult[3] != 0x00 || ByteResult[4] != 0x00 || ByteResult[5] != 0x00 ||
          ByteResult[6] != 0x00 || ByteResult[7] != 0x00 || ByteResult[8] != 0x00)
      {
        this._LastError = 100070;
        //raise custom event
        RaiseCustomEvent(ByteResult.ToArray());
        //throw new System.ArgumentException(this._ErrorList.Errors[this._LastError]);
        return this._LastError;
      }

      this.SendCommand(BillValidatorCommands.ACK);

      // SET_SECURITY (in the test case, it sends 3 bytes (0 0 0)
      ByteResult = this.SendCommand(BillValidatorCommands.SET_SECURITY, new byte[3] { 0x00, 0x00, 0x00 }).ToList();

      // If you did not receive an ACK signal from the bill acceptor
      if (ByteResult[3] != 0x00)
      {
        this._LastError = 100050;
        return this._LastError;
      }

      // IDENTIFICATION
      ByteResult = this.SendCommand(BillValidatorCommands.IDENTIFICATION).ToList();
      this.SendCommand(BillValidatorCommands.ACK);


      // POLL
      // Then we again question the bill acceptor. Should receive the INITIALIZE command
      ByteResult = this.SendCommand(BillValidatorCommands.POLL).ToList();

      // Let's check the result
      if (CheckPollOnError(ByteResult.ToArray()))
      {
        this.SendCommand(BillValidatorCommands.NAK);

        //raise custom event
        RaiseCustomEvent(ByteResult.ToArray());
        //throw new System.ArgumentException(this._ErrorList.Errors[this._LastError]);
        return this._LastError;
      }

      // Otherwise, send a confirmation tone
      this.SendCommand(BillValidatorCommands.ACK);

      // POLL
      // Then we again question the bill acceptor. Should get the UNIT DISABLE command
      ByteResult = this.SendCommand(BillValidatorCommands.POLL).ToList();

      // Let's check the result
      if (CheckPollOnError(ByteResult.ToArray()))
      {
        this.SendCommand(BillValidatorCommands.NAK);

        //raise custom event
        RaiseCustomEvent(ByteResult.ToArray());
        //throw new System.ArgumentException(this._ErrorList.Errors[this._LastError]);
        return this._LastError;
      }

      // Otherwise, send a confirmation tone
      this.SendCommand(BillValidatorCommands.ACK);

      this._IsPowerUp = true;

      return this._LastError;
    }

    private int Reset()
    {
      List<byte> ByteResult = null;

      // If the com port is not open
      if (!this._IsConnected)
      {
        return this._LastError;
      }
      // RESET
      ByteResult = this.SendCommand(BillValidatorCommands.RESET).ToList();

      // If you did not receive an ACK signal from the bill acceptor
      if (ByteResult[3] != 0x00)
      {
        this._LastError = 100050;
        return this._LastError;
      }

      // INITIALIZE
      // Then we again question the bill acceptor
      ByteResult = this.SendCommand(BillValidatorCommands.POLL).ToList();

      if (CheckPollOnError(ByteResult.ToArray()))
      {
        this.SendCommand(BillValidatorCommands.NAK);
        throw new System.ArgumentException(this._ErrorList.Errors[this._LastError]);
      }

      // Otherwise, send a confirmation tone
      this.SendCommand(BillValidatorCommands.ACK);

      // GET STATUS
      ByteResult = this.SendCommand(BillValidatorCommands.GET_STATUS).ToList();

      // The GET STATUS command returns 6 bytes of the response. 
      //If all are equal to 0, then the status is ok and you can work further, otherwise the error
      if (ByteResult[3] != 0x00 || ByteResult[4] != 0x00 || ByteResult[5] != 0x00 ||
          ByteResult[6] != 0x00 || ByteResult[7] != 0x00 || ByteResult[8] != 0x00)
      {
        this._LastError = 100070;
        //raise custom event
        RaiseCustomEvent(ByteResult.ToArray());
        //throw new System.ArgumentException(this._ErrorList.Errors[this._LastError]);
        return this._LastError;
      }

      this.SendCommand(BillValidatorCommands.ACK);

      // SET_SECURITY (in the test case, it sends 3 bytes (0 0 0)
      ByteResult = this.SendCommand(BillValidatorCommands.SET_SECURITY, new byte[3] { 0x00, 0x00, 0x00 }).ToList();

      // If you did not receive an ACK signal from the bill acceptor
      if (ByteResult[3] != 0x00)
      {
        this._LastError = 100050;
        return this._LastError;
      }

      // IDENTIFICATION
      ByteResult = this.SendCommand(BillValidatorCommands.IDENTIFICATION).ToList();
      this.SendCommand(BillValidatorCommands.ACK);

      return this._LastError;
    }

    private void RaiseCustomEvent(byte[] ByteResult)
    {
      if (_ErrorList.FailureErrors.Contains(this._LastError))
      {
        // Raise event that bill validator has failure
        OnBillFailure(new BillFailureEventArgs(this._LastError, ByteResult[3], ByteResult[4], this._ErrorList.Errors[this._LastError]));
      }
      if (_ErrorList.EndOfSessionErrors.Contains(this._LastError))
      {
        // Raise event that will end client session
        OnEndOfSession(new EndOfSessionEventArgs(this._LastError, this._ErrorList.Errors[this._LastError]));
      }
      if (_ErrorList.RaisedTouserErrors.Contains(this._LastError))
      {
        // Raise event that will show error message to client
        OnRaiseToClient(new RaisedDetailsEventArgs(this._LastError, this._ErrorList.Errors[this._LastError]));
      }
    }

    /// <summary>
    /// Enabling the mode of accepting bills
    /// </summary>
    /// <returns></returns>
    public int Enable()
    {
      List<byte> ByteResult = null;

      // If the com port is not open
      if (!this._IsConnected)
      {
        this._LastError = 100020;
        throw new System.IO.IOException(this._ErrorList.Errors[this._LastError]);
      }

      try
      {
        if (!_IsListening)
        {
          throw new InvalidOperationException("Error in the method for enabling the receipt of notes. You must call the StartListening method.");
        }

        lock (_Locker)
        {
          _IsEnableBills = true;

          // omit the ENABLE BILL TYPES command (in the test case sends 6 bytes (255 255 255 0 0 0) Hold function enabled (Escrow)
          ByteResult = this.SendCommand(BillValidatorCommands.ENABLE_BILL_TYPES, ENABLE_BILL_TYPES_WITH_ESCROW).ToList();

          // If you did not receive an ACK signal from the bill acceptor
          if (ByteResult[3] != 0x00)
          {
            this._LastError = 100050;
            throw new System.ArgumentException(this._ErrorList.Errors[this._LastError]);
          }

          // Then we again question the bill acceptor
          ByteResult = this.SendCommand(BillValidatorCommands.POLL).ToList();

          // Let's check the result
          if (CheckPollOnError(ByteResult.ToArray()))
          {
            this.SendCommand(BillValidatorCommands.NAK);
            throw new System.ArgumentException(this._ErrorList.Errors[this._LastError]);
          }

          // Otherwise, send a confirmation tone
          this.SendCommand(BillValidatorCommands.ACK);
        }
      }
      catch (Exception ex)
      {
        this._LastError = 100030;
        OnLogToClient(LogEventArgs.Error($"Error enabling cashcode -> {ex.Message}", ex.StackTrace));
      }

      return this._LastError;
    }

    /// <summary>
    /// Turn off the reception mode of notes
    /// </summary>
    /// <returns></returns>
    public int Disable()
    {
      List<byte> ByteResult = null;

      lock (_Locker)
      {
        // If the com port is not open
        if (!this._IsConnected)
        {
          //if it is not connected we shouldn't do anything...
          return this._LastError;
          //this._LastError = 100020;
          //throw new System.IO.IOException(this._ErrorList.Errors[this._LastError]);
        }

        _IsEnableBills = false;

        // send the ENABLE BILL TYPES command (in the test case, it sends 6 bytes (0 0 0 0 0 0)
        ByteResult = this.SendCommand(BillValidatorCommands.ENABLE_BILL_TYPES, new byte[6] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }).ToList();
      }

      // If you did not receive an ACK signal from the bill acceptor
      if (ByteResult[3] != 0x00)
      {
        this._LastError = 100050;
        return this._LastError;
      }

      return this._LastError;
    }

    /// <summary>
    /// Check poll result if error or not
    /// </summary>
    /// <param name="ByteResult"></param>
    /// <returns>True if there is an error</returns>
    private bool CheckPollOnError(byte[] ByteResult)
    {
      bool IsError = false;

      // If not received from the bill acceptor the third byte equal to 30N (ILLEGAL COMMAND)
      if (ByteResult[3] == 0x30)
      {
        this._LastError = 100040;
        IsError = true;
      }
      // If not received from the bill acceptor the third byte equal to 41N (Drop Cassette Full)
      else if (ByteResult[3] == 0x41)
      {
        this._LastError = 100080;
        IsError = true;
      }
      // If not received from the bill acceptor the third byte equal to 42N (Drop Cassette out of position)
      else if (ByteResult[3] == 0x42)
      {
        this._LastError = 100070;
        IsError = true;
      }
      // If not received from the bill acceptor the third byte equal to 43H (Validator Jammed)
      else if (ByteResult[3] == 0x43)
      {
        this._LastError = 100090;
        IsError = true;
      }
      // If you did not receive a third byte equal to 44N from the bill acceptor (Drop Cassette Jammed)
      else if (ByteResult[3] == 0x44)
      {
        this._LastError = 100100;
        IsError = true;
      }
      // If not received from the bill acceptor the third byte equal to 45N (Cheated)
      else if (ByteResult[3] == 0x45)
      {
        this._LastError = 100110;
        IsError = true;
      }
      // If you did not get a third byte equal to 46N from the bill acceptor (Pause)
      else if (ByteResult[3] == 0x46)
      {
        this._LastError = 100120;
        IsError = true;
      }
      // If you did not get a third byte equal to 47N (Generic Failure codes) from the bill acceptor,
      else if (ByteResult[3] == 0x47)
      {
        if (ByteResult[4] == 0x50) { this._LastError = 100130; }        // Stack Motor Failure
        else if (ByteResult[4] == 0x51) { this._LastError = 100140; }   // Transport Motor Speed Failure
        else if (ByteResult[4] == 0x52) { this._LastError = 100150; }   // Transport Motor Failure
        else if (ByteResult[4] == 0x53) { this._LastError = 100160; }   // Aligning Motor Failure
        else if (ByteResult[4] == 0x54) { this._LastError = 100170; }   // Initial Cassette Status Failure
        else if (ByteResult[4] == 0x55) { this._LastError = 100180; }   // Optic Canal Failure
        else if (ByteResult[4] == 0x56) { this._LastError = 100190; }   // Magnetic Canal Failure
        else if (ByteResult[4] == 0x5F) { this._LastError = 100200; }   // Capacitance Canal Failure
        IsError = true;
      }

      return IsError;
    }


    /// <summary>
    /// Send a command to the bill acceptor
    /// </summary>
    /// <param name="cmd"></param>
    /// <param name="Data"></param>
    /// <returns></returns>
    private byte[] SendCommand(BillValidatorCommands cmd, byte[] Data = null)
    {
      if (cmd == BillValidatorCommands.ACK || cmd == BillValidatorCommands.NAK)
      {
        byte[] bytes = null;

        if (cmd == BillValidatorCommands.ACK) { bytes = Package.CreateResponse(ResponseType.ACK); }
        if (cmd == BillValidatorCommands.NAK) { bytes = Package.CreateResponse(ResponseType.NAK); }

        if (bytes != null) { this._ComPort.Write(bytes, 0, bytes.Length); }

        return null;
      }
      else
      {
        Package package = new Package();
        package.Cmd = (byte)cmd;

        if (Data != null) { package.Data = Data; }

        byte[] CmdBytes = package.GetBytes();
        this._ComPort.Write(CmdBytes, 0, CmdBytes.Length);

        // Wait until we get data from the com port
        this._SynchCom.WaitOne(EVENT_WAIT_HANDLER_TIMEOUT);
        this._SynchCom.Reset();

        byte[] ByteResult = this._ReceivedBytes.ToArray();

        // If the CRC is ok, then check the fourth bit with the result
        // Must already get the data from the com-port, so we'll check the CRC
        if (ByteResult.Length == 0 || !Package.CheckCRC(ByteResult))
        {
          throw new ArgumentException("The checksum mismatch of the received message: The device may not be connected to the COM port. Check the connection settings. ");
        }

        return ByteResult;
      }

    }

    /// <summary>
    /// Currency code table
    /// </summary>
    /// <param name="code"></param>
    /// <returns>Amount according to bill table</returns>
    private int CashCodeTable(byte code)
    {
      int result = 0;

      if (code == 0x00) { result = 1; }           // 1 .
      else if (code == 0x01) { result = 5; }      // 5 .
      else if (code == 0x02) { result = 10; }     // 10 .
      else if (code == 0x03) { result = 20; }     // 20 .
      else if (code == 0x04) { result = 50; }     // 50 .
      else if (code == 0x05) { result = 100; }    // 100 .
      else if (code == 0x06) { result = 200; }    // 200 .
      else if (code == 0x07) { result = 500; }    // 500 .

      return result;
    }

    /// <summary>
    /// Event handler when bill received
    /// </summary>
    /// <param name="e"></param>
    private void OnBillReceived(AmountReceivedEventArgs e)
    {
      if (AmountReceived != null)
      {
        AmountReceived(this, new AmountReceivedEventArgs(e.Status, e.Value, e.RejectedReason));
      }
    }

    /// <summary>
    /// Event handler for cassete event status changed
    /// </summary>
    /// <param name="e"></param>
    private void OnBillCassetteStatus(BillCassetteEventArgs e)
    {
      if (BillCassetteStatusEvent != null)
      {
        BillCassetteStatusEvent(this, new BillCassetteEventArgs(e.Status));
      }
    }

    /// <summary>
    /// Event handler for bill stacking event
    /// </summary>
    /// <param name="e"></param>
    private void OnBillStacking(AmountReceivingEventArgs e)
    {
      if (AmountReceiving != null)
      {
        bool cancel = false;
        foreach (AmountReceivingHandler subscriber in AmountReceiving.GetInvocationList())
        {
          subscriber(this, e);

          if (e.Cancel)
          {
            cancel = true;
            break;
          }
        }

        _ReturnBill = cancel;
      }
    }

    /// <summary>
    /// Event handler for bill failure
    /// </summary>
    /// <param name="e"></param>
    private void OnBillFailure(BillFailureEventArgs e)
    {
      if (BillFailure != null)
      {
        bool cancel = false;//disable unit
        foreach (BillValidatorFailureHandler subscriber in BillFailure.GetInvocationList())
        {
          subscriber(this, e);

          if (e.Cancel)
          {
            cancel = true;
            break;
          }
        }

        _DisableBillValidator = cancel;
      }
    }

    /// <summary>
    /// Event handler for end of session
    /// </summary>
    /// <param name="e"></param>
    private void OnEndOfSession(EndOfSessionEventArgs e)
    {
      if (EndOfSession != null)
      {
        foreach (EndOfSessionHandler subscriber in EndOfSession.GetInvocationList())
        {
          subscriber(this, e);
        }
      }
    }

    /// <summary>
    /// Event handler for end of session
    /// </summary>
    /// <param name="e"></param>
    private void OnRaiseToClient(RaisedDetailsEventArgs e)
    {
      if (RaiseToClient != null)
      {
        foreach (RaiseToClienthandler subscriber in RaiseToClient.GetInvocationList())
        {
          subscriber(this, e);
        }
      }
    }

    /// <summary>
    /// Used to log any exceptions to the client
    /// </summary>
    /// <param name="e"></param>
    private void OnLogToClient(LogEventArgs e)
    {
      if (LogToClient != null)
      {
        foreach (LogToClientHandler subscriber in LogToClient.GetInvocationList())
        {
          subscriber(this, e);
        }
      }
    }

    /// <summary>
    /// Get data from the com port
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void _ComPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
    {
      //Let's go into 100 ms, in order to give the program all the data from the com port
      Thread.Sleep(100);
      this._ReceivedBytes.Clear();

      // We read bytes
      while (_ComPort.BytesToRead > 0)
      {
        this._ReceivedBytes.Add((byte)_ComPort.ReadByte());
      }

      // Unlocking
      this._SynchCom.Set();
    }

    /// <summary>
    /// Get data from the com port
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void _Listener_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
    {
      this._Listener.Stop();

      try
      {
        lock (_Locker)
        {
          List<byte> ByteResult = null;

          // send a POLL command
          ByteResult = this.SendCommand(BillValidatorCommands.POLL).ToList();
          DEBUG("Timer -> POLL");

          // If the fourth bit is not Idling (unoccupied), then go further
          if (ByteResult[3] != 0x14)
          {
            // ACCEPTING
            //If you receive a response 15H (Accepting)
            if (ByteResult[3] == 0x15)
            {
              DEBUG("Timer -> Accepting");

              // Confirm
              this.SendCommand(BillValidatorCommands.ACK);

              DEBUG("Timer -> Accepting -> ACK");
            }

            // ESCROW POSITION  
            // If the fourth bit is 1Ch (Rejecting), then the bill acceptor did not recognize the bill
            else if (ByteResult[3] == 0x1C)
            {
              DEBUG("Timer -> ESCROW");

              // Accepted some bill
              this.SendCommand(BillValidatorCommands.ACK);

              DEBUG("Timer -> ESCROW -> ACK");

              OnBillReceived(new AmountReceivedEventArgs(AmountRecievedStatus.Rejected, 0, this._ErrorList.Errors[ByteResult[4]]));

              DEBUG($"Timer -> ESCROW -> Rejected -> {this._ErrorList.Errors[ByteResult[4]]}");

            }

            // ESCROW POSITION
            // bill recognized
            else if (ByteResult[3] == 0x80)
            {
              DEBUG("Timer -> ESCROW");

              // Acknowledging
              this.SendCommand(BillValidatorCommands.ACK);

              DEBUG("Timer -> ESCROW -> ACK");

              // The event that the bill is in the process of sending to the stack
              OnBillStacking(new AmountReceivingEventArgs(CashCodeTable(ByteResult[4])));

              DEBUG("Timer -> ESCROW -> Stacking?");

              // If the program responds with a refund, then the return
              if (this._ReturnBill)
              {
                DEBUG("Timer -> ESCROW -> Return");

                // RETURN
                // If the program refuses to accept the bill, we will send RETURN
                ByteResult = this.SendCommand(BillValidatorCommands.RETURN).ToList();
                this._ReturnBill = false;
              }
              // If the program responds with a disable bill validator
              if (this._DisableBillValidator)
              {
                DEBUG("Timer -> ESCROW -> Disable");

                // Disable by disconnecting com port
                this.Disable();
                this._DisableBillValidator = false;
              }
              else
              {
                DEBUG("Timer -> ESCROW -> Stack");

                // STACK
                // If you are equal, we will send the note to the stack (STACK)
                ByteResult = this.SendCommand(BillValidatorCommands.STACK).ToList();
              }
            }

            // STACKING
            // If the fourth bit is 17h, then the process of sending a note to the stack (STACKING)
            else if (ByteResult[3] == 0x17)
            {
              DEBUG("Timer -> Stacking");

              this.SendCommand(BillValidatorCommands.ACK);

              DEBUG("Timer -> Stacking -> ACK");

            }

            // Bill stacked
            // If the fourth bit is 81h, therefore, the bill hit the stack
            else if (ByteResult[3] == 0x81)
            {
              DEBUG("Timer -> Stacked");

              // Acknowledging
              this.SendCommand(BillValidatorCommands.ACK);

              DEBUG("Timer -> Stacked -> ACK");

              var _amount = CashCodeTable(ByteResult[4]);
              OnBillReceived(new AmountReceivedEventArgs(AmountRecievedStatus.Accepted, _amount, ""));

              DEBUG($"Timer -> Stacked -> Received {_amount}");

            }

            // RETURNING
            // If the fourth bit is 18h, then the process returns
            else if (ByteResult[3] == 0x18)
            {
              DEBUG("Timer -> Returning");

              this.SendCommand(BillValidatorCommands.ACK);

              DEBUG("Timer -> Returning -> ACK");
            }

            // BILL RETURNING
            // If the fourth bit is 82h, then the note is returned
            else if (ByteResult[3] == 0x82)
            {
              DEBUG("Timer -> Returned");

              this.SendCommand(BillValidatorCommands.ACK);

              DEBUG("Timer -> Returned -> ACK");

            }

            // Drop Cassette out of position
            // The bills are removed
            else if (ByteResult[3] == 0x42)
            {
              DEBUG("Timer -> Cassette out of position");

              if (_cassettestatus != BillCassetteStatus.Removed)
              {
                // fire event
                _cassettestatus = BillCassetteStatus.Removed;
                OnBillCassetteStatus(new BillCassetteEventArgs(_cassettestatus));

              }
            }

            // Initialize
            // The cassette is inserted back into place
            else if (ByteResult[3] == 0x13)
            {
              DEBUG("Timer -> Cassette inserted back");

              if (_cassettestatus == BillCassetteStatus.Removed)
              {
                // fire event
                _cassettestatus = BillCassetteStatus.Inplace;
                OnBillCassetteStatus(new BillCassetteEventArgs(_cassettestatus));
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        OnLogToClient(LogEventArgs.Error($"Error on cashcode timer -> {ex.Message}", ex.StackTrace));
      }
      finally
      {
        // If the timer is off, then we start
        if (!this._Listener.Enabled && this._IsListening)
          this._Listener.Start();
      }

    }

    /// <summary>
    /// Destructor for finalizing the code
    /// </summary>
    ~CashCodeBillValidator() { Dispose(false); }

    /// <summary>
    /// Implements the interface IDisposable
    /// </summary>
    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this); // Прикажем GC не финализировать объект после вызова Dispose, так как он уже освобожден
    }

    /// <summary>
    /// Dispose(bool disposing) is done in two scenarios
    /// If disposing = true, the Dispose method is called explicitly or implicitly from the user code
    /// Managed and unmanaged resources can be released
    /// If disposing = false, then the method can be called runtime from the finalizer
    /// In this case only unmanaged resources can be released.
    /// </summary>
    /// <param name="disposing"></param>
    private void Dispose(bool disposing)
    {
      // Check if the Dispose method has already been called
      if (!this._Disposed)
      {
        // If disposing = true, release all managed and unmanaged resources
        if (disposing)
        {
          // Free Managed Resources Here
          try
          {
            // Stop the timer, if it works
            if (this._IsListening)
            {
              this._Listener.Enabled = this._IsListening = false;
            }

            this._Listener.Dispose();

            // Take the shutdown signal to the bill acceptor
            if (this._IsConnected)
            {
              this.Disable();
            }
          }
          catch (Exception ex)
          {
            OnLogToClient(LogEventArgs.Error($"Error disposing cashcode -> {ex.Message}", ex.StackTrace));
          }
        }

        // Put the appropriate methods to free unmanaged resources
        // If disposing = false, only the following code will be executed
        try
        {
          this._ComPort.Close();
        }
        catch (Exception ex)
        {
          OnLogToClient(LogEventArgs.Error($"Error closing com port {_ComPort} -> {ex.Message}", ex.StackTrace));
        }

        _Disposed = true;
      }
    }

    public bool IsWorking()
    {
      bool working = false;
      try
      {
        if (!IsConnected)
        {
          try
          {
            Connect();
            PowerUp();
          }
          catch (Exception exConnect)
          {
            //logger.Warn($"Cashcode failed to connect \n\t{exConnect.Message}");
            OnLogToClient(LogEventArgs.Error($"Failed to connect to cashcode -> {exConnect.Message}", exConnect.StackTrace));

            working = false;
            return working;
          }
        }

        List<byte> ByteResult = null;

        // send a POLL command
        ByteResult = this.SendCommand(BillValidatorCommands.POLL).ToList();

        // If not received from the bill acceptor the third byte equal to 41N (Drop Cassette Full)
        if (ByteResult[3] == 0x41)
        {
          working = false;
        }
        // If not received from the bill acceptor the third byte equal to 42N (Drop Cassette out of position)
        else if (ByteResult[3] == 0x42)
        {
          working = false;
        }

        working = true;
      }
      catch (Exception ex)
      {
        //logger.Warn($"Cashcode is not working \n\t {ex.Message}");
        OnLogToClient(LogEventArgs.Error($"Cashcode is not working  -> {ex.Message}", ex.StackTrace));
      }
      return working;
    }



    private void DEBUG(string message)
    {
      OnLogToClient(LogEventArgs.Debug(message));
    }
  }

}
