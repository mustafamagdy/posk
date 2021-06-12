using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSK.Client.CashCode.Interface
{

  // Delegate of the event to monitor the cassette
  public delegate void BillCassetteHandler(object Sender, BillCassetteEventArgs e);

  // Delegate of the event receiving the banknote
  //public delegate void BillReceivedHandler(object Sender, BillReceivedEventArgs e);

  // Delegate of the event in the process of sending a note on the stack (Here you can do a refund)
  //public delegate void BillStackingHandler(object Sender, BillStackedEventArgs e);

  // Delegate for event of cashcode failure, it also sends the error (code, spec_byte, message) in the event args
  public delegate void BillValidatorFailureHandler(object Sender, BillFailureEventArgs e);

  // Delegate for event that should causes end of session for the client
  public delegate void EndOfSessionHandler(object Sender, EndOfSessionEventArgs e);

  // Delegate for event that raise an error message to the client
  public delegate void RaiseToClienthandler(object Sender, RaisedDetailsEventArgs e);

  public delegate void LogToClientHandler(object sender, LogEventArgs e);

}
