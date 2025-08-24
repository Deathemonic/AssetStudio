using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AssetStudio.GUI.Logic;
using AssetStudio.GUI.Models.Panels;
using CommunityToolkit.Mvvm.ComponentModel;
using Dock.Model.Mvvm.Controls;

namespace AssetStudio.GUI.ViewModels.Panels;

public partial class SceneHierarchyPanelViewModel : Tool
{
    [ObservableProperty] private string _searchText = string.Empty;

    public SceneHierarchyPanelViewModel()
    {
        Id = "SceneHierarchy";
        Title = "Scene Hierarchy";
        CanClose = false;

        PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(SearchText)) PerformSearch();
        };
    }

    public ObservableCollection<TreeNodeItem> SceneHierarchy { get; } = [];

    public void UpdateData(List<TreeNodeItem> newHierarchy)
    {
        SceneHierarchy.Clear();
        foreach (var item in newHierarchy)
        {
            SceneHierarchy.Add(item);
        }
    }


    private void PerformSearch()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            CollapseAllNodes(SceneHierarchy);
            return;
        }

        try
        {
            TreeNodeSearch.ExpandToMatches(SceneHierarchy, SearchText);
            OnPropertyChanged(nameof(SceneHierarchy));
        }
        catch (Exception)
        {
            // Continue
        }
    }

    private void CollapseAllNodes(IEnumerable<TreeNodeItem> nodes)
    {
        foreach (var node in nodes)
        {
            node.IsExpanded = false;
            if (node.Children.Any()) CollapseAllNodes(node.Children);
        }
    }
}