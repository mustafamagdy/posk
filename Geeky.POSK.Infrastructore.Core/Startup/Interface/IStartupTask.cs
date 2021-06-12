using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geeky.POSK.Infrastructore.Core.Startup
{
  public interface IStartupTask
  {
    void Execute();
  }
}
