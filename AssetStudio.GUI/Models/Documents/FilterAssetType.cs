using CommunityToolkit.Mvvm.ComponentModel;

namespace AssetStudio.GUI.Models.Documents;

public sealed partial class FilterAssetType : ObservableObject
{
    [ObservableProperty] private string _name = string.Empty;
    [ObservableProperty] private bool _isSelected;
}
