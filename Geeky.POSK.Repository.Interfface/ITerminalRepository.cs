using Geeky.POSK.Models;
using Geeky.POSK.ORM.Contect.Core;
using Geeky.POSK.Repository.Core.Interface.Base;
using System;
using System.Collections.Generic;

namespace Geeky.POSK.Repository.Interfface
{
  public interface ITerminalRepository : IBaseRepository<Terminal, Guid>//, IDataContext<Terminal, Guid>>
  {
    IEnumerable<Terminal> GetActiveTerminals();
    Terminal GetServerTerminal();
    Terminal CreateSererTerminal();
  }
}
