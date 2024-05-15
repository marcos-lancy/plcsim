using GalaSoft.MvvmLight.Threading;
using Microsoft.VisualBasic;
using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using SimTableApplication.Properties;

namespace SimTableApplication
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static App()
        {
            DispatcherHelper.Initialize();

            //log unhandled Exceptions
            var path = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

            var logPath = Path.Combine(path, "PLCSIMADV_SimulationTables");
            try
            {
                Directory.CreateDirectory(logPath);
            }
            catch
            {
                logPath = string.Empty;
            }

            var logfile = Path.Combine(logPath, "error.log");

            //register @ unhandledExceptions and log the exception messages
            AppDomain.CurrentDomain.UnhandledException += (sender, eventArgs) =>
            {
                ExceptionHandler(logfile, sender, eventArgs != null ? eventArgs.ExceptionObject : null);
            };


        }

        private static void ExceptionHandler(string logfile, object sender, object exceptionObject)
        {
            var logCreated = false;
            var errorMessage = "";

            try
            {
                var exception = exceptionObject as Exception;

                var stringBuilder = new StringBuilder();
                var assembly = Assembly.GetExecutingAssembly();

                stringBuilder.AppendLine(new string('-', 200));
                stringBuilder.Append("timestamp: ").AppendLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss zzz", CultureInfo.InvariantCulture));

                stringBuilder.Append("full name: ");
                stringBuilder.AppendLine(assembly.GetName().FullName);

                stringBuilder.Append("sender: ");
                stringBuilder.AppendLine(sender != null ? sender.ToString() : "null");

                if (exception != null)
                {
                    stringBuilder.Append("target site: ");
                    stringBuilder.AppendLine(sender != null ? exception.TargetSite.ToString() : "null");

                    stringBuilder.Append("source: ");
                    stringBuilder.AppendLine(sender != null ? exception.Source : "null");

                    stringBuilder.Append("event: ");
                    stringBuilder.AppendLine(exception.ToString());

                    var innerException = exception.InnerException;

                    while (innerException != null)
                    {
                        stringBuilder.AppendLine(innerException.ToString());
                        innerException = innerException.InnerException;
                    }
                }
                else
                {
                    stringBuilder.Append("event: ");
                    stringBuilder.AppendLine(exceptionObject != null ? exceptionObject.ToString() : "null");
                }

                errorMessage = stringBuilder.ToString();

                File.AppendAllText(logfile, errorMessage, Encoding.UTF8);
                logCreated = true;
            }
            finally
            {
                var message = string.Format(CultureInfo.InvariantCulture, errorMessage);

                if (Application.Current != null)
                {
                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                    {
                        MessageBox.Show(Application.Current.MainWindow, message, "PLCSIM Advanced Simulation tables", MessageBoxButton.OK, MessageBoxImage.Error);
                    }));
                }
                else
                {
                    MessageBox.Show(message, "PLCSIM Advanced Simulation tables", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
