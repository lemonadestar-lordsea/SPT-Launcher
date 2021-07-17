/* PatchResultInfo.cs
 * License: NCSA Open Source License
 * 
 * Copyright: Merijn Hendriks
 * AUTHORS:
 * waffle.lord
 */

using Aki.ByteBanger;

namespace Aki.Launcher.Models.Launcher
{
    public class PatchResultInfo
    {
        public PatchResultType Status { get; }
        public bool OK { get; }

        public bool HasRemainingPatches => RemainingPatches.Length > 0;
        public string[] RemainingPatches { get; }

        protected PatchResultInfo(PatchResultType Status, bool OK, string[] RemainingPatches)
        {
            this.Status = Status;
            this.OK = OK;
            this.RemainingPatches = RemainingPatches;
        }

        public static PatchResultInfo FromSuccess(PatchResultType Status) => new PatchResultInfo(Status, true, null);
        public static PatchResultInfo FromError(PatchResultType Status, string[] RemainingPatches = null) => new PatchResultInfo(Status, false, RemainingPatches);
    }
}
