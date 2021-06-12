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
  public class LogSearchViewModel : BindableBaseViewModel
  {
    private ObservableCollection<TerminalLogDto> _logs;
    public ObservableCollection<TerminalLogDto> Logs { get { return _logs; } set { SetProperty(ref _logs, value); } }

    private ObservableCollection<Terminal> _terminals;
    public ObservableCollection<Terminal> Terminals { get { return _terminals; } set { SetProperty(ref _terminals, value); } }

    private ObservableCollection<string> _logTypes;
    public ObservableCollection<string> LogTypes { get { return _logTypes; } set { SetProperty(ref _logTypes, value); } }

    private TerminalLogFilter _filter;
    public TerminalLogFilter Filter { get { return _filter; } set { SetProperty(ref _filter, value); } }

    private bool _resultFound;
    public bool ResultFound { get { return _resultFound; } set { SetProperty(ref _resultFound, value); } }

    public DelegateCommand SearchForLogs { get; private set; }
    public DelegateCommand ResetLogDateDate { get; private set; }
    public DelegateCommand ResetLogType { get; private set; }

    public LogSearchViewModel()
    {
      Filter = new TerminalLogFilter();
      Logs = new ObservableCollection<TerminalLogDto>();
      SearchForLogs = new DelegateCommand(OnSearchForLogs);
      ResetLogDateDate = new DelegateCommand(OnResetLogDate);
      ResetLogType = new DelegateCommand(OnResetLogType);
    }

    private void OnResetLogType()
    {
      Filter.LogType = LogTypeEnum.ERROR;
    }

    private void OnResetLogDate()
    {
      Filter.LogDateFrom = Filter.LogDateTo = null;
    }

    private void OnSearchForLogs()
    {
      if (Filter.LogType  == null || Filter.Terminal == null)
      {
        MessageBox.Show(@"In order to search for logs, you've to select terminal and log type",
                         "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
        return;
      }

      var repo = ServiceLocator.Current.GetInstance<ITerminalLogRepository>();
      var result = repo.Query();

      result = result.Where(x => Filter.LogType == null || x.LogType == Filter.LogType);
      result = result.Where(x => Filter.LogDateFrom == null || x.LogDate >= Filter.LogDateFrom);
      result = result.Where(x => Filter.LogDateTo == null || x.LogDate <= Filter.LogDateTo);
      result = result.Where(x => string.IsNullOrEmpty(Filter.Message) || x.Message.Contains(Filter.Message.Trim()));

      if (Filter.Terminal != null)
        result = result.Where(x => x.Terminal.Id == Filter.Terminal.Id);

      var allResults = result.OrderByDescending(x => x.LogDate)
                             .ThenBy(x => x.Terminal.TerminalKey)
                             .AsQueryable()
                             .ProjectTo<TerminalLogDto>()
                             .ToList();

      repo = null;
      Logs.Clear();
      Logs = new ObservableCollection<TerminalLogDto>(allResults);
      ResultFound = Logs.Count > 0;

     
    }

    public void LoadTerminals()
    {
      var _terminalRepo = ServiceLocator.Current.GetInstance<ITerminalRepository>();
      var terminals = _terminalRepo.FindAll();
      Terminals = new ObservableCollection<Terminal>(terminals);
      LogTypes = new ObservableCollection<string>(Enum.GetNames(typeof(LogTypeEnum)).Cast<string>());


    }

  }

  public class TerminalLogFilter : BindableBaseViewModel
  {
    private Terminal _terminal;
    public Terminal Terminal { get { return _terminal; } set { SetProperty(ref _terminal, value); } }

    private DateTime? _logDateFrom;
    public DateTime? LogDateFrom { get { return _logDateFrom; } set { SetProperty(ref _logDateFrom, value); } }

    private DateTime? _logDateTo;
    public DateTime? LogDateTo { get { return _logDateTo; } set { SetProperty(ref _logDateTo, value); } }

    private LogTypeEnum? _logType;
    public LogTypeEnum? LogType { get { return _logType; } set { SetProperty(ref _logType, value); } }

    private string _message;
    public string Message { get { return _message; } set { SetProperty(ref _message, value); } }

  }

}
