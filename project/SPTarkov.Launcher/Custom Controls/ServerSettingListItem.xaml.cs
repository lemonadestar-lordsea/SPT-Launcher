/* ServerSettingListItem.xaml.cs
 * License: NCSA Open Source License
 * 
 * Copyright: Merijn Hendriks
 * AUTHORS:
 * Merijn Hendriks
 */


using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SPTarkov.Launcher.Custom_Controls
{
    /// <summary>
    /// Interaction logic for ServerSettingListItem.xaml
    /// </summary>
    public partial class ServerSettingListItem : UserControl
    {
        public ServerSettingListItem()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ServerNameProperty =
            DependencyProperty.Register("ServerName", typeof(string), typeof(ServerSettingListItem), new PropertyMetadata(string.Empty));
        public string ServerName
        {
            get => (string)GetValue(ServerNameProperty);
            set => SetValue(ServerNameProperty, value);
        }

        public static readonly DependencyProperty IsDefaultProperty =
            DependencyProperty.Register("IsDefault", typeof(bool), typeof(ServerSettingListItem), new PropertyMetadata(false));
        public bool IsDefault
        {
            get => (bool)GetValue(IsDefaultProperty);
            set => SetValue(IsDefaultProperty, value);
        }

        public static readonly DependencyProperty RemoveCommandProperty =
            DependencyProperty.Register("RemoveCommand", typeof(ICommand), typeof(ServerSettingListItem), new PropertyMetadata(null));
        public ICommand RemoveCommand
        {
            get => (ICommand)GetValue(RemoveCommandProperty);
            set => SetValue(RemoveCommandProperty, value);
        }

        public static readonly DependencyProperty SetDefaultCommandProperty =
            DependencyProperty.Register("SetDefaultCommand", typeof(ICommand), typeof(ServerSettingListItem), new PropertyMetadata(null));
        public ICommand SetDefaultCommand
        {
            get => (ICommand)GetValue(SetDefaultCommandProperty);
            set => SetValue(SetDefaultCommandProperty, value);
        }
    }
}
