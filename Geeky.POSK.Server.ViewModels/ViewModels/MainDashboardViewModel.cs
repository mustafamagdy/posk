using AutoMapper.QueryableExtensions;
using CommonServiceLocator;
using Geeky.POSK.DataContracts;
using Geeky.POSK.Infrastructore.Core;
using Geeky.POSK.Models;
using Geeky.POSK.Repository.Interfface;
using Geeky.POSK.ServiceContracts;
using Geeky.POSK.WPF.Core.Base;
using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Timers;

namespace Geeky.POSK.Server.ViewModels
{
  public class MainDashboardViewModel : BindableBaseViewModel, IDisposable
  {
    private ObservableCollection<TerminalPingStatusDto> _terminalPingStatus;
    public ObservableCollection<TerminalPingStatusDto> TerminalPingStatus { get { return _terminalPingStatus; } set { SetProperty(ref _terminalPingStatus, value); } }

    private ChartViewModel _salesByVendorVm;
    public ChartViewModel SalesByVendorVm { get { return _salesByVendorVm; } set { SetProperty(ref _salesByVendorVm, value); } }

    private ChartViewModel _salesByTerminalVm;
    public ChartViewModel SalesByTerminalVm { get { return _salesByTerminalVm; } set { SetProperty(ref _salesByTerminalVm, value); } }

    private ChartViewModel _salesByProductVm;
    public ChartViewModel SalesByProductVm { get { return _salesByProductVm; } set { SetProperty(ref _salesByProductVm, value); } }

    private SalesReportViewModel _salesReportVm;
    public SalesReportViewModel SalesReportVm { get { return _salesReportVm; } set { SetProperty(ref _salesReportVm, value); } }

    public event Action<StatisticsChartEnum> PrintChartImage = delegate { };

    private System.Timers.Timer _tmrTerminalStatus = new System.Timers.Timer();
    private float StatusTimerInterval = 1f;//in minutes
    SynchronizationContext _uiContext;

    public MainDashboardViewModel(SynchronizationContext uiContext)
    {
      //Used to update UI in another thread
      _uiContext = uiContext;
      //do this for first time
      GetTerminalStatus();

      //timer to check for terminal status (ping and errors)
      _tmrTerminalStatus.Interval = 1 * 1000 * StatusTimerInterval;
      _tmrTerminalStatus.Elapsed += _tmrTerminalStatus_Elapsed;
      _tmrTerminalStatus.Start();

      SalesByVendorVm = new ChartViewModel();
      SalesByTerminalVm = new ChartViewModel();
      SalesByProductVm = new ChartViewModel();
      SalesReportVm = new SalesReportViewModel();

      SalesByVendorVm.PrintChartImage += SalesByVendorVm_PrintChartImage;
      SalesByTerminalVm.PrintChartImage += SalesByTerminalVm_PrintChartImage;
      SalesByProductVm.PrintChartImage += SalesByProductVm_PrintChartImage;

      SalesByVendorVm.UpdateChart(StatisticsChartEnum.SalesByVendor);
      SalesByTerminalVm.UpdateChart(StatisticsChartEnum.SalesByTerminal);
      SalesByProductVm.UpdateChart(StatisticsChartEnum.SalesByProduct);

      //TerminalPingEvent.Ping += TerminalPingEvent_Ping;
    }

    private void _tmrTerminalStatus_Elapsed(object sender, ElapsedEventArgs e)
    {
      _tmrTerminalStatus.Stop();
      GetTerminalStatus();
      _tmrTerminalStatus.Start();
    }

    private void SalesByProductVm_PrintChartImage()
    {
      PrintChartImage(StatisticsChartEnum.SalesByProduct);
    }

    private void SalesByTerminalVm_PrintChartImage()
    {
      PrintChartImage(StatisticsChartEnum.SalesByTerminal);
    }

    private void SalesByVendorVm_PrintChartImage()
    {
      PrintChartImage(StatisticsChartEnum.SalesByVendor);
    }

    private void GetTerminalStatus()
    {
      try
      {
        TerminalPingStatus = new ObservableCollection<TerminalPingStatusDto>();
        var dbContext = ServiceLocator.Current.GetInstance<DbContext>();
        var _terminalRepo = ServiceLocator.Current.GetInstance<ITerminalRepository>();

        var _terminals = _terminalRepo
                        .FindAll()
                        //.AsQueryable()
                        //.ProjectTo<TerminalPingStatusDto>()
                        .ToList();

        _terminals.ForEach(t => dbContext.Entry<Terminal>(t).Reload());
        _terminalRepo = null;
        foreach (var terminal in _terminals)
        {
          //if terminal has not been ping us for more than 5 minutes, it is not a live now
          var terminalAlive = terminal.LastPing.HasValue && DateTime.Now.Subtract(terminal.LastPing.Value) < TimeSpan.FromMinutes(5);
          var pingStatus = terminalAlive ? PingStatusEnum.Ok : PingStatusEnum.Off;

          if (terminal.LastErrorCode != 0 || terminal.LastError.Trim() != "")
            pingStatus = PingStatusEnum.HasError;

          _uiContext.Send(x =>
          {
            TerminalPingStatus.Add(new TerminalPingStatusDto
            {
              Id = terminal.Id,
              TerminalKey = terminal.TerminalKey,
              PingStatus = pingStatus,
              LastErrorCode = terminal.LastErrorCode,
              LastError = terminal.LastError,
              CashCodeFull = terminal.CashCodeFull,
              CashCodeDisabled = terminal.CashCodeDisabled,
              CashCodeRemoved = terminal.CashCodeRemoved,
            });
          }, null);
        }
      }
      catch (Exception ex)
      {

      }
    }

    public void InitializeSalesReport()
    {
      SalesReportVm.LoadData();
    }
    //private void TerminalPingEvent_Ping(TerminalEventArg args)
    //{
    //  var terminal = TerminalPingStatus.FirstOrDefault(x => x.Id == args.TerminalId);
    //  terminal.PingStatus = true;
    //  TerminalPingStatus[TerminalPingStatus.IndexOf(terminal)] = terminal;
    //}

    public void Dispose()
    {
      //TerminalPingEvent.Ping -= TerminalPingEvent_Ping;
    }
  }
}
