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
            var core = VFS.GetDirectories(VFS.Combine(filepath, "Aki_Data/Launcher/Patches/"));
            var mods = VFS.GetDirectories(VFS.Combine(filepath, "user/mods/"));
            var patches = new List<string>();

            // delete all previous applied patches
            RestorePatched(filepath);

            // get patches to apply
            patches.AddRange(core);

            foreach (var mod in mods)
            {
                var modPatch = VFS.Combine(filepath, string.Format("user/mods/{0}/patches/", mod));

                if (VFS.Exists(modPatch))
                {
                    patches.Add(modPatch);
                }
            }

            // apply patches
            foreach (var patch in patches)
            {
                if (!FilePatcher.Run(filepath, patch))
                {
                    return false;
                }
            }

            return true;
        }

        public static void RestorePatched(string filepath)
        {
            RestorePatchedRecurse(new DirectoryInfo(filepath));
        }

        static void RestorePatchedRecurse(DirectoryInfo basedir)
        {
            // scan subdirectories
            foreach (var dir in basedir.EnumerateDirectories())
            {
                RestorePatchedRecurse(dir);
            }

            // scan files
            var files = basedir.GetFiles();

            foreach (var file in files)
            {
                if (file.Extension == ".bak")
                {
                    var target = Path.ChangeExtension(file.FullName, null);

                    // remove patched file
                    var patched = new FileInfo(target);
                    patched.IsReadOnly = false;
                    patched.Delete();

                    // restore from backup
                    File.Copy(file.FullName, target);
                    file.IsReadOnly = false;
                    file.Delete();
                }
            }
        }
    }
}
