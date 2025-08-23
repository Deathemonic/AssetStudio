using System.Collections.ObjectModel;
using System.ComponentModel;

namespace AssetStudio.GUI.Models.Panels;

public class TreeNodeItem : INotifyPropertyChanged
{
    private string _name = string.Empty;
    private bool _isExpanded;
    private bool _isSelected;
    private ObservableCollection<TreeNodeItem> _children = new();

    public string Name
    {
        get => _name;
        set
        {
            _name = value;
            OnPropertyChanged(nameof(Name));
        }
    }

    public bool IsExpanded
    {
        get => _isExpanded;
        set
        {
            _isExpanded = value;
            OnPropertyChanged(nameof(IsExpanded));
        }
    }

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            _isSelected = value;
            OnPropertyChanged(nameof(IsSelected));
        }
    }

    public ObservableCollection<TreeNodeItem> Children
    {
        get => _children;
        set
        {
            _children = value;
            OnPropertyChanged(nameof(Children));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
