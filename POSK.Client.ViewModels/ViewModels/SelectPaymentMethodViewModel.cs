using CommonServiceLocator;
using Geeky.POSK.Infrastructore.Core;
using Geeky.POSK.Models;
using Geeky.POSK.WPF.Core.Base;
using Microsoft.Practices.Prism.Commands;
using POSK.Payment.Interface;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace POSK.Client.ViewModels
{
  public class SelectPaymentMethodViewModel : BindableBaseViewModel
  {
    private decimal _totalValue = 0;
    public decimal TotalValue { get { return _totalValue; } set { SetProperty(ref _totalValue, value); } }

    private decimal _totalPaid = 0;
    public decimal TotalPaid { get { return _totalPaid; } set { SetProperty(ref _totalPaid, value); } }

    private decimal _remaining = 0;
    public decimal Remaining { get { return _remaining; } set { SetProperty(ref _remaining, value); } }

    private ObservableCollection<PaymentMethod> _paymentMethods;
    public ObservableCollection<PaymentMethod> PaymentMethods { get { return _paymentMethods; } set { SetProperty(ref _paymentMethods, value); } }

    private string _descriptionImageName;
    public string DescriptionImageName { get { return _descriptionImageName; } set { SetProperty(ref _descriptionImageName, value); } }

    private Guid _vendorId;
    private byte[] _vendorLogo;
    private ProductTypeEnum _productType;

    //public DelegateCommand ModifyOrder { get; private set; }
    public DelegateCommand<PaymentMethod> SelectPaymentMethod { get; private set; }
    public DelegateCommand GoBackToProducts { get; private set; }

    //public event Action GoToShoppingCart = delegate { };
    public event Action<PaymentMethod> PaymentMethodSelected = delegate { };
    public event Action<ProductTypeEnum, Guid, byte[]> BackToProducts = delegate { };

    public DelegateCommand ExitSession { get; private set; }
    public event Action ExitSessionNow = delegate { };

    public SelectPaymentMethodViewModel()
    {
      PaymentMethods = new ObservableCollection<PaymentMethod>();
     
      //ModifyOrder = new DelegateCommand(OnModifyOrder);
      SelectPaymentMethod = new DelegateCommand<PaymentMethod>(OnSelectPaymentMethod);
      GoBackToProducts = new DelegateCommand(OnGoBackToProducts);
      ExitSession = new DelegateCommand(OnExitSession);
    }
    private void OnExitSession()
    {
      ExitSessionNow();
    }

    private void OnSelectPaymentMethod(PaymentMethod pm)
    {
      DescriptionImageName = pm.DescriptionImageName;
      PaymentMethodSelected(pm);
    }

    //private void OnModifyOrder()
    //{
    //  GoToShoppingCart();
    //}

    private void OnGoBackToProducts()
    {
      BackToProducts(_productType, _vendorId, _vendorLogo);
    }

    public void SetPaymentMethods(List<IPaymentMethod> paymentMethods)
    {
      PaymentMethods = new ObservableCollection<PaymentMethod>();
      //var paymentMethods = ServiceLocator.Current.GetAllInstances<IPaymentMethod>();
      foreach (var pm in paymentMethods)
      {
        PaymentMethods.Add(new PaymentMethod { Code = pm.Code, Name = pm.Name, ButtonImageName = pm.ButtonImageName, DescriptionImageName = pm.DescriptionImageName });
      }
    }

    public void SetCart(UserCart cart)
    {
      TotalValue = cart.TotalValue;
      TotalPaid = cart.TotalPaid;
      Remaining = cart.Remaining;
    }

    public void SetSelectedService(ProductTypeEnum productType, Guid vendorId, byte[] vendorLogo)
    {
      _productType = productType;
      _vendorId = vendorId;
      _vendorLogo = vendorLogo;
    }

  }

}
