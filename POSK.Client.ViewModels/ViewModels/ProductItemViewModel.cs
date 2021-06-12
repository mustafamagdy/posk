using Geeky.POSK.WPF.Core.Base;
using Geeky.POSK.Models;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Geeky.POSK.DataContracts;

namespace POSK.Client.ViewModels
{
  public class ProductItemViewModel : BindableBaseViewModel
  {
    private Guid _productId;
    public Guid ProductId { get { return _productId; } set { SetProperty(ref _productId, value); } }


    private string _productTitle;
    public string ProductTitle { get { return _productTitle; } set { SetProperty(ref _productTitle, value); } }


    private decimal _price;
    public decimal Price { get { return _price; } set { SetProperty(ref _price, value); } }

    private bool _soldOut;
    public bool SoldOut { get { return _soldOut; } set { SetProperty(ref _soldOut, value); } }

    private byte[] _vendorLogo;
    public byte[] VendorLogo { get { return _vendorLogo; } set { SetProperty(ref _vendorLogo, value); } }

    private ProductDto _product;
    public ProductDto Product { get { return _product; } set { SetProperty(ref _product, value); } }

    public DelegateCommand PurchaseItem { get; private set; }

    public ProductItemViewModel()
    {
      PurchaseItem = new DelegateCommand(OnPurchaseItem);
    }

    public ProductItemViewModel(ProductDto prdouct, byte[] vendorLogo) : this()
    {
      this.Product = prdouct;
      Price = prdouct.Price;
      ProductTitle = LocalizationHelper.GetLocalized(prdouct);
      ProductId = prdouct.Id;
      VendorLogo = vendorLogo;
      SoldOut = prdouct.SoldOut;
    }

    public event Action<ProductItemViewModel> ItemPurchase = delegate { };
    private void OnPurchaseItem()
    {
      ItemPurchase(this);
    }


  }
}
