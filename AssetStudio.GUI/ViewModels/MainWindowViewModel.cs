using CommunityToolkit.Mvvm.ComponentModel;
using Dock.Model.Core;
using System.Collections.Generic;
using System.Linq;

namespace AssetStudio.GUI.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _statusText = "Ready";

    [ObservableProperty]
    private bool _isProgressVisible = false;

    [ObservableProperty]
    private int _progressValue = 0;

    [ObservableProperty]
    private IDock? _dockLayout;

    private readonly MainDockFactory _dockFactory;

    public MainWindowViewModel()
    {
        _dockFactory = new MainDockFactory(this);
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
