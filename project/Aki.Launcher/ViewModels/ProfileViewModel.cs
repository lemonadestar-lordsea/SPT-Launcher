using Aki.Launcher.Models.Launcher;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aki.Launcher.ViewModels
{
    public class ProfileViewModel : ViewModelBase
    {
        public ProfileViewModel(IScreen Host, LoginModel Login) : base(Host)
        {
        }
    }
}
