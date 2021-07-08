/* IUpdateProgress
 * License: NCSA Open Source License
 * 
 * Copyright: Merijn Hendriks
 * AUTHORS:
 * waffle.lord
 */

using System;

namespace Aki.Launcher.Interfaces
{
    public interface IUpdateProgress
    {
        /// <summary>
        /// The task that will report progress.
        /// </summary>
        public Action ProgressableTask { get; }

        /// <summary>
        /// Cancel the ProgressableTask with a [string] reason.
        /// </summary>
        public event EventHandler<string> TaskCancelled;

        /// <summary>
        /// Update the progress of the task with an [int] percentage and an optional [string] message
        /// </summary>
        public event EventHandler<(int, string)> ProgressChanged;
    }
}
