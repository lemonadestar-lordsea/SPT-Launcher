/* ProgressReportingPatchRunner.cs
 * License: NCSA Open Source License
 * 
 * Copyright: Merijn Hendriks
 * AUTHORS:
 * waffle.lord
 */

using Aki.Launcher.Interfaces;
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
            catch(Exception ex)
            {
            }
            finally
            {
                //Static events can be super not cool if they don't get unsubscribed. Adding unsubscribe in finally to ensure class instance is collected by GC.
                PatchManager.PatchProgress -= PatchManager_PatchProgress;
                FilePatcher.PatchProgress -= FilePatcher_PatchProgress;
            }
        }

        private void FilePatcher_PatchProgress(object sender, (int, string) e)
        {
            RaiseSubProcessChanged(e.Item1, e.Item2);
        }

        private void PatchManager_PatchProgress(object sender, (int, string) e)
        {
            RaiseProgressChanged(e.Item1, e.Item2);
        }

        public event EventHandler<(int, string)> ProgressChanged;
        protected virtual void RaiseProgressChanged(int Percentage, string Text)
        {
            ProgressChanged?.Invoke(this, (Percentage, Text));
        }

        public event EventHandler<string> TaskCancelled;
        protected virtual void RaiseTaskCancelled(string Reason)
        {
            TaskCancelled?.Invoke(this, Reason);
        }

        public event EventHandler<(int, string)> SubProgressChanged;
        protected virtual void RaiseSubProcessChanged(int Percentage, string Message)
        {
            SubProgressChanged?.Invoke(this, (Percentage, Message));
        }

        public ProgressReportingPatchRunner(string GamePath)
        {
            this.GamePath = GamePath;
        }
    }
}
