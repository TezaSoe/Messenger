using Messenger.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Unity;
using System;
using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace Messenger
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
        }

        protected override void OnInitialized()
        {
            Current.MainWindow.Show();
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule(typeof(MessengerModule));
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Current.DispatcherUnhandledException += Application_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            Current.DispatcherUnhandledException -= Application_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;
        }

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            ReportUnhandledException(e.Exception);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            ReportUnhandledException(e.ExceptionObject as Exception);
        }

        private void ReportUnhandledException(Exception ex)
        {
            Current.Shutdown();
        }
    }
}
