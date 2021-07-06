/* PatchManager.cs
 * License: NCSA Open Source License
 * 
 * Copyright: Merijn Hendriks
 * AUTHORS:
 * Merijn Hendriks
 */

using System.Collections.Generic;
using System.IO;
using Aki.Launcher.Helpers;
using Aki.Launcher.MiniCommon;

namespace Aki.Launcher
{
    public static class PatchManager
    {
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

            foreach (var patch in patches)
            {
                if (!FilePatcher.Run(filepath, patch))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
