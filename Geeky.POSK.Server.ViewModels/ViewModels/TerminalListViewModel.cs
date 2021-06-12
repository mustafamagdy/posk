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
  public class TerminalListViewModel : BindableBaseViewModel
  {
    private ObservableCollection<TerminalViewModel> _data;
    public ObservableCollection<TerminalViewModel> Data { get { return _data; } set { SetProperty(ref _data, value); } }

    public DelegateCommand<TerminalViewModel> EditItem { get; private set; }
    public DelegateCommand AddNewItem { get; private set; }
    public DelegateCommand<TerminalViewModel> DeleteItem { get; private set; }

    public event Action<TerminalViewModel> AddNewClicked = delegate { };
    public event Action<TerminalViewModel> EditItemClicked = delegate { };
    public event Action CloseDialog = delegate { };

    public TerminalListViewModel()
    {
      Data = new ObservableCollection<TerminalViewModel>();
      EditItem = new DelegateCommand<TerminalViewModel>(OnEditItem);
      AddNewItem = new DelegateCommand(OnAddNew);
      DeleteItem = new DelegateCommand<TerminalViewModel>(OnDeleteItem);
    }

    private void OnDeleteItem(TerminalViewModel obj)
    {
      if (ConfirmDelete(obj))
      {
        try
        {
          var repo = ServiceLocator.Current.GetInstance<ITerminalRepository>();
          var _Terminal = repo.Get(obj.Dto.Id);
          repo.Remove(_Terminal);
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
    private bool ConfirmDelete(TerminalViewModel arg)
    {
      var result = MessageBox.Show("Are you sure", "Confirm Delete", MessageBoxButton.YesNo);
      return result == MessageBoxResult.Yes;
    }
    private void OnAddNew()
    {
      var vm = new TerminalViewModel(null);
      vm.Close += Service_Close;
      AddNewClicked(vm);
      LoadData();
    }
    private void OnEditItem(TerminalViewModel obj)
    {
      EditItemClicked(obj);
    }

    public void LoadData()
    {
      Data = new ObservableCollection<TerminalViewModel>();
      var terminalRepo = ServiceLocator.Current.GetInstance<ITerminalRepository>();
      var _terminals = terminalRepo
                        .FindAll()
                        .OrderBy(x => x.TerminalKey)
                        .AsQueryable().ProjectTo<TerminalDto>().ToList();

      foreach (var terminal in _terminals)
      {
        if (terminal.ServerTerminal) continue;

        var vm = new TerminalViewModel(terminal);
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
