/* MainWindow.xaml.cs
 * License: NCSA Open Source License
 * 
 * Copyright: Merijn Hendriks
 * AUTHORS:
 * waffle.lord
 * Merijn Hendriks
 */

using Aki.Launcher.Generics;
using Aki.Launcher.Helpers;
using Aki.Launcher.ViewModel;
using System;
using System.Windows;
using System.Windows.Input;

namespace Aki.Launcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public GenericICommand ShowSettingsCommand { get; set; }
        public GenericICommand CloseAppCommand { get; set; }

        public NavigationViewModel FullSpanNavigationViewModel { get; set; }
        public NavigationViewModel navigationViewModel { get; set; }
        public MainWindow()
        {
            InitializeComponent();

            if (LauncherSettingsProvider.Instance.FirstRun)
            {
                LauncherSettingsProvider.Instance.FirstRun = false;
                LauncherSettingsProvider.Instance.SaveSettings();
                LocalizationProvider.TryAutoSetLocale();
            }

            var viewmodel = new NavigationViewModel();
            viewmodel.SelectedViewModel = new ConnectServerViewModel(viewmodel);
            navigationViewModel = viewmodel;


            var fullSpanViewModel = new NavigationViewModel();
            FullSpanNavigationViewModel = fullSpanViewModel;

            ShowSettingsCommand = new GenericICommand(OnShowSettingsCommand);
            CloseAppCommand = new GenericICommand(OnCloseAppCommand);
        }

        public void OnCloseAppCommand(object parameter)
        {
            Environment.Exit(0);
        }

        public void OnShowSettingsCommand(object parameter)
        {
            if (LauncherSettingsProvider.Instance.IsEditingSettings)
            {
                return;
            }

            navigationViewModel.SelectedViewModel = null;

            navigationViewModel.NotificationQueue.CloseQueue();

            LauncherSettingsProvider.Instance.IsEditingSettings = true;
            FullSpanNavigationViewModel.SelectedViewModel = new SettingsViewModel(FullSpanNavigationViewModel, navigationViewModel);
        }

        private void StackPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }
    }
}
