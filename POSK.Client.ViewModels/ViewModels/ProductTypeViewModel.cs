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
  public class ProductTypeViewModel : BindableBaseViewModel
  {
    private Guid _vendorId;
    private byte[] _vendorLogo;

    public DelegateCommand GoBackToServices { get; private set; }
    public DelegateCommand DataSelected { get; private set; }
    public DelegateCommand VoiceSelected { get; private set; }

    public event Action BackToServices = delegate { };
    public event Action<ProductTypeEnum, Guid, byte[]> ProductTypeSelected = delegate { };

    private CheckoutViewModel _checkoutVm;
    public CheckoutViewModel CheckoutVm { get { return _checkoutVm; } set { SetProperty(ref _checkoutVm, value); } }

    public DelegateCommand ExitSession { get; private set; }
    public event Action ExitSessionNow = delegate { };

    public ProductTypeViewModel()
    {
      DataSelected = new DelegateCommand(OnDataSelected);
      VoiceSelected = new DelegateCommand(OnVoiceSelected);
      GoBackToServices = new DelegateCommand(OnGoBackToServices);
      ExitSession = new DelegateCommand(OnExitSession);
    }
    private void OnExitSession()
    {
      ExitSessionNow();
    }

    private void OnDataSelected()
    {
      ProductTypeSelected(ProductTypeEnum.Data, _vendorId, _vendorLogo);
    }
    private void OnVoiceSelected()
    {
      ProductTypeSelected(ProductTypeEnum.Voice, _vendorId, _vendorLogo);
    }

    private void OnGoBackToServices()
    {
      BackToServices();
    }
    public void LoadProductTypes(Guid vendorId, byte[] vendorLogo)
    {
      _vendorId = vendorId;
      _vendorLogo = vendorLogo;

      //todo mark for sold out
    }


  }
}
