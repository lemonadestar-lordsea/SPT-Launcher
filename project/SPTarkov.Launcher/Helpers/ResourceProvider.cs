/* ResourceProvider.cs
 * License: NCSA Open Source License
 * 
 * Copyright: Merijn Hendriks
 * AUTHORS:
 * waffle.lord
 */


using System;

namespace SPTarkov.Launcher.Helpers
{
    //Only really do it this way incase we want to extend this later. No idea why we would want to, but who knows *shrug.
    public static class ResourceProvider
    {
        public static string DefaultImagesFolderPath = $"{Environment.CurrentDirectory}\\Launcher_Data\\Images";
        public static string BackgroundImagePath { get; } = $"{DefaultImagesFolderPath}\\bg.png";
    }
}
