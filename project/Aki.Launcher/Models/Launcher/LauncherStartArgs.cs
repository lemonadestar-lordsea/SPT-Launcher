/* LauncherStartArgs.cs
 * License: NCSA Open Source License
 * 
 * Copyright: Merijn Hendriks
 * AUTHORS:
 * waffle.lord
 */

using System;
using System.Windows;

namespace Aki.Launcher.Models.Launcher
{
    public class LauncherStartArgs
    {
        public string Username = String.Empty;
        public string Password = String.Empty;
        public bool HasAuthentication = false;

        public LauncherStartArgs(StartupEventArgs sea)
        {
            if(sea.Args.Length == 2)
            {
                if (sea.Args[0].StartsWith("username:"))
                {
                    Username = sea.Args[0].Replace("username:", "");
                }

                if (sea.Args[1].StartsWith("password:"))
                {
                    Password = sea.Args[1].Replace("password:", "");
                }

                if(!String.IsNullOrWhiteSpace(Username) && !String.IsNullOrWhiteSpace(Password))
                {
                    HasAuthentication = true;
                }
            }
        }
    }
}
