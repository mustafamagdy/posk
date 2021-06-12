using Geeky.POSK.Infrastructore.Core;
using Geeky.POSK.Server.ViewModels;
using Geeky.POSK.WPF.Core;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Geeky.POSK.Server.Dashboard.Views
{
  public partial class MainDashboardView : UserControl
  {
    public MainDashboardView()
    {
      InitializeComponent();
      this.DataContextChanged += MainDashboardView_DataContextChanged;
    }

    private void MainDashboardView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      if (this.DataContext != null)
      {
        var vm = this.DataContext as MainDashboardViewModel;
        vm.PrintChartImage += MainDashboardView_PrintChartImage;
      }
    }

    private void MainDashboardView_PrintChartImage(StatisticsChartEnum chartType)
    {
      switch (chartType)
      {
        case StatisticsChartEnum.SalesByProduct:
          {
            break;
          }
        case StatisticsChartEnum.SalesByTerminal:
          {
            break;
          }
        case StatisticsChartEnum.SalesByVendor:
        default:
          {
            var view = VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(salesByVendorCtrl, 0), 0);
            var chart = VisualTreeHelperEx.FindElementByName<CartesianChart>((FrameworkElement)view, "chart");

            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
              printDialog.PrintVisual(chart, "Sales by vendor");
            }
            break;
          }
      }
    }
  }
}
