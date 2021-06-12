using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Madaa.Lib.Win.Services.Msdtc
{
    public enum NetworkDTCAccessStatus
    {
        Unknown = -1,
        Off = 0,
        On = 1
    }

    public enum NetworkDtcAccessTransactionsStatus
    {
        Unknown = -1,
        Off = 0,
        On = 1
    }

    public enum NetworkDtcAccessInboundStatus
    {
        Unknown = -1,
        Off = 0,
        On = 1
    }


    public enum NetworkDtcAccessOutboundStatus
    {
        Unknown = -1,
        Off = 0,
        On = 1
    }

    public enum AllowOnlySecureRpcCallsStatus
    {
        Unknown = -1,
        Off = 0,
        On = 1
    }

    public enum FallbackToUnsecureRPCIfNecessaryStatus
    {
        Unknown = -1,
        Off = 0,
        On = 1
    }

    public enum TurnOffRpcSecurityStatus
    {
        Unknown = -1,
        Off = 0,
        On = 1
    }

    public enum AuthenticationRequiredType
    {
        MutualAuthenticationRequired = 0
       ,
        IncomingCallerAuthenticationRequired = 1
            , NoAuthenticationRequired = 2
    }
}
