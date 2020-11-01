/* AddServerControl.xaml.cs
 * License: NCSA Open Source License
 * 
 * Copyright: Merijn Hendriks
 * AUTHORS:
 * waffle.lord
 * Merijn Hendriks
 */


using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SPTarkov.Launcher.Custom_Controls
{
    /// <summary>
    /// Interaction logic for AddServerControl.xaml
    /// </summary>
    public partial class AddServerControl : UserControl
    {
        public AddServerControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty NewServerNameProperty =
            DependencyProperty.Register("NewServerName", typeof(string), typeof(AddServerControl), new PropertyMetadata(string.Empty));
        public string NewServerName
        {
            get => (string)GetValue(NewServerNameProperty);
            set => SetValue(NewServerNameProperty, value);
        }

        public static readonly DependencyProperty NewServerUrlProperty =
            DependencyProperty.Register("NewServerUrl", typeof(string), typeof(AddServerControl), new PropertyMetadata(string.Empty));
        public string NewServerUrl
        {
            get => (string)GetValue(NewServerUrlProperty);
            set => SetValue(NewServerUrlProperty, value);
        }

        public static readonly DependencyProperty IsDefaultProperty =
            DependencyProperty.Register("IsDefault", typeof(bool), typeof(AddServerControl), new PropertyMetadata(false));
        public bool IsDefault
        {
            get => (bool)GetValue(IsDefaultProperty);
            set => SetValue(IsDefaultProperty, value);
        }

        public static readonly DependencyProperty SaveCommandProperty =
            DependencyProperty.Register("SaveCommand", typeof(ICommand), typeof(AddServerControl), new PropertyMetadata(null));
        public ICommand SaveCommand
        {
            get => (ICommand)GetValue(SaveCommandProperty);
            set => SetValue(SaveCommandProperty, value);
        }

        public static readonly DependencyProperty BackCommandProperty =
            DependencyProperty.Register("BackCommand", typeof(ICommand), typeof(AddServerControl), new PropertyMetadata(null));
        public ICommand BackCommand
        {
            get => (ICommand)GetValue(BackCommandProperty);
            set => SetValue(BackCommandProperty, value);
        }

        public static readonly DependencyProperty MaxNameLengthProperty =
            DependencyProperty.Register("MaxNameLength", typeof(int), typeof(AddServerControl), new PropertyMetadata(30));
        public int MaxNameLength
        {
            get => (int)GetValue(MaxNameLengthProperty);
            set => SetValue(MaxNameLengthProperty, value);
        }
    }
}
