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
  public class OnePaymentMethodWarningViewModel : BindableBaseViewModel
  {
    private bool _cashCode;
    public bool CashCode { get { return _cashCode; } set { SetProperty(ref _cashCode, value); } }
    private bool _cardReader;
    public bool CardReader { get { return _cardReader; } set { SetProperty(ref _cardReader, value); } }
    private bool _printer;
    public bool Printer { get { return _printer; } set { SetProperty(ref _printer, value); } }

    public DelegateCommand MoveOn { get; private set; }
    public event Action StartTheSession = delegate { };

    public OnePaymentMethodWarningViewModel()
    {
      MoveOn = new DelegateCommand(OnMoveOn);
    }

    private void OnMoveOn()
    {
      StartTheSession();
    }
  }
}
