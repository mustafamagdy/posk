using AutoMapper;
using CommonServiceLocator;
using Geeky.POSK.DataContracts;
using Geeky.POSK.Models;
using Geeky.POSK.Repository.Interfface;
using Geeky.POSK.WPF.Core.Base;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Drawing;

namespace Geeky.POSK.Server.ViewModels
{
  public class VendorViewModel : BindableBaseViewModel
  {
    private VendorDto _dto;
    public VendorDto Dto { get { return _dto; } set { SetProperty(ref _dto, value); } }

    public DelegateCommand SaveItem { get; private set; }
    public DelegateCommand CancelSave { get; private set; }

    public event Action Close = delegate { };
    public VendorViewModel(VendorDto dto)
    {
      Dto = dto ?? new VendorDto();
      SaveItem = new DelegateCommand(OnSaveItem);
      CancelSave = new DelegateCommand(OnCancelSave);
    }

    private void OnSaveItem()
    {
      //todo logic to save
      var repo = ServiceLocator.Current.GetInstance<IVendorRepository>();
      if (Dto.Id == Guid.Empty)
        repo.Add(Mapper.Map<Vendor>(Dto));
      else
      {
        var _vendor = repo.Get(Dto.Id);
        Mapper.Map(Dto, _vendor);
        repo.Update(_vendor);
      }

      Close();
    }
    private void OnCancelSave()
    {
      if (Dto != null && Dto.Id != Guid.Empty)
      {
        var repo = ServiceLocator.Current.GetInstance<IVendorRepository>();
        var _vendor = repo.Get(Dto.Id);
        Dto = null;
        Dto = Mapper.Map<VendorDto>(_vendor);
      }

      Close();
    }
  }
}
