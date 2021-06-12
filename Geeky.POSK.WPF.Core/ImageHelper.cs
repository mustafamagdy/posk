using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Geeky.POSK.WPF.Core
{
  public class ImageHelper
  {
    public static Bitmap LoadImage(byte[] imageData)
    {
      if (imageData == null || imageData.Length == 0) return null;
      Bitmap image;
      using (var ms = new MemoryStream(imageData))
      {
        image = new Bitmap(ms);
      }
      return image;
    }

    public static byte[] GetBytes(Bitmap image)
    {
      if (image == null) return null;
      using (var stream = new MemoryStream())
      {
        image.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
        return stream.ToArray();
      }
    }
  }
}
