using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konak.Logging.Config
{
    /// <summary>
    /// Settings for logwriter to file
    /// </summary>
    internal class FileWriterSettings : ConfigurationElement
    {
        /// <summary>
        /// Property that indicates if file write logger is enabled
        /// </summary>
        [ConfigurationProperty("enabled", DefaultValue = true, IsRequired = false)]
        internal bool Enabled
        {
            get { return (bool)this["enabled"]; }
            set { this["enabled"] = value; }
        }

        /// <summary>
        /// Folder the log files will be written in
        /// </summary>
        [ConfigurationProperty("folder", DefaultValue = @"C:\Temp", IsRequired = false)]
        internal string Folder
        {
            get { return (string)this["folder"]; }
            set { this["folder"] = value; }
        }

        /// <summary>
        /// Prefix of the log file
        /// </summary>
        [ConfigurationProperty("file_name_prefix", DefaultValue = "", IsRequired = false)]
        internal string FileNamePrefix
        {
            get { return (string)this["file_name_prefix"]; }
            set { this["file_name_prefix"] = value; }
        }

        /// <summary>
        /// Extension of the log file
        /// </summary>
        [ConfigurationProperty("file_extension", DefaultValue = "log", IsRequired = false)]
        internal string FileExtension
        {
            get { return (string)this["file_extension"]; }
            set { this["file_extension"] = value; }
        }

        /// <summary>
        /// Maximum size of the log file
        /// </summary>
        [ConfigurationProperty("max_size", DefaultValue = 2048000, IsRequired = false)]
        internal int MaxSize
        {
            get { return (int)this["max_size"]; }
            set { this["max_size"] = value; }
        }

        /// <summary>
        /// Level of the log message
        /// </summary>
        [ConfigurationProperty("level", DefaultValue = LogLevel.OFF, IsRequired = false)]
        internal LogLevel Level
        {
            get { return (LogLevel)this["level"]; }
            set { this["level"] = value.ToString(); }
        }
    }
}
