using System;
using Geeky.POSK.Infrastructore.Core;
using Geeky.POSK.Infrastructore.Extensions;
using Geeky.POSK.Models;

namespace POSK.Client.ViewModels
{
  public sealed partial class MainViewModel
  {

    /// <summary>
    /// Event handler for language changing
    /// </summary>
    /// <param name="cultureCode">chosen language culture code</param>
    private void _selectLanguageVm_LanguageSelected(string cultureCode)
    {
      //set local current language to culture code selected
      CurrentLanguage = cultureCode;
      //fire language changed event tell the window to change ui language
      LanguageChanged();

      bool onePaymentMethod = false;
      bool printerWorking = false;
      //if terminal is not working, disable it and do nothing
      if (!CheckTerminalStatus(out onePaymentMethod, out printerWorking))
      {
        Info("Terminal is disabled");
        CurrentVm = _terminalDisabledVm;
        return;
      }

      if (onePaymentMethod)
      {
        Info("One payment method is working");
        //_navigationTimer.Start(interval_onePaymentMethod, "OnePaymentMethod");
        //CurrentVm = _onePaymentMethodWarning;
        //return;
      }

      StartingSessionNow();
    }



    /// <summary>
    /// User select a service
    /// </summary>
    /// <param name="service">selected service</param>
    private void _serviceListVm_ServiceSelected(ServiceViewModel service)
    {
      //load products types for this service
      _productTypeVm.LoadProductTypes(service.VendorId, service.VendorLogo);
      //start timeout timer
      StartTimer(interval_productTypes, $"ServiceSelected()");
      //move to product list screen
      CurrentVm = _productTypeVm;
    }

    private void _productTypeVm_ProductTypeSelected(ProductTypeEnum productType, Guid vendorId, byte[] vendorLogo)
    {
      //load products for this service
      _productListVm.LoadProducts(productType, vendorId, vendorLogo);
      //set selected Service on Order payment screen to support back functionality
      _orderPaymentVm.SetSelectedService(productType, vendorId, vendorLogo);
      _selectPaymentMethodVm.SetSelectedService(productType, vendorId, vendorLogo);

      //start timeout timer
      StartTimer(interval_products, $"ProductTypeSelected()");
      //move to product list screen
      CurrentVm = _productListVm;
    }
    private void _productTypeVm_BackToServices()
    {
      StartTimer(interval_services, $"BackToServices()");
      _serviceListVm.LoadAvailableServices();
      CurrentVm = _serviceListVm;
    }

    /// <summary>
    /// User clicked on GoTo checkout screen to review the order details
    /// </summary>
    private void _productListVm_GotoCheckout()
    {
      //go to checkout
      CheckOut();
    }

    /// <summary>
    /// user choose an item to purchase, we then add a pin from this item type to user's cart
    /// </summary>
    /// <param name="item">the chosen item</param>
    private void _productListVm_ItemPurchased(ProductItemViewModel item)
    {
      //this should not be called, just in testing
      if (item.SoldOut)
      {
        //start timeout timer
        StartTimer(interval_noStock, $"ItemPurchase():No Stock");
        //move to no stock screen screen
        CurrentVm = _noStockAvailable;
      }
      else
      {
        //add item to cart
        Cart.AddItem(item);

        //go to checkout
        CheckOut();
      }
    }


    /// <summary>
    /// user asked to go back to service list
    /// </summary>
    private void _productListVm_BackToProductTypes(Guid vendorId, byte[] vendorLogo)
    {
      StartTimer(interval_productTypes, $"BackToProductTypes()"); 
      _productTypeVm.LoadProductTypes(vendorId, vendorLogo);
      CurrentVm = _productTypeVm;
    }

    /// <summary>
    /// user remove an item from the cart
    /// </summary>
    /// <param name="pinId">removed pin</param>
    private void _shoppingCartVm_RemoveItemInCart(Guid pinId)
    {
      Cart.RemoveItem(pinId);
    }

    /// <summary>
    /// user wants to continue shopping, we should navigate to service list again
    /// </summary>
    private void _shoppingCartVm_ContinueShopping()
    {
     StartTimer(interval_services, $"ContinueShopping()");
      _serviceListVm.LoadAvailableServices();
      CurrentVm = _serviceListVm;
    }

    /// <summary>
    /// User revied the order, and want to proceed to payment
    /// </summary>
    private void _shoppingCartVm_GotoOrderPayment()
    {
      PayOrder();
    }

    /// <summary>
    /// user want to modify or review the order details again
    /// </summary>
    private void _orderPaymentVm_GoToShoppingCart()
    {
      if (Cart.Items.Count > 0)
      {//go to checkout
        CheckOut();
      }
      else
      {
        StartTimer(interval_default, $"GotoShoppingCart():Not Items in Cart");
        CurrentVm = _serviceListVm;

        //propogate to UI
        PropogateCart();
      }
    }

    /// <summary>
    /// User asked to go back to choose another product
    /// we should remove the current selected product and 
    /// wait for user to choose another one
    /// </summary>
    private void _orderPaymentVm_BackToSelectPaymentMethod(ProductTypeEnum productType, Guid vendorId, byte[] vendorLogo)
    {
      StopAllPaymentMethods();

      _selectPaymentMethodVm.SetSelectedService(productType, vendorId, vendorLogo);
      //start timeout timer
      StartTimer(interval_payment, $"BackTpSelectPaymentMethod()");
      //move to product list screen
      _selectPaymentMethodVm.SetPaymentMethods(PaymentMethods);
      CurrentVm = _selectPaymentMethodVm;
    }

    /// <summary>
    /// user picked up a payment method, we should enable this method hardware to proceed
    /// </summary>
    /// <param name="method">selected method</param>
    private void _selectPaymentMethodVm_PaymentMethodSelected(PaymentMethod method)
    {
      //todo JUST FOR TESTING
      if (Simulated)
        _cashCode._required = CustomCeiling(Cart.TotalValue);

      foreach (var _method in PaymentMethods)
      {
        if (_method != null && _method.Code == method.Code)
        {
          if (!_method.IsConnected)
            _method.Connect();

          _method.Start();
          //_selectedPm = _method;

          //update order payment vm to show payment method image accordingly
          _orderPaymentVm.SetSelectedPaymentMethod(method);
          StartTimer(interval_payment, "Payment method selected");
          CurrentVm = _orderPaymentVm;
        }
        else if (_method != null)
        {
          _method.Stop();
        }

      }
    }

    /// <summary>
    /// User asked to go back to choose another product
    /// we should remove the current selected product and 
    /// wait for user to choose another one
    /// </summary>
    private void _selectPaymentMethodVm_BackToProducts(ProductTypeEnum productType, Guid vendorId, byte[] vendorLogo)
    {
      //Empty the cart
      Cart.ClearItems();
      //load products for this service
      _productListVm.LoadProducts(productType, vendorId, vendorLogo);
      //start timeout timer
      StartTimer(interval_services, $"BackToProducts()");
      //move to product list screen
      CurrentVm = _productListVm;
    }

    /// <summary>
    /// Go back to shopping cart, user want to review or edit order again
    /// </summary>
    private void _finishOrderVm_GoToShoppingCart()
    {
      //go to checkout
      CheckOut();
    }

    /// <summary>
    /// Finilize order, print, sell, clear user session
    /// </summary>
    private void _finishOrderVm_FinilizeOrder()
    {
      //if user pay full amount, go to checkout (printing screen) and skip going to payment screen
      if (Cart.Ready)
      {
        //ShowTimeoutWarning = false;

        StopAllPaymentMethods();

        ProcessOrder();
      }
    }

    /// <summary>
    /// User want to extend timeout for the session
    /// </summary>
    private void _timeoutWarningVm_ExtendSession()
    {
      //mark session as extended, so timeout timer exxit 
      //Cart.ExtendTimeout = true;

      //hide timeout warning message
      //ShowTimeoutWarning = false;

      //start receiving money again
      //_selectedPm?.Start();

      //reset timeout
      LogSession(Cart.Session, $"User asked to extend session\n\t[Order Total: {CustomCeiling(Cart.TotalValue)}, Total Paid: {Cart.TotalPaid}]");
      CurrentVm = _orderPaymentVm;
      //Cart.ClearTimeoutWarning();
      StartTimer(_navigationTimer.Interval, $"ExtendSession()");
      if (_cashCode != null && _cashCode.IsConnected)
      {
        _cashCode.Start();
      }
    }

    /// <summary>
    /// User asked to finish the session manually
    /// </summary>
    private void _timeoutWarningVm_FinishNow()
    {
      LogSession(Cart.Session, "User clicked End Session from warning screen");
      EndSession(force: true);
      //Cart.ClearTimeoutWarning();
    }


    /// <summary>
    /// Go to payment step or to checkout if payment full filled
    /// </summary>
    private void _checkoutVm_GotoPayment()
    {
      PayOrder();
    }

    /// <summary>
    /// Only one payment method workin and client agreed to move on
    /// </summary>
    private void _onePaymentMethodWarning_StartTheSession()
    {
      StartingSessionNow();
    }


    private void _productTypeVm_ExitSessionNow()
    {
      ExitSession();
    }

    private void _serviceListVm_ExitSessionNow()
    {
      ExitSession();
    }

    private void _productListVm_ExitSessionNow()
    {
      ExitSession();
    }

    private void _selectPaymentMethodVm_ExitSessionNow()
    {
      ExitSession();
    }

    private void _orderPaymentVm_ExitSessionNow()
    {
      ExitSession();
    }

  }

}
