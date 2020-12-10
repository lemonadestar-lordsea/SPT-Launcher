/* LoginView.xaml.cs
 * License: NCSA Open Source License
 * 
 * Copyright: Merijn Hendriks
 * AUTHORS:
 * Merijn Hendriks
 */


using SPTarkov.Launcher.Interfaces;
using System.Windows.Controls;

namespace SPTarkov.Launcher.Views
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : UserControl, IHavePassword
    {
        public LoginView()
        {
            InitializeComponent();
        }

        public string Password => passBox.Password;
    }
}
