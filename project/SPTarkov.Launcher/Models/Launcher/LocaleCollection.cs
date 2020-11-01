/* LocaleCollection.cs
 * License: NCSA Open Source License
 * 
 * Copyright: Merijn Hendriks
 * AUTHORS:
 * waffle.lord
 */


using SPTarkov.Launcher.Helpers;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace SPTarkov.Launcher.Models.Launcher
{
    public class LocaleCollection : INotifyPropertyChanged
    {
        private string _SelectedLocale;
        public string SelectedLocale
        {
            get => _SelectedLocale;
            set
            {
                if(_SelectedLocale != value)
                {
                    _SelectedLocale = value;
                    RaisePropertyChanged(nameof(SelectedLocale));
                    LocalizationProvider.LoadLocaleFromFile(value);
                }
            }
        }

        public ObservableCollection<string> AvailableLocales { get; set; } = LocalizationProvider.GetAvailableLocales();

        public event PropertyChangedEventHandler PropertyChanged;

        public LocaleCollection()
        {
            SelectedLocale = LauncherSettingsProvider.Instance.DefaultLocale;
        }

        protected virtual void RaisePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
