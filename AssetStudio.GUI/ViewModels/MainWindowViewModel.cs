using CommunityToolkit.Mvvm.ComponentModel;
using Dock.Model.Core;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private int _selectedFilterMethod = 0;

    [ObservableProperty]
    private int _selectedIncludeExclude = 0;

    [ObservableProperty]
    private int _selectedAssetTypeFilter = 0;

    public ObservableCollection<string> AvailableAssetTypes { get; } = new()
    {
        "All"
    };

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

    public void UpdateAvailableAssetTypes(IEnumerable<string> assetTypes)
    {
        AvailableAssetTypes.Clear();
        AvailableAssetTypes.Add("All");
        
        foreach (var assetType in assetTypes.Distinct().OrderBy(t => t))
        {
            if (!string.IsNullOrEmpty(assetType) && assetType != "All")
            {
                AvailableAssetTypes.Add(assetType);
            }
        }
    }
}
