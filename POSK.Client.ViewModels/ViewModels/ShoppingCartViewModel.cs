using CommonServiceLocator;
using Geeky.POSK.DataContracts;
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
  public class ShoppingCartViewModel : BindableBaseViewModel
  {
    private ObservableCollection<EncryptedPinDto> _cartItems;
    public ObservableCollection<EncryptedPinDto> CartItems { get { return _cartItems; } set { SetProperty(ref _cartItems, value); } }

    public DelegateCommand PayOrder { get; private set; }
    public DelegateCommand ContinueShopping { get; private set; }
    public DelegateCommand<Guid?> RemoveItemFromCart { get; private set; }

    private decimal _totalValue;
    public decimal TotalValue { get { return _totalValue; } set { SetProperty(ref _totalValue, value); } }

    private decimal _totalPaid;
    public decimal TotalPaid { get { return _totalPaid; } set { SetProperty(ref _totalPaid, value); } }

    public event Action GotoPayment = delegate { };
    public event Action ContinueShoppingCart = delegate { };
    public event Action<Guid> RemoveItemInCart = delegate { };

    public ShoppingCartViewModel()
    {
      PayOrder = new DelegateCommand(OnGoToOrderPayment);
      ContinueShopping = new DelegateCommand(OnContinueShopping);
      RemoveItemFromCart = new DelegateCommand<Guid?>(OnRemoveItemFromCart);
    }

    private void OnRemoveItemFromCart(Guid? id)
    {
      RemoveItemInCart(id.Value);
    }

    private void OnContinueShopping()
    {
      ContinueShoppingCart();
    }

    private void OnGoToOrderPayment()
    {
      GotoPayment();
    }

    internal void SetCart(UserCart cart)
    {
      CartItems = new ObservableCollection<EncryptedPinDto>(cart.Items);
      TotalValue = cart.TotalValue;
      TotalPaid = cart.TotalPaid;
    }

    public string GetLocalized(string[] allLangs)
    {
      return LocalizationHelper.GetLocalized(allLangs);
    }
  }
}
