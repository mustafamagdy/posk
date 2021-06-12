using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geeky.POSK.Infrastructore.Core.Enums
{
  public enum ServerFaults
  {
    SERVER_TERMINAL_ALREADY_FOUND = 1010,

    PIN_DONT_BELONG_TO_TERMINAL = 2000,

    DATA_VALIDATION_ERROR = 9998,
    INTERNAL_ERROR = 9999,
  }
}
