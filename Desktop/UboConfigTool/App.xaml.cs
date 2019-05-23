using Microsoft.Shell;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace UboConfigTool
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// 
    /// See: http://blogs.microsoft.co.il/arik/2010/05/28/wpf-single-instance-application/
    /// 
    /// </summary>
    public partial class App : Application, ISingleInstanceApp
    {
        private const string UniqueName = @"{C4F588CD-D652-4329-8FE3-8F73CA2B7E05}";
        private static App _application;

        [STAThread]
        public static void Main()
        {
            if (SingleInstance<App>.InitializeAsFirstInstance(UniqueName))
            {
                // ServiceInjector.InjectServices();
                _application = new App();

                _application.InitializeComponent();
                _application.Run();

                // Allow single instance code to perform cleanup operations
                SingleInstance<App>.Cleanup();
            }
        }


        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

#if (DEBUG)
            RunInDebugMode();
#else
            RunInReleaseMode();
#endif
            this.ShutdownMode = ShutdownMode.OnMainWindowClose;
        }

        private static void RunInDebugMode()
        {
            UboConfigToolBootstrapper bootstrapper = new UboConfigToolBootstrapper();
            bootstrapper.Run();
        }

        private static void RunInReleaseMode()
        {
            AppDomain.CurrentDomain.UnhandledException += AppDomainUnhandledException;
            try
            {
                UboConfigToolBootstrapper bootstrapper = new UboConfigToolBootstrapper();
                bootstrapper.Run();
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        private static void AppDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            HandleException(e.ExceptionObject as Exception);
        }

        private static void HandleException(Exception ex)
        {
            if (ex == null)
                return;
            MessageBox.Show(ex.Message);
            Environment.Exit(1);
        }


        #region ISingleInstanceApp Members

        public bool SignalExternalCommandLineArgs(IList<string> args)
        {
            // this is executed in the original instance

            // we get the arguments to the second instance and can send them to the existing instance if desired

            // here we bring the existing instance to the front
            _application.MainWindow.BringToFront();

            // handle command line arguments of second instance

            return true;
        }

        #endregion
    }
}