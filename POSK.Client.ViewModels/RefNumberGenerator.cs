using Geeky.POSK.Infrastructore.Core.Extensions;

namespace POSK.Client.ViewModels
{
  public sealed class RefNumberGenerator
  {
    public static string random()
    {
      return Randomz.GetRandomNumber(10);
    }



  }
}
