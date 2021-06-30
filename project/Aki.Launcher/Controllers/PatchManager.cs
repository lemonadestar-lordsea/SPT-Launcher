/* PatchManager.cs
 * License: NCSA Open Source License
 * 
 * Copyright: Merijn Hendriks
 * AUTHORS:
 * Merijn Hendriks
 */

using System.IO;
using Aki.Launcher.Helpers;
using Aki.Launcher.MiniCommon;

namespace Aki.Launcher
{
    public static class PatchManager
    {
        public static bool ApplyPatches(string filepath)
        {
            var modsPath = VFS.Combine(filepath, "Aki_Data/Launcher/Patches/");
            var mods = VFS.GetDirectories(modsPath);

            RestorePatched(filepath);

            foreach (var mod in mods)
            {
                if (!FilePatcher.Run(filepath, VFS.Combine(modsPath, mod)))
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
