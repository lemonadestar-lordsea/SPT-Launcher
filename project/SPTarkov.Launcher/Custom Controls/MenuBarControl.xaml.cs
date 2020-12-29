using SPTarkov.Launcher.Models.Launcher;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace SPTarkov.Launcher.Custom_Controls
{
    /// <summary>
    /// Interaction logic for MenuBarControl.xaml
    /// </summary>
    public partial class MenuBarControl : UserControl
    {
        public MenuBarControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty MenuBarItemCollectionProperty =
            DependencyProperty.Register("MenuBarItemCollection", typeof(List<MenuBarItem>), typeof(MenuBarControl), new PropertyMetadata(default));

        public List<MenuBarItem> MenuBarItemCollection
        {
            get => (List<MenuBarItem>)GetValue(MenuBarItemCollectionProperty);
            set => SetValue(MenuBarItemCollectionProperty, value);
        }
    }
}
