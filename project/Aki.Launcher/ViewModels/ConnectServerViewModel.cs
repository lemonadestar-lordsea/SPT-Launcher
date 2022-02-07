﻿using Aki.Launcher.Helpers;
using Aki.Launcher.Models.Launcher;
using ReactiveUI;
using System.Reactive.Disposables;
using System.Threading.Tasks;

namespace Aki.Launcher.ViewModels
{
    public class ConnectServerViewModel : ViewModelBase
    {
        public ConnectServerModel connectModel { get; set; } = new ConnectServerModel()
        {
            InfoText = LocalizationProvider.Instance.server_connecting
        };

        public ConnectServerViewModel(IScreen Host) : base(Host)
        {
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
            await ServerManager.LoadDefaultServerAsync(LauncherSettingsProvider.Instance.Server.Url);

            bool connected = ServerManager.PingServer();

            connectModel.ConnectionFailed = !connected;

            connectModel.InfoText = connected ? LocalizationProvider.Instance.ok : string.Format(LocalizationProvider.Instance.server_unavailable_format_1, LauncherSettingsProvider.Instance.Server.Name);

            if (connected)
            {
                NavigateTo(new LoginViewModel(HostScreen));
            }
        }

        public void RetryCommand()
        {
            connectModel.InfoText = LocalizationProvider.Instance.server_connecting;

            connectModel.ConnectionFailed = false;

            Task.Run(async () =>
            {
                await ConnectServer();
            });
        }
    }
}
