using Geeky.POSK.Infrastructore.Core;
using Geeky.POSK.Infrastructore.Extensions;
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

namespace LicTool
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    const string pass = "geeksclub.io";

    public MainWindow()
    {
      InitializeComponent();
    }

    private void GenerateLicBtn_Click(object sender, RoutedEventArgs e)
    {
      var validPinCount = PinCountTxt.Text;
      var validTerminalCount = TerminalCountTxt.Text;
      var licenseKey = $"{validTerminalCount}-{validPinCount}";
      LicKeyStringTxt.Text = licenseKey;
      var lic = StringCipher.Encrypt(licenseKey, pass);
      LicKeyTxt.Text = lic;
    }

    private void ValidateLicBtn_Click(object sender, RoutedEventArgs e)
    {
      if (LicKeyTxt.Text == "")
      {
        MessageBox.Show("Enter license key first");
        return;
      }

      var dec_lic = StringCipher.Decrypt(LicKeyTxt.Text, pass);
      var lic_parts = dec_lic.Split("-".ToCharArray());
      var lic_pins = lic_parts[0];
      var lic_terminals = lic_parts[1];

      var licenseKey = $"{lic_terminals}-{lic_pins}";
      LicKeyStringTxt.Text = licenseKey;
    }
  }
}
