using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Konak.Common.Helpers;
using Konak.HttpBrowser;

namespace Konak.WebCrawler
{
    public class Crawler
    {
        public event ErrorDelegate ErrorEvent;
        public event GetDataDelegate GetDataEvent;

        protected object _sync;
        protected ConcurrentQueue<ICrawlerJobItem> _jobs;

        private bool _processingJobs;

        private WebBrowser _browser;

        public Crawler()
        {
            _sync = new object();
            _jobs = new ConcurrentQueue<ICrawlerJobItem>();
            _browser = new WebBrowser();

            _processingJobs = false;
        }

        #region Raise event methods
        protected void RaiseErrorEvent(Exception ex)
        {
            ErrorDelegate evt = ErrorEvent;

            if (evt == null) return;

            foreach(Delegate dlg in evt.GetInvocationList())
                try
                {
                    dlg.DynamicInvoke(this, ex);
                }
                catch (Exception exx)
                {
                    System.Diagnostics.Debug.WriteLine(exx);
                }
        }

        protected void RaiseGetDataEvent(CrawledData data)
        {
            GetDataDelegate evt = GetDataEvent;

            if (evt == null) return;

            foreach (Delegate dlg in evt.GetInvocationList())
                try
                {
                    dlg.DynamicInvoke(data);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                }
        }
        #endregion

        public void AddJob(ICrawlerJobItem jobItem)
        {
            _jobs.Enqueue(jobItem);

            lock(_sync)
                if (_processingJobs)
                    return;
                else
                    ThreadPool.QueueUserWorkItem(new WaitCallback(JobProcessingThread));
        }

        protected void JobProcessingThread(object arguments)
        {
            lock (_sync)
                if (_processingJobs)
                    return;
                else
                    _processingJobs = true;

            ICrawlerJobItem job;

            try
            {
                while(_jobs.TryDequeue(out job))
                {
                    CrawledData data = GetJobData(job);

                    if (CH.IsEmpty(data)) continue;

                    RaiseGetDataEvent(data);
                }
            }
            catch (Exception ex)
            {
                RaiseErrorEvent(ex);
            }
            finally
            {
                lock(_sync)
                    _processingJobs = false;
            }
        }

        private CrawledData GetJobData(ICrawlerJobItem job)
        {
            _browser.BaseUri = job.BaseUri;
            _browser.Referer = job.Referer;
            if (job.ClearCoockies)
                _browser.ClearCoockies();

            _browser.LoadCoockies(job.Coockies);

            if (job.ClearHeaders)
                _browser.ClearHeaders();

            _browser.AddHeader(job.Headers);

            try
            {
                _browser.Navigate(job.RequestResource);
            }
            catch (Exception ex)
            {
                throw;
            }

            return new CrawledData()
            {
                Job = job,
                Charset = _browser.Charset,
                Coockies = _browser.GetCoockies(),
                DataBytes = _browser.GetBytes(),
                DataEncoding = _browser.DataEncoding,
                DataType = _browser.DataType,
                Html = _browser.GetHtml(),
                ResponseStatusCode = _browser.ResponseStatusCode
            };
        }
    }
}
