using Aki.Launcher.MiniCommon;
using Aki.Launcher.Models;
using Aki.Launcher.Models.Launcher;
using ReactiveUI;
using Splat;
using System.Threading.Tasks;

namespace Aki.Launcher.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        public LoginModel Login { get; set; } = new LoginModel();

        public LoginViewModel(IScreen Host) : base(Host)
        {
            var backgroundImage = Locator.Current.GetService<ImageHelper>();

            ImageRequest.CacheBackgroundImage();

            backgroundImage.Touch();
        }

        public async Task LoginCommand()
        {
            await AccountManager.LoginAsync(Login);
        }

        public void GoToRegisterCommand()
        {
            NavigateTo(new RegisterViewModel(HostScreen));
        }
        public void GoToSettingsCommand()
        {
            NavigateTo(new SettingsViewModel(HostScreen));
        }
    }
}
