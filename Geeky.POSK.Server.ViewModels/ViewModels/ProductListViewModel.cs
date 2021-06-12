﻿using AutoMapper.Configuration;
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
  public class ProductListViewModel : BindableBaseViewModel
  {
    private ObservableCollection<ProductViewModel> _data;
    public ObservableCollection<ProductViewModel> Data { get { return _data; } set { SetProperty(ref _data, value); } }

    public DelegateCommand<ProductViewModel> EditItem { get; private set; }
    public DelegateCommand AddNewItem { get; private set; }
    public DelegateCommand<ProductViewModel> DeleteItem { get; private set; }

    public event Action<ProductViewModel> AddNewClicked = delegate { };
    public event Action<ProductViewModel> EditItemClicked = delegate { };
    public event Action CloseDialog = delegate { };

    public ProductListViewModel()
    {
      Data = new ObservableCollection<ProductViewModel>();
      EditItem = new DelegateCommand<ProductViewModel>(OnEditItem);
      AddNewItem = new DelegateCommand(OnAddNew);
      DeleteItem = new DelegateCommand<ProductViewModel>(OnDeleteItem);
    }

    private void OnDeleteItem(ProductViewModel obj)
    {
      if (ConfirmDelete(obj))
      {
        try
        {
          var repo = ServiceLocator.Current.GetInstance<IProductRepository>();
          var _Product = repo.Get(obj.Dto.Id);
          repo.Remove(_Product);
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
    private bool ConfirmDelete(ProductViewModel arg)
    {
      var result = MessageBox.Show("Are you sure", "Confirm Delete", MessageBoxButton.YesNo);
      return result == MessageBoxResult.Yes;
    }
    private void OnAddNew()
    {
      var vm = new ProductViewModel(null);
      vm.Close += Service_Close;
      AddNewClicked(vm);
      LoadData();
    }
    private void OnEditItem(ProductViewModel obj)
    {
      EditItemClicked(obj);
    }

    public void LoadData()
    {
      Data = new ObservableCollection<ProductViewModel>();
      var _productProductRepo = ServiceLocator.Current.GetInstance<IProductRepository>();
      var _Products = _productProductRepo
                      .FindAll()
                      .OrderBy(x => x.Vendor.Order)
                      .ThenBy(x => x.Order)
                      .AsQueryable().ProjectTo<ProductDto>().ToList();

      foreach (var product in _Products)
      {
        var vm = new ProductViewModel(product);
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
