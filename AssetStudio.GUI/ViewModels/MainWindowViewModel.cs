using CommunityToolkit.Mvvm.ComponentModel;
using Dock.Model.Core;

namespace AssetStudio.GUI.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly MainDockFactory _dockFactory;

    [ObservableProperty] private IDock? _dockLayout;

    [ObservableProperty] private bool _isProgressVisible;

    [ObservableProperty] private int _progressValue;

    [ObservableProperty] private string _statusText = "Ready";

    public MainWindowViewModel()
    {
        _dockFactory = new MainDockFactory();
        InitializeDockLayout();
    }

    private void InitializeDockLayout()
    {
        DockLayout = _dockFactory.CreateLayout();
        _dockFactory.InitLayout(DockLayout);
    }

    public void ResetLayout()
    {
        InitializeDockLayout();
    }
}