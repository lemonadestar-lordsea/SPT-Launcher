/* LocalizationProvider.cs
 * License: NCSA Open Source License
 * 
 * Copyright: Merijn Hendriks
 * AUTHORS:
 * waffle.lord
 */


using Aki.Launcher.Extensions;
using Aki.Launcher.MiniCommon;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Aki.Launcher.Helpers
{
    public static class LocalizationProvider
    {
        public static string DefaultLocaleFolderPath = $"{Environment.CurrentDirectory}\\Aki_Data\\Launcher\\Locales";

        public static Dictionary<string, string> LocaleNameDictionary = GetLocaleDictionary();

        public static void LoadLocaleFromFile(string localeName)
        {
            string localeRomanName = LocaleNameDictionary.GetKeyByValue(localeName);

            if (String.IsNullOrEmpty(localeRomanName))
            {
                localeRomanName = localeName;
            }

            LocaleData newLocale = Json.LoadClassWithoutSaving<LocaleData>($"{DefaultLocaleFolderPath}\\{localeRomanName}.json");

            if (newLocale != null)
            {
                foreach(var prop in Instance.GetType().GetProperties())
                {
                    prop.SetValue(Instance, newLocale.GetType().GetProperty(prop.Name).GetValue(newLocale));
                }
                
                LauncherSettingsProvider.Instance.DefaultLocale = localeRomanName;
                LauncherSettingsProvider.Instance.SaveSettings();
            }

            //could possibly raise an event here to say why the local wasn't changed.
        }

        private static string GetSystemLocale()
        {
            string UIlocaleName = CultureInfo.CurrentUICulture.DisplayName;

            var regexMatch = Regex.Match(UIlocaleName, @"^(\w+)");

            if (regexMatch.Groups.Count == 2)
            {
                string localRomanName = LocaleNameDictionary.GetValueOrDefault(regexMatch.Groups[1].Value, "");

                bool localExists = GetAvailableLocales().Where(x => x == localRomanName).Count() > 0;

                if (localExists)
                {
                    return localRomanName;
                }
            }

            return "English";
        }

        public static void TryAutoSetLocale()
        {
            LoadLocaleFromFile(GetSystemLocale());
        }

        public static LocaleData GenerateEnglishLocale()
        {
            //Create default english locale data and save if the default locale data file dosen't exist.
            //This is to (hopefully) prevent the launcher from becoming 100% broken if no locale files exist or the locale files are outdated (missing data).
            LocaleData englishLocale = new LocaleData();

            #region Set All English Defaults
            englishLocale.native_name = "English";
            englishLocale.retry = "Retry";
            englishLocale.server_connecting = "Connecting";
            englishLocale.server_unavailable_format_1 = "Default server '{0}' is not available.";
            englishLocale.no_servers_available = "No Servers found. Check server list in settings.";
            englishLocale.settings_menu = "Settings Menu";
            englishLocale.back = "Back";
            englishLocale.wipe = "Wipe";
            englishLocale.wipe_profile = "Wipe Profile";
            englishLocale.username = "Username";
            englishLocale.password = "Password";
            englishLocale.update = "Update";
            englishLocale.edit_profile_update_error = "Some Issues occurred while updating your profile.";
            englishLocale.register = "Register";
            englishLocale.go_to_register = "Go to Register";
            englishLocale.registration_failed = "Registration Failed";
            englishLocale.login = "Login";
            englishLocale.go_to_login = "Go to Login";
            englishLocale.login_automatically = "Login Automatically";
            englishLocale.incorrect_login = "Username or password are incorrect";
            englishLocale.login_failed = "Login Failed";
            englishLocale.edition = "Edition";
            englishLocale.id = "ID";
            englishLocale.logout = "Logout";
            englishLocale.edit_profile = "Edit Profile";
            englishLocale.start_game = "Start Game";
            englishLocale.installed_in_live_game_warning = "Aki shouldn't be installed into live game. Please make a copy of the gamefiles and install Aki there.";
            englishLocale.no_official_game_warning = "Escape From Tarkov isn't installed on your computer. Please buy a copy of the game and support the developers!";
            englishLocale.eft_exe_not_found_warning = "EscapeFromTarkov.exe not found at game path.";
            englishLocale.account_exist = "Account already exists";
            englishLocale.local_server_remove_warning = "You are about to remove the local server.";
            englishLocale.remove_server_question = "Remove Server?";
            englishLocale.exit_settings_tooltip = "Exit Settings";
            englishLocale.name = "Name";
            englishLocale.save = "Save";
            englishLocale.url = "URL";
            englishLocale.new_server = "New Server";
            englishLocale.server_list_back_arrow_tooltip = "Back to server list";
            englishLocale.make_default = "Make Default";
            englishLocale.@default = "Default";
            englishLocale.server_list = "Server List";
            englishLocale.add = "Add";
            englishLocale.remove_server_tooltip = "Remove Server";
            englishLocale.default_language = "Default Language";
            englishLocale.game_path = "Game Path";
            englishLocale.clear_game_settings = "Clear Game Settings";
            englishLocale.clear_game_settings_succeeded = "Game settings cleared";
            englishLocale.clear_game_settings_failed = "Some issues occurred while clearing game settings";
            englishLocale.remove_registry_keys = "Remove Registry Keys";
            englishLocale.remove_registry_keys_succeeded = "Registry keys removed";
            englishLocale.remove_registry_keys_failed = "Some issues occurred while removing registry keys";
            englishLocale.clean_temp_files = "Clean Temp Files";
            englishLocale.clean_temp_files_succeeded = "Temp files cleaned";
            englishLocale.clean_temp_files_failed = "Some issues occurred while cleaning temp files";
            englishLocale.select_folder = "Select Folder";
            englishLocale.server_url_and_name_empty = "Name and Url cannot be empty";
            englishLocale.server_url_exists = "Url already exists";
            englishLocale.server_name_exists = "Name already exists";
            englishLocale.minimize_action = "Minimize";
            englishLocale.do_nothing_action = "Do nothing";
            englishLocale.exit_action = "Close Launcher";
            englishLocale.launcher_start_game_action = "Launcher Start Game Action";
            englishLocale.game = "Game";
            englishLocale.new_password = "New Password";
            englishLocale.wipe_warning_format_2 = "Changing your account edition requires a profile wipe.\n'{0}' -> '{1}'\nAre you sure you want to wipe your profile?";
            englishLocale.cancel = "Cancel";
            #endregion

            Directory.CreateDirectory(LocalizationProvider.DefaultLocaleFolderPath);
            LauncherSettingsProvider.Instance.DefaultLocale = "English";
            LauncherSettingsProvider.Instance.SaveSettings();
            Json.SaveWithFormatting($"{LocalizationProvider.DefaultLocaleFolderPath}\\English.json", englishLocale, Newtonsoft.Json.Formatting.Indented);

            return englishLocale;
        }

        public static Dictionary<string, string> GetLocaleDictionary()
        {
            List<FileInfo> localeFiles = new List<FileInfo>(Directory.GetFiles(DefaultLocaleFolderPath).Select(x => new FileInfo(x)).ToList());
            Dictionary<string, string> localeDictionary = new Dictionary<string, string>();

            foreach(FileInfo file in localeFiles)
            {
                localeDictionary.Add(file.Name.Replace(".json",""), Json.GetPropertyByName<string>(file.FullName, "native_name"));
            }

            return localeDictionary;
        }
        public static ObservableCollection<string> GetAvailableLocales()
        {
            return new ObservableCollection<string>(LocaleNameDictionary.Values);
        }

        public static LocaleData Instance { get; private set; } = Json.LoadClassWithoutSaving<LocaleData>($"{DefaultLocaleFolderPath}\\{LauncherSettingsProvider.Instance.DefaultLocale}.json") ?? GenerateEnglishLocale();
    }

    public class LocaleData : INotifyPropertyChanged
    {
        //this is going to be some pretty long boiler plate code. So I'm putting everything into regions.

        #region All Properties

        #region native_name
        private string _native_name;
        public string native_name
        {
            get => _native_name;
            set
            {
                if (_native_name != value)
                {
                    _native_name = value;
                    RaisePropertyChanged(nameof(native_name));
                }
            }
        }
        #endregion

        #region retry
        private string _retry;
        public string retry
        {
            get => _retry;
            set
            {
                if (_retry != value)
                {
                    _retry = value;
                    RaisePropertyChanged(nameof(retry));
                }
            }
        }
        #endregion

        #region server_connecting
        private string _server_connecting;
        public string server_connecting
        {
            get => _server_connecting;
            set
            {
                if (_server_connecting != value)
                {
                    _server_connecting = value;
                    RaisePropertyChanged(nameof(server_connecting));
                }
            }
        }
        #endregion

        #region server_unavailable_format_1
        private string _server_unavailable_format_1;
        public string server_unavailable_format_1
        {
            get => _server_unavailable_format_1;
            set
            {
                if (_server_unavailable_format_1 != value)
                {
                    _server_unavailable_format_1 = value;
                    RaisePropertyChanged(nameof(server_unavailable_format_1));
                }
            }
        }
        #endregion

        #region no_servers_available
        private string _no_servers_available;
        public string no_servers_available
        {
            get => _no_servers_available;
            set
            {
                if (_no_servers_available != value)
                {
                    _no_servers_available = value;
                    RaisePropertyChanged(nameof(no_servers_available));
                }
            }
        }
        #endregion

        #region settings_menu
        private string _settings_menu;
        public string settings_menu
        {
            get => _settings_menu;
            set
            {
                if (_settings_menu != value)
                {
                    _settings_menu = value;
                    RaisePropertyChanged(nameof(settings_menu));
                }
            }
        }
        #endregion

        #region back
        private string _back;
        public string back
        {
            get => _back;
            set
            {
                if (_back != value)
                {
                    _back = value;
                    RaisePropertyChanged(nameof(back));
                }
            }
        }
        #endregion

        #region wipe
        private string _wipe;
        public string wipe
        {
            get => _wipe;
            set
            {
                if (_wipe != value)
                {
                    _wipe = value;
                    RaisePropertyChanged(nameof(wipe));
                }
            }
        }
        #endregion

        #region wipe_profile
        private string _wipe_profile;
        public string wipe_profile
        {
            get => _wipe_profile;
            set
            {
                if (_wipe_profile != value)
                {
                    _wipe_profile = value;
                    RaisePropertyChanged(nameof(wipe_profile));
                }
            }
        }
        #endregion

        #region username
        private string _username;
        public string username
        {
            get => _username;
            set
            {
                if (_username != value)
                {
                    _username = value;
                    RaisePropertyChanged(nameof(username));
                }
            }
        }
        #endregion

        #region password
        private string _password;
        public string password
        {
            get => _password;
            set
            {
                if (_password != value)
                {
                    _password = value;
                    RaisePropertyChanged(nameof(password));
                }
            }
        }
        #endregion

        #region update
        private string _update;
        public string update
        {
            get => _update;
            set
            {
                if (_update != value)
                {
                    _update = value;
                    RaisePropertyChanged(nameof(update));
                }
            }
        }
        #endregion

        #region edit_profile_update_error
        private string _edit_profile_update_error;
        public string edit_profile_update_error
        {
            get => _edit_profile_update_error;
            set
            {
                if (_edit_profile_update_error != value)
                {
                    _edit_profile_update_error = value;
                    RaisePropertyChanged(nameof(edit_profile_update_error));
                }
            }
        }
        #endregion

        #region register
        private string _register;
        public string register
        {
            get => _register;
            set
            {
                if (_register != value)
                {
                    _register = value;
                    RaisePropertyChanged(nameof(register));
                }
            }
        }
        #endregion

        #region go_to_register
        private string _go_to_register;
        public string go_to_register
        {
            get => _go_to_register;
            set
            {
                if (_go_to_register != value)
                {
                    _go_to_register = value;
                    RaisePropertyChanged(nameof(_go_to_register));
                }
            }
        }
        #endregion

        #region login
        private string _login;
        public string login
        {
            get => _login;
            set
            {
                if (_login != value)
                {
                    _login = value;
                    RaisePropertyChanged(nameof(login));
                }
            }
        }
        #endregion

        #region go_to_login
        private string _go_to_login;
        public string go_to_login
        {
            get => _go_to_login;
            set
            {
                if (_go_to_login != value)
                {
                    _go_to_login = value;
                    RaisePropertyChanged(nameof(go_to_login));
                }
            }
        }
        #endregion

        #region login_automatically
        private string _login_automatically;
        public string login_automatically
        {
            get => _login_automatically;
            set
            {
                if (_login_automatically != value)
                {
                    _login_automatically = value;
                    RaisePropertyChanged(nameof(login_automatically));
                }
            }
        }
        #endregion

        #region incorrect_login
        private string _incorrect_login;
        public string incorrect_login
        {
            get => _incorrect_login;
            set
            {
                if (_incorrect_login != value)
                {
                    _incorrect_login = value;
                    RaisePropertyChanged(nameof(incorrect_login));
                }
            }
        }
        #endregion

        #region login_failed
        private string _login_failed;
        public string login_failed
        {
            get => _login_failed;
            set
            {
                if (_login_failed != value)
                {
                    _login_failed = value;
                    RaisePropertyChanged(nameof(login_failed));
                }
            }
        }
        #endregion

        #region edition
        private string _edition;
        public string edition
        {
            get => _edition;
            set
            {
                if (_edition != value)
                {
                    _edition = value;
                    RaisePropertyChanged(nameof(edition));
                }
            }
        }
        #endregion

        #region id
        private string _id;
        public string id
        {
            get => _id;
            set
            {
                if (_id != value)
                {
                    _id = value;
                    RaisePropertyChanged(nameof(id));
                }
            }
        }
        #endregion

        #region logout
        private string _logout;
        public string logout
        {
            get => _logout;
            set
            {
                if (_logout != value)
                {
                    _logout = value;
                    RaisePropertyChanged(nameof(logout));
                }
            }
        }
        #endregion

        #region edit_profile
        private string _edit_profile;
        public string edit_profile
        {
            get => _edit_profile;
            set
            {
                if (_edit_profile != value)
                {
                    _edit_profile = value;
                    RaisePropertyChanged(nameof(edit_profile));
                }
            }
        }
        #endregion

        #region start_game
        private string _start_game;
        public string start_game
        {
            get => _start_game;
            set
            {
                if (_start_game != value)
                {
                    _start_game = value;
                    RaisePropertyChanged(nameof(start_game));
                }
            }
        }
        #endregion

        #region installed_in_live_game_warning
        private string _installed_in_live_game_warning;
        public string installed_in_live_game_warning
        {
            get => _installed_in_live_game_warning;
            set
            {
                if (_installed_in_live_game_warning != value)
                {
                    _installed_in_live_game_warning = value;
                    RaisePropertyChanged(nameof(installed_in_live_game_warning));
                }
            }
        }
        #endregion

        #region no_official_game_warning
        private string _no_official_game_warning;
        public string no_official_game_warning
        {
            get => _no_official_game_warning;
            set
            {
                if (_no_official_game_warning != value)
                {
                    _no_official_game_warning = value;
                    RaisePropertyChanged(nameof(no_official_game_warning));
                }
            }
        }
        #endregion

        #region eft_exe_not_found_warning
        private string _eft_exe_not_found_warning;
        public string eft_exe_not_found_warning
        {
            get => _eft_exe_not_found_warning;
            set
            {
                if (_eft_exe_not_found_warning != value)
                {
                    _eft_exe_not_found_warning = value;
                    RaisePropertyChanged(nameof(eft_exe_not_found_warning));
                }
            }
        }
        #endregion

        #region account_exist
        private string _account_exist;
        public string account_exist
        {
            get => _account_exist;
            set
            {
                if (_account_exist != value)
                {
                    _account_exist = value;
                    RaisePropertyChanged(nameof(account_exist));
                }
            }
        }
        #endregion

        #region local_server_remove_warning
        private string _local_server_remove_warning;
        public string local_server_remove_warning
        {
            get => _local_server_remove_warning;
            set
            {
                if (_local_server_remove_warning != value)
                {
                    _local_server_remove_warning = value;
                    RaisePropertyChanged(nameof(local_server_remove_warning));
                }
            }
        }
        #endregion

        #region remove_server_question
        private string _remove_server_question;
        public string remove_server_question
        {
            get => _remove_server_question;
            set
            {
                if (_remove_server_question != value)
                {
                    _remove_server_question = value;
                    RaisePropertyChanged(nameof(remove_server_question));
                }
            }
        }
        #endregion

        #region exit_settings_tooltip
        private string _exit_setttings_tooltip;
        public string exit_settings_tooltip
        {
            get => _exit_setttings_tooltip;
            set
            {
                if (_exit_setttings_tooltip != value)
                {
                    _exit_setttings_tooltip = value;
                    RaisePropertyChanged(nameof(exit_settings_tooltip));
                }
            }
        }
        #endregion

        #region name
        private string _name;
        public string name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    RaisePropertyChanged(nameof(name));
                }
            }
        }
        #endregion

        #region save
        private string _save;
        public string save
        {
            get => _save;
            set
            {
                if (_save != value)
                {
                    _save = value;
                    RaisePropertyChanged(nameof(save));
                }
            }
        }
        #endregion

        #region url
        private string _url;
        public string url
        {
            get => _url;
            set
            {
                if (_url != value)
                {
                    _url = value;
                    RaisePropertyChanged(nameof(url));
                }
            }
        }
        #endregion

        #region new_server
        private string _new_server;
        public string new_server
        {
            get => _new_server;
            set
            {
                if (_new_server != value)
                {
                    _new_server = value;
                    RaisePropertyChanged(nameof(new_server));
                }
            }
        }
        #endregion

        #region server_list_back_arrow_tooltip
        private string _server_list_back_arrow_tooltip;
        public string server_list_back_arrow_tooltip
        {
            get => _server_list_back_arrow_tooltip;
            set
            {
                if (_server_list_back_arrow_tooltip != value)
                {
                    _server_list_back_arrow_tooltip = value;
                    RaisePropertyChanged(nameof(server_list_back_arrow_tooltip));
                }
            }
        }
        #endregion

        #region make_default
        private string _make_default;
        public string make_default
        {
            get => _make_default;
            set
            {
                if (_make_default != value)
                {
                    _make_default = value;
                    RaisePropertyChanged(nameof(make_default));
                }
            }
        }
        #endregion

        #region default
        private string _default;
        //@ symbol is because 'default' is a reserved word
        public string @default
        {
            get => _default;
            set
            {
                if (_default != value)
                {
                    _default = value;
                    RaisePropertyChanged(nameof(@default));
                }
            }
        }
        #endregion

        #region server_list
        private string _server_list;
        public string server_list
        {
            get => _server_list;
            set
            {
                if (_server_list != value)
                {
                    _server_list = value;
                    RaisePropertyChanged(nameof(server_list));
                }
            }
        }
        #endregion

        #region add
        private string _add;
        public string add
        {
            get => _add;
            set
            {
                if (_add != value)
                {
                    _add = value;
                    RaisePropertyChanged(nameof(add));
                }
            }
        }
        #endregion

        #region remove_server_tooltip
        private string _remove_server_tooltip;
        public string remove_server_tooltip
        {
            get => _remove_server_tooltip;
            set
            {
                if (_remove_server_tooltip != value)
                {
                    _remove_server_tooltip = value;
                    RaisePropertyChanged(nameof(remove_server_tooltip));
                }
            }
        }
        #endregion

        #region default_language
        private string _default_language;
        public string default_language
        {
            get => _default_language;
            set
            {
                if (_default_language != value)
                {
                    _default_language = value;
                    RaisePropertyChanged(nameof(default_language));
                }
            }
        }
        #endregion

        #region game_path
        private string _game_path;
        public string game_path
        {
            get => _game_path;
            set
            {
                if (_game_path != value)
                {
                    _game_path = value;
                    RaisePropertyChanged(nameof(game_path));
                }
            }
        }
        #endregion

        #region clear_game_settings
        private string _clear_game_settings;
        public string clear_game_settings
        {
            get => _clear_game_settings;
            set
            {
                if (_clear_game_settings != value)
                {
                    _clear_game_settings = value;
                    RaisePropertyChanged(nameof(clear_game_settings));
                }
            }
        }
        #endregion

        #region clear_game_settings_succeeded
        private string _clear_game_settings_succeeded;
        public string clear_game_settings_succeeded
        {
            get => _clear_game_settings_succeeded;
            set
            {
                if (_clear_game_settings_succeeded != value)
                {
                    _clear_game_settings_succeeded = value;
                    RaisePropertyChanged(nameof(clear_game_settings_succeeded));
                }
            }
        }
        #endregion

        #region clear_game_settings_failed
        private string _clear_game_settings_failed;
        public string clear_game_settings_failed
        {
            get => _clear_game_settings_failed;
            set
            {
                if (_clear_game_settings_failed != value)
                {
                    _clear_game_settings_failed = value;
                    RaisePropertyChanged(nameof(clear_game_settings_failed));
                }
            }
        }
        #endregion

        #region remove_registry_keys
        private string _remove_registry_keys;
        public string remove_registry_keys
        {
            get => _remove_registry_keys;
            set
            {
                if (_remove_registry_keys != value)
                {
                    _remove_registry_keys = value;
                    RaisePropertyChanged(nameof(remove_registry_keys));
                }
            }
        }
        #endregion

        #region remove_registry_keys_succeeded
        private string _remove_registry_keys_succeeded;
        public string remove_registry_keys_succeeded
        {
            get => _remove_registry_keys_succeeded;
            set
            {
                if (_remove_registry_keys_succeeded != value)
                {
                    _remove_registry_keys_succeeded = value;
                    RaisePropertyChanged(nameof(remove_registry_keys_succeeded));
                }
            }
        }
        #endregion

        #region remove_registry_keys_failed
        private string _remove_registry_keys_failed;
        public string remove_registry_keys_failed
        {
            get => _remove_registry_keys_failed;
            set
            {
                if (_remove_registry_keys_failed != value)
                {
                    _remove_registry_keys_failed = value;
                    RaisePropertyChanged(nameof(remove_registry_keys_failed));
                }
            }
        }
        #endregion

        #region clean_temp_files
        private string _clean_temp_files;
        public string clean_temp_files
        {
            get => _clean_temp_files;
            set
            {
                if (_clean_temp_files != value)
                {
                    _clean_temp_files = value;
                    RaisePropertyChanged(nameof(clean_temp_files));
                }
            }
        }
        #endregion

        #region clean_temp_files_succeeded
        private string _clean_temp_files_succeeded;
        public string clean_temp_files_succeeded
        {
            get => _clean_temp_files_succeeded;
            set
            {
                if (_clean_temp_files_succeeded != value)
                {
                    _clean_temp_files_succeeded = value;
                    RaisePropertyChanged(nameof(clean_temp_files_succeeded));
                }
            }
        }
        #endregion

        #region clean_temp_files_failed
        private string _clean_temp_files_failed;
        public string clean_temp_files_failed
        {
            get => _clean_temp_files_failed;
            set
            {
                if (_clean_temp_files_failed != value)
                {
                    _clean_temp_files_failed = value;
                    RaisePropertyChanged(nameof(clean_temp_files_failed));
                }
            }
        }
        #endregion

        #region select_folder
        private string _select_folder;
        public string select_folder
        {
            get => _select_folder;
            set
            {
                if (_select_folder != value)
                {
                    _select_folder = value;
                    RaisePropertyChanged(nameof(select_folder));
                }
            }
        }
        #endregion

        #region server_url_and_name_empty
        private string _server_url_and_name_empty;
        public string server_url_and_name_empty
        {
            get => _server_url_and_name_empty;
            set
            {
                if (_server_url_and_name_empty != value)
                {
                    _server_url_and_name_empty = value;
                    RaisePropertyChanged(nameof(server_url_and_name_empty));
                }
            }
        }
        #endregion

        #region server_url_exists
        private string _server_url_exists;
        public string server_url_exists
        {
            get => _server_url_exists;
            set
            {
                if (_server_url_exists != value)
                {
                    _server_url_exists = value;
                    RaisePropertyChanged(nameof(server_url_exists));
                }
            }
        }
        #endregion

        #region server_name_exists
        private string _server_name_exists;
        public string server_name_exists
        {
            get => _server_name_exists;
            set
            {
                if (_server_name_exists != value)
                {
                    _server_name_exists = value;
                    RaisePropertyChanged(nameof(server_name_exists));
                }
            }
        }
        #endregion

        #region registration_failed
        private string _registration_failed;
        public string registration_failed
        {
            get => _registration_failed;
            set
            {
                if (_registration_failed != value)
                {
                    _registration_failed = value;
                    RaisePropertyChanged(nameof(registration_failed));
                }
            }
        }
        #endregion

        #region minimize_action
        private string _minimize_action;
        public string minimize_action
        {
            get => _minimize_action;
            set
            {
                if (_minimize_action != value)
                {
                    _minimize_action = value;
                    RaisePropertyChanged(nameof(minimize_action));
                }
            }
        }
        #endregion

        #region do_nothing_action
        private string _do_nothing_action;
        public string do_nothing_action
        {
            get => _do_nothing_action;
            set
            {
                if (_do_nothing_action != value)
                {
                    _do_nothing_action = value;
                    RaisePropertyChanged(nameof(do_nothing_action));
                }
            }
        }
        #endregion

        #region exit_action
        private string _exit_action;
        public string exit_action
        {
            get => _exit_action;
            set
            {
                if (_exit_action != value)
                {
                    _exit_action = value;
                    RaisePropertyChanged(nameof(exit_action));
                }
            }
        }
        #endregion

        #region launcher_start_game_action
        private string _launcher_start_game_action;
        public string launcher_start_game_action
        {
            get => _launcher_start_game_action;
            set
            {
                if (_launcher_start_game_action != value)
                {
                    _launcher_start_game_action = value;
                    RaisePropertyChanged(nameof(launcher_start_game_action));
                }
            }
        }
        #endregion

        #region game
        private string _game;
        public string game
        {
            get => _game;
            set
            {
                if (_game != value)
                {
                    _game = value;
                    RaisePropertyChanged(nameof(game));
                }
            }
        }
        #endregion

        #region new_password
        private string _new_password;
        public string new_password
        {
            get => _new_password;
            set
            {
                if (_new_password != value)
                {
                    _new_password = value;
                    RaisePropertyChanged(nameof(new_password));
                }
            }
        }
        #endregion

        #region wipe_warning_format_2
        private string _wipe_warning_format_1;
        public string wipe_warning_format_2
        {
            get => _wipe_warning_format_1;
            set
            {
                if (_wipe_warning_format_1 != value)
                {
                    _wipe_warning_format_1 = value;
                    RaisePropertyChanged(nameof(wipe_warning_format_2));
                }
            }
        }
        #endregion

        #region cancel
        private string _cancel;
        public string cancel
        {
            get => _cancel;
            set
            {
                if (_cancel != value)
                {
                    _cancel = value;
                    RaisePropertyChanged(nameof(cancel));
                }
            }
        }
        #endregion

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
