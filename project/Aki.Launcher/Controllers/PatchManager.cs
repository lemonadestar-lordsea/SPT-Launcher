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
        public static bool ApplyPatches(string filepath)
        {
            var targetfile = $@"{filepath}EscapeFromTarkov_Data/Managed/Assembly-CSharp.dll";
            var files = new DirectoryInfo($@"{filepath}Aki_Data/Launcher/Patches/").GetFiles();

            foreach (var file in files)
            {
                // patch from clean files
                RestorePatched(filepath);

                // apply patch
                if (ApplyPatch(targetfile, file.FullName))
                {
                    // game patch found
                    return true;
                }
            }

            // patch failed
            return false;
        }

        static bool ApplyPatch(string targetfile, string patchfile)
        {
            byte[] target = File.ReadAllBytes(targetfile);
            byte[] patch = File.ReadAllBytes(patchfile);

            // backup before patching
            if (!File.Exists($@"{targetfile}.bak"))
            {
                File.Copy(targetfile, $@"{targetfile}.bak");
            }

            // patch
            PatchResult result = PatchUtil.Patch(target, PatchInfo.FromBytes(patch));

            switch (result.Result)
            {
                case PatchResultType.Success:                   // successfully patched, write the file
                    File.WriteAllBytes(targetfile, result.PatchedData);
                    return true;

                case PatchResultType.AlreadyPatched:            // input file is already patched (size & hash match)
                case PatchResultType.InputChecksumMismatch:     // input file's hash mismatches
                case PatchResultType.InputLengthMismatch:       // input file's length mismatches
                    return true;

                case PatchResultType.OutputChecksumMismatch:    // patched hash mismatches intended output hash
                    return false;

                default:                                        // something else that shouldn't happen
                    return false;
            }
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
