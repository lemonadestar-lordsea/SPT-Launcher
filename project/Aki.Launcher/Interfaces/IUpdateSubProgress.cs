using System;

namespace Aki.Launcher.Interfaces
{
    public interface IUpdateSubProgress
    {
        public event EventHandler<(int, string)> SubProgressChanged;
    }
}
