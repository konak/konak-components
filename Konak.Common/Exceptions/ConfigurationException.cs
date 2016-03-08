using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konak.Common.Exceptions
{
    public class ConfigurationException : GenericException
    {

        #region constructors
        public ConfigurationException() : base(Resources.Exceptions.ConfigurationException.CONFIGURATION_GENERIC_ERROR) { }
        public ConfigurationException(Exception innerException) : base(Resources.Exceptions.ConfigurationException.CONFIGURATION_GENERIC_ERROR, innerException) { }
        public ConfigurationException(string message) : base(message) { }
        public ConfigurationException(string message, Exception innerException) : base(message, innerException) { }

        #endregion

    }
}
