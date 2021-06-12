using NLog;
using System;

namespace POSK.Client.CashCode.Interface
{
  public class LogEventArgs : EventArgs
  {
    public string Message { get; private set; }
    public string ExtraDetails { get; private set; }
    public LogLevel Level { get; private set; }
    public LogEventArgs(string message, string extraDetails)
    {
      this.Message = message;
      this.ExtraDetails = extraDetails;
    }

    public static LogEventArgs Info(string message, string extraDetails = "")
    {
      var log = new LogEventArgs(message, extraDetails);
      log.Level = LogLevel.Info;
      return log;
    }

    public static LogEventArgs Warn(string message, string extraDetails = "")
    {
      var log = new LogEventArgs(message, extraDetails);
      log.Level = LogLevel.Warn;
      return log;
    }

    public static LogEventArgs Error(string message, string extraDetails = "")
    {
      var log = new LogEventArgs(message, extraDetails);
      log.Level = LogLevel.Error;
      return log;
    }

    public static LogEventArgs Trace(string message, string extraDetails = "")
    {
      var log = new LogEventArgs(message, extraDetails);
      log.Level = LogLevel.Trace;
      return log;
    }
    public static LogEventArgs Debug(string message, string extraDetails = "")
    {
      var log = new LogEventArgs(message, extraDetails);
      log.Level = LogLevel.Debug;
      return log;
    }

  }
}
