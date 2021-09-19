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

namespace Aki.Launcher.Helpers
{
    public class ProgressReportingPatchRunner : IUpdateProgress
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
            }

            if (Patches.Count <= 0)
            {
                //no patches to apply. Should probably raisetaskcancelled, but for now this is probably fine.
                RaiseProgressChanged(100, LocalizationProvider.Instance.ok);
            }

            try
            {
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

        #region EventHandlers
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
