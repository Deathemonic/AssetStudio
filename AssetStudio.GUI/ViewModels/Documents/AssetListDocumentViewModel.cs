using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using AssetStudio.GUI.Logic;
using AssetStudio.GUI.Models.Documents;
using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using Dock.Model.Mvvm.Controls;

namespace AssetStudio.GUI.ViewModels.Documents;

public partial class AssetListDocumentViewModel : Document
{
    [ObservableProperty] private DataGridCollectionView? _collectionView;
    [ObservableProperty] private string _searchText = string.Empty;
    [ObservableProperty] private int _selectedIncludeExcludeMode;
    [ObservableProperty] private ObservableCollection<AssetItem> _selectedItems = [];
    [ObservableProperty] private int _selectedSearchFilterMode;
    [ObservableProperty] private AssetItem? _selectedAsset;

    public event Action<AssetItem>? AssetSelected;

    public AssetListDocumentViewModel()
    {
        Id = "AssetList";
        Title = "Assets";
        CanClose = false;

        InitializeAssetTypeFilters();
        RefreshView();

        PropertyChanged += OnPropertyChanged;
    }

    public ObservableCollection<FilterAssetType> AssetTypeFilters { get; } = [];
    private ObservableCollection<AssetItem> Assets { get; } = [];

    partial void OnSelectedAssetChanged(AssetItem? value)
    {
        if (value != null)
        {
            AssetSelected?.Invoke(value);
        }
    }

    public void UpdateData(List<AssetItem> newAssets)
    {
        Assets.Clear();
        foreach (var asset in newAssets)
        {
            Assets.Add(asset);
        }

        InitializeAssetTypeFilters();
        RefreshView();
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(SearchText):
            case nameof(SelectedSearchFilterMode):
            case nameof(SelectedIncludeExcludeMode):
                RefreshView();
                break;
        }
    }


    private void RefreshView()
    {
        var selectedTypes = AssetTypeFilters.Where(x => x.IsSelected).Select(x => x.Name);
        var filteredAssets = AssetSearch.PerformCompleteSearch(
            Assets,
            SearchText,
            (SearchMethod)SelectedSearchFilterMode,
            (IncludeExcludeMode)SelectedIncludeExcludeMode,
            selectedTypes);

        CollectionView = new DataGridCollectionView(new ObservableCollection<AssetItem>(filteredAssets));
    }

    private void InitializeAssetTypeFilters()
    {
        foreach (var filter in AssetTypeFilters)
            filter.PropertyChanged -= OnAssetTypeSelectionChanged;

        AssetTypeFilters.Clear();

        var allType = new FilterAssetType { Name = "All", IsSelected = true };
        allType.PropertyChanged += OnAssetTypeSelectionChanged;
        AssetTypeFilters.Add(allType);

        var uniqueTypes = Assets
            .Select(asset => asset.Type)
            .Where(type => !string.IsNullOrEmpty(type))
            .Distinct()
            .OrderBy(type => type);

        foreach (var type in uniqueTypes)
        {
            var filterType = new FilterAssetType { Name = type, IsSelected = false };
            filterType.PropertyChanged += OnAssetTypeSelectionChanged;
            AssetTypeFilters.Add(filterType);
        }
    }

    private void OnAssetTypeSelectionChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(FilterAssetType.IsSelected) ||
            sender is not FilterAssetType changedType ||
            !changedType.IsSelected)
        {
            RefreshView();
            return;
        }

        HandleMutualExclusiveSelection(changedType);
        RefreshView();
    }

    private void HandleMutualExclusiveSelection(FilterAssetType changedType)
    {
        if (changedType.Name == "All")
            SetTypesSelection(type => type.Name != "All", false);
        else
            SetTypesSelection(type => type.Name == "All", false);
    }

    private void SetTypesSelection(Func<FilterAssetType, bool> predicate, bool isSelected)
    {
        var typesToUpdate = AssetTypeFilters.Where(predicate).Where(t => t.IsSelected != isSelected).ToList();

        foreach (var type in typesToUpdate)
        {
            type.PropertyChanged -= OnAssetTypeSelectionChanged;
            type.IsSelected = isSelected;
            type.PropertyChanged += OnAssetTypeSelectionChanged;
        }
    }
}