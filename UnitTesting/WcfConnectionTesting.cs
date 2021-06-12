using Geeky.POSK.Client.Proxy;
using Geeky.POSK.DataContracts;
using Geeky.POSK.Infrastructore.Extensions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTesting
{
  [TestFixture]
  class WcfConnectionTesting : BaseUnitTest
  {

    [Test]
    public void TestPingService()
    {
      SystemClient proxy = new SystemClient();
      //Will fail bexause we didn't set credentials here .. check helper method SetCredential in ServiceSecurityHelper.cs

      var terminalId = Guid.NewGuid();
      var info = new ExtraInfo { };
      proxy.Ping(terminalId, info);
      proxy.Close();

      Assert.IsNotNull(proxy);
      Assert.AreEqual(proxy.State, System.ServiceModel.CommunicationState.Closed);
    }


    [Test]
    public void TestMultipleClients()
    {
      100.Loop(i =>
      {
        ProductsClient proxy = new ProductsClient();
        //Will fail bexause we didn't set credentials here .. check helper method SetCredential in ServiceSecurityHelper.cs

        proxy.Close();
      });
    }
  }
}
