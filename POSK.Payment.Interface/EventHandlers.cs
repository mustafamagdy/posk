using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSK.Payment.Interface
{
  // Delegate of the event receiving the banknote
  public delegate void AmountReceivedHandler(object Sender, AmountReceivedEventArgs e);

  // Delegate of the event in the process of sending a note on the stack (Here you can do a return)
  public delegate void AmountReceivingHandler(object Sender, AmountReceivingEventArgs e);
}
