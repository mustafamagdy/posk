using Geeky.POSK.WPF.Core.Base;
using Microsoft.Practices.Prism.Commands;
using System;
using Geeky.POSK.DataContracts;

namespace POSK.Client.ViewModels
{
  public class ServiceViewModel : BindableBaseViewModel
  {
    private Guid _vendorId;
    public Guid VendorId { get { return _vendorId; } set { SetProperty(ref _vendorId, value); } }

    private string _title;
    public string Title { get { return _title; } set { SetProperty(ref _title, value); } }

    private bool _soldOut;
    public bool SoldOut { get { return _soldOut; } set { SetProperty(ref _soldOut, value); } }

    private byte[] _vendorLogo;
    public byte[] VendorLogo { get { return _vendorLogo; } set { SetProperty(ref _vendorLogo, value); } }

    public DelegateCommand SelectService { get; private set; }

    public ServiceViewModel()
    {
      SelectService = new DelegateCommand(OnServiceClicked);
    }
    public ServiceViewModel(VendorDto vendor) : this()
    {
      Title = LocalizationHelper.GetLocalized(vendor);
      VendorId = vendor.Id;
      VendorLogo = vendor.Logo;
      SoldOut = vendor.SoldOut;
    }


    public event Action<ServiceViewModel> ServiceSelected = delegate { };
    private void OnServiceClicked()
    {
      ServiceSelected(this);
    }
  }


}
