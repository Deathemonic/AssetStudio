using System.Collections.ObjectModel;
using AssetStudio.GUI.Models.Panels;
using Dock.Model.Mvvm.Controls;

namespace AssetStudio.GUI.ViewModels.Panels;

public class DumpPanelViewModel : Tool
{
    public DumpPanelViewModel()
    {
        Id = "Dump";
        Title = "Dump";
        CanClose = false;

        var initialNode = new TreeNodeItem 
        { 
            Name = "No asset selected for dumping.",
            Children = []
        };
        DumpTree.Add(initialNode);
    }

    public ObservableCollection<TreeNodeItem> DumpTree { get; } = new();

    public void UpdateDump(TreeNodeItem? dumpTreeRoot)
    {
        DumpTree.Clear();
        if (dumpTreeRoot != null)
        {
            DumpTree.Add(dumpTreeRoot);
        }
    }
}