using Geeky.POSK.WPF.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace POSK.Client.ViewModels
{
  public class SessionExpiredViewModel : BindableBaseViewModel
  {
    private string _content;
    public string Content { get { return _content; } set { SetProperty(ref _content, value); } }

    private bool _hasPayment;
    public bool HasPayment { get { return _hasPayment; } set { SetProperty(ref _hasPayment, value); } }
    private bool _hasNoPayment;
    public bool HasNoPayment { get { return _hasNoPayment; } set { SetProperty(ref _hasNoPayment, value); } }

    public SessionExpiredViewModel()
    {
    }

    public void ShowUserSessionInfo(UserCart cart)
    {

    }
  }
}
