using CommonServiceLocator;
using Geeky.POSK.Infrastructore.Core.Extensions;
using Geeky.POSK.Models;
using Geeky.POSK.Repository.Interfface;
using Geeky.POSK.WPF.Core.Base;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace POSK.Client.ViewModels
{
  public class CheckoutViewModel : BindableBaseViewModel
  {   
    private int _cartItemsCount;
    public int CartItemsCount { get { return _cartItemsCount; } set { SetProperty(ref _cartItemsCount, value); } }

    private decimal _totalValue;
    public decimal TotalValue { get { return _totalValue; } set { SetProperty(ref _totalValue, value); } }

    private decimal _totalPaid;
    public decimal TotalPaid { get { return _totalPaid; } set { SetProperty(ref _totalPaid, value); } }

    private decimal _remaining;
    public decimal Remaining { get { return _remaining; } set { SetProperty(ref _remaining, value); } }

    private bool _hasItems;
    public bool HasItems { get { return _hasItems; } set { SetProperty(ref _hasItems, value); } }

    public DelegateCommand PayOrder { get; private set; }
    public event Action GotoPayment = delegate { };

    public CheckoutViewModel()
    {
      PayOrder = new DelegateCommand(OnGoToOrderPayment);
    }

    private void OnGoToOrderPayment()
    {
      GotoPayment();
    }

    public void SetCart(UserCart cart)
    {
      CartItemsCount = cart.ItemCount;
      HasItems = cart.ItemCount > 0;
      TotalValue = cart.TotalValue;
      TotalPaid = cart.TotalPaid;
      Remaining = cart.Remaining;
    }

  }
}
