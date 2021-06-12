using System;
using Geeky.POSK.DataContracts;
using Geeky.POSK.Infrastructore.Core;
using Geeky.POSK.Infrastructore.Extensions;
using NLog;
using POSK.Client.CashCode.Interface;
using POSK.Payment.Interface;

namespace POSK.Client.ViewModels
{
  public sealed partial class MainViewModel
  {

    /// <summary>
    /// Raised by cashcode interface to let you know that something happened in cash cassette
    /// </summary>
    /// <param name="Sender"></param>
    /// <param name="e"></param>
    private void _cashCode_BillCassetteStatusEvent(object Sender, BillCassetteEventArgs e)
    {
      Warning("**CASSETTE**" + e.Status);
      try
      {
        if (e.Status == BillCassetteStatus.Removed)
          AddExtraInfoForPing(ExtraInfo.CashCodeIsRemoved());
        else if (e.Status == BillCassetteStatus.Inplace)
          AddExtraInfoForPing(ExtraInfo.CashCodeIsRemoved(false));

        _clientSvc.LogTerminalStatus(CurrentTerminalId, Cart.Session.Id, LogTypeEnum.WARNING,
          $"Cashcode cassete status changed Status is :{e.Status}");
      }
      catch (Exception ex)
      {
        Error(ex.Message + ex.StackTrace);
      }
    }

    /// <summary>
    /// A bill has been received, it could be accepted or rejecte, check on its status and them proceed
    /// in adding its value to cart payments
    /// </summary>
    /// <param name="Sender"></param>
    /// <param name="e"></param>
    private void _cashCode_AmountReceived(object Sender, AmountReceivedEventArgs e)
    {
      try
      {
        LogSession(Cart.Session, $"[Cash] -> received amout: {e.Value} { (e.Status == AmountRecievedStatus.Rejected ? "and it has been returned " + e.RejectedReason : "") }");

        if (e.Status == AmountRecievedStatus.Accepted)
        {
          //todo show total amount gathered untill now
          Cart.AddPaidValue(PaymentHelper.CashCode(e.Value, e.Value));
          Cart.IsReceivingMoney = false;

          try
          {
            //don't use Cart.Session here, if this payment enough to fullfill the order, 
            //the order will be finished and Session will be null here
            Cart.AddTerminalLog(LogTypeEnum.INFO, $"Amount stacked {e.Value}");
          }
          catch (Exception ex)
          {
            Error(ex.Message + ex.StackTrace);
          }
        }
        else
        {
          //todo show error why this has been rejected
          Cart.AddPaidValue(PaymentHelper.CashCode(0, e.Value, e.RejectedReason));
          Cart.IsReceivingMoney = false;

          try
          {
            Cart.AddTerminalLog(LogTypeEnum.INFO, $"Amount rejected {e.Value} because of {e.RejectedReason}");
          }
          catch (Exception ex)
          {
            Error(ex.Message + ex.StackTrace);
          }
        }

      }
      catch (Exception ex)
      {

      }
    }

    /// <summary>
    /// Cashcode is getting a new bill, check if you want  it or not, if not return it back to user
    /// </summary>
    /// <param name="Sender"></param>
    /// <param name="e"></param>
    private void _cashCode_AmountReceiving(object Sender, AmountReceivingEventArgs e)
    {
      try
      {
        Cart.IsReceivingMoney = true;
        LogSession(Cart.Session, $"[Cash] -> receiving amout: {e.Value}");

        if (Cart.Session == null || CustomCeiling(Cart.TotalValue) < 1)
        {
          LogSession(null, $"Session is null or total value is zero {CustomCeiling(Cart.TotalValue)} and accepting amount {e.Value} we will return it");
          //return the money, the session is scrwed up
          e.Cancel = true;
          Cart.IsReceivingMoney = false;
          LogSession(null, $"Amount {e.Value} has been returned as session is null");
        }
        else if ((Cart.TotalPaid + e.Value) > CustomCeiling(Cart.TotalValue))
        {
          LogSession(Cart.Session, $"Amount {e.Value} has been rejected as user paid more than required");
          Cart.AddTerminalLog(LogTypeEnum.INFO, $"Amount returned to client because it exceed the requested amount (requested: {Cart.TotalValue - Cart.TotalPaid} - received: {e.Value})");

          //todo show error that this amount greater than required
          Cart.ShowError("AMOUNT_EXCEED_DUE_AMOUNT");

          //then return the amount
          e.Cancel = true;
          Cart.IsReceivingMoney = false;
        }
      }
      catch (Exception ex)
      {
        Error(ex.Message);
      }
    }

    /// <summary>
    /// Show error message to client
    /// </summary>
    /// <param name="Sender"></param>
    /// <param name="e"></param>
    private void _cashCode_RaiseToClient(object Sender, RaisedDetailsEventArgs e)
    {
      Error("**CASH**" + e.ErrorCode + " : " + e.ErrorMessage);
      try
      {
        Cart.AddTerminalLog(LogTypeEnum.ERROR,
            $"Unknow error from cashcode Code: {e.ErrorCode}, Error Message: {e.ErrorMessage}");
        Cart.ShowError(e.ErrorMessage);
      }
      catch (Exception ex)
      {
        Error(ex.Message + ex.StackTrace);
      }
      finally
      {
        StartTimer(interval_payment, "Raise To Client: by cash coder Extend timer, as amount received is not sutiable"); ;
      }
    }

    /// <summary>
    /// End client session as client is doing something wrong (like cheating)
    /// </summary>
    /// <param name="Sender"></param>
    /// <param name="e"></param>
    private void _cashCode_EndOfSession(object Sender, EndOfSessionEventArgs e)
    {
      Error("**CASH** Try to cheat (End Session)");
      try
      {
        Cart.AddTerminalLog(LogTypeEnum.ERROR,
          $"Cashcode refused to cheating with Error Code: {e.ErrorCode}, Error Message: {e.ErrorMessage}");
      }
      catch (Exception ex)
      {
        Error(ex.Message + ex.StackTrace);
      }

      StopAllPaymentMethods();
      EndSession();
    }

    /// <summary>
    /// Stop cash payment method now
    /// </summary>
    /// <param name="Sender"></param>
    /// <param name="e"></param>
    private void _cashCode_BillFailure(object Sender, BillFailureEventArgs e)
    {
      Error("**CASH** Failure : " + e.ErrorCode + " : " + e.ErrorMessage);
      try
      {
        /*
         100070 -> Error checking the status of the bill acceptor. The plug is removed.
         100080 -> Error checking the status of the bill acceptor. The Stacker is full.
         100090 -> Error checking the status of the bill acceptor. A bill was stuck in the validator.
         100100 -> Error checking the status of the bill acceptor. A bill was stuck in the stack.
                -> 
         100130 -> Error in the bill acceptor operation. Stack Motor Failure.
         100140 -> Error in the bill acceptor operation. Transport Motor Speed Failure.
         100150 -> Error in the bill acceptor operation. Transport Motor Failure.
         100160 -> Error in the bill acceptor operation. Aligning Motor Failure.
         100170 -> Error in the bill acceptor operation. Initial Cassette Status Failure.
         100180 -> Error in the bill acceptor operation. Optic Canal Failure.
         100190 -> Error in the bill acceptor operation. Magnetic Canal Failure.
         100200 -> Error in the bill acceptor operation. Capacitance Canal Failure.
          */

        switch (e.ErrorCode)
        {
          case 100080://The Stacker is full
            AddExtraInfoForPing(ExtraInfo.CashCodeIsFull());
            break;
          default:
            AddExtraInfoForPing(ExtraInfo.CashCodeFailure(e.ErrorCode, e.ErrorMessage));
            break;
        }

        _clientSvc.LogTerminalStatus(CurrentTerminalId, Cart.Session.Id, LogTypeEnum.ERROR,
            $"Cashcode failure Error Code: {e.ErrorCode}, Error Message: {e.ErrorMessage}");
      }
      catch (Exception ex)
      {
        Error(ex.Message + ex.StackTrace);
      }

      EndSession();
    }

    /// <summary>
    /// Log messages to client
    /// </summary>
    /// <param name="Sender"></param>
    /// <param name="e"></param>
    private void _cashCode_LogToClient(object Sender, LogEventArgs e)
    {
      if (e.Level == LogLevel.Info)
      {
        Info(e.Message);
      }
      else if (e.Level == LogLevel.Trace)
      {
        LogSession(null, e.Message);
      }
      else if (e.Level == LogLevel.Warn)
      {
        Warning(e.Message);
      }
      else if (e.Level == LogLevel.Debug)
      {
        Debug(e.Message);
      }
      else if (e.Level == LogLevel.Error)
      {
        Error(e.Message);
      }
    }
    public static decimal CustomCeiling(decimal value)
    {
      if (CustomCeilingMode)
        return Math.Ceiling(value / 5.0M) * 5.0m;
      else
        return Math.Ceiling(value);

    }

  }

}
