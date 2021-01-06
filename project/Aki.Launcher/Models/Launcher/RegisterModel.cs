/* RegisterModel.cs
 * License: NCSA Open Source License
 * 
 * Copyright: Merijn Hendriks
 * AUTHORS:
 * Merijn Hendriks
 */


using System.ComponentModel;

namespace Aki.Launcher.Models.Launcher
{
    public class RegisterModel : INotifyPropertyChanged
    {
        private string _Email;
        public string Username
        {
            get => _Email;
            set
            {
                if(_Email != value)
                {
                    _Email = value;
                    RaisePropertyChanged(nameof(Username));
                }
            }
        }

        private string _Password;
        public string Password
        {
            get => _Password;
            set
            {
                if(_Password != value)
                {
                    _Password = value;
                    RaisePropertyChanged(nameof(Password));
                }
            }
        }

        public EditionCollection EditionsCollection { get; set; } = new EditionCollection();

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
