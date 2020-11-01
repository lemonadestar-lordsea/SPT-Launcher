/* SettingsViewModel.cs
 * License: NCSA Open Source License
 * 
 * Copyright: Merijn Hendriks
 * AUTHORS:
 * waffle.lord
 * Merijn Hendriks
 */


using SPTarkov.Launcher.Generics;
using SPTarkov.Launcher.Helpers;
using SPTarkov.Launcher.Models.Launcher;
using System;
using System.IO;
using System.Windows;
using WinForms = System.Windows.Forms;

namespace SPTarkov.Launcher.ViewModel
{
    public class SettingsViewModel
    {
        public GenericICommand BackCommand { get; set; }
        public GenericICommand RemoveServerCommand { get; set; }
        public GenericICommand SetServerAsDefaultCommand { get; set; }
        public GenericICommand AddServerCommand { get; set; }
        public GenericICommand SaveNewServerCommand { get; set; }
        public GenericICommand ShowServerListCommand { get; set; }
        public GenericICommand CleanTempFilesCommand { get; set; }
        public GenericICommand SelectGameFolderCommand { get; set; }
        public GenericICommand RemoveRegistryKeysCommand { get; set; }
        public GenericICommand ClearGameSettingsCommand { get; set; }
        public ServerSetting NewServer { get; set; }
        public LocaleCollection Locales { get; set; } = new LocaleCollection();
        private NavigationViewModel fullSpanNavigationViewModel { get; set; }
        private NavigationViewModel mainNavigationViewModel { get; set; }

        private GameStarter gameStarter = new GameStarter();
        public SettingsViewModel(NavigationViewModel fullSpanViewModel, NavigationViewModel mainViewModel)
        {
            fullSpanNavigationViewModel = fullSpanViewModel;
            mainNavigationViewModel = mainViewModel;

            #region Settings Commands
            BackCommand = new GenericICommand(OnBackCommand);
            CleanTempFilesCommand = new GenericICommand(OnCleanTempFilesCommand);
            SelectGameFolderCommand = new GenericICommand(OnSelectGameFolderCommand);
            RemoveRegistryKeysCommand = new GenericICommand(OnRemoveRegistryKeysCommand);
            ClearGameSettingsCommand = new GenericICommand(OnClearGameSettingsCommand);
            #endregion

            #region Server List Commands
            RemoveServerCommand = new GenericICommand(OnRemoveServerCommand);
            AddServerCommand = new GenericICommand(OnAddServerCommand);
            SetServerAsDefaultCommand = new GenericICommand(OnSetServerAsDefaultCommand);
            SaveNewServerCommand = new GenericICommand(OnSaveNewServerCommand);
            ShowServerListCommand = new GenericICommand(OnShowServerListCommand);
            #endregion

            ServerSetting tmpSettings = new ServerSetting();

            NewServer = tmpSettings;

            Application.Current.MainWindow.Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(LauncherSettingsProvider.Instance.IsEditingSettings)
            {
                LauncherSettingsProvider.Instance.SaveSettings();
            }
        }

        #region General Use Methods
        /// <summary>
        /// Get a folder using a folder browse dialog
        /// </summary>
        /// <returns>returns the path to the selected folder or null</returns>
        private string GetFolderPath()
        {
            using (WinForms.FolderBrowserDialog dialog = new WinForms.FolderBrowserDialog())
            {
                WinForms.DialogResult result = dialog.ShowDialog();

                if(result == WinForms.DialogResult.OK && !String.IsNullOrEmpty(dialog.SelectedPath))
                {
                    return dialog.SelectedPath;
                }
            }

            return null;
        }
        #endregion

        #region Settings Commands
        public void OnBackCommand(object parameter)
        {
            fullSpanNavigationViewModel.NotificationQueue.CloseQueue();

            LauncherSettingsProvider.Instance.SaveSettings();

            LauncherSettingsProvider.Instance.IsEditingSettings = false;

            mainNavigationViewModel.SelectedViewModel = new ConnectServerViewModel(mainNavigationViewModel);
            fullSpanNavigationViewModel.SelectedViewModel = null;
        }

        public void OnCleanTempFilesCommand(object parameter)
        {
            bool filesCleared = gameStarter.CleanTempFiles();

            if(filesCleared)
            {
                fullSpanNavigationViewModel.NotificationQueue.Enqueue(LocalizationProvider.Instance.clean_temp_files_succeeded, true);
            }
            else
            {
                fullSpanNavigationViewModel.NotificationQueue.Enqueue(LocalizationProvider.Instance.clean_temp_files_failed, true);
            }
        }

        public void OnRemoveRegistryKeysCommand(object parameter)
        {
            bool regKeysRemoved = gameStarter.RemoveRegisteryKeys();

            if(regKeysRemoved)
            {
                fullSpanNavigationViewModel.NotificationQueue.Enqueue(LocalizationProvider.Instance.remove_registry_keys_succeeded, true);
            }
            else
            {
                fullSpanNavigationViewModel.NotificationQueue.Enqueue(LocalizationProvider.Instance.remove_registry_keys_failed, true);
            }
        }

        public void OnClearGameSettingsCommand(object parameter)
        {
            string EFTSettingsFolder = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\\Escape from Tarkov";

            if(Directory.Exists(EFTSettingsFolder))
            {
                Directory.Delete(EFTSettingsFolder, true);

                if(Directory.Exists(EFTSettingsFolder))
                {
                    fullSpanNavigationViewModel.NotificationQueue.Enqueue(LocalizationProvider.Instance.clear_game_settings_failed, true);
                    return;
                }
            }

            fullSpanNavigationViewModel.NotificationQueue.Enqueue(LocalizationProvider.Instance.clear_game_settings_succeeded, true);
        }

        public void OnSelectGameFolderCommand(object parameter)
        {
            string path = GetFolderPath();

            if(!String.IsNullOrEmpty(path))
            {
                LauncherSettingsProvider.Instance.GamePath = path;
            }
        }
        #endregion

        #region Server List Commands
        public void OnAddServerCommand(object parameter)
        {
            NewServer.Name = "";
            NewServer.Url = "";
            NewServer.IsDefault = false;

            LauncherSettingsProvider.Instance.IsAddingServer = true;
        }

        public void OnRemoveServerCommand(object parameter)
        {
            if(parameter is ServerSetting setting)
            {
                if (setting.Url == "https://127.0.0.1")
                {
                    fullSpanNavigationViewModel.NotificationQueue.Enqueue($"{LocalizationProvider.Instance.local_server_remove_warning}\n\n{LocalizationProvider.Instance.remove_server_question}",
                                                                          $"{LocalizationProvider.Instance.remove_server_tooltip}", () => 
                    {
                        LauncherSettingsProvider.Instance.RemoveServerAndSave(setting);
                    }, true);

                    return;
                }

                LauncherSettingsProvider.Instance.RemoveServerAndSave(setting);
            }
        }

        public void OnSetServerAsDefaultCommand(object parameter)
        {
            if (parameter is ServerSetting setting)
            {
                LauncherSettingsProvider.Instance.SetDefaultServerAndSave(setting);
            }
        }

        public void OnSaveNewServerCommand(object parameter)
        {
            ServerAddStatus addStatus = LauncherSettingsProvider.Instance.AddServerAndSave(NewServer);

            if(addStatus.AddSucceeded)
            {
                LauncherSettingsProvider.Instance.IsAddingServer = false;
            }
            else
            {
                fullSpanNavigationViewModel.NotificationQueue.Enqueue(addStatus.Message);
            }
        }

        public void OnShowServerListCommand(object parameter)
        {
            LauncherSettingsProvider.Instance.IsAddingServer = false;
        }
        #endregion
    }
}
