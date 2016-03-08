using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Konak.WindowsServiceCommon
{
    public abstract class TimerThread : WorkerThread
    {
        #region Private properties
        private Timer _timer = null;
        private int _delay = 0;
        private int _interval = 0;
        //private DateTime _lastExecutionTime = DateTime.MinValue;
        private object _lockObject = new object();
        #endregion

        #region properties

        #region DefaultDelay
        public int DefaultDelay { get { return 0; } }
        #endregion

        #region Delay
        public int Delay
        {
            get
            {
                return _delay;
            }
            set
            {
                this._delay = value < 0 ? this.DefaultDelay : value;
            }
        }
        #endregion

        #region DefaultInterval
        public int DefaultInterval { get { return 1000; } }
        #endregion

        #region Interval property
        public int Interval
        {
            get
            {
                return _interval;
            }
            set
            {
                lock (this._lockObject)
                {
                    if (_interval == value)
                        return;

                    this._interval = value < 0 ? this.DefaultInterval : value;
                }

                if (IsStarted)
                {
                    Start();
                }
            }
        }
        #endregion

        #region LastExecutionTime property
        public DateTime LastExecutionTime { get; private set;}
        #endregion

        #endregion

        #region Constructors
        public TimerThread ( )
        {
            this.LastExecutionTime = DateTime.MinValue;
        }

        public TimerThread(string name, int delay, int interval)
            : this()
        {
            this.Name = name;
            this.Delay = delay;
            this.Interval = interval;
        }
        #endregion

        #region Methods

        #region Run method
        protected abstract void Run(object source);
        #endregion

        #region TimerThreadCallback
        protected virtual void TimerThreadCallback(object source)
        {
            if (!Monitor.TryEnter(this._timer))
                return;

            lock (this._lockObject)
            {
                try
                {
                    if (this.IsProcessing || this.IsDisabled || this.IsPaused)
                        return;

                    this.IsProcessing = true;

                    Run(source);

                    this.LastExecutionTime = DateTime.Now;
                }
                finally
                {
                    this.IsProcessing = false;

                    Monitor.Exit(_timer);
                }
            }
        }
        #endregion

        #region Start
        public override void Start()
        {
            // In case this thread is already running, stop it.
            Stop();

            lock (this._lockObject)
            {
                if (Konak.Common.Helpers.CH.IsEmpty(StartTime))
                    StartTime = DateTime.Now;

                if (_timer == null)
                    _timer = new Timer(new TimerCallback(TimerThreadCallback), null, _delay, _interval);
            }
        }
        #endregion

        #region Stop
        public override void Stop ( )
        {
            lock (this._lockObject)
            {
                if (_timer != null)
                {
                    _timer.Dispose();
                    _timer = null;
                }
            }
        }
        #endregion

        #endregion
    }
}
