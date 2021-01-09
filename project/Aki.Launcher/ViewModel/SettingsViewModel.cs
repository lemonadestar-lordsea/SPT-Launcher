/* SettingsViewModel.cs
 * License: NCSA Open Source License
 * 
 * Copyright: Merijn Hendriks
 * AUTHORS:
 * waffle.lord
 * Merijn Hendriks
 */


using Aki.Launcher.Generics;
using Aki.Launcher.Helpers;
using Aki.Launcher.Models.Launcher;
using System;
using System.IO;
using System.Windows;
using WinForms = System.Windows.Forms;

namespace Aki.Launcher.ViewModel
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
        public GenericICommand ReApplyPatchCommand { get; set; }
        public LocaleCollection Locales { get; set; } = new LocaleCollection();
        private NavigationViewModel navigationViewModel { get; set; }

        private GameStarter gameStarter = new GameStarter();
        public SettingsViewModel(NavigationViewModel viewModel)
        {
            navigationViewModel = viewModel;

            #region Settings Commands
            //BackCommand = new GenericICommand(OnBackCommand);
            CleanTempFilesCommand = new GenericICommand(OnCleanTempFilesCommand);
            SelectGameFolderCommand = new GenericICommand(OnSelectGameFolderCommand);
            RemoveRegistryKeysCommand = new GenericICommand(OnRemoveRegistryKeysCommand);
            ClearGameSettingsCommand = new GenericICommand(OnClearGameSettingsCommand);
            ReApplyPatchCommand = new GenericICommand(OnReApplyPatchCommand);
            #endregion

            Application.Current.MainWindow.Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (LauncherSettingsProvider.Instance.IsEditingSettings)
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

                if (result == WinForms.DialogResult.OK && !String.IsNullOrEmpty(dialog.SelectedPath))
                {
                    return dialog.SelectedPath;
                }
            }

            return null;
        }
        #endregion

        #region Settings Commands

        public void OnReApplyPatchCommand(object parameter)
        {
            navigationViewModel.NotificationQueue.Enqueue("This button doesn't do anything yet.  :(");
        }

        public void OnCleanTempFilesCommand(object parameter)
        {
            bool filesCleared = gameStarter.CleanTempFiles();

            if (filesCleared)
            {
                navigationViewModel.NotificationQueue.Enqueue(LocalizationProvider.Instance.clean_temp_files_succeeded, true);
            }
            else
            {
                navigationViewModel.NotificationQueue.Enqueue(LocalizationProvider.Instance.clean_temp_files_failed, true);
            }
        }

        public void OnRemoveRegistryKeysCommand(object parameter)
        {
            bool regKeysRemoved = gameStarter.RemoveRegisteryKeys();

            if (regKeysRemoved)
            {
                navigationViewModel.NotificationQueue.Enqueue(LocalizationProvider.Instance.remove_registry_keys_succeeded, true);
            }
            else
            {
                navigationViewModel.NotificationQueue.Enqueue(LocalizationProvider.Instance.remove_registry_keys_failed, true);
            }
        }

        public void OnClearGameSettingsCommand(object parameter)
        {
            string EFTSettingsFolder = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\\Escape from Tarkov";

            if (Directory.Exists(EFTSettingsFolder))
            {
                Directory.Delete(EFTSettingsFolder, true);

                if (Directory.Exists(EFTSettingsFolder))
                {
                    navigationViewModel.NotificationQueue.Enqueue(LocalizationProvider.Instance.clear_game_settings_failed, true);
                    return;
                }
            }

            navigationViewModel.NotificationQueue.Enqueue(LocalizationProvider.Instance.clear_game_settings_succeeded, true);
        }

        public void OnSelectGameFolderCommand(object parameter)
        {
            string path = GetFolderPath();

            if (!String.IsNullOrEmpty(path))
            {
                LauncherSettingsProvider.Instance.GamePath = path;
            }
        }
        #endregion
    }
}
