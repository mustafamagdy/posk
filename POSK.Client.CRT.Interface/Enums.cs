using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSK.Client.CRT.Interface
{
  public enum CRTCommands
  {
    INIT = 0x30, LATCH = 0xB0, SN = 0xA2, STATUS = 0X31, LED = 0X80, AUTO_TEST = 0X50, 
    CPU_CARD_OP = 0X51, SLE_CARD_OP = 0X53,
    MAG_CARD_OP = 0X36, SAM_CARD_OP = 0X52, IC_CARD_OP = 0X54, RF_CARD_OP = 0X60
  }

  public enum Cmd_Power { OFF = 0X31, STATUS = 0X32, }



  public enum CRTConnectionType { USB, Serial }
}
