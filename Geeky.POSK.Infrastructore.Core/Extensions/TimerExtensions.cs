using NLog;
using System;
using System.Windows.Forms;
using System.Windows.Threading;

namespace Geeky.POSK.Infrastructore.Extensions
{
  public static class TimerExtensions
  {
    private static Logger logger = NLog.LogManager.GetCurrentClassLogger();
    private static Action<int, string> LogTimer = (interval, s) => { logger.Trace($"[Timer({interval})] -> {s}"); };

    public static void Start(this DispatcherTimer timer, int interval, string whoStartMe)
    {
      LogTimer(interval, whoStartMe);

      if (timer == null) return;

      timer.IsEnabled = false;
      timer.Stop();
      timer = new DispatcherTimer();
      timer.Interval = TimeSpan.FromMilliseconds(interval);
      timer.IsEnabled = true;
    }

    public static int Secods(this int seconds)
    {
      return seconds * 1000;
    }

    public static int Minutes(this double minutes)
    {
      return (int)(minutes * 60.0 * 1000.0);
    }

    public static int Hourse(this double hours)
    {
      return (int)(hours * 60.0 * 60.0 * 1000.0);
    }
  }
}