using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace ThermalProtectionBuilding.UI
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            AppDomain.CurrentDomain.UnhandledException += Handle_UnhandledException_AppDomain;
            Dispatcher.UnhandledException += Handle_UnhandledException_Dispatcher;
        }

        private void Handle_UnhandledException_AppDomain(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = (Exception)e.ExceptionObject;
            MessageBoxResult result = MessageBox.Show(exception.Message,
                                          "Confirmation",
                                          MessageBoxButton.OK,
                                          MessageBoxImage.Error);
        }

        private void Handle_UnhandledException_Dispatcher(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            Exception exception = e.Exception;
            MessageBoxResult result = MessageBox.Show(exception.Message,
                                          "Confirmation",
                                          MessageBoxButton.OK,
                                          MessageBoxImage.Error);
        }
    }
}
