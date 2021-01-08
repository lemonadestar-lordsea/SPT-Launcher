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
using Aki.Launcher.Helpers;

namespace Aki.Launcher
{
	public static class PatchManager
	{
        public static bool ApplyPatches()
        {
            string filepath = LauncherSettingsProvider.Instance.GamePath ?? Environment.CurrentDirectory;
            string targetFile = $@"{filepath}EscapeFromTarkov_Data/Managed/Assembly-CSharp.dll";
            string patchFile = $@"{filepath}Aki_Data/Launcher/Patches/aki-core.bpf";

            return PatchFile(targetFile, patchFile);
        }

        public static bool PatchFile(string targetFile, string patchFile)
        {            
            byte[] target = File.ReadAllBytes(targetFile);
            byte[] patch = File.ReadAllBytes(patchFile);
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
    }
}
