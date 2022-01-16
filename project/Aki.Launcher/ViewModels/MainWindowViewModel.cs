using Avalonia;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Disposables;

namespace Aki.Launcher.ViewModels
{
    public class MainWindowViewModel : ReactiveObject, IActivatableViewModel, IScreen
    {
        public RoutingState Router { get; } = new RoutingState();
        public ViewModelActivator Activator { get; } = new ViewModelActivator();

        public ReactiveCommand<Unit, Unit> CloseCommand => ReactiveCommand.Create(() =>
        {
            if (Application.Current.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktopApp)
            {
                desktopApp.MainWindow.Close();
            }
        });

        public ReactiveCommand<Unit, Unit> MinimizeCommand => ReactiveCommand.Create(() =>
        {
            if (Application.Current.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktopApp)
            {
                desktopApp.MainWindow.WindowState = Avalonia.Controls.WindowState.Minimized;
            }
        });

        public MainWindowViewModel()
        {
            this.WhenActivated((CompositeDisposable disposables) =>
            {
                Router.Navigate.Execute(new ConnectServerViewModel(this));
            });
        }
    }
}
