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
using Aki.Launcher.Models.Launcher;
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
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;

            //I'm not sure if you want application specific exception handling. AppDomain should handle them all AFAIK. You had something similar before, so I'm just adding this in. (might cause duplicate messageboxes though)
            Current.DispatcherUnhandledException += (sender, args) => HandleException(args.Exception);

            //check params - Example: Launcher.exe username:waffelord password:12345
            LauncherStartArgs startArgs = new LauncherStartArgs(e);

            if (startArgs.HasAuthentication)
            {
                ServerManager.LoadServer(LauncherSettingsProvider.Instance.Server.Url);
                int status = AccountManager.Login(startArgs.Username, startArgs.Password);

                //we only care if we actually logged in, to make sure we can start the game.
                if (status == 1)
                {
                    GameStarter gm = new GameStarter();
                    GameStarterResult gameStartResult = gm.LaunchGame(ServerManager.SelectedServer, AccountManager.SelectedAccount);

                    if(!gameStartResult.Succeeded)
                    {
                        MessageBox.Show(gameStartResult.Message);
                    }
                }
                else
                {
                    MessageBox.Show(LocalizationProvider.Instance.login_failed);
                }

                Application.Current.Shutdown(0);
            }
            else
            {
                // run launcher normally
                MainWindow LauncherWindow = new MainWindow();

                LauncherWindow.ShowDialog();
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
