/******************************************************************************************   
*  Name        : [MSDTC Manager]
*  Version     : Beta    
*  Developer   : Tammam Koujan	
*  Description : Class library to manage and configure Microsoft Distributed Transaction Coordinator service.  
*******************************************************************************************
*                             Modification History     
*     
*  DATE			    	NAME	        			Version            NATURE OF CHANGES						
*------------------------------------------------------------------------------------------- 
*  Feb 18 2014		    Tammam Koujan				 1.0               Intial Version			
*      
*     
*------------------------------------------------------------------------------------------- 
*
*                             Refrences     
*     
*  Title			    		        			URL            					
*------------------------------------------------------------------------------------------- 
*  Distributed Transaction Coordinator		    	http://technet.microsoft.com/en-us/library/cc759136(v=ws.10).aspx			 
*      
*     
*------------------------------------------------------------------------------------------- 
*******************************************************************************************/
using System;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using Microsoft.Win32;
using NetFwTypeLib;


namespace Madaa.Lib.Win.Services.Msdtc
{
    public class MsdtcManager
    {


        #region Enums

        private enum ServiceActions
        {
            Start = 0,
            Stop = 1,
            Pause = 2,
            Continue = 3,
            Restart = 4
        }


        #endregion Enums

        #region Variables

        private bool _autoRestartService;
        private static int _timeoutMilliseconds;

        #endregion Variables

        #region Properties


        #region Private

        private NetworkDtcAccessTransactionsStatus NetworkDtcAccessTransactions
        {
            get
            {
                return GetNetworkDtcAccessTransactions();
            }
            set
            {
                SetNetworkDtcAccessTransactions(value);
            }
        }

        private NetworkDtcAccessInboundStatus NetworkDtcAccessInbound
        {
            get
            {
                return GetNetworkDtcAccessInbound();
            }
            set
            {
                SetNetworkDtcAccessInbound(value);
            }
        }

        private NetworkDtcAccessOutboundStatus NetworkDtcAccessOutbound
        {
            get
            {
                return GetNetworkDtcAccessOutbound();
            }
            set
            {
                SetNetworkDtcAccessOutbound(value);
            }
        }

        private AllowOnlySecureRpcCallsStatus AllowOnlySecureRpcCalls
        {
            get
            {
                return GetAllowOnlySecureRpcCalls();
            }
            set
            {
                SetAllowOnlySecureRpcCalls(value);
            }
        }

        private FallbackToUnsecureRPCIfNecessaryStatus FallbackToUnsecureRpcIfNecessary
        {
            get
            {
                return GetFallbackToUnsecureRpcIfNecessary();
            }
            set
            {
                SetFallbackToUnsecureRpcIfNecessary(value);
            }
        }

        private TurnOffRpcSecurityStatus TurnOffRpcSecurity
        {
            get
            {
                return GetTurnOffRpcSecurity();
            }
            set
            {
                SetTurnOffRpcSecurity(value);
            }
        }

        #endregion Private

        #region Public

        private static bool _needRestart;
        public bool NeedRestart
        {
            get
            {
                return _needRestart;
            }
        }

        public NetworkDTCAccessStatus NetworkDtcAccess
        {
            get
            {
                return GetNetworkDtcAccess();
            }
            set
            {
                SetNetworkDtcAccess(value);
                if (_autoRestartService && _needRestart == true)
                {
                    ServiceApplyAction(ServiceActions.Restart, _timeoutMilliseconds);
                }

            }
        }

        public bool AllowInbound
        {
            get
            {
                bool result = NetworkDtcAccess == NetworkDTCAccessStatus.On &&
                              NetworkDtcAccessTransactions == NetworkDtcAccessTransactionsStatus.On &&
                              NetworkDtcAccessInbound == NetworkDtcAccessInboundStatus.On;

                return result;
            }
            set
            {
                if (value == true)
                {
                    NetworkDtcAccess = NetworkDTCAccessStatus.On;
                    NetworkDtcAccessTransactions = NetworkDtcAccessTransactionsStatus.On;
                    NetworkDtcAccessInbound = NetworkDtcAccessInboundStatus.On;
                }
                else
                {
                    NetworkDtcAccessInbound = NetworkDtcAccessInboundStatus.Off;
                }
                if (_autoRestartService && _needRestart == true)
                {
                    ServiceApplyAction(ServiceActions.Restart, _timeoutMilliseconds);
                }

            }
        }

        public bool AllowOutbound
        {
            get
            {
                bool result = NetworkDtcAccess == NetworkDTCAccessStatus.On &&
                              NetworkDtcAccessTransactions == NetworkDtcAccessTransactionsStatus.On &&
                              NetworkDtcAccessOutbound == NetworkDtcAccessOutboundStatus.On;

                return result;
            }
            set
            {
                if (value == true)
                {
                    NetworkDtcAccess = NetworkDTCAccessStatus.On;
                    NetworkDtcAccessTransactions = NetworkDtcAccessTransactionsStatus.On;
                    NetworkDtcAccessOutbound = NetworkDtcAccessOutboundStatus.On;
                }
                else
                {
                    NetworkDtcAccessOutbound = NetworkDtcAccessOutboundStatus.Off;
                }
                if (_autoRestartService && _needRestart == true)
                {
                    ServiceApplyAction(ServiceActions.Restart, _timeoutMilliseconds);
                }

            }
        }

        public AuthenticationRequiredType AuthenticationRequired
        {
            get
            {

                if (AllowOnlySecureRpcCalls == AllowOnlySecureRpcCallsStatus.On &&
                    FallbackToUnsecureRpcIfNecessary == FallbackToUnsecureRPCIfNecessaryStatus.On &&
                    TurnOffRpcSecurity == TurnOffRpcSecurityStatus.On)
                {
                    return AuthenticationRequiredType.MutualAuthenticationRequired;
                }
                else if (AllowOnlySecureRpcCalls == AllowOnlySecureRpcCallsStatus.Off &&
                       FallbackToUnsecureRpcIfNecessary == FallbackToUnsecureRPCIfNecessaryStatus.On &&
                       TurnOffRpcSecurity == TurnOffRpcSecurityStatus.Off)
                {
                    return AuthenticationRequiredType.IncomingCallerAuthenticationRequired;
                }
                else
                {
                    return AuthenticationRequiredType.NoAuthenticationRequired;
                }


            }
            set
            {
                switch (value)
                {
                    case AuthenticationRequiredType.MutualAuthenticationRequired:
                        {
                            AllowOnlySecureRpcCalls = AllowOnlySecureRpcCallsStatus.On;
                            FallbackToUnsecureRpcIfNecessary = FallbackToUnsecureRPCIfNecessaryStatus.Off;
                            TurnOffRpcSecurity = TurnOffRpcSecurityStatus.Off;
                            break;
                        }
                    case AuthenticationRequiredType.IncomingCallerAuthenticationRequired:
                        {
                            AllowOnlySecureRpcCalls = AllowOnlySecureRpcCallsStatus.Off;
                            FallbackToUnsecureRpcIfNecessary = FallbackToUnsecureRPCIfNecessaryStatus.On;
                            TurnOffRpcSecurity = TurnOffRpcSecurityStatus.Off;
                            break;
                        }
                    case AuthenticationRequiredType.NoAuthenticationRequired:
                        {
                            AllowOnlySecureRpcCalls = AllowOnlySecureRpcCallsStatus.Off;
                            FallbackToUnsecureRpcIfNecessary = FallbackToUnsecureRPCIfNecessaryStatus.Off;
                            TurnOffRpcSecurity = TurnOffRpcSecurityStatus.On;
                            break;
                        }
                }
                if (_autoRestartService && _needRestart == true)
                {
                    ServiceApplyAction(ServiceActions.Restart, _timeoutMilliseconds);
                }

            }
        }

        public bool EnableXaTransactions
        {
            get
            {
                return GetXaTransactions();
            }
            set
            {
                SetXaTransactions(value);
                if (_autoRestartService && _needRestart == true)
                {
                    ServiceApplyAction(ServiceActions.Restart, _timeoutMilliseconds);
                }

            }
        }

        public bool EnableSnaLuTransactions
        {
            get
            {
                return GetSnaLuTransactions();
            }
            set
            {
                SetSnaLuTransactions(value);
                if (_autoRestartService && _needRestart == true)
                {
                    ServiceApplyAction(ServiceActions.Restart, _timeoutMilliseconds);
                }

            }
        }

        #endregion Public


        #endregion Properties

        #region Constructors

        public MsdtcManager()
        {
            _autoRestartService = false;
            _timeoutMilliseconds = 250;
            _needRestart = false;
        }

        public MsdtcManager(bool autoRestartService)
            : base()
        {
            _autoRestartService = autoRestartService;
        }

        public MsdtcManager(bool autoRestartService, int timeoutMilliseconds)
            : base()
        {
            _autoRestartService = autoRestartService;
            _timeoutMilliseconds = timeoutMilliseconds;
        }

        #endregion Constructors

        #region Methods

        #region Private

        #region Registry

        private object ReadRegistryKeyValue(string keyPath, string valueName)
        {
            return Registry.GetValue(keyPath, valueName, null);
        }

        private void WriteRegistryKeyValue(string keyPath, string valueName, object value)
        {
            Registry.SetValue(keyPath, valueName, value);
        }

        #endregion Registry

        #region Network DTC Access

        private NetworkDTCAccessStatus GetNetworkDtcAccess()
        {
            NetworkDTCAccessStatus result = NetworkDTCAccessStatus.Unknown;
            object keyValue = ReadRegistryKeyValue(Consts.NetworkDtcAccessKeyPath, Consts.NetworkDtcAccessValueName);
            if (keyValue != null)
            {
                switch (keyValue.ToString())
                {
                    case "0":
                        {
                            result = NetworkDTCAccessStatus.Off;
                            break;
                        }
                    case "1":
                        {
                            result = NetworkDTCAccessStatus.On;
                            break;
                        }
                }

            }
            return result;
        }

        private void SetNetworkDtcAccess(NetworkDTCAccessStatus value)
        {
            object objValue = null;

            if (value == NetworkDTCAccessStatus.Unknown) return;

            switch (value)
            {
                case NetworkDTCAccessStatus.On:
                    {
                        objValue = 1;
                        break;
                    }
                case NetworkDTCAccessStatus.Off:
                    {
                        objValue = 0;
                        break;
                    }
            }

            if (objValue == null) return;

            WriteRegistryKeyValue(Consts.NetworkDtcAccessKeyPath, Consts.NetworkDtcAccessValueName, objValue);
            _needRestart = true;
        }

        #endregion Network DTC Access

        #region NetworkDtcAccessTransactions

        private NetworkDtcAccessTransactionsStatus GetNetworkDtcAccessTransactions()
        {
            NetworkDtcAccessTransactionsStatus result = NetworkDtcAccessTransactionsStatus.Unknown;
            object keyValue = ReadRegistryKeyValue(Consts.NetworkDtcAccessTransactionsKeyPath,
                Consts.NetworkDtcAccessTransactionsValueName);
            if (keyValue != null)
            {
                switch (keyValue.ToString())
                {
                    case "0":
                        {
                            result = NetworkDtcAccessTransactionsStatus.Off;
                            break;
                        }
                    case "1":
                        {
                            result = NetworkDtcAccessTransactionsStatus.On;
                            break;
                        }
                }

            }
            return result;
        }

        private void SetNetworkDtcAccessTransactions(NetworkDtcAccessTransactionsStatus value)
        {
            object objValue = null;

            if (value == NetworkDtcAccessTransactionsStatus.Unknown) return;

            switch (value)
            {
                case NetworkDtcAccessTransactionsStatus.On:
                    {
                        objValue = 1;
                        break;
                    }
                case NetworkDtcAccessTransactionsStatus.Off:
                    {
                        objValue = 0;
                        break;
                    }
            }

            if (objValue == null) return;

            WriteRegistryKeyValue(Consts.NetworkDtcAccessTransactionsKeyPath,
                Consts.NetworkDtcAccessTransactionsValueName, objValue);
            _needRestart = true;
        }

        #endregion NetworkDtcAccessTransactions

        #region NetworkDtcAccessInbound

        private NetworkDtcAccessInboundStatus GetNetworkDtcAccessInbound()
        {
            NetworkDtcAccessInboundStatus result = NetworkDtcAccessInboundStatus.Unknown;
            object keyValue = ReadRegistryKeyValue(Consts.NetworkDtcAccessInboundKeyPath,
                Consts.NetworkDtcAccessInboundValueName);
            if (keyValue != null)
            {
                switch (keyValue.ToString())
                {
                    case "0":
                        {
                            result = NetworkDtcAccessInboundStatus.Off;
                            break;
                        }
                    case "1":
                        {
                            result = NetworkDtcAccessInboundStatus.On;
                            break;
                        }
                }

            }
            return result;
        }

        private void SetNetworkDtcAccessInbound(NetworkDtcAccessInboundStatus value)
        {
            object objValue = null;

            if (value == NetworkDtcAccessInboundStatus.Unknown) return;

            switch (value)
            {
                case NetworkDtcAccessInboundStatus.On:
                    {
                        objValue = 1;
                        break;
                    }
                case NetworkDtcAccessInboundStatus.Off:
                    {
                        objValue = 0;
                        break;
                    }
            }

            if (objValue == null) return;

            WriteRegistryKeyValue(Consts.NetworkDtcAccessInboundKeyPath, Consts.NetworkDtcAccessInboundValueName,
                objValue);
            _needRestart = true;
        }

        #endregion NetworkDtcAccessInbound

        #region NetworkDtcAccessOutbound

        private NetworkDtcAccessOutboundStatus GetNetworkDtcAccessOutbound()
        {
            NetworkDtcAccessOutboundStatus result = NetworkDtcAccessOutboundStatus.Unknown;
            object keyValue = ReadRegistryKeyValue(Consts.NetworkDtcAccessOutboundKeyPath,
                Consts.NetworkDtcAccessOutboundValueName);
            if (keyValue != null)
            {
                switch (keyValue.ToString())
                {
                    case "0":
                        {
                            result = NetworkDtcAccessOutboundStatus.Off;
                            break;
                        }
                    case "1":
                        {
                            result = NetworkDtcAccessOutboundStatus.On;
                            break;
                        }
                }

            }
            return result;
        }

        private void SetNetworkDtcAccessOutbound(NetworkDtcAccessOutboundStatus value)
        {
            object objValue = null;

            if (value == NetworkDtcAccessOutboundStatus.Unknown) return;

            switch (value)
            {
                case NetworkDtcAccessOutboundStatus.On:
                    {
                        objValue = 1;
                        break;
                    }
                case NetworkDtcAccessOutboundStatus.Off:
                    {
                        objValue = 0;
                        break;
                    }
            }

            if (objValue == null) return;

            WriteRegistryKeyValue(Consts.NetworkDtcAccessOutboundKeyPath, Consts.NetworkDtcAccessOutboundValueName,
                objValue);
            _needRestart = true;
        }

        #endregion NetworkDtcAccessOutbound

        #region AllowOnlySecureRpcCalls

        private AllowOnlySecureRpcCallsStatus GetAllowOnlySecureRpcCalls()
        {
            AllowOnlySecureRpcCallsStatus result = AllowOnlySecureRpcCallsStatus.Unknown;
            object keyValue = ReadRegistryKeyValue(Consts.AllowOnlySecureRpcCallsKeyPath, Consts.AllowOnlySecureRpcCallsValueName);
            if (keyValue != null)
            {
                switch (keyValue.ToString())
                {
                    case "0":
                        {
                            result = AllowOnlySecureRpcCallsStatus.Off;
                            break;
                        }
                    case "1":
                        {
                            result = AllowOnlySecureRpcCallsStatus.On;
                            break;
                        }
                }

            }
            return result;
        }

        private void SetAllowOnlySecureRpcCalls(AllowOnlySecureRpcCallsStatus value)
        {
            object objValue = null;

            if (value == AllowOnlySecureRpcCallsStatus.Unknown) return;

            switch (value)
            {
                case AllowOnlySecureRpcCallsStatus.On:
                    {
                        objValue = 1;
                        break;
                    }
                case AllowOnlySecureRpcCallsStatus.Off:
                    {
                        objValue = 0;
                        break;
                    }
            }

            if (objValue == null) return;

            WriteRegistryKeyValue(Consts.AllowOnlySecureRpcCallsKeyPath, Consts.AllowOnlySecureRpcCallsValueName, objValue);
            _needRestart = true;
        }

        #endregion AllowOnlySecureRpcCalls

        #region FallbackToUnsecureRPCIfNecessary

        private FallbackToUnsecureRPCIfNecessaryStatus GetFallbackToUnsecureRpcIfNecessary()
        {
            FallbackToUnsecureRPCIfNecessaryStatus result = FallbackToUnsecureRPCIfNecessaryStatus.Unknown;
            object keyValue = ReadRegistryKeyValue(Consts.FallbackToUnsecureRpcIfNecessaryKeyPath, Consts.FallbackToUnsecureRpcIfNecessaryValueName);
            if (keyValue != null)
            {
                switch (keyValue.ToString())
                {
                    case "0":
                        {
                            result = FallbackToUnsecureRPCIfNecessaryStatus.Off;
                            break;
                        }
                    case "1":
                        {
                            result = FallbackToUnsecureRPCIfNecessaryStatus.On;
                            break;
                        }
                }

            }
            return result;
        }

        private void SetFallbackToUnsecureRpcIfNecessary(FallbackToUnsecureRPCIfNecessaryStatus value)
        {
            object objValue = null;

            if (value == FallbackToUnsecureRPCIfNecessaryStatus.Unknown) return;

            switch (value)
            {
                case FallbackToUnsecureRPCIfNecessaryStatus.On:
                    {
                        objValue = 1;
                        break;
                    }
                case FallbackToUnsecureRPCIfNecessaryStatus.Off:
                    {
                        objValue = 0;
                        break;
                    }
            }

            if (objValue == null) return;

            WriteRegistryKeyValue(Consts.FallbackToUnsecureRpcIfNecessaryKeyPath, Consts.FallbackToUnsecureRpcIfNecessaryValueName, objValue);
            _needRestart = true;
        }

        #endregion FallbackToUnsecureRPCIfNecessary

        #region TurnOffRpcSecurity

        private TurnOffRpcSecurityStatus GetTurnOffRpcSecurity()
        {
            TurnOffRpcSecurityStatus result = TurnOffRpcSecurityStatus.Unknown;
            object keyValue = ReadRegistryKeyValue(Consts.TurnOffRpcSecurityKeyPath, Consts.TurnOffRpcSecurityValueName);
            if (keyValue != null)
            {
                switch (keyValue.ToString())
                {
                    case "0":
                        {
                            result = TurnOffRpcSecurityStatus.Off;
                            break;
                        }
                    case "1":
                        {
                            result = TurnOffRpcSecurityStatus.On;
                            break;
                        }
                }

            }
            return result;
        }

        private void SetTurnOffRpcSecurity(TurnOffRpcSecurityStatus value)
        {
            object objValue = null;

            if (value == TurnOffRpcSecurityStatus.Unknown) return;

            switch (value)
            {
                case TurnOffRpcSecurityStatus.On:
                    {
                        objValue = 1;
                        break;
                    }
                case TurnOffRpcSecurityStatus.Off:
                    {
                        objValue = 0;
                        break;
                    }
            }

            if (objValue == null) return;

            WriteRegistryKeyValue(Consts.TurnOffRpcSecurityKeyPath, Consts.TurnOffRpcSecurityValueName, objValue);
            _needRestart = true;
        }

        #endregion TurnOffRpcSecurity

        #region "XaTransactions"

        private bool GetXaTransactions()
        {
            bool result = false;
            object keyValue = ReadRegistryKeyValue(Consts.XaTransactionsKeyPath,
                Consts.XaTransactionsValueName);
            if (keyValue != null)
            {
                switch (keyValue.ToString())
                {
                    case "1":
                        {
                            result = true;
                            break;
                        }
                }

            }
            return result;
        }

        private void SetXaTransactions(bool value)
        {
            object objValue = null;

            switch (value)
            {
                case true:
                    {
                        objValue = 1;
                        break;
                    }
                case false:
                    {
                        objValue = 0;
                        break;
                    }
            }


            WriteRegistryKeyValue(Consts.XaTransactionsKeyPath,
                Consts.XaTransactionsValueName, objValue);
            _needRestart = true;
        }

        #endregion "XaTransactions"

        #region "SnaLuTransactions"

        private bool GetSnaLuTransactions()
        {
            bool result = false;
            object keyValue = ReadRegistryKeyValue(Consts.SnaLuTransactionsKeyPath,
                Consts.SnaLuTransactionsValueName);
            if (keyValue != null)
            {
                switch (keyValue.ToString())
                {
                    case "1":
                        {
                            result = true;
                            break;
                        }
                }

            }
            return result;
        }

        private void SetSnaLuTransactions(bool value)
        {
            object objValue = null;

            switch (value)
            {
                case true:
                    {
                        objValue = 1;
                        break;
                    }
                case false:
                    {
                        objValue = 0;
                        break;
                    }
            }


            WriteRegistryKeyValue(Consts.SnaLuTransactionsKeyPath,
                Consts.SnaLuTransactionsValueName, objValue);
            _needRestart = true;
        }

        #endregion "SnaLuTransactions"

        #region MSDTC Service

        private void ServiceApplyAction(ServiceActions serviceAction, int timeoutMilliseconds)
        {
            int millisec1 = Environment.TickCount;
            TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

            ServiceController service = new ServiceController(Consts.ServiceName);

            switch (serviceAction)
            {
                case ServiceActions.Start:
                    {
                        service.Start();
                        service.WaitForStatus(ServiceControllerStatus.Running, timeout);
                        _needRestart = false;
                        break;
                    }
                case ServiceActions.Stop:
                    {
                       if (!service.CanStop) throw new InvalidOperationException("Cannot Stop Service");                   
                            service.Stop();
                            service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
                        break;
                    }
                case ServiceActions.Pause:
                {
                    if (!service.CanPauseAndContinue)
                        throw new InvalidOperationException("Cannot Pause And Continue Service");
                    service.Pause();
                    service.WaitForStatus(ServiceControllerStatus.Paused, timeout);
                    break;
                }
                case ServiceActions.Continue:
                {
                    if (!service.CanPauseAndContinue)
                        throw new InvalidOperationException("Cannot Pause And Continue Service");
                    service.Continue();
                    service.WaitForStatus(ServiceControllerStatus.Running, timeout);
                    break;
                }
                case ServiceActions.Restart:
                    {

                        service.Stop();
                        service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);

                        // count the rest of the timeout
                        int millisec2 = Environment.TickCount;
                        timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds - (millisec2 - millisec1));

                        service.Start();
                        service.WaitForStatus(ServiceControllerStatus.Running, timeout);
                        _needRestart = false;
                        break;
                    }
            }



        }

        #endregion MSDTC Service

        #region Firewall

        private void ChangeWindowsFirewallExceptionStatus(NET_FW_PROFILE_TYPE2_ profileType, bool enable)
        {
            INetFwPolicy2 firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(
                Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
            firewallPolicy.EnableRuleGroup((int)profileType, "Distributed Transaction Coordinator", enable);
        }


        #endregion Firewall

        public void ResetToDefaultSettings()
        {
            SetNetworkDtcAccess(NetworkDTCAccessStatus.Off);
            SetNetworkDtcAccessTransactions(NetworkDtcAccessTransactionsStatus.Off);
            SetNetworkDtcAccessInbound(NetworkDtcAccessInboundStatus.Off);
            SetNetworkDtcAccessOutbound(NetworkDtcAccessOutboundStatus.Off);
            SetAllowOnlySecureRpcCalls(AllowOnlySecureRpcCallsStatus.On);
            SetFallbackToUnsecureRpcIfNecessary(FallbackToUnsecureRPCIfNecessaryStatus.Off);
            SetTurnOffRpcSecurity(TurnOffRpcSecurityStatus.Off);
            SetXaTransactions(false);
            SetSnaLuTransactions(false);

            if (_autoRestartService && _needRestart == true)
            {
                ServiceApplyAction(ServiceActions.Restart, _timeoutMilliseconds);
            }

        }

        #endregion

        #region Public

        #region MSDTC Service


        public bool IsServiceInstalled()
        {
            ServiceController[] services = ServiceController.GetServices();
            return services.Any(service => service.ServiceName == Consts.ServiceName);
        }

        public bool IsServiceInstalledAndRunning()
        {
            ServiceController[] services = ServiceController.GetServices();
            return (from service in services where service.ServiceName == Consts.ServiceName select service.Status != ServiceControllerStatus.Stopped).FirstOrDefault();
        }

        public ServiceInfo GetServiceInfo()
        {
            ServiceInfo serviceInfo = new ServiceInfo();
            serviceInfo.IsInstalled = false;
            serviceInfo.Status = new ServiceControllerStatus();
            ServiceController[] services = ServiceController.GetServices();
            foreach (ServiceController service in services)
            {
                if (service.ServiceName == Consts.ServiceName)
                {
                    serviceInfo.IsInstalled = true;
                    serviceInfo.DisplayName = service.DisplayName;
                    serviceInfo.MachineName = service.MachineName;
                    serviceInfo.Status = service.Status;
                    serviceInfo.CanStop = service.CanStop;
                    return serviceInfo;
                }
            }


            throw new ServiceNotInstalledException();
        }

        public ServiceControllerStatus GetServiceStatus()
        {
            ServiceController[] services = ServiceController.GetServices();
            foreach (ServiceController service in services)
            {
                if (service.ServiceName == Consts.ServiceName)
                {
                    return service.Status;
                }
            }


            throw new ServiceNotInstalledException();
        }

        //Mtxoci.dll is a dynamic-link library (DLL) 
        //that is used internally by the Microsoft ODBC Driver for Oracle and the Microsoft OLEDB Provider for Oracle 
        //in conjunction with Microsoft Distributed Transaction Coordinator (DTC) to provide transactional support to Oracle databases. 
        //Specifically, it translates the DTC transactions into the XA transactions that Oracle can understand. 
        //This component currently has no way of tracing the DTC and application messages received by it nor XA messages sent by it. 
        //This can make troubleshooting some problems extremely difficult.
        public void RegMtxoci()
        {
            System.Diagnostics.Process.Start(@"cmd.exe", @"/C " + "regsvr32 mtxoci.dll");
        }

        public void InstallService()
        {
            System.Diagnostics.Process.Start(@"cmd.exe", @"/C " + Consts.ServiceFileName + " -install");
        }

        public void UninstallService()
        {
            System.Diagnostics.Process.Start(@"cmd.exe", @"/C " + Consts.ServiceFileName + " -uninstall");
        }


        public void ChangeLogonAccount(String user, String password)
        {
            var startInfo = new System.Diagnostics.ProcessStartInfo("sc.exe")
            {
                Arguments = String.Format("config {0} obj= {1}", Consts.ServiceName, user),
                RedirectStandardOutput = true,
                UseShellExecute = false
            };

            if (!String.IsNullOrEmpty(password))
            {
                startInfo.Arguments += String.Format(" password= {0}", password);
            }

            using (var process = new System.Diagnostics.Process())
            {
                process.StartInfo = startInfo;

                if (process.Start())
                {
                    process.WaitForExit();
                    _needRestart = true;
                }

            }

            if (_autoRestartService && _needRestart == true)
            {
                ServiceApplyAction(ServiceActions.Restart, _timeoutMilliseconds);
            }
        } 



        public void StartService()
        {
            StartService(_timeoutMilliseconds);
        }

        public void StartService(int timeoutMilliseconds)
        {
            ServiceApplyAction(ServiceActions.Start, timeoutMilliseconds);
        }

        public void StopService()
        {
            StopService(_timeoutMilliseconds);
        }

        public void StopService(int timeoutMilliseconds)
        {
            ServiceApplyAction(ServiceActions.Stop, timeoutMilliseconds);
        }

        public void PauseService()
        {
            PauseService(_timeoutMilliseconds);
        }

        public void PauseService(int timeoutMilliseconds)
        {
            ServiceApplyAction(ServiceActions.Pause, timeoutMilliseconds);
        }

        public void ContinueService()
        {
            ContinueService(_timeoutMilliseconds);
        }

        public void ContinueService(int timeoutMilliseconds)
        {
            ServiceApplyAction(ServiceActions.Continue, timeoutMilliseconds);
        }

        public void RestartService()
        {
            RestartService(_timeoutMilliseconds);
        }

        public void RestartService(int timeoutMilliseconds)
        {
            ServiceApplyAction(ServiceActions.Restart, timeoutMilliseconds);
        }


        #endregion MSDTC Service

        #region Firewall

        public void EnableWindowsFirewallException(NET_FW_PROFILE_TYPE2_ profileType)
        {
            ChangeWindowsFirewallExceptionStatus(profileType, true);
        }

        public void DisableWindowsFirewallException(NET_FW_PROFILE_TYPE2_ profileType)
        {
            ChangeWindowsFirewallExceptionStatus(profileType, false);
        }

        public bool IsMsdtcRuleGroupEnabled(NET_FW_PROFILE_TYPE2_ profileType)
        {
            INetFwPolicy2 firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(
                Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
            return firewallPolicy.IsRuleGroupEnabled((int) profileType, Consts.ServiceFirewallRuleName);

        }

        #endregion Firewall


        #endregion Public

        #endregion Methods

    }
}
