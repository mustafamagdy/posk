using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geeky.POSK.Infrastructore.Core.Logging
{
  public interface ILogger
  {
    void Debug(object message);
    void Info(object message);
    void Warn(object message);
    void Error(object message);
    void Fatal(object message);
  }
  public class Logger : ILogger
  {
    public void Debug(object message)
    {
#if DEBUG
      System.Diagnostics.Debug.WriteLine(message);
#endif
    }

    public void Error(object message)
    {
      Trace.WriteLine(message);
    }

    public void Fatal(object message)
    {
      Trace.WriteLine(message);
    }

    public void Info(object message)
    {
      Trace.WriteLine(message);
    }

    public void Warn(object message)
    {
      Trace.WriteLine(message);
    }
  }
}
