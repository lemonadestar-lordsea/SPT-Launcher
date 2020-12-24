/* LauncherSettingsProvider.cs
 * License: NCSA Open Source License
 * 
 * Copyright: Merijn Hendriks
 * AUTHORS:
 * waffle.lord
 * Merijn Hendriks
 */


using Newtonsoft.Json;
using SPTarkov.Launcher.MiniCommon;
using SPTarkov.Launcher.Models.Launcher;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace SPTarkov.Launcher.Helpers
{
    public static class LauncherSettingsProvider
    {
        public static string DefaultSettingsFileLocation = $"{Environment.CurrentDirectory}\\Aki_Data\\Launcher\\launcher.config.json";

        public static ServerSetting GetDefaultServer()
        {
            return Instance.ServerCollection.Where(x => x.IsDefault).FirstOrDefault();
        }

        public static Settings Instance { get; } = Json.Load<Settings>(DefaultSettingsFileLocation) ?? new Settings();
    }

    public class ServerAddStatus
    {
        public bool AddSucceeded { get; set; }
        public string Message { get; set; }

        public ServerAddStatus(bool Status, string Message)
        {
            AddSucceeded = Status;
            this.Message = Message;
        }
    }

    public class Settings : INotifyPropertyChanged
    {
        public bool FirstRun { get; set; } = true;

        public void SaveSettings()
        {
            Json.SaveWithFormatting(LauncherSettingsProvider.DefaultSettingsFileLocation, this, Formatting.Indented);
        }

        public string DefaultLocale { get; set; } = "English";

        public void SetDefaultServerAndSave(ServerSetting serverSetting)
        {
            foreach(ServerSetting setting in ServerCollection)
            {
                if(setting == serverSetting)
                {
                    setting.IsDefault = true;
                }
                else
                {
                    setting.IsDefault = false;
                }
            }

            SaveSettings();
        }

        public void RemoveServerAndSave(ServerSetting setting)
        {
            bool WasDefault = setting.IsDefault;

            if (ServerCollection.Contains(setting))
            {
                ServerCollection.Remove(setting);
            }

            if (WasDefault && ServerCollection.Count > 0)
            {
                SetDefaultServerAndSave(ServerCollection[0]);
            }
            else
            {
                SaveSettings();
            }
        }
        public ServerAddStatus AddServerAndSave(ServerSetting NewServer)
        {
            if (NewServer.Name == "" || NewServer.Url == "")
            {
                return new ServerAddStatus(false, LocalizationProvider.Instance.server_url_and_name_empty);
            }


            foreach (ServerSetting server in ServerCollection)
            {
                if (server.Name == NewServer.Name)
                {
                    return new ServerAddStatus(false, LocalizationProvider.Instance.server_name_exists);
                }

                if(server.Url == NewServer.Url)
                {
                    return new ServerAddStatus(false, LocalizationProvider.Instance.server_url_exists);
                }
            }

            ServerSetting newSetting = new ServerSetting { Name = NewServer.Name, Url = NewServer.Url, IsDefault = false };
            ServerCollection.Add(newSetting);

            if (NewServer.IsDefault || ServerCollection.Count == 1)
            {
                SetDefaultServerAndSave(newSetting);
            }
            else
            {
                SaveSettings();
            }

            return new ServerAddStatus(true, "OK");
        }

        private bool _IsAddingServer;
        [JsonIgnore]
        public bool IsAddingServer
        {
            get => _IsAddingServer;
            set
            {
                if(_IsAddingServer != value)
                {
                    _IsAddingServer = value;
                    RaisePropertyChanged(nameof(IsAddingServer));
                }
            }
        }

        private bool _IsEditingSettings;
        [JsonIgnore]
        public bool IsEditingSettings
        {
            get => _IsEditingSettings;
            set
            {
                if (_IsEditingSettings != value)
                {
                    _IsEditingSettings = value;
                    RaisePropertyChanged(nameof(IsEditingSettings));
                }
            }
        }

        private bool _AllowSettings;
        [JsonIgnore]
        public bool AllowSettings
        {
            get => _AllowSettings;
            set
            {
                if (_AllowSettings != value)
                {
                    _AllowSettings = value;
                    RaisePropertyChanged(nameof(AllowSettings));
                }
            }
        }

        private bool _GameRunning;
        [JsonIgnore]
        public bool GameRunning
        {
            get => _GameRunning;
            set
            {
                if(_GameRunning != value)
                {
                    _GameRunning = value;
                    RaisePropertyChanged(nameof(GameRunning));
                }
            }
        }

        private LauncherAction _LauncherStartGameAction;
        public LauncherAction LauncherStartGameAction
        {
            get => _LauncherStartGameAction;
            set
            {
                if(_LauncherStartGameAction != value)
                {
                    _LauncherStartGameAction = value;
                    RaisePropertyChanged(nameof(LauncherStartGameAction));
                }
            }
        }

        private bool _UseAutoLogin;
        public bool UseAutoLogin
        {
            get => _UseAutoLogin;
            set
            {
                if (_UseAutoLogin != value)
                {
                    _UseAutoLogin = value;
                    RaisePropertyChanged(nameof(UseAutoLogin));
                }
            }
        }

        private string _GamePath;
        public string GamePath
        {
            get => _GamePath;
            set
            {
                if(_GamePath != value)
                {
                    _GamePath = value;
                    RaisePropertyChanged(nameof(GamePath));
                }
            }
        }

        public ObservableCollection<ServerSetting> ServerCollection { get; set; } = new ObservableCollection<ServerSetting>();

        public Settings()
        {
            if(!File.Exists(LauncherSettingsProvider.DefaultSettingsFileLocation))
            {
                LauncherStartGameAction = LauncherAction.MinimizeAction;
                UseAutoLogin = true;
                GamePath = Environment.CurrentDirectory;

                ServerCollection.Add(new ServerSetting { Name = "Local SPT-AKI Server", Url = "https://127.0.0.1", IsDefault = true });
                SaveSettings();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
