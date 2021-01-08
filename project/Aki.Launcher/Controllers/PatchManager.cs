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
    public enum PatchStatus
    {
        Success = 0,
        NoPatchReceived,
        FailedCorePatch,
        FailedModPatch
    }

    public static class PatchManager
    {
        public static PatchStatus ApplyPatches(string filepath)
        {
            var patches = RequestPatches();

            // get patches from server
            if (patches == null)
            {
                return PatchStatus.NoPatchReceived;
            }

            // patch from clean files
            RestorePatched(filepath);

            // apply core patch
            if (!ApplyCorePatch(filepath))
            {
                return PatchStatus.FailedCorePatch;
            }

            // apply mod patches
            /*
            for (var patch in patches)
            {
                if (!PatchFile(filepath))
                {
                    return PatchStatus.FailedModPatch;
                }
            }
            */

            return PatchStatus.Success;
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
            byte[] target = File.ReadAllBytes(targetFile);
            byte[] patch = File.ReadAllBytes(patchFile);

            // backup before patching
            if (!File.Exists($@"{targetFile}.bak"))
            {
                File.Copy(targetFile, $@"{targetFile}.bak");
            }

            // patch
            PatchResult result = PatchUtil.Patch(target, PatchInfo.FromBytes(patch));

            switch (result.Result)
            {
                case PatchResultType.Success:   // successfully patched, write the file
                    File.WriteAllBytes(targetFile, result.PatchedData);
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
