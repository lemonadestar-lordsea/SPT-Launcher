/* ProgressDialog.xaml.cs
 * License: NCSA Open Source License
 * 
 * Copyright: Merijn Hendriks
 * AUTHORS:
 * waffle.lord
 */

using Aki.Launcher.Interfaces;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Aki.Launcher.Custom_Controls.Dialogs
{
    /// <summary>
    /// Interaction logic for ProgressDialog.xaml
    /// </summary>
    public partial class ProgressDialog : UserControl, IReturnDialogResult
    {
        IUpdateProgress TaskInfo;

        public ProgressDialog(IUpdateProgress TaskInfo)
        {
            InitializeComponent();

            this.TaskInfo = TaskInfo;
            TaskInfo.ProgressChanged += TaskInfo_ProgressChanged;
        }

        private void TaskInfo_ProgressChanged(object sender, (int, string) e)
        {
            //Item 1 is progress, Item2 is the message to show (can be empty to not update the message)
            UpdateProgress(e.Item1, e.Item2);
        }

        /// <summary>
        /// Updates the progress dialog
        /// </summary>
        /// <param name="Percentage">The percentage the progress bar should display</param>
        /// <param name="NewMessage">The text to display in the dialog</param>
        private void UpdateProgress(int Percentage, string NewMessage = "")
        {
            //update progress bar (optionally update the message)
            if (!String.IsNullOrWhiteSpace(NewMessage)) Application.Current.Dispatcher.Invoke(() => { InfoText_Label.Content = NewMessage; });

            Application.Current.Dispatcher.Invoke(() => { Progress_Bar.Value = Percentage; });

            //if the percentage is 100%, return our dialog results (to close the dialog)
            if (Percentage >= 100)
            {
                RaiseResultsReady(null);
            }
        }

        /// <summary>
        /// Set the progress bar's indeterminate value
        /// </summary>
        /// <param name="IsIndeterminate"></param>
        public void UpdateIndeterminateProgress(bool IsIndeterminate) => Application.Current.Dispatcher.Invoke(() => { Progress_Bar.IsIndeterminate = IsIndeterminate; });

        public event EventHandler<object> ResultsReady;

        protected virtual void RaiseResultsReady(object results)
        {
            ResultsReady?.Invoke(this, results);
        }

        private void ProgressDialog_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Task.Factory.StartNew(TaskInfo.ProgressableTask);
        }
    }
}
