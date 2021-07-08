/* PatchManager.cs
 * License: NCSA Open Source License
 * 
 * Copyright: Merijn Hendriks
 * AUTHORS:
 * Merijn Hendriks
 */

using System;
using System.Collections.Generic;
using Aki.Launcher.Helpers;
using Aki.Launcher.MiniCommon;

namespace Aki.Launcher
{
    public static class PatchManager
    {
        public static event EventHandler<(int, string)> PatchProgress;
        private static void RaisePatchProgressChanged(int Percenatage, string Text)
        {
            PatchProgress?.Invoke(null, (Percenatage, Text));
        }

        public static bool ApplyPatches(string filepath)
        {
            var patches = new List<string>();
            patches.AddRange(GetCorePatches(filepath));
            patches.AddRange(GetModPatches(filepath));
            return PatchFiles(filepath, patches.ToArray());
        }

        public static string[] GetCorePatches(string filepath)
        {
            return VFS.GetDirectories(VFS.Combine(filepath, "Aki_Data/Launcher/Patches/"));
        }

        public static string[] GetModPatches(string filepath)
        {
            var basepath = "user/mods/";
            
            if (!VFS.Exists(basepath))
            {
                return new string[0];
            }

            var result = new List<string>();
            var mods = VFS.GetDirectories(VFS.Combine(filepath, basepath));

            foreach (var mod in mods)
            {
                var modPatch = VFS.Combine(filepath, string.Format("{0}{1}/patches/", basepath, mod));

                if (VFS.Exists(modPatch))
                {
                    result.Add(modPatch);
                }
            }

            return result.ToArray();
        }

        public static bool PatchFiles(string filepath, string[] patches)
        {
            FilePatcher.Restore(filepath);

            int processed = 0;

            foreach (var patch in patches)
            {
                int percentage = (int)Math.Floor((double)processed / patches.Length * 100);
                RaisePatchProgressChanged(percentage, LocalizationProvider.Instance.patching);

                if (!FilePatcher.Run(filepath, patch))
                {
                    return false;
                }
            }

            RaisePatchProgressChanged(100, LocalizationProvider.Instance.ok);
            return true;
        }
    }
}
