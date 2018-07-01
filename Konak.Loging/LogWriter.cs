using Konak.Logging.Config;
using Konak.Logging.Writers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konak.Logging
{
    /// <summary>
    /// Logging level
    /// </summary>
    public enum LogLevel
    {
        ALL = 0,
        DEBUG = 1,
        INFO = 2,
        WARNING = 3,
        ERROR = 4,
        FATAL = 5,
        OFF = 6
    }

    /// <summary>
    /// Static class that implement functionality of logging errors and debugging information
    /// </summary>
    public static class LogWriter
    {
        #region static internal members
        /// <summary>
        /// Static property with configuration of log writer
        /// </summary>
        internal static ConfigSection CONFIG;

        /// <summary>
        /// List of different type of log writing objects
        /// </summary>
        internal static List<LogWriterBase> WRITERS;
        #endregion

        #region static constructor
        /// <summary>
        /// static construcror of LogWriter static class that inits settings for log writing and creates different writers
        /// </summary>
        static LogWriter()
        {
            CONFIG = ConfigSection.GetSection();
            WRITERS = new List<LogWriterBase>();

            if(CONFIG.FileWriter.Enabled)
                WRITERS.Add(new FileWriter(CONFIG.FileWriter));
            
        }
        #endregion

        #region Debug
        /// <summary>
        /// Write debugging information with provided log sender and message
        /// </summary>
        /// <param name="sender">source of the sended log</param>
        /// <param name="message">message to log</param>
        public static void Debug(string sender, string message)
        {
            DateTime date = DateTime.Now;

            foreach (LogWriterBase writer in WRITERS)
                writer.Enqueue(LogLevel.DEBUG, sender, message, date);
        }

        /// <summary>
        /// Write debugging information with provided log sender, message and exception instance
        /// </summary>
        /// <param name="sender">source of the sended log</param>
        /// <param name="message">message to log</param>
        /// <param name="exception">exception instance to write in log</param>
        public static void Debug(string sender, string message, Exception exception)
        {
            DateTime date = DateTime.Now;

            foreach (LogWriterBase writer in WRITERS)
                writer.Enqueue(LogLevel.DEBUG, sender, message, date, exception);
        }

        /// <summary>
        /// Write debugging information with provided log sender and exception instance
        /// </summary>
        /// <param name="sender">source of the sended log</param>
        /// <param name="exception">exception instance to write in log</param>
        public static void Debug(string sender, Exception exception)
        {
            DateTime date = DateTime.Now;

            foreach (LogWriterBase writer in WRITERS)
                writer.Enqueue(LogLevel.DEBUG, sender, exception.Message, date, exception);
        }
        #endregion

        #region Info
        /// <summary>
        /// Write information level log with provided log sender and message
        /// </summary>
        /// <param name="sender">source of the sended log</param>
        /// <param name="message">message to log</param>
        public static void Info(string sender, string message)
        {
            DateTime date = DateTime.Now;

            foreach (LogWriterBase writer in WRITERS)
                writer.Enqueue(LogLevel.INFO, sender, message, date);
        }

        /// <summary>
        /// Write information level log with provided log sender, message and exception instance
        /// </summary>
        /// <param name="sender">source of the sended log</param>
        /// <param name="message">message to log</param>
        /// <param name="exception">exception instance to write in log</param>
        public static void Info(string sender, string message, Exception exception)
        {
            DateTime date = DateTime.Now;

            foreach (LogWriterBase writer in WRITERS)
                writer.Enqueue(LogLevel.INFO, sender, message, date, exception);
        }

        /// <summary>
        /// Write information level log with provided log sender and exception instance
        /// </summary>
        /// <param name="sender">source of the sended log</param>
        /// <param name="exception">exception instance to write in log</param>
        public static void Info(string sender, Exception exception)
        {
            DateTime date = DateTime.Now;

            foreach (LogWriterBase writer in WRITERS)
                writer.Enqueue(LogLevel.INFO, sender, exception.Message, date, exception);
        }
        #endregion

        #region Warning
        /// <summary>
        /// Write warning information with provided log sender and message
        /// </summary>
        /// <param name="sender">source of the sended log</param>
        /// <param name="message">message to log</param>
        public static void Warning(string sender, string message)
        {
            DateTime date = DateTime.Now;

            foreach (LogWriterBase writer in WRITERS)
                writer.Enqueue(LogLevel.WARNING, sender, message, date);
        }

        /// <summary>
        /// Write warning information with provided log sender, message and exception instance
        /// </summary>
        /// <param name="sender">source of the sended log</param>
        /// <param name="message">message to log</param>
        /// <param name="exception">exception instance to write in log</param>
        public static void Warning(string sender, string message, Exception exception)
        {
            DateTime date = DateTime.Now;

            foreach (LogWriterBase writer in WRITERS)
                writer.Enqueue(LogLevel.WARNING, sender, message, date, exception);
        }

        /// <summary>
        /// Write warning information with provided log sender and exception instance
        /// </summary>
        /// <param name="sender">source of the sended log</param>
        /// <param name="exception">exception instance to write in log</param>
        public static void Warning(string sender, Exception exception)
        {
            DateTime date = DateTime.Now;

            foreach (LogWriterBase writer in WRITERS)
                writer.Enqueue(LogLevel.WARNING, sender, exception.Message, date, exception);
        }
        #endregion

        #region Error
        /// <summary>
        /// Write error information with provided log sender and message
        /// </summary>
        /// <param name="sender">source of the sended log</param>
        /// <param name="message">message to log</param>
        public static void Error(string sender, string message)
        {
            DateTime date = DateTime.Now;

            foreach (LogWriterBase writer in WRITERS)
                writer.Enqueue(LogLevel.ERROR, sender, message, date);
        }

        /// <summary>
        /// Write error information with provided log sender, message and exception instance
        /// </summary>
        /// <param name="sender">source of the sended log</param>
        /// <param name="message">message to log</param>
        /// <param name="exception">exception instance to write in log</param>
        public static void Error(string sender, string message, Exception exception)
        {
            DateTime date = DateTime.Now;

            foreach (LogWriterBase writer in WRITERS)
                writer.Enqueue(LogLevel.ERROR, sender, message, date, exception);
        }

        /// <summary>
        /// Write error information with provided log sender and exception instance
        /// </summary>
        /// <param name="sender">source of the sended log</param>
        /// <param name="exception">exception instance to write in log</param>
        public static void Error(string sender, Exception exception)
        {
            DateTime date = DateTime.Now;

            foreach (LogWriterBase writer in WRITERS)
                writer.Enqueue(LogLevel.ERROR, sender, exception.Message, date, exception);
        }
        #endregion

        #region Fatal
        /// <summary>
        /// Write fatal error information with provided log sender and message
        /// </summary>
        /// <param name="sender">source of the sended log</param>
        /// <param name="message">message to log</param>
        public static void Fatal(string sender, string message)
        {
            DateTime date = DateTime.Now;

            foreach (LogWriterBase writer in WRITERS)
                writer.Enqueue(LogLevel.FATAL, sender, message, date);
        }

        /// <summary>
        /// Write fatal error information with provided log sender, message and exception instance
        /// </summary>
        /// <param name="sender">source of the sended log</param>
        /// <param name="message">message to log</param>
        /// <param name="exception">exception instance to write in log</param>
        public static void Fatal(string sender, string message, Exception exception)
        {
            DateTime date = DateTime.Now;

            foreach (LogWriterBase writer in WRITERS)
                writer.Enqueue(LogLevel.FATAL, sender, message, date, exception);
        }

        /// <summary>
        /// Write fatal error information with provided log sender and exception instance
        /// </summary>
        /// <param name="sender">source of the sended log</param>
        /// <param name="exception">exception instance to write in log</param>
        public static void Fatal(string sender, Exception exception)
        {
            DateTime date = DateTime.Now;

            foreach (LogWriterBase writer in WRITERS)
                writer.Enqueue(LogLevel.FATAL, sender, exception.Message, date, exception);
        }
        #endregion
    }
}
