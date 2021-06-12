using Geeky.POSK.WPF.Core.Base;
using Geeky.POSK.Models;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Geeky.POSK.DataContracts;
using System.Collections.ObjectModel;
using CommonServiceLocator;
using Geeky.POSK.Repository.Interfface;
using Geeky.POSK.ServiceContracts;
using System.Windows;
using Geeky.POSK.Infrastructore.Core;
using AutoMapper.QueryableExtensions;

namespace Geeky.POSK.Server.ViewModels
{
  public class PinTransferListViewModel : BindableBaseViewModel
  {
    private ObservableCollection<PinTransferViewModel> _data;
    public ObservableCollection<PinTransferViewModel> Data { get { return _data; } set { SetProperty(ref _data, value); } }

    public DelegateCommand<PinTransferViewModel> EditItem { get; private set; }
    public DelegateCommand AddNewItem { get; private set; }
    public DelegateCommand<PinTransferViewModel> DeleteItem { get; private set; }

    public event Action<PinTransferViewModel> AddNewClicked = delegate { };
    public event Action<PinTransferViewModel> EditItemClicked = delegate { };
    public event Action CloseDialog = delegate { };

    public PinTransferListViewModel()
    {
      Data = new ObservableCollection<PinTransferViewModel>();
      EditItem = new DelegateCommand<PinTransferViewModel>(OnEditItem);
      AddNewItem = new DelegateCommand(OnAddNew);
      DeleteItem = new DelegateCommand<PinTransferViewModel>(OnDeleteItem);
    }

    private void OnDeleteItem(PinTransferViewModel obj)
    {
      if (obj.Dto.Status == TransferTrxStatusEnum.Hold && ConfirmDelete(obj))
      {
        try
        {
          var repo = ServiceLocator.Current.GetInstance<ITransferTrxRepository>();
          var _trx = repo.Get(obj.Dto.Id);
          repo.Remove(_trx);
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
    private bool ConfirmDelete(PinTransferViewModel arg)
    {
      var result = MessageBox.Show("Are you sure", "Confirm Delete", MessageBoxButton.YesNo);
      return result == MessageBoxResult.Yes;
    }
    private void OnAddNew()
    {
      var vm = new PinTransferViewModel(null);
      vm.LoadDefaultData();
      vm.Close += Service_Close;
      AddNewClicked(vm);
      LoadData();
    }
    private void OnEditItem(PinTransferViewModel obj)
    {
      EditItemClicked(obj);
    }

    public void LoadData()
    {
      Data = new ObservableCollection<PinTransferViewModel>();
      var _transferRepo = ServiceLocator.Current.GetInstance<ITransferTrxRepository>();
      var _transfers = _transferRepo
                      .FindAll()
                      .OrderByDescending(x => x.CreateDate)
                      .AsQueryable()
                      .ProjectTo<TransferTrxDto>().ToList();

      foreach (var transfer in _transfers)
      {
        var vm = new PinTransferViewModel(transfer);
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
