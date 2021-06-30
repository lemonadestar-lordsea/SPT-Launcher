using System.IO;
using Aki.ByteBanger;
using Aki.Launcher.MiniCommon;

namespace Aki.Launcher.Helpers
{
    public static class FilePatcher
    {
        public static bool Patch(string targetfile, string patchfile)
        {
            byte[] target = VFS.ReadFile(targetfile);
            byte[] patch = VFS.ReadFile(patchfile);

            PatchResult result = PatchUtil.Patch(target, PatchInfo.FromBytes(patch));

            switch (result.Result)
            {
                case PatchResultType.Success:
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

            foreach (FileInfo file in di.GetFiles())
            {
                FileInfo target;

                switch (file.Extension)
                {
                    // patch
                    case ".bpf":
                        {
                            target = new FileInfo(VFS.Combine(targetpath, file.Name.Replace(".bpf", "")));

                            if (!Patch(target.FullName, file.FullName))
                            {
                                // patch failed
                                return false;
                            }
                        }
                        break;

                    // add new files
                    case ".new":
                        {
                            target = new FileInfo(VFS.Combine(targetpath, file.Name.Replace(".new", "")));
                            VFS.WriteFile(target.FullName, Zlib.Decompress(VFS.ReadFile(file.FullName)));
                        }
                        break;

                    // delete old files
                    case ".del":
                        {
                            target = new FileInfo(VFS.Combine(targetpath, file.Name.Replace(".del", "")));
                            target.IsReadOnly = false;
                            target.Delete();
                        }
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

            return true;
        }

        public static bool Run(string targetPath, string patchPath)
        {
            return PatchAll(targetPath, patchPath);
        }
    }
}