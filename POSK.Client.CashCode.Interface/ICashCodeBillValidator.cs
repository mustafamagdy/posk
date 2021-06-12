using POSK.Payment.Interface;
using System;

namespace POSK.Client.CashCode.Interface
{
  public interface ICashCodeBillValidator : IPaymentMethod
  {
    event BillCassetteHandler BillCassetteStatusEvent;
    event BillValidatorFailureHandler BillFailure;
    event EndOfSessionHandler EndOfSession;
    event RaiseToClienthandler RaiseToClient;
    event LogToClientHandler LogToClient;

  }
}