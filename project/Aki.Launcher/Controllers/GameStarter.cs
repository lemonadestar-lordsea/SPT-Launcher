/* GameStarter.cs
 * License: NCSA Open Source License
 * 
 * Copyright: Merijn Hendriks
 * AUTHORS:
 * waffle.lord
 * reider123
 * Merijn Hendriks
 */


using Aki.Launcher.Custom_Controls;
using Aki.Launcher.Custom_Controls.Dialogs;
using Aki.Launcher.Helpers;
using Aki.Launcher.MiniCommon;
using Aki.Launcher.Models.Launcher;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Aki.Launcher
{
    public class GameStarter
    {
        const string registeryInstall = @"Software\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\EscapeFromTarkov";
        const string registerySettings = @"Software\Battlestate Games\EscapeFromTarkov";

        public async Task<GameStarterResult> LaunchGame(ServerInfo server, AccountInfo account)
        {
            var gamepath = $@"{LauncherSettingsProvider.Instance.GamePath}\" ?? Environment.CurrentDirectory;

            // setup directories
            if (IsInstalledInLive())
            {
                return GameStarterResult.FromError(-1);
            }

            SetupGameFiles(gamepath);

            if (IsPiratedCopy() > 1)
            {
                return GameStarterResult.FromError(-2);
            }

            if (account.wipe)
            {
                RemoveRegisteryKeys();
                CleanTempFiles();
            }

            //create nlog.dll.nlog
            if(!NLogCreator.Create())
            {
                return GameStarterResult.FromError(-7);
            }


            // check game path
            var clientExecutable = $@"{gamepath}EscapeFromTarkov.exe";

            if (!File.Exists(clientExecutable))
            {
                return GameStarterResult.FromError(-6);
            }


            // apply patches
            ProgressReportingPatchRunner patchRunner = new ProgressReportingPatchRunner(gamepath);
            ProgressDialog pDialog = new ProgressDialog(patchRunner);
            var result = await DialogHost.ShowDialog(pDialog);

            if (result != null)
            {
                bool handled = false;

                if (result is PatchResultInfo pri && pri.Status == ByteBanger.PatchResultType.InputChecksumMismatch)
                {
                    ConfirmationDialog confirmContinuePatching = new ConfirmationDialog(LocalizationProvider.Instance.file_mismatch_dialog_message, LocalizationProvider.Instance.yes, LocalizationProvider.Instance.no);

                    var confirmResult = await DialogHost.ShowDialog(confirmContinuePatching);

                    if (confirmResult != null && confirmResult is bool proceed && proceed)
                    {
                        ProgressReportingPatchRunner continuePatcher = new ProgressReportingPatchRunner(gamepath, pri.RemainingPatches, true);
                        ProgressDialog continueDialog = new ProgressDialog(continuePatcher);

                        var continuedPatchResult = await DialogHost.ShowDialog(continueDialog);

                        if (continuedPatchResult != null)
                        {
                            return GameStarterResult.FromError(-4);
                        }

                        handled = true;
                    }
                }
                
                if(!handled && result is Exception ex)
                {
                    //show error message
                    MessageDialog msgDialgo = new MessageDialog(ex.Message);
                    await DialogHost.ShowDialog(msgDialgo);
                    return GameStarterResult.FromError(-4);
                }

                if (!handled) return GameStarterResult.FromError(-4);
            }

            //start game
            var clientProcess = new ProcessStartInfo(clientExecutable)
            {
                Arguments = $"-force-gfx-jobs native -token={account.id} -config={Json.Serialize(new ClientConfig(server.backendUrl))}",
                UseShellExecute = false,
                WorkingDirectory = gamepath
            };

            Process.Start(clientProcess);
            return GameStarterResult.FromSuccess();
        }

        bool IsInstalledInLive()
        {
            var value0 = false;

            try
            {
                var value1 = Registry.LocalMachine.OpenSubKey(registeryInstall, false).GetValue("UninstallString");
                var value2 = (value1 != null) ? value1.ToString() : "";
                var value3 = new FileInfo(value2);
                var value4 = new FileInfo[]
                {
                    new FileInfo(value2.Replace(value3.Name, @"Launcher.exe")),
                    new FileInfo(value2.Replace(value3.Name, @"Server.exe")),
                    new FileInfo(value2.Replace(value3.Name, @"EscapeFromTarkov_Data\Managed\0Harmony.dll")),
                    new FileInfo(value2.Replace(value3.Name, @"EscapeFromTarkov_Data\Managed\NLog.dll.nlog")),
                    new FileInfo(value2.Replace(value3.Name, @"EscapeFromTarkov_Data\Managed\Nlog.Aki.Loader.dll")),
                };
                var value5 = new FileInfo[]
                {
                    new FileInfo(value2.Replace(value3.Name, @"EscapeFromTarkov_Data\Managed\Assembly-CSharp.dll.bak")),
                    new FileInfo(value2.Replace(value3.Name, @"EscapeFromTarkov_Data\Managed\Assembly-CSharp.dll")),
                };
                var value6 = new DirectoryInfo(value2.Replace(value3.Name, @"Aki_Data"));

                foreach (var value in value4)
                {
                    if (File.Exists(value.FullName))
                    {
                        File.Delete(value.FullName);
                        value0 = true;
                    }
                }

                if (File.Exists(value5[0].FullName))
                {
                    File.Delete(value5[1].FullName);
                    File.Move(value5[0].FullName, value5[1].FullName);
                    value0 = true;
                }

                if (Directory.Exists(value6.FullName))
                {
                    RemoveFilesRecurse(value6);
                    value0 = true;
                }
            }
            catch
            {
            }

            return value0;
        }

        void SetupGameFiles(string filepath)
        {
            var files = new string[]
            {
                Path.Combine(filepath, "BattlEye"),
                Path.Combine(filepath, "Logs"),
                Path.Combine(filepath, "ConsistencyInfo"),
                Path.Combine(filepath, "EscapeFromTarkov_BE.exe"),
                Path.Combine(filepath, "Uninstall.exe"),
                Path.Combine(filepath, "UnityCrashHandler64.exe"),
                Path.Combine(filepath, "WinPixEventRuntime.dll")
            };

            foreach (var file in files)
            {
                if (Directory.Exists(file))
                {
                    RemoveFilesRecurse(new DirectoryInfo(file));
                }

                if (File.Exists(file))
                {
                    File.Delete(file);
                }
            }
        }

        int IsPiratedCopy()
        {
            var value0 = 0;

            try
            {
                var value1 = Registry.LocalMachine.OpenSubKey(registeryInstall, false).GetValue("UninstallString");
                var value2 = (value1 != null) ? value1.ToString() : "";
                var value3 = new FileInfo(value2);
                var value4 = new FileInfo[3]
                {
                    value3,
                    new FileInfo(value2.Replace(value3.Name, @"BattlEye\BEClient_x64.dll")),
                    new FileInfo(value2.Replace(value3.Name, @"BattlEye\BEService_x64.dll"))
                };

                value0 = value4.Length;

                foreach (var value in value4)
                {
                    if (File.Exists(value.FullName))
                    {
                        --value0;
                    }
                }
            }
            catch
            {
                value0 = 5;
            }

            return value0;
        }

        /// <summary>
        /// Remove the registry keys
        /// </summary>
        /// <returns>returns true if the keys were removed. returns false if an exception occured</returns>
		public bool RemoveRegisteryKeys()
        {
            try
            {
                var key = Registry.CurrentUser.OpenSubKey(registerySettings, true);

                foreach (var value in key.GetValueNames())
                {
                    key.DeleteValue(value);
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Clean the temp folder
        /// </summary>
        /// <returns>returns true if the temp folder was cleaned succefully or doesn't exist. returns false if something went wrong.</returns>
		public bool CleanTempFiles()
        {
            var rootdir = new DirectoryInfo(Path.Combine(Path.GetTempPath(), @"Battlestate Games\EscapeFromTarkov"));

            if (!rootdir.Exists)
            {
                return true;
            }

            return RemoveFilesRecurse(rootdir);
        }

        bool RemoveFilesRecurse(DirectoryInfo basedir)
        {
            if (!basedir.Exists)
            {
                return true;
            }

            try
            {
                // remove subdirectories
                foreach (var dir in basedir.EnumerateDirectories())
                {
                    RemoveFilesRecurse(dir);
                }

                // remove files
                var files = basedir.GetFiles();

                foreach (var file in files)
                {
                    file.IsReadOnly = false;
                    file.Delete();
                }

                // remove directory
                basedir.Delete();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}
