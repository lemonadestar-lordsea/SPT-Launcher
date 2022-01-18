using Aki.Launcher.ViewModels;
using System;

namespace Aki.Launcher.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public abstract class NavigationPreCondition : Attribute
    {
        public abstract bool TestPreCondition();
    }
}
