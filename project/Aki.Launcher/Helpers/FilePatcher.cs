/* FilePatcher.cs
 * License: NCSA Open Source License
 * 
 * Copyright: Merijn Hendriks
 * AUTHORS:
 * Merijn Hendriks
 * waffle.lord
 */

using System;
using System.IO;
using Aki.ByteBanger;
using Aki.Launcher.MiniCommon;

namespace Aki.Launcher.Helpers
{
    public static class FilePatcher
    {
        public static event EventHandler<(int, string)> PatchProgress;
        private static void RaisePatchProgress(int Percentage, string Message)
        {
            PatchProgress?.Invoke(null, (Percentage, Message));
        }

        public static bool Patch(string targetfile, string patchfile)
        {
            byte[] target = VFS.ReadFile(targetfile);
            byte[] patch = VFS.ReadFile(patchfile);

            PatchResult result = PatchUtil.Patch(target, PatchInfo.FromBytes(patch));

            switch (result.Result)
            {
                case PatchResultType.Success:
                    File.Copy(file.FullName, $"{file.FullName}.bak");
                    VFS.WriteFile(targetfile, result.PatchedData);
                    return true;

                case PatchResultType.AlreadyPatched:
                case PatchResultType.InputChecksumMismatch:
                case PatchResultType.InputLengthMismatch:
                    return true;

                case PatchResultType.OutputChecksumMismatch:
                default:
                    return false;
            }
        }

        private static bool PatchAll(string targetpath, string patchpath)
        {
            DirectoryInfo di = new DirectoryInfo(patchpath);

            //TODO - Simply file iteration
            //I feel like we could simplify this to not iterate over all the files, but I'm having a brain fart and can't think.
            //Basically, the only issue is we need to be able to get the target file path. I'll look at this again later if no one else does. Brain is being dumb... -waffle
            int countfiles = di.GetFiles("*.bpf", SearchOption.AllDirectories).Length;

            int processed = 0;

            foreach (FileInfo file in di.GetFiles())
            {
                FileInfo target;

                switch (file.Extension)
                {
                    // patch
                    case ".bpf":
                        {
                            int progress = (int)Math.Floor((double)processed / countfiles * 100);
                            RaisePatchProgress(progress, $"{LocalizationProvider.Instance.patching} {file.Name} ...");

                            target = new FileInfo(VFS.Combine(targetpath, file.Name.Replace(".bpf", "")));

                            if (!Patch(target.FullName, file.FullName))
                            {
                                // patch failed
                                return false;
                            }

                            processed++;
                        }
                        break;

                    default:
                        break;
                }
            }

            foreach (DirectoryInfo directory in di.GetDirectories())
            {
                PatchAll(VFS.Combine(targetpath, directory.Name), directory.FullName);
            }

            di.Refresh();

            if (di.GetFiles().Length == 0 && di.GetDirectories().Length == 0)
            {
                // remove empty folders
                di.Delete();
            }

            RaisePatchProgress(100, LocalizationProvider.Instance.ok);

            return true;
        }

        public static bool Run(string targetPath, string patchPath)
        {
            return PatchAll(targetPath, patchPath);
        }

        public static void Restore(string filepath)
        {
            RestoreRecurse(new DirectoryInfo(filepath));
        }

        static void RestoreRecurse(DirectoryInfo basedir)
        {
            // scan subdirectories
            foreach (var dir in basedir.EnumerateDirectories())
            {
                RestoreRecurse(dir);
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