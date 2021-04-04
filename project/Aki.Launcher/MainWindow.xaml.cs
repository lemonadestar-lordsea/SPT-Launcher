/* MainWindow.xaml.cs
 * License: NCSA Open Source License
 * 
 * Copyright: Merijn Hendriks
 * AUTHORS:
 * waffle.lord
 * Merijn Hendriks
 */

using Aki.Launcher.Custom_Controls;
using Aki.Launcher.Generics;
using Aki.Launcher.Helpers;
using Aki.Launcher.Models.Launcher;
using Aki.Launcher.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Aki.Launcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public GenericICommand MinimizeAppCommand { get; set; }
        public GenericICommand CloseAppCommand { get; set; }
        public GenericICommand MenuItemCommand { get; set; }
        public NavigationViewModel navigationViewModel { get; set; }
        public ObservableCollection<MenuBarItem> MenuItemCollection { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            DialogHost.DefaultHost = "mainDialogHost";

            if (LauncherSettingsProvider.Instance.FirstRun)
            {
                LauncherSettingsProvider.Instance.FirstRun = false;
                LauncherSettingsProvider.Instance.SaveSettings();
                LocalizationProvider.TryAutoSetLocale();
            }

            var viewmodel = new NavigationViewModel();
            navigationViewModel = viewmodel;

            MinimizeAppCommand = new GenericICommand(OnMinimizeAppCommand);
            CloseAppCommand = new GenericICommand(OnCloseAppCommand);
            MenuItemCommand = new GenericICommand(OnMenuItemCommand);

            ObservableCollection<MenuBarItem> tempMenuItemCollection = new ObservableCollection<MenuBarItem>();

            tempMenuItemCollection.Add(new MenuBarItem
            {
                Name = LocalizationProvider.Instance.game,
                ItemAction = () =>
                {
                    navigationViewModel.SelectedViewModel = new ConnectServerViewModel(navigationViewModel);
                }
            });

            tempMenuItemCollection.Add(new MenuBarItem
            {
                Name = LocalizationProvider.Instance.account,
                ItemAction = () =>
                {
                    navigationViewModel.SelectedViewModel = new EditProfileViewModel(navigationViewModel);
                },
                CanUseAction = () => !LauncherSettingsProvider.Instance.GameRunning && AccountManager.SelectedAccount != null,
                OnFailedToUseAction = () =>
                {
                    navigationViewModel.NotificationQueue.Enqueue(LocalizationProvider.Instance.account_page_denied);
                }
            });

            tempMenuItemCollection.Add(new MenuBarItem
            {
                Name = LocalizationProvider.Instance.settings_menu,
                ItemAction = () =>
                {
                    navigationViewModel.SelectedViewModel = new SettingsViewModel(navigationViewModel);
                }
            });

            MenuItemCollection = tempMenuItemCollection;

            OnMenuItemCommand(MenuItemCollection[0]);
        }

        public void OnMinimizeAppCommand(object parameter)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        public void OnCloseAppCommand(object parameter)
        {
            Environment.Exit(0);
        }

        public void OnMenuItemCommand(object parameter)
        {
            if (parameter is MenuBarItem menuItem)
            {
                if (menuItem.IsSelected)
                {
                    return;
                }

                if(!menuItem.CanUseAction.Invoke())
                {
                    menuItem.OnFailedToUseAction?.Invoke();

                    return;
                }

                foreach(MenuBarItem m in MenuItemCollection)
                {
                    m.IsSelected = false;
                }

                menuItem.IsSelected = true;
                menuItem.ItemAction?.Invoke();
            }
        }

        private void WindowMove_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }
    }
}
