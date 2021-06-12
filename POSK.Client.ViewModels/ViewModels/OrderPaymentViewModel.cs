using CommonServiceLocator;
using Geeky.POSK.Infrastructore.Core;
using Geeky.POSK.Infrastructore.Core.Extensions;
using Geeky.POSK.Models;
using Geeky.POSK.Repository.Interfface;
using Geeky.POSK.WPF.Core.Base;
using Microsoft.Practices.Prism.Commands;
using POSK.Payment.Interface;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace POSK.Client.ViewModels
{
  public class OrderPaymentViewModel : BindableBaseViewModel
  {
    private decimal _totalValue = 0;
    public decimal TotalValue { get { return _totalValue; } set { SetProperty(ref _totalValue, value); } }

    private decimal _totalPaid = 0;
    public decimal TotalPaid { get { return _totalPaid; } set { SetProperty(ref _totalPaid, value); } }

    private decimal _remaining = 0;
    public decimal Remaining { get { return _remaining; } set { SetProperty(ref _remaining, value); } }

    private string _descriptionImageName;
    public string DescriptionImageName { get { return _descriptionImageName; } set { SetProperty(ref _descriptionImageName, value); } }

    private bool _allowGoBack;
    public bool AllowGoBack { get { return _allowGoBack; } set { SetProperty(ref _allowGoBack, value); } }


    private Guid _vendorId;
    private byte[] _vendorLogo;
    private ProductTypeEnum _productType;

    //public DelegateCommand ModifyOrder { get; private set; }
    public DelegateCommand GoBackToSelectPaymentMethod { get; private set; }

    //public event Action GoToShoppingCart = delegate { };
    public event Action<ProductTypeEnum, Guid, byte[]> BackToSelectPaymentMethod = delegate { };
    public DelegateCommand ExitSession { get; private set; }
    public event Action ExitSessionNow = delegate { };


    public OrderPaymentViewModel()
    {
      //ModifyOrder = new DelegateCommand(OnModifyOrder);
      GoBackToSelectPaymentMethod = new DelegateCommand(OnGoBackToSelectPaymentMethod);
      ExitSession = new DelegateCommand(OnExitSession);
    }
    private void OnExitSession()
    {
      ExitSessionNow();
    }

    //private void OnModifyOrder()
    //{
    //  GoToShoppingCart();
    //}

    private void OnGoBackToSelectPaymentMethod()
    {
      BackToSelectPaymentMethod(_productType, _vendorId, _vendorLogo);
    }
    public void SetCart(UserCart cart)
    {
      TotalValue = cart.TotalValue;
      TotalPaid = cart.TotalPaid;
      Remaining = cart.Remaining;

      //allow go back only if he didn't pay anything
      AllowGoBack = TotalPaid < 1;
    }

    public void SetSelectedService(ProductTypeEnum productType, Guid vendorId, byte[] vendorLogo)
    {
      _productType = productType;
      _vendorId = vendorId;
      _vendorLogo = vendorLogo;
    }

    public void SetSelectedPaymentMethod(PaymentMethod pm)
    {
      DescriptionImageName = pm.DescriptionImageName;
    }

  }

}
