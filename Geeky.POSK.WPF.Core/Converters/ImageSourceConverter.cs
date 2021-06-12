using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Geeky.POSK.WPF.Core
{
  public class ImageConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return CreateBitmapSourceFromGdiBitmap(ImageHelper.LoadImage((byte[])value));
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return ImageSourceToBytes(value as ImageSource);
    }


    public static BitmapSource CreateBitmapSourceFromGdiBitmap(Bitmap bitmap)
    {
      if (bitmap == null)
      {
        //throw new ArgumentNullException("bitmap");
        return null;
      }

      var rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

      var bitmapData = bitmap.LockBits(
          rect,
          ImageLockMode.ReadWrite,
          System.Drawing.Imaging.PixelFormat.Format32bppArgb);

      try
      {
        var size = (rect.Width * rect.Height) * 4;

        return BitmapSource.Create(
            bitmap.Width,
            bitmap.Height,
            bitmap.HorizontalResolution,
            bitmap.VerticalResolution,
            PixelFormats.Bgra32,
            null,
            bitmapData.Scan0,
            size,
            bitmapData.Stride);
      }
      finally
      {
        bitmap.UnlockBits(bitmapData);
      }
    }
    public static byte[] ImageSourceToBytes(ImageSource imageSource)
    {
      BitmapEncoder encoder = new PngBitmapEncoder();
      byte[] bytes = null;
      var bitmapSource = imageSource as BitmapSource;

      if (bitmapSource != null)
      {
        encoder.Frames.Add(BitmapFrame.Create(bitmapSource));

        using (var stream = new MemoryStream())
        {
          encoder.Save(stream);
          bytes = stream.ToArray();
        }
      }

      return bytes;
    }
  }
}