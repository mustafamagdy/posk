using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Geeky.POSK.WPF.Core.Base
{
  [DataContract]
  public abstract class BaseTimerViewModel : BindableBaseViewModel
  {
    private Timer _timer;
    private int _timerInterval;
    private Action CallBack = null;
    public BaseTimerViewModel(int timerInterval)
    {
      _timerInterval = timerInterval;
      CallBack = () => { };
      ResetTimer();
    }

    public void ResetTimer()
    {
      if (_timer != null)
        _timer.Stop();
      _timer = new Timer();
      _timer.Interval = _timerInterval;
      _timer.Elapsed += _timer_Elapsed;
    }

    public void Start(Action callBack)
    {
      ResetTimer();
      CallBack = callBack;
      _timer.Start();
    }

    private void _timer_Elapsed(object sender, ElapsedEventArgs e)
    {
      _timer.Stop();
      CallBack();
      ResetTimer();
    }
  }
}
