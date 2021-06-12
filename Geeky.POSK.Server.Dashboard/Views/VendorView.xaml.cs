using Geeky.POSK.Server.ViewModels;
using Geeky.POSK.WPF.Core;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
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
using Image = System.Drawing.Image;
using ImageConverter = Geeky.POSK.WPF.Core.ImageConverter;

namespace Geeky.POSK.Server.Dashboard.Views
{
  public partial class VendorView : UserControl
  {
    public VendorView()
    {
      InitializeComponent();
    }

    private void changeLogo_Click(object sender, RoutedEventArgs e)
    {
      OpenFileDialog dlg = new OpenFileDialog();
      dlg.Filter = "Files|*.jpg;*.jpeg;*.png;";
      if (dlg.ShowDialog() == true)
      {
        var imageFile = dlg.FileName;
        try
        {
          var _image = Image.FromFile(imageFile);
          var bitmap = (Bitmap)_image;
          var imageBytes = ImageHelper.GetBytes(bitmap);
          var vm = this.DataContext as VendorViewModel;
          vm.Dto.Logo = imageBytes;
        }
        catch (Exception ex)
        {
          //todo
        }
      }
    }

    private void changePrintableLogo_Click(object sender, RoutedEventArgs e)
    {
      OpenFileDialog dlg = new OpenFileDialog();
      dlg.Filter = "Files|*.jpg;*.jpeg;*.png;";
      if (dlg.ShowDialog() == true)
      {
        var imageFile = dlg.FileName;
        try
        {
          var _image = Image.FromFile(imageFile);
          var bitmap = (Bitmap)_image;
          var imageBytes = ImageHelper.GetBytes(bitmap);
          var vm = this.DataContext as VendorViewModel;
          vm.Dto.PrintedLogo = imageBytes;
        }
        catch (Exception ex)
        {
          //todo
        }
      }
    }

    public static void SaveJpeg(Image img, Stream stream, long quality)
    {
      var encoderParameters = new EncoderParameters(1);
      encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
      img.Save(stream, GetEncoder(ImageFormat.Png), encoderParameters);
    }

    static ImageCodecInfo GetEncoder(ImageFormat format)
    {
      ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
      return codecs.Single(codec => codec.FormatID == format.Guid);
    }
  }
}
