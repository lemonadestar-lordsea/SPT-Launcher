/* ServerSetting.cs
 * License: NCSA Open Source License
 * 
 * Copyright: Merijn Hendriks
 * AUTHORS:
 * waffle.lord
 * Merijn Hendriks
 */


using System.ComponentModel;

namespace SPTarkov.Launcher.Models.Launcher
{
    public class ServerSetting : INotifyPropertyChanged
    {
        public LoginModel AutoLoginCreds { get; set; } = null;

        private string _Name;
        public string Name
        {
            get => _Name;
            set
            {
                if (_Name != value)
                {
                    _Name = value;
                    RaisePropertyChanged(nameof(Name));
                }
            }
        }

        private string _Url;
        public string Url
        {
            get => _Url;
            set
            {
                if(_Url != value)
                {
                    _Url = value;
                    RaisePropertyChanged(nameof(Url));
                }
            }
        }

        private bool _IsDefault;
        public bool IsDefault
        {
            get => _IsDefault;
            set
            {
                if(_IsDefault != value)
                {
                    _IsDefault = value;
                    RaisePropertyChanged(nameof(IsDefault));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
