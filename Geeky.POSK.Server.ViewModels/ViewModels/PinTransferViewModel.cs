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
using AutoMapper;

namespace Geeky.POSK.Server.ViewModels
{
  public class PinTransferViewModel : BindableBaseViewModel
  {
    private Terminal _fromTerminal;
    public Terminal FromTerminal { get { return _fromTerminal; } set { SetProperty(ref _fromTerminal, value); UpdateAvailableCount(); } }

    private Terminal _toTerminal;
    public Terminal ToTerminal { get { return _toTerminal; } set { SetProperty(ref _toTerminal, value); UpdateAvailableCount(); } }

    private Vendor _selectedVendor;
    public Vendor SelectedVendor { get { return _selectedVendor; } set { SetProperty(ref _selectedVendor, value); OnSelectVendor(); } }

    private Product _selectedProduct;
    public Product SelectedProduct { get { return _selectedProduct; } set { SetProperty(ref _selectedProduct, value); OnSelectProduct(); } }

    private ObservableCollection<Terminal> _terminals;
    public ObservableCollection<Terminal> Terminals { get { return _terminals; } set { SetProperty(ref _terminals, value); } }

    private ObservableCollection<Vendor> _vendors;
    public ObservableCollection<Vendor> Vendors { get { return _vendors; } set { SetProperty(ref _vendors, value); } }

    private ObservableCollection<Product> _products;
    public ObservableCollection<Product> Products { get { return _products; } set { SetProperty(ref _products, value); } }

    private int _transferCount;
    public int TransferCount
    {
      get { return _transferCount; }
      set
      {
        SetProperty(ref _transferCount, value);
        UpdateCanTransfer();
      }
    }

    private int _availabelCount;
    public int AvailabelCount
    {
      get { return _availabelCount; }
      set
      {
        SetProperty(ref _availabelCount, value);
        UpdateCanTransfer();
      }
    }

    private bool _canTransfer;
    public bool CanTransfer { get { return _canTransfer; } set { SetProperty(ref _canTransfer, value); } }

    public DelegateCommand TransferSelectedPins { get; private set; }

    private TransferTrxDto _dto;
    public TransferTrxDto Dto { get { return _dto; } set { SetProperty(ref _dto, value); } }

    public DelegateCommand SaveItem { get; private set; }
    public DelegateCommand CancelSave { get; private set; }

    public event Action Close = delegate { };
    public PinTransferViewModel(TransferTrxDto dto)
    {
      Dto = dto ?? new TransferTrxDto();
      Terminals = new ObservableCollection<Terminal>();
      Vendors = new ObservableCollection<Vendor>();
      Products = new ObservableCollection<Product>();

      TransferSelectedPins = new DelegateCommand(OnTransferSelectedPins);

      SaveItem = new DelegateCommand(OnSaveItem);
      CancelSave = new DelegateCommand(OnCancelSave);
    }

    private void OnSaveItem()
    {
      if (!CanTransfer)
      {
        MessageBox.Show("Selected values are not valid for transfer", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
        return;
      }

      if (FromTerminal == null || ToTerminal == null || TransferCount <= 0)
      {
        MessageBox.Show("Please enter valid data for transfer operation", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
        return;
      }

      var svc = ServiceLocator.Current.GetInstance<IProductManagementService>();
      try
      {
        svc.TransferProductsBetweenTerminals(FromTerminal.Id, ToTerminal.Id, SelectedProduct.Id, TransferCount);
        UpdateAvailableCount();
        MessageBox.Show("Transfered successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
      }
      catch (InvalidOperationException ex)
      {
        MessageBox.Show("Available count to be transfered not enough", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
      }
      catch (Exception ex)
      {
        MessageBox.Show("Transaction failed, no pins transfered", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
      }

      Close();
    }
    private void OnCancelSave()
    {
      if (Dto != null && Dto.Id != Guid.Empty)
      {
        var repo = ServiceLocator.Current.GetInstance<ITransferTrxRepository>();
        var _data = repo.Get(Dto.Id);
        Dto = null;
        Dto = Mapper.Map<TransferTrxDto>(_data);
      }

      Close();
    }

    private void UpdateCanTransfer()
    {
      //update ui
      CanTransfer = 
        AvailabelCount > 0 && 
        TransferCount <= AvailabelCount && 
        TransferCount > 0 && 
        FromTerminal?.Id != ToTerminal?.Id; ;
    }

    private void UpdateAvailableCount()
    {
      if (SelectedProduct == null || FromTerminal == null)
      {
        AvailabelCount = 0;
      }
      else
      {
        var _pinRepo = ServiceLocator.Current.GetInstance<IPinRepository>();
        AvailabelCount = _pinRepo.GetAvailablePins(SelectedProduct.Id, FromTerminal.Id);
      }
    }

    private void OnSelectTerminal()
    {
      UpdateAvailableCount();
    }

    private void OnSelectVendor()
    {
      if (SelectedVendor == null)
      {
        Products = new ObservableCollection<Product>();
        return;
      }
      var _productRepo = ServiceLocator.Current.GetInstance<IProductRepository>();
      var products = _productRepo.FindAll(x => x.Vendor.Id == SelectedVendor.Id);
      Products = new ObservableCollection<Product>(products);
    }

    private void OnSelectProduct()
    {
      UpdateAvailableCount();
    }

    public void LoadDefaultData()
    {
      var _terminalRepo = ServiceLocator.Current.GetInstance<ITerminalRepository>();
      var _vendorRepo = ServiceLocator.Current.GetInstance<IVendorRepository>();

      Terminals = new ObservableCollection<Terminal>(_terminalRepo.FindAll());
      Vendors = new ObservableCollection<Vendor>(_vendorRepo.FindAll());
    }


    private void OnTransferSelectedPins()
    {

    }

  }
}
