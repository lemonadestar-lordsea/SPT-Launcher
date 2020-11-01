/* ConnectServerViewModel.cs
 * License: NCSA Open Source License
 * 
 * Copyright: Merijn Hendriks
 * AUTHORS:
 * waffle.lord
 * Merijn Hendriks
 */


using SPTarkov.Launcher.Generics.AsyncCommand;
using SPTarkov.Launcher.Helpers;
using SPTarkov.Launcher.Models.Launcher;
using System;
using System.Threading.Tasks;

namespace SPTarkov.Launcher.ViewModel
{
    public class ConnectServerViewModel
    {
        public ConnectServerModel connectInfo { get; set; }
        public AwaitableDelegateCommand RetryAsyncCommand { get; set; }
        private NavigationViewModel navigationViewModel { get; set; }

        public ConnectServerViewModel(NavigationViewModel viewModel)
        {
            navigationViewModel = viewModel;

            RetryAsyncCommand = new AwaitableDelegateCommand(OnRetryAsyncCommand);

            ConnectServerModel tmpConnectInfo = new ConnectServerModel();
            connectInfo = tmpConnectInfo;

            LauncherSettingsProvider.Instance.AllowSettings = true;

            _ = OnRetryAsyncCommand();
        }

        public async Task OnRetryAsyncCommand()
        {
            if(!LauncherSettingsProvider.Instance.AllowSettings)
            {
                return;
            }

            if(LauncherSettingsProvider.Instance.ServerCollection.Count == 0)
            {
                connectInfo.InfoText = LocalizationProvider.Instance.no_servers_available;
                return;
            }

            LauncherSettingsProvider.Instance.AllowSettings = false;
            connectInfo.InfoText = $"{LocalizationProvider.Instance.server_connecting} ...";

            ServerSetting DefaultServer = LauncherSettingsProvider.GetDefaultServer();

            if(DefaultServer == null)
            {
                return;
            }

            await ServerManager.LoadDefaultServerAsync(DefaultServer.Url);

            //This should only be the server we are loading from default. So it should be safe if the count is equal 1.
            if (ServerManager.AvailableServers.Count == 1)
            {
                ServerManager.SelectServer(0);
                RequestHandler.ChangeBackendUrl(ServerManager.AvailableServers[0].backendUrl);

                if (DefaultServer.AutoLoginCreds == null || DefaultServer.AutoLoginCreds.Email == "" || DefaultServer.AutoLoginCreds.Password == "")
                {
                    LauncherSettingsProvider.Instance.AllowSettings = true;

                    navigationViewModel.SelectedViewModel = new RegisterViewModel(navigationViewModel);
                }
                else
                {
                    if (LauncherSettingsProvider.Instance.UseAutoLogin && DefaultServer.AutoLoginCreds != null)
                    {
                        int status = await AccountManager.LoginAsync(DefaultServer.AutoLoginCreds);

                        if (status == 1)
                        {
                            LauncherSettingsProvider.Instance.AllowSettings = true;

                            navigationViewModel.SelectedViewModel = new ProfileViewModel(navigationViewModel);
                        }
                        else
                        {
                            LauncherSettingsProvider.Instance.AllowSettings = true;

                            navigationViewModel.SelectedViewModel = new LoginViewModel(navigationViewModel);
                        }
                    }
                    else
                    {
                        LauncherSettingsProvider.Instance.AllowSettings = true;

                        navigationViewModel.SelectedViewModel = new LoginViewModel(navigationViewModel);
                    }
                }
            }
            else
            {
                connectInfo.InfoText = String.Format(LocalizationProvider.Instance.server_unavailable_format_1, DefaultServer.Name);
            }

            LauncherSettingsProvider.Instance.AllowSettings = true;
        }
    }
}
