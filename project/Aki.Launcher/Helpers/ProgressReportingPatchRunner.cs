/* ProgressReportingPatchRunner.cs
 * License: NCSA Open Source License
 * 
 * Copyright: Merijn Hendriks
 * AUTHORS:
 * waffle.lord
 */

using Aki.Launcher.Interfaces;
using Aki.Launcher.Models.Launcher;
using System;

namespace Aki.Launcher.Helpers
{
    public class ProgressReportingPatchRunner : IUpdateProgress, IUpdateSubProgress
    {
        private string GamePath = "";

        public Action ProgressableTask => RunPatcher;

        private void RunPatcher()
        {
            try
            {
                FilePatcher.PatchProgress += FilePatcher_PatchProgress;
                PatchManager.PatchProgress += PatchManager_PatchProgress;
                if (!PatchManager.ApplyPatches(GamePath))
                {
                    RaiseTaskCancelled("Patching Failed"); // the reason doesn't really matter in this case since gamestarter returns a result of it's own.
                }
            }
            catch
            {
            }
            finally
            {
                //Static events can be super not cool if they don't get unsubscribed. Adding unsubscribe in finally to ensure class instance is collected by GC.
                PatchManager.PatchProgress -= PatchManager_PatchProgress;
                FilePatcher.PatchProgress -= FilePatcher_PatchProgress;
            }
        }

        private void FilePatcher_PatchProgress(object sender, ProgressInfo e)
        {
            //Update our progress dialog's sub progress bar whenever the file patcher updates it's progress
            RaiseSubProgressChanged(e.Percentage, e.Message);
        }

        private void PatchManager_PatchProgress(object sender, ProgressInfo e)
        {
            //update our progress dialog's main progress bar whenever the patch manager updates it's progress
            RaiseProgressChanged(e.Percentage, e.Message);
        }

        //Change main progress bar
        public event EventHandler<ProgressInfo> ProgressChanged;
        protected virtual void RaiseProgressChanged(int Percentage, string Text)
        {
            ProgressChanged?.Invoke(this, new ProgressInfo(Percentage, Text));
        }

        public event EventHandler<string> TaskCancelled;
        protected virtual void RaiseTaskCancelled(string Reason)
        {
            TaskCancelled?.Invoke(this, Reason);
        }

        //Change sub progress bar
        public event EventHandler<ProgressInfo> SubProgressChanged;
        protected virtual void RaiseSubProgressChanged(int Percentage, string Message)
        {
            SubProgressChanged?.Invoke(this, new ProgressInfo(Percentage, Message));
        }

        public ProgressReportingPatchRunner(string GamePath)
        {
            this.GamePath = GamePath;
        }
    }
}
