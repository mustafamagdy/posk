using AutoMapper.Configuration;
using AutoMapper.QueryableExtensions;
using CommonServiceLocator;
using Geeky.POSK.DataContracts;
using Geeky.POSK.Repository.Interfface;
using Geeky.POSK.WPF.Core.Base;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Geeky.POSK.Server.ViewModels
{
  public class VendorListViewModel : BindableBaseViewModel
  {
    private ObservableCollection<VendorViewModel> _data;
    public ObservableCollection<VendorViewModel> Data { get { return _data; } set { SetProperty(ref _data, value); } }

    public DelegateCommand<VendorViewModel> EditItem { get; private set; }
    public DelegateCommand AddNewItem { get; private set; }
    public DelegateCommand<VendorViewModel> DeleteItem { get; private set; }

    public event Action<VendorViewModel> AddNewClicked = delegate { };
    public event Action<VendorViewModel> EditItemClicked = delegate { };
    public event Action CloseDialog = delegate { };

    public VendorListViewModel()
    {
      Data = new ObservableCollection<VendorViewModel>();
      EditItem = new DelegateCommand<VendorViewModel>(OnEditItem);
      AddNewItem = new DelegateCommand(OnAddNew);
      DeleteItem = new DelegateCommand<VendorViewModel>(OnDeleteItem);
    }

    private void OnDeleteItem(VendorViewModel obj)
    {
      if (ConfirmDelete(obj))
      {
        try
        {
          var repo = ServiceLocator.Current.GetInstance<IVendorRepository>();
          var _vendor = repo.Get(obj.Dto.Id);
          repo.Remove(_vendor);
          Data.Remove(obj);
          LoadData();
        }
        catch (Exception ex)
        {
          //Faild to delete
          MessageBox.Show($"Failed to delete item \r\n{ex.Message}", "Failed to delete", MessageBoxButton.OK, MessageBoxImage.Error);
        }
      }
    }
    private bool ConfirmDelete(VendorViewModel arg)
    {
      var result = MessageBox.Show("Are you sure", "Confirm Delete", MessageBoxButton.YesNo);
      return result == MessageBoxResult.Yes;
    }
    private void OnAddNew()
    {
      var vm = new VendorViewModel(null);
      vm.Close += Service_Close;
      AddNewClicked(vm);
      LoadData();
    }
    private void OnEditItem(VendorViewModel obj)
    {
      EditItemClicked(obj);
    }

    public void LoadData()
    {
      Data = new ObservableCollection<VendorViewModel>();
      var vendorRepo = ServiceLocator.Current.GetInstance<IVendorRepository>();
      var _vendors = vendorRepo
                      .FindAll()
                      .OrderBy(x=>x.Order)
                      .AsQueryable().ProjectTo<VendorDto>().ToList();

      foreach (var vendor in _vendors)
      {
        var vm = new VendorViewModel(vendor);
        vm.Close += Service_Close;

        Data.Add(vm);
      }
    }

    private void Service_Close()
    {
      CloseDialog();
    }
  }
}
