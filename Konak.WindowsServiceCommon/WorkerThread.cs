using Konak.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Konak.WindowsServiceCommon
{
    public abstract class WorkerThread
    {
        #region Private properties
        private string _name = String.Empty;
        #endregion

        #region Properties

        protected bool IsDisabled { get; set; }
        protected bool IsProcessing { get; set; }
        protected bool IsStarted
        {
            get
            {
                return this.StartTime.HasValue;
            }
        }


        public DateTime? StartTime { get; set; }
        public bool IsStopping { get; set; }
        public bool IsPaused { get; set; }
        public TimeSpan LifeSpan
        {
            get
            {
                if (!this.StartTime.HasValue)
                    return new TimeSpan(0);

                return (DateTime.Now - this.StartTime.Value);
            }
        }
        public string Name
        {
            get
            {
                if (_name.IsEmpty())
                    return GetType().Name;

                return _name;
            }
            set
            {
                _name = value;
            }
        }
        #endregion

        #region constructor
        public WorkerThread()
        {
            this.StartTime = DateTime.MinValue;
            this.IsDisabled = false;
            this.IsPaused = false;
            this.IsProcessing = false;
            this.IsStopping = false;
        }
        #endregion

        #region methods
        #region SetThreadName method
        public void SetThreadName()
        {
            SetThreadName(_name);
        }

        public static void SetThreadName ( string name )
        {
            if (Thread.CurrentThread.Name.IsEmpty())
                Thread.CurrentThread.Name = name.IsEmpty() ? string.Empty : name;
        }
        #endregion

        #region Start methods
        public abstract void Start();
        #endregion

        #region Stop method
        public abstract void Stop();
        #endregion

        #endregion

    }
}
