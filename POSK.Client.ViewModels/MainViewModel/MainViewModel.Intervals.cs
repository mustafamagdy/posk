using Geeky.POSK.Infrastructore.Extensions;

namespace POSK.Client.ViewModels
{
  public sealed partial class MainViewModel 
  {
    //todo move to terminal settings
    private int interval_default = 10.Secods();
    private int interval_services = 10.Secods();
    private int interval_productTypes = 10.Secods();
    private int interval_products = 10.Secods();
    private int interval_payment = 10.Secods();
    private int interval_checkout = 10.Secods();
    private int interval_noStock = 10.Secods();
    private int interval_onePaymentMethod = 10.Secods();
    private int interval_failedToPay = 7.Secods();
    private int interval_thankYou = 3.Secods();
    private int interval_failedException = 7.Secods();
    private int interval_timeoutWarning = 10.Secods();
    private int interval_printReceipt = 3.Secods();
    private int interval_sessionExpired = 3.Secods();
    private int interval_finishOrder = 5.Secods();

  }

}
