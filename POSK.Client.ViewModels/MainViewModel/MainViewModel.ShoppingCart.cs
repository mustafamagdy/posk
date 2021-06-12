using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Geeky.POSK.DataContracts;
using Geeky.POSK.Infrastructore.Extensions;

namespace POSK.Client.ViewModels
{
  public sealed partial class MainViewModel
  {

    /// <summary>
    /// event raise by cart telling you that an amount has been received, so if you want to update UI
    /// you can list to that event, and get the total paid amount from it on <see cref="UserCart.TotalPaid"/>
    /// </summary>
    private void Cart_AmountPaid()
    {
      try
      {
        _navigationTimer.Stop();

        //update total paid, and remaining on order payment
        _orderPaymentVm.SetCart(Cart);
        _selectPaymentMethodVm.SetCart(Cart);

        //update checkout data
        PropogateCart();

        //Log($"Amount paid -> {Cart.TotalPaid}/{Cart.TotalValue}");

        //if user pay full amount, go to checkout (printing screen)
        if (Cart.Ready)
        {
          //Log("[Timer]: Cart.Amount Paid :User paid full -> print and finish order");
          //ShowTimeoutWarning = false;

          StopAllPaymentMethods();
          ProcessOrder();

          //StartTimer(interval_finishOrder, $"AmountReceived({Cart.TotalPaid}):TotalPaid>=TotalValue"); ;
          //CurrentVm = _thankYouVm;
        }
        else //if still payment not full filled, we need more
        {
          //Log("[Timer]: Cart.Amount is not enought .. stay in payment");
          StartTimer(interval_payment, $"AmountReceived({Cart.TotalPaid}):TotalPaid<TotalValue"); ;
          CurrentVm = _orderPaymentVm;
        }
      }
      catch (Exception ex)
      {
        Error(ex.Message + "\n\r" + ex.StackTrace);
      }
    }



    /// <summary>
    /// Usser has removed an item from the cart, you can update UI based on this event
    /// </summary>
    private void Cart_CartItemRemoved()
    {
      //update total paid, and remaining on order payment
      _orderPaymentVm.SetCart(Cart);
      _selectPaymentMethodVm.SetCart(Cart);

      //update checkout data
      PropogateCart();

    }

    /// <summary>
    /// User added new item to the cart, you can update UI based on this event.
    /// </summary>
    private void Cart_CartItemAdded()
    {
      //update total paid, and remaining on order payment
      _orderPaymentVm.SetCart(Cart);
      _selectPaymentMethodVm.SetCart(Cart);


      //update checkout data
      PropogateCart();

    }

    private void Cart_CartDisposed()
    {

    }

    private void Cart_Print(List<DecryptedPinDto> pins)
    {
      //var receiptGenerator = new ReceiptGenerator();
      int width = 300;
      Printers.ForEach(p =>
      {
        pins.ForEach(pin =>
        {
          //var fileName = receiptGenerator.GenerateReceipt(pin, Cart.Session.Id, width);
          //p.Print(fileName, width);
          //File.Delete(fileName);
          p.Print(pin, Cart.Session.Id, width);
        });
      });
    }


    private void Cart_SessionCreated()
    {
      __CURRENT_SESSION_ID = Cart.Session.Id;
    }

  }

}
