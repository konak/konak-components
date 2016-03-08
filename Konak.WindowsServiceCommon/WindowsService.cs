using Konak.Common.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration.Install;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Konak.WindowsServiceCommon
{
    public class WindowsService : ServiceBase
    {
        #region Private properties
        private string _displayName = String.Empty;
        #endregion

        #region Properties

        #region DisplayName
        public string DisplayName
        {
            get
            {
                return CH.IsEmpty(this._displayName) ? this.ServiceName : this._displayName;
            }
            set
            {
                _displayName = value;
            }
        }
        #endregion

        #region WorkerThreads
        public WorkerThread[] WorkerThreads { get; set; }
        #endregion

        #region IsInteractive
        protected static bool IsInteractive
        {
            get
            {
                return Environment.UserInteractive;
            }
        }
        #endregion

        #endregion

        #region Methods

        #region ShowMessageBox
        protected static void ShowMessageBox(string message, params object[] args)
        {
            try
            {
                if (!IsInteractive) return;

                Type type;
                object[] info;

                type = Type.GetType("System.Windows.Forms.MessageBox, " +
                                    "System.Windows.Forms, " +
                                    "Version=1.0.5000.0, " +
                                    "Culture=neutral, " +
                                    "PublicKeyToken=b77a5c561934e089");

                info = new Object[2];
                info[0] = String.Format(message, args);
                info[1] = Assembly.GetExecutingAssembly().GetName().Name;
                type.InvokeMember("Show", BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod, null, null, info);
            }
            catch { }
        }
        #endregion

        #region InstallOrUninstall
        protected static bool InstallOrUninstall(string[] args, Installer serviceInstaller)
        {
            bool mustInstall = MustInstall(args);
            bool mustUninstall = MustUnInstall(args);

            if (!(mustInstall || mustUninstall))
                return false;

            if (CH.IsEmpty(serviceInstaller) ||
                CH.IsEmpty(serviceInstaller.Installers))
                return false;

            if (mustUninstall)
            {
                Uninstall(serviceInstaller, IsSilent(args));
                return true;
            }

            // Install services using the installer class.
            Install(serviceInstaller, IsSilent(args));
            return true;
        }
        #endregion

        #region Install
        protected static void Install(Installer serviceInstaller, bool isSilent)
        {
            if (CH.IsEmpty(serviceInstaller) || CH.IsEmpty(serviceInstaller.Installers)) return;

            StringBuilder names = new StringBuilder();

            foreach (Installer temp in serviceInstaller.Installers)
            {
                try
                {
                    if (temp is ServiceInstaller)
                    {
                        string serviceName = ((ServiceInstaller)temp).ServiceName;

                        if (!CH.IsEmpty(serviceName))
                            names.AppendLine("- " + serviceName);

                        if (IsRunning(serviceName))
                        {
                            try
                            {
                                StopService(serviceName, 0);
                            }
                            catch { /* Not a critical error. */ }
                        }
                    }
                }
                catch { /* Not a critical error. */ }
            }

            TransactedInstaller installer = new TransactedInstaller();

            installer.Installers.Add(serviceInstaller);

            String path = String.Format("/assemblypath={0}", Process.GetCurrentProcess().MainModule.FileName);
            String[] cmdline = { path };
            InstallContext ctx = new InstallContext("", cmdline);

            installer.Context = ctx;

            try
            {
                installer.Install(new Hashtable());
            }
            catch (Exception ex)
            {
                if (isSilent)
                    throw;

                StringBuilder msgs = new StringBuilder();

                while (ex != null)
                {
                    if (!CH.IsEmpty(ex.Message))
                        msgs.Append(" ").Append(ex.Message);

                    ex = ex.InnerException;
                }

                ShowMessageBox("SETUP ERROR:{0}{0}{1}", Environment.NewLine, msgs.ToString());

                return;
            }

            if (isSilent)
                return;

            ShowMessageBox("Successfully installed Windows service(s):{0}{0}{1}", Environment.NewLine, names.ToString());

        }
        #endregion

        #region Uninstall
        protected static void Uninstall(Installer serviceInstaller, bool isSilent)
        {
            if (CH.IsEmpty(serviceInstaller) || CH.IsEmpty(serviceInstaller.Installers))
                return;

            StringBuilder names = new StringBuilder();

            // Stop each service if needed.
            foreach (Installer temp in serviceInstaller.Installers)
            {
                try
                {
                    if (temp is ServiceInstaller)
                    {
                        string serviceName = ((ServiceInstaller)temp).ServiceName;

                        if (CH.IsEmpty(serviceName))
                            continue;

                        names.AppendLine("- " + serviceName);

                        if (IsRunning(serviceName))
                        {
                            try
                            {
                                StopService(serviceName, 0);
                            }
                            catch { /* Not a critical error. */ }
                        }
                    }
                }
                catch { /* Not a critical error. */ }
            }

            TransactedInstaller installer = new TransactedInstaller();

            installer.Installers.Add(serviceInstaller);

            String path = String.Format("/assemblypath={0}", Process.GetCurrentProcess().MainModule.FileName);
            String[] cmdline = { path };
            InstallContext ctx = new InstallContext("", cmdline);

            installer.Context = ctx;

            try
            {
                installer.Uninstall(null);
            }
            catch (Exception ex)
            {
                if (isSilent)
                    throw;

                StringBuilder msgs = new StringBuilder();

                while (ex != null)
                {
                    if (!CH.IsEmpty(ex.Message))
                        msgs.Append(" ").Append(ex.Message);
                    ex = ex.InnerException;
                }

                ShowMessageBox("SETUP ERROR:{0}{0}{1}", Environment.NewLine, msgs.ToString());

                return;
            }

            if (isSilent)
                return;

            ShowMessageBox("Successfully uninstalled Windows service(s):{0}{0}{1}", Environment.NewLine, names.ToString());
        }
        #endregion

        #region IsInstalled
        protected static bool IsInstalled(string serviceName)
        {
            try
            {
                ServiceController scm = new ServiceController(serviceName);
            }
            catch
            {
                return false;
            }

            return true;
        }
        #endregion

        #region IsRunning
        protected static bool IsRunning ( string serviceName )
        {
            try
            {
                ServiceController scm = new ServiceController(serviceName);

                return (scm.Status == ServiceControllerStatus.Running);
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region IsSilent
        protected static bool IsSilent ( params object[] args )
        {
            if (!IsInteractive)
                return false;

            foreach (string arg in args)
            {
                if (!CH.IsEmpty(arg))
                {
                    switch(arg.Trim().ToLower())
                    {
                        case "/s":
                        case "/silent":
                            return true;

                        default:
                            break;
                    }
                }
            }
            return false;
        }
        #endregion

        #region MustInstall
        protected static bool MustInstall(params object[] args)
        {
            foreach (string arg in args)
            {
                if (!CH.IsEmpty(arg))
                {
                    switch (arg.Trim().ToLower())
                    {
                        case "/i":
                        case "/inst":
                        case "/install":
                        case "/r":
                        case "/reg":
                        case "/register":
                        case "/regserver":
                        case "/regservice":
                            return true;

                        default:
                            break;
                    }
                }
            }
            return false;
        }
        #endregion

        #region MustUnInstall
        protected static bool MustUnInstall(params object[] args)
        {
            foreach (string arg in args)
            {
                if (!CH.IsEmpty(arg))
                {
                    switch (arg.Trim().ToLower())
                    {
                        case "/u":
                        case "/uninst":
                        case "/uninstall":
                        case "/unreg":
                        case "/unregister":
                        case "/unregserver":
                        case "/unregservice":
                            return true;

                        default:
                            break;
                    }
                }
            }
            return false;
        }
        #endregion

        #region OnContinue
        protected override void OnContinue()
        {
            if (CH.IsEmpty(WorkerThreads))
                return;

            for (int i = 0; i < WorkerThreads.Length; i++)
                WorkerThreads[i].IsPaused = false;

            base.OnContinue();
        }
        #endregion

        #region OnPause
        protected override void OnPause()
        {
            if (CH.IsEmpty(WorkerThreads))
                return;

            for (int i = 0; i < WorkerThreads.Length; i++)
                WorkerThreads[i].IsPaused = true;

            base.OnPause();
        }
        #endregion

        #region Start
        protected virtual void Start(string[] args)
        {
            if (CH.IsEmpty(WorkerThreads))
                return;

            for (int i = 0; i < WorkerThreads.Length; i++)
                if (WorkerThreads[i] != null)
                    WorkerThreads[i].Start();
        }
        #endregion

        #region Stop
        protected new virtual void Stop()
        {
            if (CH.IsEmpty(WorkerThreads))
                return;

            for (int i = 0; i < WorkerThreads.Length; i++)
            {
                WorkerThread wt = WorkerThreads[i];

                if (wt != null)
                {
                    wt.IsStopping = true;
                    wt.Stop();
                }
            }
        }
        #endregion

        #region OnStart
        protected override void OnStart(string[] args)
        {
            Start(args);
        }
        #endregion

        #region OnStop
        protected override void OnStop()
        {
            Stop();
        }
        #endregion

        #region Run
        protected static void Run ( ServiceBase[] services, string[] args )
        {
            if (CH.IsEmpty(services))
                return;

            if (IsInteractive)
            {
                foreach (ServiceBase service in services)
                {
                    if (service != null && service is WindowsService)
                        ((WindowsService)service).Start(args);
                }

                Thread.CurrentThread.Priority = ThreadPriority.Lowest;

                while (true)
                    Thread.Sleep(1000);
            }
            else
            {
                ServiceBase.Run(services);
            }
        }
        #endregion

        #region StartService
        public static void StartService(string serviceName)
        {
            StartService(serviceName, 0);
        }
        public static void StartService(string serviceName, int timeout)
        {
            ServiceController scm;
            try
            {
                scm = new ServiceController(serviceName);
            }
            catch (Exception)
            {
                throw;
            }

            if (scm.Status == ServiceControllerStatus.Running)
                return;

            if (scm.Status != ServiceControllerStatus.StartPending)
            {
                try
                {
                    scm.Start();
                }
                catch (Exception ex)
                {
                    throw new Exception("Cannot start the Windows service '" + serviceName + "'.", ex);
                }
            }

            if (timeout == 0)
                return;

            if (timeout > 0)
                timeout *= 1000;	// msec

            int sleepTime = 100;					// msec

            while (scm.Status != ServiceControllerStatus.Running && timeout != 0)
            {
                Thread.Sleep(sleepTime);

                if (timeout > 0)
                    timeout -= sleepTime;
            }
        }
        #endregion

        #region StopService
        public static void StopService(string serviceName)
        {
            StopService(serviceName, 0);
        }

        public static void StopService(string serviceName, int timeout)
        {
            ServiceController scm;
            try
            {
                scm = new ServiceController(serviceName);
            }
            catch (Exception)
            {
                throw;
            }

            if (scm.Status == ServiceControllerStatus.Stopped)
                return;

            if (scm.Status != ServiceControllerStatus.StopPending)
            {
                try
                {
                    scm.Stop();
                }
                catch (Exception ex)
                {
                    throw new Exception("Cannot stop the Windows service '" + serviceName + "'.", ex);
                }
            }

            if (timeout == 0)
                return;

            if (timeout > 0)
                timeout *= 1000;	// msec

            int sleepTime = 100;	// msec

            while (scm.Status != ServiceControllerStatus.Stopped && timeout != 0)
            {
                Thread.Sleep(sleepTime);

                if (timeout > 0)
                    timeout -= sleepTime;
            }
        }
        #endregion

        #endregion
    }
}
