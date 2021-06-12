using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSK.Client.CashCode.Interface
{
  public enum BillValidatorCommands
  {
    ACK = 0x00, NAK = 0xFF, POLL = 0x33, RESET = 0x30, GET_STATUS = 0x31, SET_SECURITY = 0x32,
    IDENTIFICATION = 0x37, ENABLE_BILL_TYPES = 0x34, STACK = 0x35, RETURN = 0x36, HOLD = 0x38
  }
  public enum ResponseType { ACK, NAK };

  public enum BillCassetteStatus { Inplace, Removed };

}
