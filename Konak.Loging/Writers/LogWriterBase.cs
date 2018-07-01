using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konak.Logging.Writers
{
    /// <summary>
    /// Base class for log writers
    /// </summary>
    internal abstract class LogWriterBase
    {
        #region protected members
        protected ConcurrentQueue<LogMessage> _logQueue = new ConcurrentQueue<LogMessage>();
        protected object _writingSync = new object();
        protected bool _isWriting = false;
        protected LogLevel _minLogLevel = LogLevel.OFF;
        #endregion

        #region Enqueue

        /// <summary>
        /// Enqueue log message for writing by log writer
        /// </summary>
        /// <param name="level">Level of the log to write</param>
        /// <param name="sender">source of the sended log</param>
        /// <param name="message">message to log</param>
        /// <param name="logDate">The date the log was written</param>
        internal void Enqueue(LogLevel level, string sender, string message, DateTime logDate)
        {
            if (level < _minLogLevel)
                return;

            _logQueue.Enqueue(new LogMessage() { Date = logDate, Sender = sender, Text = message });

            StartLogWritingThread();
        }

        /// <summary>
        /// Enqueue log message for writing by log writer
        /// </summary>
        /// <param name="level">Level of the log to write</param>
        /// <param name="sender">source of the sended log</param>
        /// <param name="message">message to log</param>
        /// <param name="logDate">The date the log was written</param>
        /// <param name="exception">Exception instance to write in log</param>
        internal void Enqueue(LogLevel level, string sender, string message, DateTime logDate, Exception exception)
        {
            if (level <= _minLogLevel)
                return;

            _logQueue.Enqueue(new LogMessage() { Date = logDate, Sender = sender, Text = message, LogException = exception });

            StartLogWritingThread();
        }
        #endregion

        #region StartLogWritingThread
        /// <summary>
        /// Start the thread of log writing process
        /// </summary>
        private void StartLogWritingThread()
        {
            System.Diagnostics.Debug.WriteLine("LogWriterBase.StartLogWritingThread entering lock");
            lock (_writingSync)
            {
                System.Diagnostics.Debug.WriteLine("LogWriterBase.StartLogWritingThread lock entered");
                if (_isWriting)
                    return;

                _isWriting = true;
                System.Diagnostics.Debug.WriteLine("LogWriterBase.StartLogWritingThread releasing lock");
            }
            System.Diagnostics.Debug.WriteLine("LogWriterBase.StartLogWritingThread lock released");

            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(WriteThread));
        }
        #endregion

        /// <summary>
        /// If overriden implements data writing method
        /// </summary>
        /// <param name="state">state of asyncronous process</param>
        internal abstract void WriteThread(object state);

    }
}
