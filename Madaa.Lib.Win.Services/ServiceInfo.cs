using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Madaa.Lib.Win.Services.Msdtc
{
    public struct ServiceInfo
    {
        public string DisplayName;
        public string MachineName;
        public bool IsInstalled;
        public bool CanStop;
        public System.ServiceProcess.ServiceControllerStatus Status;

        public override string ToString()
        {
            string result = string.Empty;
            result += "Display Name : " + this.DisplayName + Environment.NewLine;
            result += "Machine Name : " + this.MachineName + Environment.NewLine;
            result += "Is Installed : " + this.IsInstalled + Environment.NewLine;
            result += "Can Stop : " + this.CanStop + Environment.NewLine;
            result += "Status : " + this.Status + Environment.NewLine;

            return result;
        }
    }
}
