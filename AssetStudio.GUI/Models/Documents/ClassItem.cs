using System.ComponentModel;

namespace AssetStudio.GUI.Models.Documents;

public class ClassItem : INotifyPropertyChanged
{
    private int _id;
    private string _className = string.Empty;

    public int ID
    {
        get => _id;
        set
        {
            _id = value;
            OnPropertyChanged(nameof(ID));
        }
    }

    public string ClassName
    {
        get => _className;
        set
        {
            _className = value;
            OnPropertyChanged(nameof(ClassName));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
