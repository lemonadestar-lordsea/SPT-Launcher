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
using System.Text;
using Aki.Launcher.MiniCommon;
using System.Linq;
using Aki.Launcher.Helpers;
using System.Windows;

namespace Aki.Launcher
{
	public class GameStarter
	{
        private string clientExecutable = $"{LauncherSettingsProvider.Instance.GamePath}\\EscapeFromTarkov.exe";
        private const string registeryInstall = @"Software\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\EscapeFromTarkov";
        private const string registerySettings = @"Software\Battlestate Games\EscapeFromTarkov";
        private const string tempDir = @"Battlestate Games\EscapeFromTarkov";

        public int LaunchGame(ServerInfo server, AccountInfo account)
        {
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

            if (!File.Exists(clientExecutable))
			{
				return -3;
			}
			
			ProcessStartInfo clientProcess = new ProcessStartInfo(clientExecutable)
			{
				Arguments = $"-force-gfx-jobs native -token={account.id} -config={Json.Serialize(new ClientConfig(server.backendUrl))}",
				UseShellExecute = false,
				WorkingDirectory = LauncherSettingsProvider.Instance.GamePath ?? Environment.CurrentDirectory
			};

			Process.Start(clientProcess);

			return 1;
		}

        private bool IsInstalledInLive()
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

        private void SetupGameFiles()
        {
            string filepath = LauncherSettingsProvider.Instance.GamePath ?? Environment.CurrentDirectory;
            string[] files = new string[]
            {
                Path.Combine(filepath, "BattlEye"),
                Path.Combine(filepath, "Logs"),
                Path.Combine(filepath, "ConsistencyInfo"),
                Path.Combine(filepath, "EscapeFromTarkov_BE.exe"),
                Path.Combine(filepath, "Uninstall.exe"),
                Path.Combine(filepath, "UnityCrashHandler64.exe"),
                Path.Combine(filepath, "WinPixEventRuntime.dll")
            };

            foreach (string file in files)
            {
                if (Directory.Exists(file))
                {
                    try
                    {
                        Directory.Delete(file, true);
                    }
                    catch(Exception)
                    {
                        //something prevented the recursive deletion of the directory, try again.
                        try
                        {
                            Directory.Delete(file, true);
                        }
                        catch(Exception)
                        {
                            // *Shrug
                        }
                    }
                }

                if (File.Exists(file))
                {
                    File.Delete(file);
                }
            }
        }

        private int IsPiratedCopy()
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
				RegistryKey key = Registry.CurrentUser.OpenSubKey(registerySettings, true);

				foreach (string value in key.GetValueNames())
				{
					key.DeleteValue(value);
				}

                return true;
			}
			catch
			{
                return false;
			}
		}

        /// <summary>
        /// Clean the temp folder
        /// </summary>
        /// <returns>returns true if the temp folder was cleaned succefully or doesn't exist. returns false if something went wrong.</returns>
		public bool CleanTempFiles()
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(Path.Combine(Path.GetTempPath(), tempDir));

			if (!Directory.Exists(tempDir))
			{
				return true;
			}

            try
            {
                foreach (FileInfo file in directoryInfo.GetFiles())
                {
                    file.Delete();
                }

                foreach (DirectoryInfo directory in directoryInfo.GetDirectories())
                {
                    directory.Delete(true);
                }

                directoryInfo.Refresh();

                if(directoryInfo.GetFiles().Count() > 0 || directoryInfo.GetDirectories().Count() > 0)
                {
                    return false;
                }

                return true;
            }
            catch(Exception)
            {
                return false;
            }
		}
	}
}
