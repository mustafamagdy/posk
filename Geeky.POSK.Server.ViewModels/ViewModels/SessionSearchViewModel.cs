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
using AutoMapper;

namespace Geeky.POSK.Server.ViewModels
{
  public class SessionSearchViewModel : BindableBaseViewModel
  {
    private ObservableCollection<SessionViewModel> _sessions;
    public ObservableCollection<SessionViewModel> Sessions { get { return _sessions; } set { SetProperty(ref _sessions, value); } }

    private ObservableCollection<Terminal> _terminals;
    public ObservableCollection<Terminal> Terminals { get { return _terminals; } set { SetProperty(ref _terminals, value); } }

    private ObservableCollection<string> _logTypes;
    public ObservableCollection<string> LogTypes { get { return _logTypes; } set { SetProperty(ref _logTypes, value); } }

    private SessionFilter _filter;
    public SessionFilter Filter { get { return _filter; } set { SetProperty(ref _filter, value); } }

    private bool _resultFound;
    public bool ResultFound { get { return _resultFound; } set { SetProperty(ref _resultFound, value); } }

    public DelegateCommand SearchForSession { get; private set; }
    public DelegateCommand ResetSessionDate { get; private set; }

    public SessionSearchViewModel()
    {
      Filter = new SessionFilter();
      SearchForSession = new DelegateCommand(OnSearchForSessions);
      ResetSessionDate = new DelegateCommand(OnResetSessionDate);
    }

    private void OnResetSessionDate()
    {
      Filter.SessionDateFrom = Filter.SessionDateTo = null;
    }

    private void OnSearchForSessions()
    {
      var repo = ServiceLocator.Current.GetInstance<IClientSessionRepository>();
      var result = repo.Query();

      result = result.Where(x => Filter.SessionDateFrom == null || x.StartTime >= Filter.SessionDateFrom);
      result = result.Where(x => Filter.SessionDateTo == null || x.StartTime <= Filter.SessionDateTo);
      result = result.Where(x => string.IsNullOrEmpty(Filter.RefNumber) || x.RefNumber.Contains(Filter.RefNumber.Trim()));

      if (Filter.Terminal != null)
        result = result.Where(x => x.Terminal.Id == Filter.Terminal.Id);

      var allResults = result.OrderByDescending(x => x.FinishTime)
                             .ThenBy(x => x.Terminal.TerminalKey)
                             .ToList();

      repo = null;
      Sessions = null;


      Sessions = null;
      Sessions = new ObservableCollection<SessionViewModel>();

      foreach (var session in allResults)
      {
        var pins = session.Pins
                          .AsQueryable()
                          .ProjectTo<DecryptedPinDto>()
                          .ToList();
        var _session = Mapper.Map<ClientSessionDto>(session);
        var vm = new SessionViewModel(_session, new ObservableCollection<DecryptedPinDto>(pins));
        Sessions.Add(vm);
      }

      ResultFound = Sessions.Count > 0;
    }

    public void LoadTerminals()
    {
      var _terminalRepo = ServiceLocator.Current.GetInstance<ITerminalRepository>();
      var terminals = _terminalRepo.FindAll();
      Terminals = new ObservableCollection<Terminal>(terminals);
      LogTypes = new ObservableCollection<string>(Enum.GetNames(typeof(LogTypeEnum)).Cast<string>());
    }

  }

  public class SessionFilter : BindableBaseViewModel
  {
    private Terminal _terminal;
    public Terminal Terminal { get { return _terminal; } set { SetProperty(ref _terminal, value); } }

    private DateTime? _sessionDateFrom;
    public DateTime? SessionDateFrom { get { return _sessionDateFrom; } set { SetProperty(ref _sessionDateFrom, value); } }

    private DateTime? _sessionDateTo;
    public DateTime? SessionDateTo { get { return _sessionDateTo; } set { SetProperty(ref _sessionDateTo, value); } }

    private string _refNumber;
    public string RefNumber { get { return _refNumber; } set { SetProperty(ref _refNumber, value); } }

  }

}
