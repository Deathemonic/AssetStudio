using CommunityToolkit.Mvvm.ComponentModel;

namespace AssetStudio.GUI.Models.Documents;

public sealed partial class ClassItem : ObservableObject
{
    [ObservableProperty] private int _id;
    [ObservableProperty] private string _className = string.Empty;
}
