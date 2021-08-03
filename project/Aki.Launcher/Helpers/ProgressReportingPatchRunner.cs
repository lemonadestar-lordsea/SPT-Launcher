/* ProgressReportingPatchRunner.cs
 * License: NCSA Open Source License
 * 
 * Copyright: Merijn Hendriks
 * AUTHORS:
 * waffle.lord
 */

using Aki.Launcher.Interfaces;
using Aki.Launcher.MiniCommon;
using Aki.Launcher.Models.Launcher;
using System;
using System.Collections.Generic;
using System.IO;

namespace Aki.Launcher.Helpers
{
    public class ProgressReportingPatchRunner : IUpdateProgress, IUpdateSubProgress
    {
        private string GamePath = "";
        private List<string> Patches = new List<string>();
        private bool SetPatches = false;
        private bool IgnoreInputHashMismatch = false;

        public Action ProgressableTask => RunPatcher;

        private void RunPatcher()
        {
            if (!SetPatches)
            {
                Patches.AddRange(GetCorePatches());
                Patches.AddRange(GetModPatches());
            }

            if (Patches.Count <= 0)
            {
                //no patches to apply. Should probably raisetaskcancelled, but for now this is probably fine.
                RaiseProgressChanged(100, LocalizationProvider.Instance.ok);
            }

            try
            {
                FilePatcher.PatchProgress += FilePatcher_PatchProgress;

                PatchResultInfo result = PatchFiles();

                if (!result.OK)
                {
                    RaiseTaskCancelled(PatchResultInfo.FromError(result.Status, Patches.ToArray()));
                }
            }
            catch(Exception ex)
            {
                RaiseTaskCancelled(ex);
            }
            finally
            {
                //Static events can be super not cool if they don't get unsubscribed. Adding unsubscribe in finally to ensure class instance is collected by GC.
                FilePatcher.PatchProgress -= FilePatcher_PatchProgress;
            }
        }

        private PatchResultInfo PatchFiles()
        {
            FilePatcher.Restore(GamePath);

            int processed = 0;
            int countpatches = Patches.Count;

            var _patches = Patches.ToArray();
            foreach (var patch in _patches)
            {
                int percentage = (int)Math.Floor((double)processed / countpatches * 100);
                RaiseProgressChanged(percentage, LocalizationProvider.Instance.patching);

                PatchResultInfo result = FilePatcher.Run(GamePath, patch, IgnoreInputHashMismatch);

                if (!result.OK)
                {
                    return result;
                }

                Patches.Remove(patch);

                processed++;
            }

            RaiseProgressChanged(100, LocalizationProvider.Instance.ok);
            return PatchResultInfo.FromSuccess(ByteBanger.PatchResultType.Success);
        }

        private string[] GetCorePatches()
        {
            return VFS.GetDirectories(VFS.Combine(GamePath, "Aki_Data/Launcher/Patches/"));
        }

        private string[] GetModPatches()
        {
            var basepath = "user/mods/";

            if (!VFS.Exists(basepath))
            {
                return new string[0];
            }

            var result = new List<string>();
            var mods = VFS.GetDirectories(VFS.Combine(GamePath, basepath));

            foreach (var mod in mods)
            {
                var modPatch = VFS.Combine(GamePath, string.Format("{0}{1}/patches/", basepath, mod));

                if (VFS.Exists(modPatch))
                {
                    result.Add(modPatch);
                }
            }

            return result.ToArray();
        }

        #region EventHandlers
        private void FilePatcher_PatchProgress(object sender, ProgressInfo e)
        {
            //Update our progress dialog's sub progress bar whenever the file patcher updates it's progress
            RaiseSubProgressChanged(e.Percentage, e.Message);
        }

        //Change main progress bar
        public event EventHandler<ProgressInfo> ProgressChanged;
        protected virtual void RaiseProgressChanged(int Percentage, string Text)
        {
            ProgressChanged?.Invoke(this, new ProgressInfo(Percentage, Text));
        }

        public event EventHandler<object> TaskCancelled;
        protected virtual void RaiseTaskCancelled(object Data)
        {
            TaskCancelled?.Invoke(this, Data);
        }

        //Change sub progress bar
        public event EventHandler<ProgressInfo> SubProgressChanged;
        protected virtual void RaiseSubProgressChanged(int Percentage, string Message)
        {
            SubProgressChanged?.Invoke(this, new ProgressInfo(Percentage, Message));
        }
        #endregion

        public ProgressReportingPatchRunner(string GamePath, string[] Patches = null, bool IgnoreInputHashMismatch = false)
        {
            this.GamePath = GamePath;
            this.IgnoreInputHashMismatch = IgnoreInputHashMismatch;

            if(Patches != null && Patches.Length > 0)
            {
                this.Patches = new List<string>(Patches);
            }
        }
    }
}
