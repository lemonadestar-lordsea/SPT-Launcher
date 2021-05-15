/* DebugWindow.xaml.cs
 * License: NCSA Open Source License
 * 
 * Copyright: Merijn Hendriks
 * AUTHORS:
 * waffle.lord
 */

using System.Windows;

namespace Aki.Launcher
{
    /// <summary>
    /// Interaction logic for DebugWindow.xaml
    /// </summary>
    public partial class MiniDebugWindow : Window
    {
        public MiniDebugWindow()
        {
            InitializeComponent();
        }


        //I'm lazy and just ripped this from here: https://stackoverflow.com/a/19315242
        private bool AutoScroll = true;
        private void sv_ScrollChanged(object sender, System.Windows.Controls.ScrollChangedEventArgs e)
        {
            // User scroll event : set or unset auto-scroll mode
            if (e.ExtentHeightChange == 0)
            {   // Content unchanged : user scroll event
                if (sv.VerticalOffset == sv.ScrollableHeight)
                {   // Scroll bar is in bottom
                    // Set auto-scroll mode
                    AutoScroll = true;
                }
                else
                {   // Scroll bar isn't in bottom
                    // Unset auto-scroll mode
                    AutoScroll = false;
                }
            }

            // Content scroll event : auto-scroll eventually
            if (AutoScroll && e.ExtentHeightChange != 0)
            {   // Content changed and auto-scroll mode set
                // Autoscroll
                sv.ScrollToVerticalOffset(sv.ExtentHeight);
            }
        }
    }
}
