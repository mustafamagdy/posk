using Geeky.POSK.DataContracts.Base;
using System.Runtime.Serialization;

namespace Geeky.POSK.DataContracts
{
  [DataContract]
  public class TerminalSettingDto : BaseDto
  {
    [DataMember] public bool DisableTerminal { get; set; }
    [DataMember] public bool DisableShoppingCart { get; set; }

  }
}
