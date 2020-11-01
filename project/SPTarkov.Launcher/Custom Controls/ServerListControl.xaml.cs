/* ServerListControl.xaml.cs
 * License: NCSA Open Source License
 * 
 * Copyright: Merijn Hendriks
 * AUTHORS:
 * Merijn Hendriks
 */


using SPTarkov.Launcher.Models.Launcher;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SPTarkov.Launcher.Custom_Controls
{
    /// <summary>
    /// Interaction logic for ServerSettingsControl.xaml
    /// </summary>
    public partial class ServerListControl : UserControl
    {
        public ServerListControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty SourceListProperty =
            DependencyProperty.Register("SourceList", typeof(ObservableCollection<ServerSetting>), typeof(ServerListControl), new PropertyMetadata(null));
        public ObservableCollection<ServerSetting> SourceList
        {
            get => (ObservableCollection<ServerSetting>)GetValue(SourceListProperty);
            set => SetValue(SourceListProperty, value);
        }

        public static readonly DependencyProperty AddCommandProperty =
            DependencyProperty.Register("AddCommand", typeof(ICommand), typeof(ServerListControl), new PropertyMetadata(null));
        public ICommand AddCommand
        {
            get => (ICommand)GetValue(AddCommandProperty);
            set => SetValue(AddCommandProperty, value);
        }

        public static readonly DependencyProperty RemoveCommandProperty =
            DependencyProperty.Register("RemoveCommand", typeof(ICommand), typeof(ServerListControl), new PropertyMetadata(null));
        public ICommand RemoveCommand
        {
            get => (ICommand)GetValue(RemoveCommandProperty);
            set => SetValue(RemoveCommandProperty, value);
        }

        public static readonly DependencyProperty SetDefaultCommandProperty =
            DependencyProperty.Register("SetDefaultCommand", typeof(ICommand), typeof(ServerListControl), new PropertyMetadata(null));
        public ICommand SetDefaultCommand
        {
            get => (ICommand)GetValue(SetDefaultCommandProperty);
            set => SetValue(SetDefaultCommandProperty, value);
        }
    }
}
