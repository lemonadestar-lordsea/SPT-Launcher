/* LoginViewModel.cs
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
    public class LoginViewModel
    {
        public LoginModel login { get; set; }
        public ServerSetting DefaultServer { get; set; }
        public GenericICommand ShowRegisterCommand { get; set; }
        public AwaitableDelegateCommand LoginCommand { get; set; }
        private NavigationViewModel navigationViewModel { get; set; }

        public LoginViewModel(NavigationViewModel viewModel)
        {
            navigationViewModel = viewModel;

            ShowRegisterCommand = new GenericICommand(OnShowRegisterCommand);
            LoginCommand = new AwaitableDelegateCommand(OnLoginCommand);

            DefaultServer = LauncherSettingsProvider.GetDefaultServer();

            LoginModel tmpLogin = new LoginModel();

            if (DefaultServer.AutoLoginCreds != null)
            {
                tmpLogin.Email = DefaultServer.AutoLoginCreds.Email ?? "";
                tmpLogin.Password = DefaultServer.AutoLoginCreds.Password ?? "";
            }

            login = tmpLogin;
        }

        public void OnShowRegisterCommand(object parameter)
        {
            navigationViewModel.SelectedViewModel = new RegisterViewModel(navigationViewModel);
        }

        public async Task OnLoginCommand()
        {
            LauncherSettingsProvider.Instance.AllowSettings = false;

            int status = await AccountManager.LoginAsync(login);

            LauncherSettingsProvider.Instance.AllowSettings = true;


            switch (status)
            {
                case 1:
                    if (LauncherSettingsProvider.Instance.UseAutoLogin && DefaultServer.AutoLoginCreds != login)
                    {
                        DefaultServer.AutoLoginCreds = login;
                    }

                    LauncherSettingsProvider.Instance.SaveSettings();

                    navigationViewModel.SelectedViewModel = new ProfileViewModel(navigationViewModel);
                    break;

                case -1:
                    navigationViewModel.NotificationQueue.Enqueue(LocalizationProvider.Instance.incorrect_login);
                    return;

                case -2:
                    navigationViewModel.NotificationQueue.Enqueue(LocalizationProvider.Instance.login_failed);
                    navigationViewModel.SelectedViewModel = new ConnectServerViewModel(navigationViewModel);
                    return;
            }
        }
    }
}
