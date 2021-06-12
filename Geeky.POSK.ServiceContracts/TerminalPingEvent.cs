using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geeky.POSK.ServiceContracts
{
  //public class TerminalEventArg : EventArgs
  //{
  //  public Guid TerminalId { get; set; }
  //  public TerminalEventArg(Guid terminalId)
  //  {
  //    this.TerminalId = terminalId;
  //  }
  //}

  //public delegate void TerminalPingHandler(TerminalEventArg args);

  ////Thia won't work :D as this is called from service and you want to handle it on the dashboard app
  //// another solution could be to use a callback from the service when this event fired on the service
  //// maybe will do it later
  //public static class TerminalPingEvent
  //{
  //  public static event TerminalPingHandler Ping = delegate { };
  //  public static void RaisePingEvent(Guid terminaId)
  //  {
  //    if (Ping != null)
  //      Ping.Invoke(new TerminalEventArg(terminaId));
  //  }
  //}
}
