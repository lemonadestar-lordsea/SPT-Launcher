/* GameStarter.cs
 * License: NCSA Open Source License
 * 
 * Copyright: Merijn Hendriks
 * AUTHORS:
 * waffle.lord
 * reider123
 * Merijn Hendriks
 */


using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using Aki.Launcher.MiniCommon;
using Aki.Launcher.Helpers;

namespace Aki.Launcher
{
	public class GameStarter
	{
        const string registeryInstall = @"Software\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\EscapeFromTarkov";
        const string registerySettings = @"Software\Battlestate Games\EscapeFromTarkov";
        string gamepath;

        public int LaunchGame(ServerInfo server, AccountInfo account)
        {
            gamepath = LauncherSettingsProvider.Instance.GamePath ?? Environment.CurrentDirectory;

            // setup directories
            if (IsInstalledInLive())
            {
                return -1;
            }

            SetupGameFiles();

            if (IsPiratedCopy() > 1)
            {
                return -2;
            }

            if (account.wipe)
            {
                RemoveRegisteryKeys();
                CleanTempFiles();
            }

            // apply patches
            var patchStatus = PatchManager.ApplyPatches(gamepath);

            if (patchStatus < 1)
            {
                // patching failed
                PatchManager.RestorePatched(gamepath);

                switch (patchStatus)
                {
                    case -1:
                        // failed to receive patches
                        return -3;

                    case -2:
                        // failed to apply core patch
                        return -4;

                    case -3:
                        // failed to apply mod patch
                        return -5;
                }
            }

            // start game
            var clientExecutable = $"{gamepath}\\EscapeFromTarkov.exe";

            if (!File.Exists(clientExecutable))
			{
				return -4;
			}
			
			var clientProcess = new ProcessStartInfo(clientExecutable)
			{
				Arguments = $"-force-gfx-jobs native -token={account.id} -config={Json.Serialize(new ClientConfig(server.backendUrl))}",
				UseShellExecute = false,
				WorkingDirectory = gamepath
			};

			Process.Start(clientProcess);
			return 1;
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
                    new FileInfo(value2.Replace(value3.Name, @"EscapeFromTarkov_Data\Managed\Nlog.Aki.Common.dll")),
                    new FileInfo(value2.Replace(value3.Name, @"EscapeFromTarkov_Data\Managed\Nlog.Aki.Core.dll")),
                    new FileInfo(value2.Replace(value3.Name, @"EscapeFromTarkov_Data\Managed\Nlog.Aki.SinglePlayer.dll")),
                    new FileInfo(value2.Replace(value3.Name, @"EscapeFromTarkov_Data\Managed\Nlog.Aki.Tools.dll")),
                };

                foreach (var value in value4)
                {
                    if (File.Exists(value.FullName))
                    {
                        File.Delete(value.FullName);
                        value0 = true;
                    }
                }

                if (value0)
                {
                    File.Delete(@"EscapeFromTarkov_Data\Managed\Assembly-CSharp.dll");
                }
            }
            catch
            {
            }

            return value0;
        }

        void SetupGameFiles()
        {
            var filepath = LauncherSettingsProvider.Instance.GamePath ?? Environment.CurrentDirectory;
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
            var basepath = @"Battlestate Games\EscapeFromTarkov";
			var rootdir = new DirectoryInfo(Path.Combine(Path.GetTempPath(), basepath));

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
