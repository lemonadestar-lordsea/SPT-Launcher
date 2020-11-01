/* WipeProfileViewModel.cs
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
    public class WipeProfileViewModel
    {
        public AwaitableDelegateCommand WipeCommand { get; set; }
        public GenericICommand BackCommand { get; set; }
        public WipeProfileModel ProfileWipe { get; set; }
        private NavigationViewModel navigationViewModel { get; set; }

        public WipeProfileViewModel(NavigationViewModel viewModel)
        {
            navigationViewModel = viewModel;
            WipeCommand = new AwaitableDelegateCommand(OnWipeCommand);
            BackCommand = new GenericICommand(OnBackCommand);

            WipeProfileModel tmpWipeProfile = new WipeProfileModel();

            ProfileWipe = tmpWipeProfile;

            //LauncherSettingsProvider.Instance.AllowSettings = true;
        }

        public void OnBackCommand(object parameter)
        {
            navigationViewModel.SelectedViewModel = new EditProfileViewModel(navigationViewModel);
        }

        public async Task OnWipeCommand()
        {
            LauncherSettingsProvider.Instance.AllowSettings = false;

            int status = await AccountManager.WipeAsync(ProfileWipe.EditionsCollection.SelectedEdition);

            LauncherSettingsProvider.Instance.AllowSettings = true;


            switch (status)
            {
                case 1:
                    navigationViewModel.SelectedViewModel = new ProfileViewModel(navigationViewModel);
                    break;

                case -1:
                    navigationViewModel.NotificationQueue.Enqueue(LocalizationProvider.Instance.login_failed);
                    navigationViewModel.SelectedViewModel = new ConnectServerViewModel(navigationViewModel);
                    return;

                case -2:
                    navigationViewModel.NotificationQueue.Enqueue(LocalizationProvider.Instance.edit_profile_update_error);
                    navigationViewModel.SelectedViewModel = new ConnectServerViewModel(navigationViewModel);
                    return;
            }
        }
    }
}
