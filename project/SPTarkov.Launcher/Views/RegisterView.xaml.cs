/* RegisterView.xaml.cs
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
    /// Interaction logic for RegisterView.xaml
    /// </summary>
    public partial class RegisterView : UserControl, IHavePassword
    {
        public RegisterView()
        {
            InitializeComponent();
        }

        public string Password => passBox.Password;
    }
}
