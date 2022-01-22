using Aki.Launcher.Models.Launcher;
using ReactiveUI;
using System.Threading.Tasks;

namespace Aki.Launcher.ViewModels
{
    public class RegisterViewModel : ViewModelBase
    {
        public LoginModel Login { get; set; } = new LoginModel();

        public RegisterViewModel(IScreen Host) : base(Host)
        {
        }

        public async Task RegisterCommand()
        {

        }

        public void GoToLoginCommand()
        {
            NavigateTo(new LoginViewModel(HostScreen));
        }

        public void GoToSettingsCommand()
        {
            NavigateTo(new SettingsViewModel(HostScreen));
        }
    }
}
