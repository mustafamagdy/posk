using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSK.Payment.Interface
{
  public interface IPaymentMethod : IDisposable
  {
    bool IsConnected { get; }
    string Code { get; }
    string Name { get; }
    string ButtonImageName { get; }
    string DescriptionImageName { get; }

    int Enable();
    int Disable();

    int PowerUp();
    int Connect();

    void Start();
    void Stop();
    void StopListening();

    event AmountReceivedHandler AmountReceived;
    event AmountReceivingHandler AmountReceiving;
#if DEBUG
    decimal _required { get; set; }
#endif

    /// <summary>
    /// This will be called to check if this method is working 
    /// to display a notification to the user to tell him it is not
    /// working if it is not
    /// </summary>
    /// <returns>true if working, false if else</returns>
    bool IsWorking();
  }
}
