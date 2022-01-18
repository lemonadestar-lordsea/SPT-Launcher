using Aki.Launcher.Helpers;
using Aki.Launcher.Models.Launcher;
using ReactiveUI;

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

        }
    }
}
