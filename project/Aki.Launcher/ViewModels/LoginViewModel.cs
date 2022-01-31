using Aki.Launcher.Attributes;
using Aki.Launcher.Helpers;
using Aki.Launcher.MiniCommon;
using Aki.Launcher.Models;
using Aki.Launcher.Models.Launcher;
using ReactiveUI;
using Splat;
using System.Reactive;
using System.Threading.Tasks;

namespace Aki.Launcher.ViewModels
{
    [RequireServerConnected]
    public class LoginViewModel : ViewModelBase
    {
        public LoginModel Login { get; set; } = new LoginModel();

        public ReactiveCommand<Unit, Unit> LoginCommand { get; set; }

        public LoginViewModel(IScreen Host) : base(Host)
        {
            //setup reactive commands
            LoginCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                AccountStatus status = await AccountManager.LoginAsync(Login);

                switch (status)
                {
                    case AccountStatus.OK:
                        {
                            //TODO - navigate to profile viewmodel
                            break;
                        }
                    case AccountStatus.LoginFailed:
                        {
                            NotificationManager.Show(new Avalonia.Controls.Notifications.Notification("", "Login failed", Avalonia.Controls.Notifications.NotificationType.Error));
                            //TODO - profile doesn't exist, ask to add it.
                            break;
                        }
                    case AccountStatus.NoConnection:
                        {
                            NavigateTo(new ConnectServerViewModel(HostScreen));
                            break;
                        }
                }
            });

            //cache and touch background image
            var backgroundImage = Locator.Current.GetService<ImageHelper>("bgimage");

            ImageRequest.CacheBackgroundImage();

            backgroundImage.Touch();

            //handle auto-login
            if(LauncherSettingsProvider.Instance.UseAutoLogin && LauncherSettingsProvider.Instance.Server.AutoLoginCreds != null)
            {
                Task.Run(() =>
                {
                    Login = LauncherSettingsProvider.Instance.Server.AutoLoginCreds;
                    LoginCommand.Execute();
                });
            }
        }

        public void GoToSettingsCommand()
        {
            NavigateTo(new SettingsViewModel(HostScreen));
        }
    }
}
