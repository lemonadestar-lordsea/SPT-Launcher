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
using Aki.Launcher.Models.Launcher;

namespace Aki.Launcher.Helpers
{
    public static class FilePatcher
    {
        public static event EventHandler<ProgressInfo> PatchProgress;
        private static void RaisePatchProgress(int Percentage, string Message)
        {
            PatchProgress?.Invoke(null, new ProgressInfo(Percentage, Message));
        }

        public static PatchResultInfo Patch(string targetfile, string patchfile, bool IgnoreInputHashMismatch = false)
        {
            byte[] target = VFS.ReadFile(targetfile);
            byte[] patch = VFS.ReadFile(patchfile);

            PatchResult result = PatchUtil.Patch(target, PatchInfo.FromBytes(patch));

            switch (result.Result)
            {
                case PatchResultType.Success:
                    File.Copy(targetfile, $"{targetfile}.bak");
                    VFS.WriteFile(targetfile, result.PatchedData);
                    return PatchResultInfo.FromSuccess(result.Result);

                case PatchResultType.AlreadyPatched:
                    return PatchResultInfo.FromSuccess(result.Result);

                case PatchResultType.InputChecksumMismatch:
                    if (IgnoreInputHashMismatch) return PatchResultInfo.FromSuccess(result.Result);
                    return PatchResultInfo.FromError(result.Result);

                default:
                    return PatchResultInfo.FromError(result.Result);
            }
        }

        private static PatchResultInfo PatchAll(string targetpath, string patchpath, bool IgnoreInputHashMismatch = false)
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

                            PatchResultInfo result = Patch(targetpath, patchpath, IgnoreInputHashMismatch);

                            if (!result.OK)
                            {
                                // patch failed
                                return result;
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

            return PatchResultInfo.FromSuccess(PatchResultType.Success);
        }

        public static PatchResultInfo Run(string targetPath, string patchPath, bool IgnoreInputHashMismatch = false)
        {
            return PatchAll(targetPath, patchPath, IgnoreInputHashMismatch);
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