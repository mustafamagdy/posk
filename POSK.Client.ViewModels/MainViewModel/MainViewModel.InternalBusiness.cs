using System;
using Timer = System.Timers.Timer;
using Geeky.POSK.Infrastructore.Extensions;
using System.Transactions;
using Geeky.POSK.DataContracts;
using POSK.Printers.Interface;
using System.Collections.Generic;

namespace POSK.Client.ViewModels
{
  public sealed partial class MainViewModel
  {

    private void CheckOut()
    {
      //start timeout timer
      StartTimer(interval_checkout, "Checkout"); ;

      //update cart on UI
      PropogateCart();

      //navigate to shopping cart
      //CurrentVm = _shoppingCartVm;
      _selectPaymentMethodVm.SetPaymentMethods(PaymentMethods);
      CurrentVm = _selectPaymentMethodVm;

    }

    /// <summary>
    /// check if cart is fullfilled then go to checkout screen, else it will go to payment screen
    /// </summary>
    private void PayOrder()
    {
      //NOT USED
      //if user pay full amount, go to checkout (printing screen) and skip going to payment screen
      //if (Cart.Ready)
      //{
      // StartTimer(interval_printReceipt, "Payorder():Cart Is Ready");
      //CurrentVm = _finishOrderVm;
      //}
      //else
      //{
      // StartTimer(interval_payment, "Payorder():Cart Not Ready");
      //  _selectPaymentMethodVm.SetPaymentMethods(PaymentMethods);
      //  CurrentVm = _selectPaymentMethodVm;
      //}
    }

    /// <summary>
    /// Checkout the order, and print the required pins
    /// </summary>
    private void ProcessOrder()
    {
      try
      {
        using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
        {
          //tell cart to check out
          Cart.Checkout();

          LogSession(Cart.Session, $"Session completed, process order");

          //Dispose cart
          Cart.DisposeCart(releaseReservedItems: false);

          //if print fails, throw excption and item will not marked as sold
          scope.Complete();

          LogSession(null, $"************************************");
          
          //navigate to thank you message
          StartTimer(interval_thankYou, "ProcessOrder: Finished order going to thank you screen"); ;
          CurrentVm = _thankYouVm;
        }
      }
      catch (ApplicationException ex)
      {
        CurrentVm = _faildTrxVm;
        StartTimer(interval_failedException, "ProcessOrder: Application Exception -> " + ex.Message); ;
        Error("Process order failure:" + ex.Message);
      }
      catch (Exception ex)
      {
        CurrentVm = _faildTrxVm;
        StartTimer(interval_failedException, "ProcessOrder: Exception -> " + ex.Message); ;
        Error("Process order failure:" + ex.Message);
      }
    }

    /// <summary>
    /// starting new session, initiates new instance of <see cref="UserCart"/> 
    /// </summary>
    private void StartSession()
    {
      Cart = UserCart.Create();
      Cart.CartItemAdded += Cart_CartItemAdded;
      Cart.CartItemRemoved += Cart_CartItemRemoved;
      Cart.AmountPaid += Cart_AmountPaid;
      Cart.CartDisposed += Cart_CartDisposed;
      Cart.Print += Cart_Print;
      Cart.TerminalId = CurrentTerminalId;

      //Set global session id 
      __CURRENT_SESSION_ID = Cart.Session.Id;

      LogSession(Cart.Session, "Start new session");
      PropogateCart();
    }




    /// <summary>
    /// Propogate cart to view models to update UI
    /// </summary>
    private void PropogateCart()
    {
      _checkoutVm.SetCart(Cart);
      _productListVm.UpdateCheckoutInfo(_checkoutVm);
      _serviceListVm.UpdateCheckoutInfo(_checkoutVm);
      _orderPaymentVm.SetCart(Cart);
      _selectPaymentMethodVm.SetCart(Cart);

      //_shoppingCartVm.SetCart(Cart);
    }

    /// <summary>
    /// End current session, if current paid amount > 0, we instead show warning message 
    /// however if you want to force to end it, use force flag, use this when you cann it from the timer after the
    /// warning message timeout, so we now has no way other than disposing the session and return to home screen.
    /// </summary>
    /// <param name="force">Indicates that you don't care if user has paid or not</param>
    private void EndSession(bool force = false)
    {

      //if meanwhile we are trying to end session, user insert money, and cash code updated the flag
      //of receiving money, and because this flag will not be checked untill next timer cycle,
      //we need to check it here final time, if it is true, we should not end session, we should wait again
      if (Cart.IsReceivingMoney)
      {
        LogSession(Cart.Session, "False end session, we are receiving money now");
        StartTimer(_navigationTimer.Interval, "False end session, we are receiving money now");
        return;
      }


      //---> Wrong comment:  final check if user pay some while the warning screen is on, check again if order has fullfilled
      //if user pay full amount, go to checkout (printing screen) and skip going to payment screen

      //----> right comment: this happened because timer call EndSession while user in Paymentscreen, and he payed full amount
      //the system trying to process the order but the timer called it before he does
      //so we ignore this call and let it handled by the Cart events (on the amount paid event)
      if (Cart.Ready)
      {
        
        //StopAllPaymentMethods();
        //ProcessOrder();
        return;
      }
      else if (Cart.TotalPaid > 0 && force == false)
      {
        //stop receiving money until he choose either to continue or to stop
        //_selectedPm?.Stop();

        LogSession(Cart.Session, $"User paid some, and didn't complete, show warning now");
        //Cart.SetShowTimeoutWarning();

        //checking again just before ending session, if we are receiving money or not
        if (Cart.IsReceivingMoney)
        {
          LogSession(Cart.Session, "False end session:inside, we are receiving money now");
          StartTimer(_navigationTimer.Interval, "False end session:inside, we are receiving money now");
          return;
        }


        CurrentVm = _timeoutWarningVm;

        //Stop cashcode untill user take action
        _navigationTimer.Stop();
        StopCashCode();
        StartTimer(interval_timeoutWarning, "EndSession -> Starting timeout warning screen");
        return;
      }
      //client paid money, and system asked to finish the order NOW
      else if (Cart.TotalPaid > 0 && force == true)
      {
        //update status to timeout expired
        //Cart.SetTimeoutExpired();

        //if cart didn't check out, go to session expired screen
        //this means time has gone and user didn't finished the purchase process
        //so we should move to session expired screen
        if (!Cart.OrderFinished)
        {
          //show timeout expired screen, and show ref number to refer to support
          _sessionExpiredVm.ShowUserSessionInfo(Cart);
          //don't take money until we process the order
          StopAllPaymentMethods();

          _sessionExpiredVm.HasPayment = Cart.TotalPaid > 0;
          _sessionExpiredVm.HasNoPayment = !_sessionExpiredVm.HasPayment; 

          CurrentVm = _sessionExpiredVm;
          //navigate back to begin the cycle
          LogSession(Cart.Session, $"User paid some, and didn't complete, ending session");

          //print receipt for paid amount
          PrintEndOfSessionReceipt();

          //unlock held items
          Cart.DisposeCart(releaseReservedItems: true);

          StartTimer(interval_sessionExpired, $"EndSession -> {force}: Order didn't finished, going to session expired screen");
          return;
        }
        //if we come here that's mean session has completed normally, user paid full amount
        //and order has been processed, going to start new session
        else if (Cart.OrderFinished)
        {
          LogSession(Cart.Session, "Sessin complete, move to main screen");
          LogSession(Cart.Session, "____________________________________");
          //don't take money until we process the order
          StopAllPaymentMethods();

          //go to start screen
          NavigateToWelcome();

          //propogate to UI
          PropogateCart();
          return;
        }
      }
      //client didn't pay, restarting now
      else
      {

        //checking again just before ending session, if we are receiving money or not
        if (Cart.IsReceivingMoney)
        {
          LogSession(Cart.Session, "False end session:inside, we are receiving money now");
          StartTimer(_navigationTimer.Interval, "False end session:inside, we are receiving money now");
          return;
        }

        StopAllPaymentMethods();

        //unlock held items
        Cart.DisposeCart(releaseReservedItems: true);

        //propogate to UI
        PropogateCart();

        LogSession(Cart.Session, "Sessin complete no payment, move to main screen");
        LogSession(Cart.Session, "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX");

        //go to start screen
        NavigateToWelcome();
        return;
      }
    }



    /// <summary>
    /// Print receipt to client in case he pay and didn't complete the purchase process
    /// </summary>
    private void PrintEndOfSessionReceipt()
    {
      var lines = new List<PrinterLine>();
      lines.Add(new PrinterLine($"Amount: {Cart.TotalPaid} SAR"));
      lines.Add(new PrinterLine("********************************************"));
      lines.Add(new PrinterLine($"Ref Number: {Cart.Session.RefNumber}"));
      lines.Add(new PrinterLine($"Trx Date: {Cart.Session.StartTime}"));
      SessionEndPrinter.Print(lines.ToArray());
    }

    /// <summary>
    /// Used to stop cashcode if it is exist
    /// </summary>
    private void StopCashCode()
    {
      if (_cashCode != null && _cashCode.IsConnected)
      {
        //_cashCode.Disable();
        _cashCode.StopListening();
      }
    }

    /// <summary>
    /// stops all payment methods as client want to choose another
    /// </summary>
    private void StopAllPaymentMethods()
    {
      PaymentMethods.ForEach(p =>
      {
        if (p != null && p.IsConnected)
          p.Stop();
      });

      //reset seleceted payment method
      //_selectedPm = null;
    }
  }

}
