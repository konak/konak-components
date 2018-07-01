using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konak.Logging.Writers
{
    /// <summary>
    /// Message that will be written to log store by log writer
    /// </summary>
    internal class LogMessage
    {
        #region internal propeties
        /// <summary>
        /// The date the log was written
        /// </summary>
        internal DateTime Date { get; set; }

        /// <summary>
        /// source of the sended log
        /// </summary>
        internal string Sender { get; set; }

        /// <summary>
        /// Text to log
        /// </summary>
        internal string Text { get; set; }

        /// <summary>
        /// Exception instance to write in log
        /// </summary>
        internal Exception LogException { get; set; }
        #endregion

        #region constructor
        /// <summary>
        /// create instance of LogMessage
        /// </summary>
        internal LogMessage()
        {
            Date = DateTime.Now;
            Text = string.Empty;
            LogException = null;
        }
        #endregion

        #region ToString
        /// <summary>
        /// Convert the value of the instance to string
        /// </summary>
        /// <returns>returns string implementation of instance</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("\r\n")
                .Append(Date.ToString("yyyy-MM-dd HH:mm:ss.fff"))
                .Append('\t')
                .Append(Sender)
                .Append('\t')
                .Append(Text);

            if (LogException != null)
                sb.Append("\r\n").AppendLine(GetExceptionText(LogException));

            sb.Append("\r\n");

            return sb.ToString();
        }
        #endregion

        #region GetExceptionText
        /// <summary>
        /// Convert exception instance to string
        /// </summary>
        /// <param name="ex">Exception instance to convert</param>
        /// <returns>returns string implementation ot the exception instance</returns>
        private string GetExceptionText(Exception ex)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("============================")
                .AppendLine("Message: ")
                .AppendLine(ex.Message)
                .AppendLine("StackTrace:")
                .AppendLine(ex.StackTrace);

            if(ex.InnerException != null)
            {
                sb.AppendLine("---------------------------")
                    .AppendLine(GetExceptionText(ex.InnerException))
                    .AppendLine("---------------------------");
            }

            sb.AppendLine("============================");

            return sb.ToString();
        }
        #endregion
    }
}
