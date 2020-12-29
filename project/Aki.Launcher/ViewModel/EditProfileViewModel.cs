/* EditProfileViewModel.cs
 * License: NCSA Open Source License
 * 
 * Copyright: Merijn Hendriks
 * AUTHORS:
 * waffle.lord
 * Merijn Hendriks
 */


using Aki.Launcher.Generics;
using Aki.Launcher.Generics.AsyncCommand;
using Aki.Launcher.Helpers;
using Aki.Launcher.Models.Launcher;
using Aki.Launcher.Interfaces;
using System.Threading.Tasks;

namespace Aki.Launcher.ViewModel
{
    public class EditProfileViewModel
    {
        public LoginModel login { get; set; }

        public AwaitableDelegateCommand UpdateCommand { get; set; }
        public GenericICommand BackCommand { get; set; }
        public GenericICommand WipeProfileCommand { get; set; }

        private NavigationViewModel navigationViewModel { get; set; }
        public EditProfileViewModel(NavigationViewModel viewModel)
        {
            navigationViewModel = viewModel;

            UpdateCommand = new AwaitableDelegateCommand(OnUpdateCommand);
            BackCommand = new GenericICommand(OnBackCommand);
            WipeProfileCommand = new GenericICommand(OnWipeProfileCommand);

            ServerSetting DefaultServer = LauncherSettingsProvider.GetDefaultServer();

            LoginModel tmpLogin = new LoginModel();
            tmpLogin.Email = DefaultServer.AutoLoginCreds.Email;
            tmpLogin.Password = DefaultServer.AutoLoginCreds.Password;

            login = tmpLogin;
        }

        private string GetStatus(int status)
        {
            switch(status)
            {
                case 1:
                    return "OK";

                case -1:
                    
                    return "Login failed";

                case -2:
                    return "CONNECTION_ERROR";
            }

            return "Undefined Response";   
        }

        public async Task OnUpdateCommand(object parameter)
        {
            if(parameter is IHavePassword pass)
            {
                if (!string.IsNullOrWhiteSpace(pass.Password))
                {
                    login.Password = pass.Password;
                }
            }
            else
            {
                navigationViewModel.NotificationQueue.Enqueue(LocalizationProvider.Instance.edit_profile_update_error);
                navigationViewModel.SelectedViewModel = new ProfileViewModel(navigationViewModel);
            }

            LauncherSettingsProvider.Instance.AllowSettings = false;

            string emailStatus = GetStatus(await AccountManager.ChangeEmailAsync(login.Email));
            string passStatus = GetStatus(await AccountManager.ChangePasswordAsync(login.Password));

            LauncherSettingsProvider.Instance.AllowSettings = true;

            if (emailStatus == "OK" && passStatus == "OK")
            {
                navigationViewModel.SelectedViewModel = new ProfileViewModel(navigationViewModel);
            }
            else
            {
                navigationViewModel.NotificationQueue.Enqueue(LocalizationProvider.Instance.edit_profile_update_error);
                navigationViewModel.SelectedViewModel = new ConnectServerViewModel(navigationViewModel);
            }
        }

        public void OnBackCommand(object parameter)
        {
            navigationViewModel.SelectedViewModel = new ProfileViewModel(navigationViewModel);
        }

        public void OnWipeProfileCommand(object parameter)
        {
            navigationViewModel.SelectedViewModel = new WipeProfileViewModel(navigationViewModel);
        }
    }
}
