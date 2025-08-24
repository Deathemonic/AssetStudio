using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.LogicalTree;

namespace AssetStudio.GUI.Views.Documents;

public partial class AssetListDocumentView : UserControl
{
    public AssetListDocumentView()
    {
        InitializeComponent();
    }

    private void OnComboBoxItemPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        e.Handled = true;

        if (sender is not ComboBoxItem { DataContext: not null } item) return;
        var checkBox = item.FindDescendantOfType<CheckBox>();
        if (checkBox != null) checkBox.IsChecked = !checkBox.IsChecked;
    }
}

public static class ControlExtensions
{
    public static T? FindDescendantOfType<T>(this Control control) where T : Control
    {
        return control.GetLogicalDescendants().OfType<T>().FirstOrDefault();
    }
}