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
        public Action ProgressableTask { get; }

        public event EventHandler<(int, string)> ProgressChanged;
    }
}
