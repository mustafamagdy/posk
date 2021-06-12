using CommonServiceLocator;
using Geeky.POSK.Infrastructore.Core;
using Geeky.POSK.WPF.Core.Base;
using Microsoft.Practices.Prism.Commands;
using POSK.Client.ApplicationService.Interface;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSK.Client.ViewModels
{
  public class ProductListViewModel : BindableBaseViewModel
  {
    private Guid _vendorId;
    private byte[] _vendorLogo;

    private IClientService _pinService;
    //this is actually the product, but we will purchase pin from it, so the pricing are on product
    private ObservableCollection<ProductItemViewModel> _products;
    public ObservableCollection<ProductItemViewModel> Products { get { return _products; } set { SetProperty(ref _products, value); } }

    public DelegateCommand GoToCheckout { get; private set; }
    public DelegateCommand ExitSession { get; private set; }
    public event Action ExitSessionNow = delegate { };
    public DelegateCommand GoBackToProductTypes { get; private set; }
    public event Action GotoCheckout = delegate { };
    public event Action<Guid, byte[]> BackToProductTypes = delegate { };
    public event Action<ProductItemViewModel> ItemPurchased = delegate { };

    //private CheckoutViewModel _checkoutVm;
    //public CheckoutViewModel CheckoutVm { get { return _checkoutVm; } set { SetProperty(ref _checkoutVm, value); } }

    public ProductListViewModel()
    {
      GoToCheckout = new DelegateCommand(OnGotoCheckout);
      GoBackToProductTypes = new DelegateCommand(OnGoBackToProductTypes);
      _pinService = ServiceLocator.Current.GetInstance<IClientService>();
      Products = new ObservableCollection<ProductItemViewModel>();
      ExitSession = new DelegateCommand(OnExitSession);
    }
    private void OnExitSession()
    {
      ExitSessionNow();
    }
    private void OnGotoCheckout()
    {
      GotoCheckout();
    }

    private void OnGoBackToProductTypes()
    {
      BackToProductTypes(_vendorId, _vendorLogo);
    }
    public void LoadProducts(ProductTypeEnum productType, Guid vendorId, byte[] vendorLogo)
    {
      _vendorId = vendorId;
      _vendorLogo = vendorLogo;

      Products = new ObservableCollection<ProductItemViewModel>();
      var products = _pinService.GetVendorProducts(vendorId, productType);
      foreach (var p in products)
      {
        var product = new ProductItemViewModel(p, vendorLogo);
        product.ItemPurchase += Product_ItemPurchase;
        Products.Add(product);
      }
    }

    private void Product_ItemPurchase(ProductItemViewModel product)
    {
      //just notify parent and it will handle purchasing process, don't purchase here
      ItemPurchased(product);
    }

    public void UpdateCheckoutInfo(CheckoutViewModel vm)
    {
      //CheckoutVm = vm;
    }
  }
}
