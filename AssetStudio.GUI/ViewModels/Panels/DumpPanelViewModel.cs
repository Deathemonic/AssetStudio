using CommunityToolkit.Mvvm.ComponentModel;
using Dock.Model.Mvvm.Controls;

namespace AssetStudio.GUI.ViewModels.Panels;

public partial class DumpPanelViewModel : Tool
{
    [ObservableProperty]
    private string _dumpText = "No asset selected for dumping.\n\nSelect an asset from the Asset List to view its raw data structure here.";

    public DumpPanelViewModel()
    {
        Id = "Dump";
        Title = "Dump";
        CanClose = false;
        InitializeSampleDump();
    }

    private void InitializeSampleDump()
    {
        DumpText = @"// Asset Dump View
// Select an asset to see its structure

Example Asset Structure:
{
  ""m_ObjectHideFlags"": 0,
  ""m_CorrespondingSourceObject"": {
    ""fileID"": 0
  },
  ""m_PrefabInstance"": {
    ""fileID"": 0
  },
  ""m_PrefabAsset"": {
    ""fileID"": 0
  },
  ""m_GameObject"": {
    ""fileID"": 1234567890
  },
  ""m_Enabled"": 1,
  ""m_EditorHideFlags"": 0,
  ""m_Script"": {
    ""fileID"": 11500000,
    ""guid"": ""abcdef1234567890"",
    ""type"": 3
  },
  ""m_Name"": ""ExampleAsset"",
  ""m_EditorClassIdentifier"": """"
}";
    }
}