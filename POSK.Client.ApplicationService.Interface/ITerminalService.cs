using Geeky.POSK.DataContracts;
using System;
using System.Threading.Tasks;

namespace POSK.Client.ApplicationService.Interface
{
  public interface ITerminalService : IClientApplicationService
  {
    void CreateTerminal(Guid terminalId, string code, string terminalKey, string IP, 
                        string machineName, bool approved = true);
    void CreateTerminal(RegistrationRespopnseDto result, string IP, string machineName);

    //void CheckNewPins(Guid terminalId);
  }
}
