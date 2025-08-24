using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using AssetStudio.GUI.Models.Panels;
using AssetStudio.GUI.ViewModels.Panels;
using Avalonia.Controls;
using Avalonia.Threading;

namespace AssetStudio.GUI.Views.Panels;

public partial class SceneHierarchyPanelView : UserControl
{
    public SceneHierarchyPanelView()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (DataContext is SceneHierarchyPanelViewModel viewModel)
        {
            viewModel.SceneHierarchy.CollectionChanged += OnSceneHierarchyChanged;
            viewModel.PropertyChanged += OnViewModelPropertyChanged;
        }
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(SceneHierarchyPanelViewModel.SceneHierarchy)) RefreshExpansionState();
    }

    private void OnSceneHierarchyChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        RefreshExpansionState();
    }

    private void RefreshExpansionState()
    {
        Dispatcher.UIThread.InvokeAsync(() => { ExpandTreeViewItems(); }, DispatcherPriority.Loaded);
    }

    private void ExpandTreeViewItems()
    {
        var treeView = this.FindControl<TreeView>("SceneTreeView");
        if (treeView?.ItemsSource == null) return;
        
        foreach (var item in treeView.ItemsSource.OfType<TreeNodeItem>())
            ExpandNodeRecursively(treeView, item);
    }

    private void ExpandNodeRecursively(TreeView treeView, TreeNodeItem node)
    {
        var container = treeView.TreeContainerFromItem(node);
        if (container is TreeViewItem treeViewItem && node.IsExpanded) treeViewItem.IsExpanded = true;

        foreach (var child in node.Children) ExpandNodeRecursively(treeView, child);
    }
}