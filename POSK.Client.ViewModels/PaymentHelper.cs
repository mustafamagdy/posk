using Geeky.POSK.DataContracts;

namespace POSK.Client.ViewModels
{
  public class PaymentHelper
  {
    public static PaymentValueDto CashCode(decimal stackedAmount,decimal cashAmount, string rejectReason = "",
                                           string cashCodeStatus = "", bool isJammed = false)
    {
      return new PaymentValueDto
      {
        CashCodeStatus = cashCodeStatus,
        IsJammed = isJammed,
        PayMethod = PaymentTypeConstants.CASH_CODE,
        StackedAmount = stackedAmount,
        CashAmount = cashAmount,
        RejectionReason = rejectReason,
        PaymentRefNumber = RefNumberGenerator.random()
      };
    }
  }
}
