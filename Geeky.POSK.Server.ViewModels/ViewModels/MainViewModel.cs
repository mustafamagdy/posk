using CommonServiceLocator;
using Geeky.POSK.Infrastructore.Core;
using Geeky.POSK.Infrastructore.Core.Enums;
using Geeky.POSK.WPF.Core.Base;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Threading;

namespace Geeky.POSK.Server.ViewModels
{
  public class MainViewModel : BindableBaseViewModel
  {
    private Stopwatch _watch;
    private string _logFilePath;

    private VendorListViewModel _vendorListVm;
    private ProductListViewModel _productListVm;
    private TerminalListViewModel _terminalListVm;
    private PinImporterViewModel _importPinsVm;
    private MainDashboardViewModel _dashboardVm;
    private PinSearchViewModel _searchPinVm;
    private PinTransferListViewModel _transferPinListVm;
    private LogSearchViewModel _logSearch;
    private SessionSearchViewModel _sessionSearch;

    public DelegateCommand Home { get; private set; }
    public DelegateCommand LoadVendors { get; private set; }
    public DelegateCommand LoadProducts { get; private set; }
    public DelegateCommand LoadTerminals { get; private set; }
    public DelegateCommand ImportPins { get; private set; }
    public DelegateCommand TransferPins { get; private set; }
    public DelegateCommand LoadPinSearch { get; private set; }
    public DelegateCommand LoadTerminalLogs { get; private set; }
    public DelegateCommand LoadSearchSessions { get; private set; }

    public event Action<BindableBaseViewModel> AddNewItemClicked = delegate { };
    public event Action<BindableBaseViewModel> EditItemClicked = delegate { };
    public event Action CloseDialog = delegate { };
    private void InitializeViews()
    {
      _vendorListVm = new VendorListViewModel();
      _productListVm = new ProductListViewModel();
      _terminalListVm = new TerminalListViewModel();
      _importPinsVm = new PinImporterViewModel();
      _dashboardVm = new MainDashboardViewModel(SynchronizationContext.Current);
      _searchPinVm = new PinSearchViewModel();
      _transferPinListVm = new PinTransferListViewModel();
      _logSearch = new LogSearchViewModel();
      _sessionSearch = new SessionSearchViewModel();

      _vendorListVm.AddNewClicked += _vm_AddNewClicked;
      _vendorListVm.EditItemClicked += _vm_EditItemClicked;
      _vendorListVm.CloseDialog += _vm_CloseDialog;

      _productListVm.AddNewClicked += _vm_AddNewClicked;
      _productListVm.EditItemClicked += _vm_EditItemClicked;
      _productListVm.CloseDialog += _vm_CloseDialog;

      _terminalListVm.AddNewClicked += _vm_AddNewClicked;
      _terminalListVm.EditItemClicked += _vm_EditItemClicked;
      _terminalListVm.CloseDialog += _vm_CloseDialog;

      _transferPinListVm.AddNewClicked += _vm_AddNewClicked;
      _transferPinListVm.EditItemClicked += _vm_EditItemClicked;
      _transferPinListVm.CloseDialog += _vm_CloseDialog;


      _importPinsVm.SelectImportingFile += _importPinsVm_SelectImportingFile;

    }

    private void _importPinsVm_SelectImportingFile()
    {
      var dlg = new OpenFileDialog
      {
        Filter = "Excel Files|*.xls;*.xlsx;*.xlsm",
        InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
      };

      var result = dlg.ShowDialog();
      if (result.HasValue && result.Value)
        _importPinsVm.GetExcelColumns(dlg.FileName);
    }

    private void _vm_CloseDialog()
    {
      CloseDialog();
    }
    private void _vm_EditItemClicked(BindableBaseViewModel obj)
    {
      EditItemClicked(obj);
    }
    private void _vm_AddNewClicked(BindableBaseViewModel obj)
    {
      AddNewItemClicked(obj);
    }


    private void InitializeCommands()
    {
      Home = new DelegateCommand(OnHome);
      LoadVendors = new DelegateCommand(OnLoadVendors);
      LoadProducts = new DelegateCommand(OnLoadProducts);
      LoadTerminals = new DelegateCommand(OnLoadTerminals);
      ImportPins = new DelegateCommand(OnImportPins);
      TransferPins = new DelegateCommand(OnTransferPins);
      LoadPinSearch = new DelegateCommand(OnLoadPinSearch);
      LoadTerminalLogs = new DelegateCommand(OnLoadTerminalLogs);
      LoadSearchSessions = new DelegateCommand(OnLoadSearchSessions);
    }
    private void OnHome()
    {
      _dashboardVm = new MainDashboardViewModel(SynchronizationContext.Current);//to refresh the animation
      _dashboardVm.InitializeSalesReport();
      CurrentVm = _dashboardVm;
    }
    private void OnTransferPins()
    {
      _transferPinListVm.LoadData();
      CurrentVm = _transferPinListVm;
    }
    private void OnLoadPinSearch()
    {
      _searchPinVm.LoadTerminals();
      CurrentVm = _searchPinVm;
    }
    private void OnLoadTerminals()
    {
      _terminalListVm.LoadData();
      CurrentVm = _terminalListVm;
    }
    private void OnLoadProducts()
    {
      _productListVm.LoadData();
      CurrentVm = _productListVm;
    }
    private void OnLoadVendors()
    {
      _vendorListVm.LoadData();
      CurrentVm = _vendorListVm;
    }
    private void OnImportPins()
    {
      CurrentVm = _importPinsVm;
    }
     private void OnLoadTerminalLogs()
    {
      _logSearch.LoadTerminals();
      CurrentVm = _logSearch;
    }
      private void OnLoadSearchSessions()
    {
      _sessionSearch.LoadTerminals();
      CurrentVm = _sessionSearch;
    }   public MainViewModel()
    {
      InitializeViews();
      InitializeCommands();
      _dashboardVm.InitializeSalesReport();
      CurrentVm = _dashboardVm;
    }

    private BindableBaseViewModel _currentVm;
    public BindableBaseViewModel CurrentVm
    {
      get { return _currentVm; }
      set { SetProperty(ref _currentVm, value); }
    }

    private UILanguage _currentLanguage;
    public UILanguage CurrentLanguage { get { return _currentLanguage; } set { SetProperty(ref _currentLanguage, value); } }

  }
}
