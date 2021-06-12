using CommonServiceLocator;
using Geeky.POSK.DataContracts;
using Geeky.POSK.Filters;
using Geeky.POSK.Infrastructore.Core;
using Geeky.POSK.ServiceContracts;
using Geeky.POSK.Views;
using Geeky.POSK.WPF.Core.Base;
using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Geeky.POSK.Server.ViewModels
{
  public class ChartViewModel : BindableBaseViewModel
  {
    private StatisticsChartEnum _chartType;
    private string _chartTitle;
    public string ChartTitle { get { return _chartTitle; } set { SetProperty(ref _chartTitle, value); } }

    private FilterCriteria _filter;
    public FilterCriteria Filter { get { return _filter; } set { SetProperty(ref _filter, value); } }

    public DelegateCommand FilterData { get; private set; }
    public DelegateCommand PrintChart { get; private set; }

    private SeriesCollection _seriesCollection;
    public SeriesCollection SeriesCollection { get { return _seriesCollection; } set { SetProperty(ref _seriesCollection, value); } }

    public string[] Labels { get; set; }
    public Func<int, string> Formatter { get; set; }

    public event Action PrintChartImage = delegate { };
    public ChartViewModel()
    {
      FilterData = new DelegateCommand(OnFilterData);
      PrintChart = new DelegateCommand(OnPrintChart);
      SeriesCollection = new SeriesCollection();
    }

    private void OnFilterData()
    {
      UpdateChart(_chartType, Filter);
    }
    private void OnPrintChart()
    {
      PrintChartImage();
    }

    public void UpdateChart(StatisticsChartEnum chartType, FilterCriteria criteria = null)
    {
      SeriesCollection = new SeriesCollection();
      Filter = criteria ?? new FilterCriteria();
      _chartType = chartType;
      var service = ServiceLocator.Current.GetInstance<IStatisticsService>();
      switch (chartType)
      {
        case StatisticsChartEnum.SalesByTerminal:
          {
            var result = service.SalesByTerminal(criteria);
            ShowSalesByTerminalChart(result);
            ChartTitle = "Sales By Terminal";
            break;
          }
        case StatisticsChartEnum.SalesByVendor:
          {
            var result = service.SalesByVendor(criteria);
            ShowSalesByVendorChart(result);
            ChartTitle = "Sales By Vendor";
            break;
          }
        case StatisticsChartEnum.SalesByProduct:
        default:
          {
            var result = service.SalesByProduct(criteria);
            ShowSalesByProductChart(result);
            ChartTitle = "Sales By Product";
            break;
          }
      }
    }

    private void ShowSalesByVendorChart(IEnumerable<SalesByVendor> result)
    {
      SeriesCollection = new SeriesCollection();
      SeriesCollection.Add(new ColumnSeries { Title = "Sold" });
      SeriesCollection.Add(new ColumnSeries { Title = "Remaining" });

      var soldValues = result.Select(x => x.SoldCount).ToArray();
      var reminaingValues = result.Select(x => x.Remaining).ToArray();
      var labels = result.Select(x => x.VendorCode).ToArray();
      Labels = labels;

      SeriesCollection[0].Values = new ChartValues<int>(soldValues);
      SeriesCollection[1].Values = new ChartValues<int>(reminaingValues);

      Formatter = value => value.ToString("N");
    }
    private void ShowSalesByProductChart(IEnumerable<SalesByProduct> result)
    {
      SeriesCollection = new SeriesCollection();
      SeriesCollection.Add(new ColumnSeries { Title = "Sold" });
      SeriesCollection.Add(new ColumnSeries { Title = "Remaining" });

      var soldValues = result.Select(x => x.SoldCount).ToArray();
      var reminaingValues = result.Select(x => x.Remaining).ToArray();
      var labels = result.Select(x => x.ProductCode).ToArray();
      Labels = labels;

      SeriesCollection[0].Values = new ChartValues<int>(soldValues);
      SeriesCollection[1].Values = new ChartValues<int>(reminaingValues);

      Formatter = value => value.ToString("N");
    }
    private void ShowSalesByTerminalChart(IEnumerable<SalesByTerminal> result)
    {
      SeriesCollection = new SeriesCollection();
      SeriesCollection.Add(new ColumnSeries { Title = "Sold" });
      SeriesCollection.Add(new ColumnSeries { Title = "Remaining" });

      var soldValues = result.Select(x => x.SoldCount).ToArray();
      var reminaingValues = result.Select(x => x.Remaining).ToArray();
      var labels = result.Select(x => x.TerminalCode).ToArray();
      Labels = labels;

      SeriesCollection[0].Values = new ChartValues<int>(soldValues);
      SeriesCollection[1].Values = new ChartValues<int>(reminaingValues);

      Formatter = value => value.ToString("N");
    }
  }
}
