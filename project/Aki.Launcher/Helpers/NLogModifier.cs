/* NLogModifier.cs
 * License: NCSA Open Source License
 * 
 * Copyright: Merijn Hendriks
 * AUTHORS:
 * waffle.lord
 */

using Aki.Launcher.MiniCommon;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Aki.Launcher.Helpers
{
    public static class NLogModifier
    {
        public static string NLogConfigPath = VFS.Combine(LauncherSettingsProvider.Instance.GamePath, "NLog/NLog.config");
        public static string NLogConfigBakPath = NLogConfigPath + ".bak";

        private static bool ProcessFileOp(string Source, string Destination, bool Overwrite = false)
        {
            if (!VFS.Exists(Source))
            {
                return false;
            }

            if (VFS.Exists(Destination) && !Overwrite)
            {
                return true;
            }

            File.Copy(Source, Destination, true);

            if (!VFS.Exists(Destination))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Backup the NLog.config file
        /// </summary>
        /// <returns>Returns true if the NLog.config.bak file exists or the file was created; otherwise, returns false.</returns>
        /// <remarks><see cref="ModifyConfig"/> runs this method at the start of its execution</remarks>
        public static bool BackupConfig()
        {
            return ProcessFileOp(NLogConfigPath, NLogConfigBakPath);
        }

        /// <summary>
        /// Restores the NLog.config file from the backup
        /// </summary>
        /// <returns>Returns true if the restore process succeeds; otherwise, returns false.</returns>
        public static bool RestoreConfig()
        {
            return ProcessFileOp(NLogConfigBakPath, NLogConfigPath, true);
        }

        /// <summary>
        /// Modify the Nlog.config file to allow aki.loader to run
        /// </summary>
        /// <returns>Returns true if a backup config is present and the config was modified successfully; otherwise, returns false.</returns>
        /// <remarks>This method runs <see cref="BackupConfig"/> before trying to modify the NLog.config file</remarks>
        public static bool ModifyConfig()
        {
            //Backup config is not present or could not be made, abort.
            if (!BackupConfig()) return false;


            string akiloader = "<target name=\"Aki.Loader\"xsi:type=\"Aki.Loader\"/>";
            string nlogData = "";

            try
            {
                using (StreamReader sr = new StreamReader(NLogConfigPath))
                {
                    nlogData = sr.ReadToEnd();
                    sr.Close();
                }
            }
            catch(Exception)
            {
            }

            if (!nlogData.Contains(akiloader))
            {
                //if aki.loader does not exist in nlog.config, add it in.
                Match match = Regex.Match(nlogData, "<targets .+>");

                if (match.Success)
                {
                    string targetsNode = match.Value;
                    int index = nlogData.IndexOf(targetsNode);

                    if (index > -1)
                    {
                        nlogData = nlogData.Insert(index + targetsNode.Length, $"{Environment.NewLine}{akiloader}{Environment.NewLine}");
                    }
                }
            }


            if (Regex.Match(nlogData, "<logger name=\"\\*\"").Success)
            {
                //force debug logger level for all loggers
                nlogData = Regex.Replace(nlogData, "<logger name=\"\\*\" minlevel=\"\\w+\"", "<logger name=\"*\" minlevel=\"Debug\"");

                try
                {
                    using StreamWriter sw = new StreamWriter(NLogConfigPath, false);

                    sw.Write(nlogData);
                    sw.Close();
                }
                catch (Exception)
                {
                }
            }

            return true;
        }

    }
}
