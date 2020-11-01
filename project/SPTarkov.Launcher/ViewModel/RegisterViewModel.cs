/* RegisterViewModel.cs
 * License: NCSA Open Source License
 * 
 * Copyright: Merijn Hendriks
 * AUTHORS:
 * waffle.lord
 * Merijn Hendriks
 */


using SPTarkov.Launcher.Generics;
using SPTarkov.Launcher.Generics.AsyncCommand;
using SPTarkov.Launcher.Helpers;
using SPTarkov.Launcher.Models.Launcher;
using System.Threading.Tasks;

namespace SPTarkov.Launcher.ViewModel
{
    public class RegisterViewModel
    {
        public RegisterModel newProfile { get; set; }
        public GenericICommand ShowLoginCommand { get; set; }
        public AwaitableDelegateCommand RegisterCommand { get; set; }
        private NavigationViewModel navigationViewModel { get; set; }

        public RegisterViewModel(NavigationViewModel viewModel)
        {
            navigationViewModel = viewModel;

            ShowLoginCommand = new GenericICommand(OnShowLoginCommand);
            RegisterCommand = new AwaitableDelegateCommand(OnRegisterCommand);

            RegisterModel tmpProfile = new RegisterModel();

            newProfile = tmpProfile;
        }

        public void OnShowLoginCommand(object parameter)
        {
            navigationViewModel.SelectedViewModel = new LoginViewModel(navigationViewModel);
        }

        public async Task OnRegisterCommand()
        {
            LauncherSettingsProvider.Instance.AllowSettings = false;

            int status = await AccountManager.RegisterAsync(newProfile.Email ?? "", newProfile.Password ?? "", newProfile.EditionsCollection.SelectedEdition);

            LauncherSettingsProvider.Instance.AllowSettings = true;


            switch (status)
            {
                case 1:
                    ServerSetting DefaultServer = LauncherSettingsProvider.GetDefaultServer();

                    if (DefaultServer.AutoLoginCreds == null)
                    {
                        DefaultServer.AutoLoginCreds = new LoginModel { Email = newProfile.Email, Password = newProfile.Password };
                        LauncherSettingsProvider.Instance.SaveSettings();
                    }

                    navigationViewModel.SelectedViewModel = new ProfileViewModel(navigationViewModel);
                    break;

                case -1:
                    navigationViewModel.NotificationQueue.Enqueue(LocalizationProvider.Instance.account_exist);
                    return;

                case -2:
                    navigationViewModel.NotificationQueue.Enqueue(LocalizationProvider.Instance.registration_failed);
                    navigationViewModel.SelectedViewModel = new ConnectServerViewModel(navigationViewModel);
                    return;

                case -3:
                    navigationViewModel.NotificationQueue.Enqueue(LocalizationProvider.Instance.incorrect_login);
                    return;
            }
        }
    }
}
