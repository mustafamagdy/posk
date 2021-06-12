using AutoMapper;
using CommonServiceLocator;
using Geeky.POSK.DataContracts;
using Geeky.POSK.Infrastructore.Core;
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

namespace Geeky.POSK.Server.ViewModels
{
  public class ProductViewModel : BindableBaseViewModel
  {

    private ProductDto _dto;
    public ProductDto Dto { get { return _dto; } set { SetProperty(ref _dto, value); } }

    private ObservableCollection<VendorDto> _vendors;
    public ObservableCollection<VendorDto> Vendors { get { return _vendors; } set { SetProperty(ref _vendors, value); } }

    private ObservableCollection<string> _productTypes;
    public ObservableCollection<string> ProductTypes { get { return _productTypes; } set { SetProperty(ref _productTypes, value); } }

    public DelegateCommand SaveItem { get; private set; }
    public DelegateCommand CancelSave { get; private set; }

    public event Action Close = delegate { };
    public ProductViewModel(ProductDto dto)
    {
      var vendorRepo = ServiceLocator.Current.GetInstance<IVendorRepository>();
      var listVendors = vendorRepo.FindAll().AsQueryable().ProjectToArray<VendorDto>();
      Vendors = new ObservableCollection<VendorDto>(listVendors);

      ProductTypes = new ObservableCollection<string>(Enum.GetNames(typeof(ProductTypeEnum)).Cast<string>());

      Dto = dto ?? new ProductDto();

      SaveItem = new DelegateCommand(OnSaveItem);
      CancelSave = new DelegateCommand(OnCancelSave);

    }

    private void OnSaveItem()
    {
      //todo logic to save
      var productRepo = ServiceLocator.Current.GetInstance<IProductRepository>();
      var vendorRepo = ServiceLocator.Current.GetInstance<IVendorRepository>();
      if (Dto.Id == Guid.Empty)
      {
        var obj = Mapper.Map<Product>(Dto);
        var vendor = vendorRepo.Get(Dto.VendorId);
        obj.Vendor = vendor;
        productRepo.Add(obj);
      }
      else
      {
        var _Product = productRepo.Get(Dto.Id);
        _Product = Mapper.Map(Dto, _Product);
        var vendor = vendorRepo.Get(Dto.VendorId);
        _Product.Vendor = vendor;

        productRepo.Update(_Product);
      }

      Close();
    }
    private void OnCancelSave()
    {
      if (Dto != null && Dto.Id != Guid.Empty)
      {
        var repo = ServiceLocator.Current.GetInstance<IProductRepository>();
        var _product = repo.Get(Dto.Id);
        Dto = null;
        Dto = Mapper.Map<ProductDto>(_product);
      }

      Close();
    }
  }
}
