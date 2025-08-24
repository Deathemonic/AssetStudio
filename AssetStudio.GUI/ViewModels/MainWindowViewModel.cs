using System;
using System.Linq;
using System.Threading.Tasks;
using AssetStudio.GUI.Services;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dock.Model.Core;

namespace AssetStudio.GUI.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly AssetManager _assetManager;
    private readonly MainDockFactory _dockFactory;

    [ObservableProperty] private IDock? _dockLayout;

    [ObservableProperty] private bool _isProgressVisible;

    [ObservableProperty] private int _progressValue;

    [ObservableProperty] private string _statusText = "Ready";

    [ObservableProperty] private bool _displayAllAssets = false;

    public MainWindowViewModel()
    {
        _dockFactory = new MainDockFactory();
        _assetManager = new AssetManager();
        _assetManager.DisplayAllAssets = DisplayAllAssets;
        _dockFactory.AssetDumpFunction = _assetManager.DumpAsset;
        InitializeDockLayout();
    }

    private void InitializeDockLayout()
    {
        DockLayout = _dockFactory.CreateLayout();
        _dockFactory.InitLayout(DockLayout);
    }

    public void ResetLayout()
    {
        InitializeDockLayout();
    }

    public async Task LoadFileAsync(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            return;

        try
        {
            IsProgressVisible = true;
            StatusText = "Loading file...";
            ProgressValue = 50;

            var success = await _assetManager.LoadFileAsync(filePath);

            if (success)
            {
                StatusText = "Building asset data...";
                ProgressValue = 75;

                var (assets, classes, sceneHierarchy) = await _assetManager.BuildAssetDataAsync();

                // Update all view models with the new data
                _dockFactory.UpdateAssetData(assets, classes, sceneHierarchy);

                StatusText = $"Loaded {assets.Count} assets successfully";
                ProgressValue = 100;
            }
            else
            {
                StatusText = "Failed to load file";
            }
        }
        catch (Exception ex)
        {
            StatusText = $"Error: {ex.Message}";
        }
        finally
        {
            IsProgressVisible = false;
            ProgressValue = 0;
        }
    }

    public async Task LoadFilesAsync(string[]? filePaths)
    {
        if (filePaths == null || filePaths.Length == 0)
            return;

        try
        {
            IsProgressVisible = true;
            StatusText = "Loading files...";
            ProgressValue = 50;

            var success = await _assetManager.LoadFilesAsync(filePaths);

            if (success)
            {
                StatusText = "Building asset data...";
                ProgressValue = 75;

                var (assets, classes, sceneHierarchy) = await _assetManager.BuildAssetDataAsync();

                _dockFactory.UpdateAssetData(assets, classes, sceneHierarchy);

                StatusText = $"Loaded {assets.Count} assets successfully";
                ProgressValue = 100;
            }
            else
            {
                StatusText = "Failed to load files";
            }
        }
        catch (Exception ex)
        {
            StatusText = $"Error: {ex.Message}";
        }
        finally
        {
            IsProgressVisible = false;
            ProgressValue = 0;
        }
    }

    [RelayCommand]
    private async Task LoadFile()
    {
        try
        {
            var topLevel = TopLevel.GetTopLevel(
                Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
                    ? desktop.MainWindow
                    : null);

            if (topLevel == null)
            {
                StatusText = "Error: Could not get top level window";
                return;
            }

            var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Select Asset Bundle File",
                AllowMultiple = false,
                FileTypeFilter =
                [
                    new FilePickerFileType("All Files") { Patterns = ["*.*"] },
                    new FilePickerFileType("Asset Bundle") { Patterns = ["*.unity3d", "*.assetbundle", "*.assets"] }
                ]
            });

            if (files.Any())
            {
                await LoadFileAsync(files.First().Path.LocalPath);
            }
        }
        catch (Exception ex)
        {
            StatusText = $"Error opening file picker: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task LoadFiles()
    {
        var topLevel =
            TopLevel.GetTopLevel(
                Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
                    ? desktop.MainWindow
                    : null);
        if (topLevel == null) return;

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select Asset Bundle Files",
            AllowMultiple = true,
            FileTypeFilter =
            [
                new FilePickerFileType("All Files") { Patterns = ["*.*"] },
                new FilePickerFileType("Asset Bundle")
                    { Patterns = ["*.unity3d", "*.assetbundle", "*.assets", "*.bundle"] }
            ]
        });

        if (files.Any()) await LoadFilesAsync(files.Select(f => f.Path.LocalPath).ToArray());
    }

    [RelayCommand]
    private async Task LoadFolder()
    {
        var topLevel =
            TopLevel.GetTopLevel(
                Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
                    ? desktop.MainWindow
                    : null);
        if (topLevel == null) return;

        var folders = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "Select Folder with Asset Bundles",
            AllowMultiple = false
        });

        if (folders.Any()) await LoadFilesAsync([folders[0].Path.LocalPath]);
    }

    [RelayCommand]
    private async Task ToggleDisplayAllAssets()
    {
        DisplayAllAssets = !DisplayAllAssets;
        _assetManager.DisplayAllAssets = DisplayAllAssets;

        // Refresh asset data with new filter settings
        if (_assetManager.HasLoadedAssets)
        {
            try
            {
                StatusText = "Updating asset display...";
                var (assets, classes, sceneHierarchy) = await _assetManager.BuildAssetDataAsync();
                _dockFactory.UpdateAssetData(assets, classes, sceneHierarchy);
                StatusText = $"Loaded {assets.Count} assets successfully";
            }
            catch (Exception ex)
            {
                StatusText = $"Error updating assets: {ex.Message}";
            }
        }
    }

    [RelayCommand]
    private void Exit()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            desktop.Shutdown();
    }
}