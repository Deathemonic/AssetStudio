using System.ComponentModel;

namespace AssetStudio.GUI.Models.Documents;

public class AssetItem : INotifyPropertyChanged
{
    private string _name = string.Empty;
    private string _container = string.Empty;
    private string _type = string.Empty;
    private long _pathID;
    private long _size;

    public string Name
    {
        get => _name;
        set
        {
            _name = value;
            OnPropertyChanged(nameof(Name));
        }
    }

    public string Container
    {
        get => _container;
        set
        {
            _container = value;
            OnPropertyChanged(nameof(Container));
        }
    }

    public string Type
    {
        get => _type;
        set
        {
            _type = value;
            OnPropertyChanged(nameof(Type));
        }
    }

    public long PathID
    {
        get => _pathID;
        set
        {
            _pathID = value;
            OnPropertyChanged(nameof(PathID));
        }
    }

    public long Size
    {
        get => _size;
        set
        {
            _size = value;
            OnPropertyChanged(nameof(Size));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
