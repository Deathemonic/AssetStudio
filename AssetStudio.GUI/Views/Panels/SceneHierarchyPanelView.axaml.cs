using Avalonia.Controls;
using AssetStudio.GUI.ViewModels.Panels;
using AssetStudio.GUI.Models.Panels;
using System.Linq;

namespace AssetStudio.GUI.Views.Panels;

public partial class SceneHierarchyPanelView : UserControl
{
    public SceneHierarchyPanelView()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }
    
    private void OnDataContextChanged(object? sender, System.EventArgs e)
    {
        if (DataContext is SceneHierarchyPanelViewModel viewModel)
        {
            viewModel.SceneHierarchy.CollectionChanged += OnSceneHierarchyChanged;
            viewModel.PropertyChanged += OnViewModelPropertyChanged;
        }
    }
    
    private void OnViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(SceneHierarchyPanelViewModel.SceneHierarchy))
        {
            RefreshExpansionState();
        }
    }
    
    private void OnSceneHierarchyChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        RefreshExpansionState();
    }
    
    private void RefreshExpansionState()
    {
        Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
        {
            ExpandTreeViewItems();
        }, Avalonia.Threading.DispatcherPriority.Loaded);
    }
    
    private void ExpandTreeViewItems()
    {
        var treeView = this.FindControl<TreeView>("SceneTreeView");
        if (treeView?.ItemsSource != null)
        {
            foreach (var item in treeView.ItemsSource.OfType<TreeNodeItem>())
            {
                ExpandNodeRecursively(treeView, item);
            }
        }
    }
    
    private void ExpandNodeRecursively(TreeView treeView, TreeNodeItem node)
    {
        var container = treeView.TreeContainerFromItem(node);
        if (container is TreeViewItem treeViewItem && node.IsExpanded)
        {
            treeViewItem.IsExpanded = true;
        }
        
        foreach (var child in node.Children)
        {
            ExpandNodeRecursively(treeView, child);
        }
    }
}
