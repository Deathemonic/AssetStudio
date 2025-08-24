using System.Linq;
using AssetStudio.GUI.ViewModels;
using Avalonia.Controls;
using Avalonia.Input;

namespace AssetStudio.GUI.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        AddHandler(DragDrop.DropEvent, Drop);
        AddHandler(DragDrop.DragOverEvent, DragOver);
    }

    private static void DragOver(object? sender, DragEventArgs e)
    {
        e.DragEffects = e.Data.Contains(DataFormats.Files) ? DragDropEffects.Copy : DragDropEffects.None;
    }

    private async void Drop(object? sender, DragEventArgs e)
    {
        if (DataContext is not MainWindowViewModel viewModel || !e.Data.Contains(DataFormats.Files)) return;
        var files = e.Data.GetFiles()?.Select(f => f.Path.LocalPath).ToArray();
        if (files is not { Length: > 0 }) return;
        if (files.Length == 1)
            await viewModel.LoadFileAsync(files[0]);
        else
            await viewModel.LoadFilesAsync(files);
    }
}