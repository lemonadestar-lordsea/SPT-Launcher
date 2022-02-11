﻿using Aki.Launcher.Helpers;
using Aki.Launcher.MiniCommon;
using Aki.Launcher.Models;
using Aki.Launcher.Models.Launcher;
using Avalonia;
using ReactiveUI;
using System.Threading.Tasks;
using Aki.Launcher.Attributes;
using Aki.Launcher.ViewModels.Dialogs;
using Avalonia.Threading;

namespace Aki.Launcher.ViewModels
{
    [RequireLoggedIn]
    public class ProfileViewModel : ViewModelBase
    {
        public string CurrentUsername { get; set; }

        private string _CurrentEdition;
        public string CurrentEdition
        {
            get => _CurrentEdition;
            set => this.RaiseAndSetIfChanged(ref _CurrentEdition, value);
        }

        public string CurrentID { get; set; }

        public ProfileInfo ProfileInfo { get; set; } = AccountManager.SelectedProfileInfo;

        public ImageHelper SideImage { get; } = new ImageHelper();

        private GameStarter gameStarter = new GameStarter(new GameStarterFrontend());

        private ProcessMonitor monitor { get; set; }

        public ProfileViewModel(IScreen Host) : base(Host)
        {
            // cache and load side image if profile has a side
            if(AccountManager.SelectedProfileInfo != null && AccountManager.SelectedProfileInfo.Side != null)
            {
                ImageRequest.CacheSideImage(AccountManager.SelectedProfileInfo.Side);
                SideImage.Path = AccountManager.SelectedProfileInfo.SideImage;
                SideImage.Touch();
            }

            monitor = new ProcessMonitor("EscapeFromTarkov", 1000, aliveCallback: GameAliveCallBack, exitCallback: GameExitCallback);

            CurrentUsername = AccountManager.SelectedAccount.username;

            CurrentEdition = AccountManager.SelectedAccount.edition;

            CurrentID = AccountManager.SelectedAccount.id;
        }

        public void LogoutCommand()
        {
            AccountManager.Logout();

            NavigateTo(new ConnectServerViewModel(HostScreen, true));
        }

        public void ChangeWindowState(Avalonia.Controls.WindowState? State, bool Close = false)
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                if (Application.Current.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
                {
                    if (Close)
                    {
                        desktop.ShutdownMode = Avalonia.Controls.ShutdownMode.OnMainWindowClose;
                        desktop.Shutdown();
                    }
                    else
                    {
                        desktop.MainWindow.WindowState = State ?? Avalonia.Controls.WindowState.Normal;
                    }
                }
            });
        }

        public async Task StartGameCommand()
        {
            LauncherSettingsProvider.Instance.AllowSettings = false;

            AccountStatus status = await AccountManager.LoginAsync(AccountManager.SelectedAccount.username, AccountManager.SelectedAccount.password);

            LauncherSettingsProvider.Instance.AllowSettings = true;

            switch (status)
            {
                case AccountStatus.NoConnection:
                    NavigateTo(new ConnectServerViewModel(HostScreen));
                    return;
            }

            LauncherSettingsProvider.Instance.GameRunning = true;

            GameStarterResult gameStartResult = await gameStarter.LaunchGame(ServerManager.SelectedServer, AccountManager.SelectedAccount);

            if (gameStartResult.Succeeded)
            {
                monitor.Start();

                switch (LauncherSettingsProvider.Instance.LauncherStartGameAction)
                {
                    case LauncherAction.MinimizeAction:
                        {
                            ChangeWindowState(Avalonia.Controls.WindowState.Minimized);
                            break;
                        }
                    case LauncherAction.ExitAction:
                        {
                            ChangeWindowState(null, true);
                            break;
                        }
                }
            }
            else
            {
                SendNotification("", gameStartResult.Message, Avalonia.Controls.Notifications.NotificationType.Error);
                LauncherSettingsProvider.Instance.GameRunning = false;
            }
        }

        public async Task ChangeEditionCommand()
        {
            var result = await ShowDialog(new ChangeEditionDialogViewModel(null));

            if(result != null && result is string edition)
            {
                AccountStatus status = await AccountManager.WipeAsync(edition);

                switch (status)
                {
                    case AccountStatus.OK:
                        {
                            CurrentEdition = AccountManager.SelectedAccount.edition;
                            SendNotification("", LocalizationProvider.Instance.account_updated);
                            break;
                        }
                    case AccountStatus.NoConnection:
                        {
                            NavigateTo(new ConnectServerViewModel(HostScreen));
                            break;
                        }
                    default:
                        {
                            SendNotification("", LocalizationProvider.Instance.edit_account_update_error);
                            break;
                        }
                }
            }
        }

        public async Task CopyCommand(object parameter)
        {
            if(Application.Current.Clipboard != null && parameter != null && parameter is string text)
            {
                await Application.Current.Clipboard.SetTextAsync(text);
                SendNotification("", $"{text} {LocalizationProvider.Instance.copied}", Avalonia.Controls.Notifications.NotificationType.Success);
            }
        }

        private void UpdateProfileInfo()
        {
            AccountManager.UpdateProfileInfo();
            ImageRequest.CacheSideImage(AccountManager.SelectedProfileInfo.Side);
            ProfileInfo.UpdateDisplayedProfile(AccountManager.SelectedProfileInfo);
            if (ProfileInfo.SideImage != SideImage.Path)
            {
                SideImage.Path = ProfileInfo.SideImage;
                SideImage.Touch();
            }
        }


        //pull profile every x seconds
        private int aliveCallBackCountdown = 60;
        private void GameAliveCallBack(ProcessMonitor monitor)
        {
            aliveCallBackCountdown--;

            if (aliveCallBackCountdown <= 0)
            {
                aliveCallBackCountdown = 60;
                UpdateProfileInfo();
            }
        }

        private void GameExitCallback(ProcessMonitor monitor)
        {
            monitor.Stop();

            LauncherSettingsProvider.Instance.GameRunning = false;

            //Make sure the call to MainWindow happens on the UI thread.
            switch (LauncherSettingsProvider.Instance.LauncherStartGameAction)
            {
                case LauncherAction.MinimizeAction:
                    {
                        ChangeWindowState(Avalonia.Controls.WindowState.Normal);

                        break;
                    }
            }

            UpdateProfileInfo();
        }
    }
}
