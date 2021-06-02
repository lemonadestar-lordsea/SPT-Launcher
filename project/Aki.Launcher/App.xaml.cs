/* App.xaml.cs
 * License: NCSA Open Source License
 * 
 * Copyright: Merijn Hendriks
 * AUTHORS:
 * waffle.lord
 * Merijn Hendriks
 */

using Aki.Launcher.Controllers;
using Aki.Launcher.Helpers;
using System;
using System.Windows;

namespace Aki.Launcher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object s, StartupEventArgs e)
        {
            //setup unhandled exception handling across the application.
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            Current.DispatcherUnhandledException += (sender, args) => HandleException(args.Exception);

            //spawn the mini debug window if SpawnMiniDebugWindow is true
            //Check out MiniDebugHelper for some additional information.
            if (MiniDebugHelper.Instance.SpawnMiniDebugWindow)
            {
                MiniDebugHelper.Instance.SpawnMiniDebugWindow = false;
                MiniDebugWindow debugWindow = new MiniDebugWindow();
                debugWindow.Show();

                //here is an example of how to send information to the mini debug window.
                MiniDebugHelper.LogMessage("Debug Window Spawned, Happy hunting!  >:)");
            }
        }

        private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception exception)
            {
                HandleException(exception);
            }
            else
            {
                HandleException(new Exception("Unknown Exception!"));
            }
        }

        private static void HandleException(Exception exception)
        {
            var text = $"Exception Message:{exception.Message}{Environment.NewLine}StackTrace:{exception.StackTrace}";
            LogManager.Instance.Error(text);

            MessageBox.Show(text, "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
