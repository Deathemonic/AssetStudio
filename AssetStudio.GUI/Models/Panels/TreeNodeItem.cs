using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AssetStudio.GUI.Models.Panels;

public partial class TreeNodeItem : ObservableObject
{
    [ObservableProperty] private string _name = string.Empty;
    [ObservableProperty] private bool _isExpanded;
    [ObservableProperty] private bool _isSelected;
    [ObservableProperty] private ObservableCollection<TreeNodeItem> _children = new();
}
