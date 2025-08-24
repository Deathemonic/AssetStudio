using CommunityToolkit.Mvvm.ComponentModel;

namespace AssetStudio.GUI.Models.Documents;

public sealed partial class AssetItem : ObservableObject
{
    [ObservableProperty] private string _name = string.Empty;
    [ObservableProperty] private string _container = string.Empty;
    [ObservableProperty] private string _type = string.Empty;
    [ObservableProperty] private long _pathId;
    [ObservableProperty] private long _size;
}
