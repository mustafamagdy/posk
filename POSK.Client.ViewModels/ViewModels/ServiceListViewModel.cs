using CommonServiceLocator;
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
  public class ServiceListViewModel : BindableBaseViewModel
  {
    private IClientService _clientSvc;
    private ObservableCollection<ServiceViewModel> _services;
    public ObservableCollection<ServiceViewModel> Services { get { return _services; } set { SetProperty(ref _services, value); } }


    public DelegateCommand GoToCheckout { get; private set; }
    public event Action GotoCheckout = delegate { };
    public event Action<ServiceViewModel> ServiceSelected = delegate { };

    private CheckoutViewModel _checkoutVm;
    public CheckoutViewModel CheckoutVm { get { return _checkoutVm; } set { SetProperty(ref _checkoutVm, value); } }

    public DelegateCommand ExitSession { get; private set; }
    public event Action ExitSessionNow = delegate { };

    public ServiceListViewModel()
    {
      _clientSvc = ServiceLocator.Current.GetInstance<IClientService>();
      Services = new ObservableCollection<ServiceViewModel>();
      GoToCheckout = new DelegateCommand(OnGoToCheckout);
      ExitSession = new DelegateCommand(OnExitSession);
    }
    private void OnExitSession()
    {
      ExitSessionNow();
    }

    private void OnGoToCheckout()
    {
      GotoCheckout();
    }

    public void LoadAvailableServices()
    {
      Services = new ObservableCollection<ServiceViewModel>();
      var availableServices = _clientSvc.GetAvailableService();
      foreach (var vendor in availableServices)
      {
        var service = new ServiceViewModel(vendor);
        service.ServiceSelected += Service_ServiceSelected;
        Services.Add(service);
      }
    }

    private void Service_ServiceSelected(ServiceViewModel service)
    {
      ServiceSelected(service);
    }

    public void UpdateCheckoutInfo(CheckoutViewModel vm)
    {
      CheckoutVm = vm;
    }

  }

}
