using Geeky.POSK.WPF.Core.Base;
using Geeky.POSK.Server.ViewModels;
using System.Windows;
using System.Reflection;

namespace Geeky.POSK.Server.Dashboard
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    private PopupWindow _dlg;
    public MainWindow()
    {
      InitializeComponent();
      SetMenuAlignment();

      var vm = this.DataContext as MainViewModel;
      vm.AddNewItemClicked += Vm_AddNewItem;
      vm.EditItemClicked += Vm_EditItemClicked;
      vm.CloseDialog += Vm_CloseDialog;
      _dlg = new PopupWindow();
    }

    private void Vm_CloseDialog()
    {
      _dlg.Close();
    }

    private void Vm_DeleteItemClicked(BindableBaseViewModel obj)
    {
      _dlg = new PopupWindow();
      _dlg.SetContext(obj);
      _dlg.ShowDialog();
    }
    private void Vm_EditItemClicked(BindableBaseViewModel obj)
    {
      _dlg = new PopupWindow();
      _dlg.SetContext(obj);
      _dlg.ShowDialog();
    }
    private void Vm_AddNewItem(BindableBaseViewModel obj)
    {
      _dlg = new PopupWindow();
      _dlg.SetContext(obj);
      _dlg.ShowDialog();
    }

    public void SetMenuAlignment()
    {
      var ifLeft = SystemParameters.MenuDropAlignment;

      if (ifLeft)
      {
        // change to false
        var t = typeof(SystemParameters);
        var field = t.GetField("_menuDropAlignment", BindingFlags.NonPublic | BindingFlags.Static);
        field.SetValue(null, false);

        ifLeft = SystemParameters.MenuDropAlignment;
      }
    }


  }
}
