/* MiniDebug.cs
 * License: NCSA Open Source License
 * 
 * Copyright: Merijn Hendriks
 * AUTHORS:
 * waffle.lord
 */

using System.Collections.ObjectModel;

namespace Aki.Launcher.Helpers
{
    public static class MiniDebugHelper
    {
        /// <summary>
        /// Log a message to the Mini Debug Window.
        /// </summary>
        /// <param name="Message">The message to log</param>
        /// <remarks>This call is preferred to adding data directly to MiniLog, as it can be used while off the dispacter thread safely.</remarks>
        public static void LogMessage(string Message)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                Instance.MiniLog.Add(Message);
            });
        }
        public static MiniDebug Instance { get; private set; } = new MiniDebug();
    }

    public class MiniDebug
    {
        //Just set this to true to spawn the debug window. You can use MiniDebugHelper.LogMessage to send information to the debug window.
        //Do make sure to turn this back to false when you are done. You will also probably want to remove your LogMessage calls to keep things clean.
        public bool SpawnMiniDebugWindow = false;
        public ObservableCollection<string> MiniLog { get; set; }

        public MiniDebug()
        {
            ObservableCollection<string> tmpLog = new ObservableCollection<string>();
            MiniLog = tmpLog;
        }
    }
}
