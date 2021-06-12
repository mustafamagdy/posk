using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace POSK.Client.ApplicationService
{
  public static class ServiceSecurityHelper
  {
    private static string SERVICE_USER_PASSWORD = "";
    static ServiceSecurityHelper()
    {
      var appKey = "ServiceKey";
      SERVICE_USER_PASSWORD = ConfigurationManager.AppSettings.AllKeys.Contains(appKey) ?
              ConfigurationManager.AppSettings[appKey] : "";
    }
    public static void SetCredentials<TChannel>(this ClientBase<TChannel> proxy) where TChannel : class
    {
      //to be able to test it locally without credentials
      if (proxy.Endpoint.Address.Uri.Host.ToLower() != "localhost")
      {
        proxy.ClientCredentials.Windows.ClientCredential.UserName = "KIOSK-SERVICE";
        proxy.ClientCredentials.Windows.ClientCredential.Password = SERVICE_USER_PASSWORD;
      }
    }
  }
}
