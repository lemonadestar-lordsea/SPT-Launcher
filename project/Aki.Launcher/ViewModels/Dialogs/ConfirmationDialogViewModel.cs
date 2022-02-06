using Aki.Launcher.Helpers;
using ReactiveUI;

namespace Aki.Launcher.ViewModels.Dialogs
{
    public class ConfirmationDialogViewModel : ViewModelBase
    {
        public string Question { get; set; }
        public string ConfirmButtonText { get; set; }
        public string DenyButtonText { get; set; }

        public ConfirmationDialogViewModel(IScreen Host, string Question, string? ConfirmButtonText = null, string? DenyButtonText = null) : base(Host)
        {
            this.Question = Question;
            this.ConfirmButtonText = ConfirmButtonText ?? LocalizationProvider.Instance.yes;
            this.DenyButtonText = DenyButtonText ?? LocalizationProvider.Instance.no;
        }

        public void ButtonCommand(object parameter)
        {
            bool result;

            if (parameter is string s && bool.TryParse(s, out result))
            {
                DialogHost.DialogHost.Close("mainHost", result);
                return;
            }

            DialogHost.DialogHost.Close("mainHost", false);
        }
    }
}
