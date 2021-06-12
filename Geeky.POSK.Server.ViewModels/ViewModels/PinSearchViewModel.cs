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
  public class PinSearchViewModel : BindableBaseViewModel
  {
    private ObservableCollection<DecryptedPinDto> _pins;
    public ObservableCollection<DecryptedPinDto> Pins { get { return _pins; } set { SetProperty(ref _pins, value); } }

    private ObservableCollection<Terminal> _terminals;
    public ObservableCollection<Terminal> Terminals { get { return _terminals; } set { SetProperty(ref _terminals, value); } }

    private ObservableCollection<string> _boolStatus;
    public ObservableCollection<string> BoolStatus { get { return _boolStatus; } set { SetProperty(ref _boolStatus, value); } }

    private ProductFilter _filter;
    public ProductFilter Filter { get { return _filter; } set { SetProperty(ref _filter, value); } }

    private bool _resultFound;
    public bool ResultFound { get { return _resultFound; } set { SetProperty(ref _resultFound, value); } }

    public DelegateCommand SearchForPin { get; private set; }
    public DelegateCommand ResetExpiryDate { get; private set; }
    public DelegateCommand ResetSoldDate { get; private set; }

    public PinSearchViewModel()
    {
      Pins = new ObservableCollection<DecryptedPinDto>();
      Filter = new ProductFilter();
      SearchForPin = new DelegateCommand(OnSearchForPin);
      ResetExpiryDate = new DelegateCommand(OnResetExpiryDate);
      ResetSoldDate = new DelegateCommand(OnResetSoldDate);
    }

    private void OnResetSoldDate()
    {
      Filter.SoldDateFrom = Filter.SoldDateTo = null;
    }

    private void OnResetExpiryDate()
    {
      Filter.ExpiryDateFrom = Filter.ExpiryDateTo = null;
    }

    private void OnSearchForPin()
    {
      var searchEncrypted = !string.IsNullOrEmpty(Filter.Pin);

      if (searchEncrypted && Filter.Terminal == null)
      {
        MessageBox.Show(@"In order to search for specific PIN you've to select terminal first",
                         "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
        return;
      }

      var repo = ServiceLocator.Current.GetInstance<IPinRepository>();
      var result = repo.Query();

      result = result.Where(x => Filter.Sold == null || x.Sold == Filter.Sold);
      result = result.Where(x => Filter.ExpiryDateFrom == null || x.ExpiryDate >= Filter.ExpiryDateFrom);
      result = result.Where(x => Filter.ExpiryDateTo == null || x.ExpiryDate <= Filter.ExpiryDateTo);
      result = result.Where(x => Filter.SoldDateFrom == null || x.SoldDate >= Filter.SoldDateFrom);
      result = result.Where(x => Filter.SoldDateTo == null || x.SoldDate <= Filter.SoldDateTo);
      result = result.Where(x => string.IsNullOrEmpty(Filter.SerialNumber) || x.SerialNumber == Filter.SerialNumber.Trim());
      result = result.Where(x => string.IsNullOrEmpty(Filter.RefNumber) || x.RefNumber == Filter.RefNumber.Trim());

      if (Filter.Terminal != null)
        result = result.Where(x => x.Terminal.Id == Filter.Terminal.Id);

      if (searchEncrypted)
      {
        if (!string.IsNullOrEmpty(Filter.Pin))
        {
          var encrypted = AesEncyHelper.Encyrpt(Filter.Pin, Filter.Terminal.Id.ToByteArray(), Filter.Terminal.Id.ToByteArray());
          result = result.Where(x => x.PIN == encrypted);
        }
      }

      var allResults = result.OrderBy(x=>x.Product.Vendor.Order)
                             .ThenBy(x=>x.Product.Order)
                             .ToList();

      var decrebtedResults = allResults.AsQueryable().ProjectTo<DecryptedPinDto>().ToList();
      repo = null;
      Pins = null;
      Pins = new ObservableCollection<DecryptedPinDto>(decrebtedResults);
      ResultFound = Pins.Count > 0;
    }

    public void LoadTerminals()
    {
      var _terminalRepo = ServiceLocator.Current.GetInstance<ITerminalRepository>();
      var terminals = _terminalRepo.FindAll();
      Terminals = new ObservableCollection<Terminal>(terminals);
      BoolStatus = new ObservableCollection<string>(Enum.GetNames(typeof(BoolStatusEnum)).Cast<string>());
    }

  }

  public class ProductFilter : BindableBaseViewModel
  {
    private Terminal _terminal;
    public Terminal Terminal { get { return _terminal; } set { SetProperty(ref _terminal, value); } }

    private bool? _sold;
    public bool? Sold { get { return _sold; } set { SetProperty(ref _sold, value); } }

    private DateTime? _expiryDateFrom;
    public DateTime? ExpiryDateFrom { get { return _expiryDateFrom; } set { SetProperty(ref _expiryDateFrom, value); } }

    private DateTime? _expiryDateTo;
    public DateTime? ExpiryDateTo { get { return _expiryDateTo; } set { SetProperty(ref _expiryDateTo, value); } }

    private DateTime? _soldDateFrom;
    public DateTime? SoldDateFrom { get { return _soldDateFrom; } set { SetProperty(ref _soldDateFrom, value); } }

    private DateTime? _soldDateTo;
    public DateTime? SoldDateTo { get { return _soldDateTo; } set { SetProperty(ref _soldDateTo, value); } }

    private string _pin;
    public string Pin { get { return _pin; } set { SetProperty(ref _pin, value); } }

    private string _serialNumber;
    public string SerialNumber { get { return _serialNumber; } set { SetProperty(ref _serialNumber, value); } }

    private string _refNumber;
    public string RefNumber { get { return _refNumber; } set { SetProperty(ref _refNumber, value); } }

  }

  public enum BoolStatusEnum
  {
    Yes,
    No,
    Any
  }
}
