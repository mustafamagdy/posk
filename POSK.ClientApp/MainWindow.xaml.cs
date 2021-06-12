using Geeky.POSK.DataContracts;
using Geeky.POSK.Infrastructore.Core.Enums;
using Geeky.POSK.Models;
using Microsoft.Practices.Prism.Commands;
using POSK.Client.ViewModels;
using System;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;

namespace POSK.ClientApp
{
  public partial class MainWindow : Window
  {
    private bool _closingByAdmin = false;
    public MainWindow()
    {
      this.GotFocus += MainWindow_GotFocus;
      this.LostFocus += MainWindow_LostFocus;
      this.Closing += MainWindow_Closing;
      this.KeyDown += MainWindow_KeyDown;
      this.StateChanged += MainWindow_StateChanged;
      this.MouseEnter += MainWindow_MouseEnter;
      this.MouseMove += MainWindow_MouseMove;
      this.DataContextChanged += MainWindow_DataContextChanged;

      InitializeComponent();
    }

    private void MainWindow_StateChanged(object sender, EventArgs e)
    {
      this.WindowState = WindowState.Maximized;
    }

    private void MainWindow_KeyDown(object sender, KeyEventArgs e)
    {
      if (Keyboard.IsKeyDown(Key.RightCtrl) && e.Key == Key.D0)
      {
        _closingByAdmin = true;
        this.Close();
      }
      else
        _closingByAdmin = false;
    }

    private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if (!_closingByAdmin)
      {
        e.Cancel = true;
      }
    }

    private void MainWindow_MouseMove(object sender, MouseEventArgs e)
    {
      if (AppStartup.LOCKDOWN)
        HideMouse();
    }

    private void HideMouse()
    {
      if (AppStartup.SIMULATED_MODE)
      {
        Mouse.OverrideCursor = Cursors.Arrow;
      }
      else
      {
        Mouse.OverrideCursor = Cursors.None;
      }
    }

    private void MainWindow_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      var vm = this.DataContext as MainViewModel;
      if (vm != null)
      {
        MainViewModel.CurrentTerminalId = AppStartup.TerminalId;
        MainViewModel.TerminalSettings = AppStartup.TerminalSettings;
        MainViewModel.Simulated = AppStartup.SIMULATED_MODE;
        MainViewModel.CustomCeilingMode = AppStartup.CUSTOM_CEILING_MODE;
        vm.AddExtraInfoForPing += Vm_AddExtraInfoForPing;
        vm.LanguageChanged += Vm_LanguageChanged;
      }
    }

    private void Vm_AddExtraInfoForPing(ExtraInfo info)
    {
      AppStartup.SetExtraInfo(info);
    }

    private void Vm_LanguageChanged()
    {
      var vm = this.DataContext as MainViewModel;
      if (vm != null)
      {
        var culture = new CultureInfo(vm.CurrentLanguage);
        Thread.CurrentThread.CurrentCulture = culture;
        Thread.CurrentThread.CurrentUICulture = culture;
        LocalizationHelper.CurrentLanguage = vm.CurrentLanguage;
        WPFLocalizeExtension.Engine.LocalizeDictionary.Instance.SetCurrentThreadCulture = true;
        WPFLocalizeExtension.Engine.LocalizeDictionary.Instance.Culture = culture;
      }
    }

    private void MainWindow_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
    {
      if (AppStartup.LOCKDOWN)
        HideMouse();
    }

    private void MainWindow_GotFocus(object sender, RoutedEventArgs e)
    {
      if (AppStartup.LOCKDOWN)
        HideMouse();
    }

    private void MainWindow_LostFocus(object sender, RoutedEventArgs e)
    {
      this.Focus();
    }
  }
}
