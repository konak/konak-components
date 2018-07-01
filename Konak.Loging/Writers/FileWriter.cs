using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Konak.Logging.Config;
using System.IO;

namespace Konak.Logging.Writers
{
    /// <summary>
    /// Log writer that writes data to text file
    /// </summary>
    internal class FileWriter : LogWriterBase
    {
        #region private properties
        private FileWriterSettings _settings;
        private string _fileName;
        private int _fileSize;
        #endregion

        #region constructor
        /// <summary>
        /// Creates instance of FileWriter
        /// </summary>
        /// <param name="settings">settings of file writing logger</param>
        internal FileWriter(FileWriterSettings settings)
        {
            _minLogLevel = settings.Level;
            _settings = settings;
            ResetLogFileData();
        }
        #endregion

        #region WriteThread
        /// <summary>
        /// method that writes data to text file
        /// </summary>
        /// <param name="state"></param>
        internal override void WriteThread(object state)
        {
            try
            {
                LogMessage msg;

                while(_logQueue.TryDequeue(out msg))
                {
                    string data = msg.ToString();

                    try
                    {
                        File.AppendAllText(_fileName, data);
                    }
                    catch (Exception exx)
                    {
                        System.Diagnostics.Debug.WriteLine(exx);
                    }

                    _fileSize += data.Length;
                    if (_fileSize > _settings.MaxSize)
                        ResetLogFileData();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
            finally
            {
                lock (_writingSync)
                {
                    _isWriting = false;
                }
            }
        }
        #endregion

        #region ResetLogFileData
        /// <summary>
        /// Set new name for the log file and reset the size of the written data
        /// </summary>
        private void ResetLogFileData()
        {
            if (!Directory.Exists(_settings.Folder))
                Directory.CreateDirectory(_settings.Folder);

            _fileName = Path.Combine(_settings.Folder, _settings.FileNamePrefix + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss-fff.")  + _settings.FileExtension);
            _fileSize = 0;
        }
        #endregion
    }
}
