﻿using Aki.Launch.Models.Aki;
using Aki.Launcher.Helpers;
using Aki.Launcher.Models.Launcher;
using ReactiveUI;
using Splat;
using System.Reactive.Disposables;
using System.Threading.Tasks;

namespace Aki.Launcher.ViewModels
{
    public class ConnectServerViewModel : ViewModelBase
    {
        private bool noAutoLogin = false;

        public ConnectServerModel connectModel { get; set; } = new ConnectServerModel()
        {
            InfoText = LocalizationProvider.Instance.server_connecting
        };

        public ConnectServerViewModel(IScreen Host, bool NoAutoLogin = false) : base(Host)
        {
            noAutoLogin = NoAutoLogin;

            this.WhenActivated((CompositeDisposable disposables) =>
            {
                Task.Run(async () =>
                {
                   await ConnectServer();
                });
            });
        }

        public async Task ConnectServer()
        {
            //Launcher starts server on boot if it's not already running.
            System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcessesByName("Aki.Server");
            if (processes.Length == 0){
                
                //Not running
                System.Diagnostics.Process.Start("Aki.Server.exe");
            }
            else{
                
                //Running
            }
            await ServerManager.LoadDefaultServerAsync(LauncherSettingsProvider.Instance.Server.Url);

            bool connected = ServerManager.PingServer();

            connectModel.ConnectionFailed = !connected;

            connectModel.InfoText = connected ? LocalizationProvider.Instance.ok : string.Format(LocalizationProvider.Instance.server_unavailable_format_1, LauncherSettingsProvider.Instance.Server.Name);

            if (connected)
            {
                AkiVersion version = Locator.Current.GetService<AkiVersion>("akiversion");

                version.ParseVersionInfo(ServerManager.GetVersion());

                NavigateTo(new LoginViewModel(HostScreen, noAutoLogin));
            }
        }

        public void RetryCommand()
        {
            //Start server from retry command when failing to connect to it.
            System.Diagnostics.Process.Start("Aki.Server.exe");
            connectModel.InfoText = LocalizationProvider.Instance.server_connecting;

            connectModel.ConnectionFailed = false;

            Task.Run(async () =>
            {
                await ConnectServer();
            });
        }
    }
}
