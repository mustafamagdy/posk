using POSK.Client.CashCode.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POSK.Payment.Interface;
using Geeky.POSK.Infrastructore.Core.Extensions;
using Geeky.POSK.Infrastructore.Extensions;

namespace POSK.Client.CashCode
{
  public class CashCodeSimulator : ICashCodeBillValidator
  {
    public string Code => "BILL_VALIDATOR";
    public string Name => "CASH";
    public string ButtonImageName => "../Style/pCash.png";
    public string DescriptionImageName => "../Style/dCash.png";

    public bool IsConnected => _IsConnected;

    private int _LastError;
    private bool _IsConnected;
    private bool _IsEnableBills;
    private bool _Disposed;
    private bool _IsPowerUp;
    private bool _IsListening;
    private Random _r = new Random();
    private int[] _cashtable = new int[] { 1, 5, 10, 50, 100, 500 };
    bool _ReturnBill;
    bool _DisableBillValidator;
    decimal value = 0;
    private const int POLL_TIMEOUT = 200;    // Timeout for waiting for a response from the reader
    BillCassetteStatus _cassettestatus = BillCassetteStatus.Inplace;

    public decimal _required { get; set; }

    private CashCodeErroList _ErrorList;
    private System.Timers.Timer _Listener;  // Banner Receiver Timer

    public CashCodeSimulator()
    {
      this._ErrorList = new CashCodeErroList();

      this._Listener = new System.Timers.Timer();
      this._Listener.Interval = POLL_TIMEOUT;
      this._Listener.Enabled = false;
      this._Listener.Elapsed += new System.Timers.ElapsedEventHandler(_Listener_Elapsed);

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
        var r = _r.Next(1, 100);
        if (r.Between(1, 20))
        {
          OnBillReceived(new AmountReceivedEventArgs(AmountRecievedStatus.Rejected, _required, this._ErrorList.Errors.AnyOne()));
        }
        else if (r.Between(21, 30))
        {
          OnBillStacking(new AmountReceivingEventArgs(_required));
        }
        else if (r.Between(31, 70))
        {
          if (!this._ReturnBill)
          {
            if (r % 10 != 0)
            {
              OnBillReceived(new AmountReceivedEventArgs(AmountRecievedStatus.Accepted, _required, ""));
            }
            else
            {
              //Only pay 1 SAR and exit
              OnBillReceived(new AmountReceivedEventArgs(AmountRecievedStatus.Accepted, 1/* 1 SAR*/, ""));
              this.Stop();
            }
          }

          this._ReturnBill = false;
        }
        else if (r.Between(71, 80))
        {
          //just do nothing, to test what happened if he do nothing
          this.Stop();
        }
        else if (r.Between(81, 90))
        {
          _cassettestatus = BillCassetteStatus.Removed;
          OnBillCassetteStatus(new BillCassetteEventArgs(_cassettestatus));
        }
        else if (r.Between(91, 100))
        {
          _cassettestatus = BillCassetteStatus.Inplace;
          OnBillCassetteStatus(new BillCassetteEventArgs(_cassettestatus));
        }
      }
      catch (Exception ex)
      {
        OnRaiseToClient(new RaisedDetailsEventArgs(9999, ex.Message));
      }
      finally
      {
        // If the timer is off, then we start
        if (!this._Listener.Enabled && this._IsListening)
          this._Listener.Start();
      }
    }


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

    public event LogToClientHandler LogToClient;



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
          value = e.Value;
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

    public int PowerUp()
    {
      this._IsPowerUp = true;
      return 0;
    }

    public int Connect()
    {
      try
      {
        this._IsConnected = true;
      }
      catch
      {
        this._IsConnected = false;
        this._LastError = 100010;
      }

      return this._LastError;
    }

    public int Enable()
    {
      // If the com port is not open
      if (!this._IsConnected)
      {
        this._LastError = 100020;
        throw new System.IO.IOException(this._ErrorList.Errors[this._LastError]);
      }

      this._IsEnableBills = true;

      return this._LastError;
    }

    public int Disable()
    {
      // If the com port is not open
      if (!this._IsConnected)
      {
        this._LastError = 100020;
        throw new System.IO.IOException(this._ErrorList.Errors[this._LastError]);
      }

      this._IsEnableBills = false;
      return this._LastError;
    }

    public void Start()
    {
      if (!IsConnected)
        return;

      if (!this._IsListening)
      {
        this._IsListening = true;
        this._Listener.Start();
      }
    }

    public void Stop()
    {
      if (this._IsListening)
      {
        this._IsListening = false;
        this._Listener.Stop();
      }
    }

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this); // Прикажем GC не финализировать объект после вызова Dispose, так как он уже освобожден
    }

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
          catch { }
        }

        // Put the appropriate methods to free unmanaged resources
        // If disposing = false, only the following code will be executed
        try
        {

        }
        catch { }

        _Disposed = true;
      }
    }

    Random rWorking = new Random();
    public bool IsWorking()
    {
      return true;
      var i = rWorking.Next(1, 100);
      bool isWorking = i % 2 == 0;
      return isWorking;
    }

    public void StopListening()
    {
      Stop();
    }
  }
}
