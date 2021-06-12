using System.Collections.Generic;

namespace POSK.Client.CashCode.Interface
{
  public sealed class CashCodeErroList
  {
    public Dictionary<int, string> Errors { get; private set; }
    public List<int> FailureErrors { get; private set; }
    public List<int> EndOfSessionErrors { get; private set; }
    public List<int> RaisedTouserErrors { get; private set; }


    public CashCodeErroList()
    {
      Errors = new Dictionary<int, string>();
      FailureErrors = new List<int>();
      EndOfSessionErrors = new List<int>();
      RaisedTouserErrors = new List<int>();

      //100000 - 100060 general software errors, should be logged and stop this payment method temporary
      Errors.Add(100000, "Unknown error");

      Errors.Add(100010, "Error opening Com-port");
      Errors.Add(100020, "Com port is not open");
      Errors.Add(100030, "An error occurred when sending the bill acceptor command.");
      Errors.Add(100040, "An error occurred when sending the bill acceptor command. The POWER UP command was not received from the bill acceptor.");
      Errors.Add(100050, "An error occurred when sending the bill acceptor command. The ACK team was not received from the bill acceptor.");
      Errors.Add(100060, "An error occurred when sending the bill acceptor command. The INITIALIZE command was not received from the bill acceptor.");

      //41H, 42H, 43H, 44H errors regarding  cassette, disable this payment method
      Errors.Add(100070, "Error checking the status of the bill acceptor. The plug is removed.");
      Errors.Add(100080, "Error checking the status of the bill acceptor. The Stacker is full.");
      Errors.Add(100090, "Error checking the status of the bill acceptor. A bill was stuck in the validator.");
      Errors.Add(100100, "Error checking the status of the bill acceptor. A bill was stuck in the stack.");

      //end session normally
      Errors.Add(100110, "Error checking the status of the bill acceptor. A counterfeit bill.");
      //show error, please remove the bill untill previous one is processed
      Errors.Add(100120, "Error checking the status of the bill acceptor. The previous note has not yet hit the stack and is in the recognition mechanism.");

      //47H then 50H to 5FH, disable the cash payment method
      Errors.Add(100130, "Error in the bill acceptor operation. Stack Motor Failure.");
      Errors.Add(100140, "Error in the bill acceptor operation. Transport Motor Speed Failure.");
      Errors.Add(100150, "Error in the bill acceptor operation. Transport Motor Failure.");
      Errors.Add(100160, "Error in the bill acceptor operation. Aligning Motor Failure.");
      Errors.Add(100170, "Error in the bill acceptor operation. Initial Cassette Status Failure.");
      Errors.Add(100180, "Error in the bill acceptor operation. Optic Canal Failure.");
      Errors.Add(100190, "Error in the bill acceptor operation. Magnetic Canal Failure.");
      Errors.Add(100200, "Error in the bill acceptor operation. Capacitance Canal Failure.");

      // Known errors, log them into current session
      Errors.Add(0x60, "Rejecting due to Insertion");
      Errors.Add(0x61, "Rejecting due to Magnetic");
      Errors.Add(0x62, "Rejecting due to Remained bill in head");
      Errors.Add(0x63, "Rejecting due to Multiplying");
      Errors.Add(0x64, "Rejecting due to Conveying");
      Errors.Add(0x65, "Rejecting due to Identification1");
      Errors.Add(0x66, "Rejecting due to Verification");
      Errors.Add(0x67, "Rejecting due to Optic");
      Errors.Add(0x68, "Rejecting due to Inhibit");
      Errors.Add(0x69, "Rejecting due to Capacity");
      Errors.Add(0x6A, "Rejecting due to Operation");
      Errors.Add(0x6C, "Rejecting due to Length");




      //Failure errors
      FailureErrors.Add(100070); // Error checking the status of the bill acceptor. The plug is removed.
      FailureErrors.Add(100080);//Error checking the status of the bill acceptor. The Stacker is full.
      FailureErrors.Add(100090);//Error checking the status of the bill acceptor. A bill was stuck in the validator.
      FailureErrors.Add(100100);//Error checking the status of the bill acceptor. A bill was stuck in the stack.

      FailureErrors.Add(100130);//Error in the bill acceptor operation. Stack Motor Failure.
      FailureErrors.Add(100140);//Error in the bill acceptor operation. Transport Motor Speed Failure.
      FailureErrors.Add(100150);//Error in the bill acceptor operation. Transport Motor Failure.
      FailureErrors.Add(100160);//Error in the bill acceptor operation. Aligning Motor Failure.
      FailureErrors.Add(100170);//Error in the bill acceptor operation. Initial Cassette Status Failure.
      FailureErrors.Add(100180);//Error in the bill acceptor operation. Optic Canal Failure.
      FailureErrors.Add(100190);//Error in the bill acceptor operation. Magnetic Canal Failure.
      FailureErrors.Add(100200);//Error in the bill acceptor operation. Capacitance Canal Failure.
                                ///////////////////////////////////////////////////////////////////////////////////////

      //Errors that should end session
      EndOfSessionErrors.Add(100110);//Error checking the status of the bill acceptor. A counterfeit bill.

      ///////////////////////////////////////////////////////////////////////////////////////

      //Errors that should pop a message to the user
      RaisedTouserErrors.Add(100120);//Error checking the status of the bill acceptor. The previous note has not yet hit the stack and is in the recognition mechanism.

      ///////////////////////////////////////////////////////////////////////////////////////


    }
  }

}
