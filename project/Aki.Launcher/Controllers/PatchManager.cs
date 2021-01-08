/* PatchManager.cs
 * License: NCSA Open Source License
 * 
 * Copyright: Merijn Hendriks
 * AUTHORS:
 * Merijn Hendriks
 */


using System;
using System.IO;
using Aki.ByteBanger;

namespace Aki.Launcher
{
	public static class PatchManager
	{
        public static int ApplyPatches(string filepath)
        {
            var patches = RequestPatches();

            // get patches from server
            if (patches == null)
            {
                return -1;
            }

            // patch from clean files
            RestorePatched(filepath);

            // apply core patch
            if (!ApplyCorePatch(filepath))
            {
                return -2;
            }

            // apply mod patches
            /*
            for (var patch in patches)
            {
                if (!PatchFile(filepath))
                {
                    return -3;
                }
            }
            */

            return 1;
        }

        static PatchInfo[] RequestPatches()
        {
            // TODO: get patch files
            return new PatchInfo[1];
        }

        static bool ApplyCorePatch(string filepath)
        {
            var targetFile = $@"{filepath}EscapeFromTarkov_Data/Managed/Assembly-CSharp.dll";
            var patchFile = $@"{filepath}Aki_Data/Launcher/Patches/aki-core.bpf";

            return PatchFile(targetFile, patchFile);
        }

        static bool PatchFile(string targetFile, string patchFile)
        {            
            var target = File.ReadAllBytes(targetFile);
            var patch = File.ReadAllBytes(patchFile);
            byte[] patched = null;

            // backup before patching
            if (!File.Exists($@"{targetFile}.bak"))
            {
                File.Copy(targetFile, $@"{targetFile}.bak");
            }

            // patch
            try
            {
                patched = PatchUtil.Patch(target, PatchInfo.FromBytes(patch));
                
            }
            catch (Exception ex)
            {
                if (ex.Message == "Invalid input file")
                {
                    // already patched
                    return true;
                }

                if (ex.Message == "Output hash mismatch")
                {
                    // version mismatch
                    return false;
                }
            }

            // apply patch
            File.WriteAllBytes(targetFile, patched);
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
