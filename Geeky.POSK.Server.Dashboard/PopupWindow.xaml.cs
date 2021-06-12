﻿using Geeky.POSK.WPF.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Geeky.POSK.Server.Dashboard
{
  /// <summary>
  /// Interaction logic for PopupWindow.xaml
  /// </summary>
  public partial class PopupWindow : Window
  {
    public PopupWindow()
    {
      InitializeComponent();
    }
    public PopupWindow(BindableBaseViewModel viewModel)
    {
      InitializeComponent();
      SetContext(viewModel);
    }
    public void SetContext(BindableBaseViewModel viewModel)
    {
      DataContext = viewModel;
    }
  }
}
