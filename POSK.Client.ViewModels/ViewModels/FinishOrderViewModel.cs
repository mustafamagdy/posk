using Geeky.POSK.WPF.Core.Base;
using Microsoft.Practices.Prism.Commands;
using System;

namespace POSK.Client.ViewModels
{
  public class FinishOrderViewModel : BindableBaseViewModel
  {
    public DelegateCommand ModifyOrder { get; private set; }
    public DelegateCommand ReceiveOrder { get; private set; }

    public event Action GoToShoppingCart = delegate { };
    public event Action FinilizeOrder = delegate { };

    public FinishOrderViewModel()
    {
      ModifyOrder = new DelegateCommand(OnGoToShoppingCart);
      ReceiveOrder = new DelegateCommand(OnFinilizeOrder);
    }

    private void OnFinilizeOrder()
    {
      FinilizeOrder();
    }

    private void OnGoToShoppingCart()
    {
      GoToShoppingCart();
    }
  }

}
