using Geeky.POSK.Models;
using Geeky.POSK.ORM.Contect.Core;
using Geeky.POSK.Repository.Core.Interface.Base;
using System;
using System.Collections.Generic;

namespace Geeky.POSK.Repository.Interfface
{
  public interface ITerminalLogRepository : IBaseRepository<TerminalLog, Guid>
  {
    
  }
}
