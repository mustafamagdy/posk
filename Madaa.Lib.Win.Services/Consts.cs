using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Madaa.Lib.Win.Services.Msdtc
{
    public sealed class Consts
    {
        internal const string ServiceName = "MSDTC";
        internal const string ServiceFilePath = @"c:\windows\system32\msdtc.exe";
        internal const string ServiceFileName = @"msdtc.exe";
        internal const string ServiceFirewallRuleName = @"Distributed Transaction Coordinator";


        private const string MsdtcRegistryRootPath = @"HKEY_LOCAL_MACHINE\Software\Microsoft\MSDTC";

        internal const string NetworkDtcAccessKeyPath = MsdtcRegistryRootPath + @"\Security";
        internal const string NetworkDtcAccessValueName = @"NetworkDtcAccess";

        internal const string NetworkDtcAccessTransactionsKeyPath = MsdtcRegistryRootPath + @"\Security";
        internal const string NetworkDtcAccessTransactionsValueName = @"NetworkDtcAccessTransactions";

        internal const string NetworkDtcAccessInboundKeyPath = MsdtcRegistryRootPath + @"\Security";
        internal const string NetworkDtcAccessInboundValueName = @"NetworkDtcAccessInbound";

        internal const string NetworkDtcAccessOutboundKeyPath = MsdtcRegistryRootPath + @"\Security";
        internal const string NetworkDtcAccessOutboundValueName = @"NetworkDtcAccessOutbound";

        internal const string AllowOnlySecureRpcCallsKeyPath = MsdtcRegistryRootPath + @"\";
        internal const string AllowOnlySecureRpcCallsValueName = @"AllowOnlySecureRpcCalls";

        internal const string FallbackToUnsecureRpcIfNecessaryKeyPath = MsdtcRegistryRootPath + @"\";
        internal const string FallbackToUnsecureRpcIfNecessaryValueName = @"AllowOnlySecureRpcCalls";

        internal const string TurnOffRpcSecurityKeyPath = MsdtcRegistryRootPath + @"\";
        internal const string TurnOffRpcSecurityValueName = @"AllowOnlySecureRpcCalls";

        internal const string XaTransactionsKeyPath = MsdtcRegistryRootPath + @"\Security";
        internal const string XaTransactionsValueName = @"XaTransactions";

        internal const string SnaLuTransactionsKeyPath = MsdtcRegistryRootPath + @"\Security";
        internal const string SnaLuTransactionsValueName = @"LuTransactions";


    }
}
