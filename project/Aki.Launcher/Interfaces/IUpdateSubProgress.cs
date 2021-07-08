/* IUpdateSubProgress.cs
 * License: NCSA Open Source License
 * 
 * Copyright: Merijn Hendriks
 * AUTHORS:
 * waffle.lord
 */

using System;

namespace Aki.Launcher.Interfaces
{
    public interface IUpdateSubProgress
    {
        public event EventHandler<(int, string)> SubProgressChanged;
    }
}
