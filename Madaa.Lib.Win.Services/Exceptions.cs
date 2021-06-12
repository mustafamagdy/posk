using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Madaa.Lib.Win.Services.Msdtc
{
    [Serializable]
    public class ServiceNotInstalledException : System.Exception
    {
        public ServiceNotInstalledException()
        {
        }

        public ServiceNotInstalledException(string message)
            : base(message)
        {
        }

        public ServiceNotInstalledException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected ServiceNotInstalledException(SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}
