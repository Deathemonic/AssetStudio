using System;
using System.Linq;
using AssetStudio.GUI.Models.Documents;
using AssetStudio.GUI.ViewModels.Documents;
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

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);

        if (DataContext is not AssetListDocumentViewModel viewModel) return;
        var dataGrid = this.FindControl<DataGrid>("AssetDataGrid");
        if (dataGrid != null)
            dataGrid.SelectionChanged += (_, _) =>
            {
                if (dataGrid.SelectedItem is AssetItem selectedAsset) viewModel.SelectedAsset = selectedAsset;
            };
    }
}

public static class ControlExtensions
{
    public static T? FindDescendantOfType<T>(this Control control) where T : Control
    {
        return control.GetLogicalDescendants().OfType<T>().FirstOrDefault();
    }
}