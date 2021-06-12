using Geeky.POSK.WPF.Core.Base;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace POSK.Client.ViewModels
{
  public class TimeoutWarningViewModel : BindableBaseViewModel
  {
    public DelegateCommand ExtendTimeout { get; private set; }
    public event Action ExtendSession = delegate { };
    public DelegateCommand FinishOrder { get; private set; }
    public event Action FinishNow = delegate { };

    public TimeoutWarningViewModel()
    {
      ExtendTimeout = new DelegateCommand(OnExtendSession);
      FinishOrder = new DelegateCommand(OnFinishOrder);
    }

    private void OnExtendSession()
    {
      ExtendSession();
    }
    private void OnFinishOrder()
    {
      FinishNow();
    }
    public void ShowTimerFor(int timerInterval)
    {

    }

    internal void StopTimer()
    {

    }
  }
}
