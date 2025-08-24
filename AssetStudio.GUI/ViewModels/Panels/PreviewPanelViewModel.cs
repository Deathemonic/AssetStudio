using CommunityToolkit.Mvvm.ComponentModel;
using Dock.Model.Mvvm.Controls;

namespace AssetStudio.GUI.ViewModels.Panels;

public partial class PreviewPanelViewModel : Tool
{
    [ObservableProperty] private string _assetInfoText = "No asset selected";

    [ObservableProperty] private string _audioInfoText = string.Empty;

    [ObservableProperty] private string _classTextContent = string.Empty;

    [ObservableProperty] private string _fontPreviewContent = string.Empty;

    [ObservableProperty] private bool _isAssetInfoVisible = true;

    [ObservableProperty] private bool _isAudioPanelVisible;

    [ObservableProperty] private bool _isClassTextPreviewVisible;

    [ObservableProperty] private bool _isFontPreviewVisible;

    [ObservableProperty] private bool _isImagePreviewVisible;

    [ObservableProperty] private bool _isLoopEnabled;

    [ObservableProperty] private bool _isModelPreviewVisible;

    [ObservableProperty] private bool _isTextPreviewVisible;

    [ObservableProperty] private double _progressValue;

    [ObservableProperty] private string _textPreviewContent = string.Empty;

    [ObservableProperty] private string _timeText = "00:00 / 00:00";

    [ObservableProperty] private double _volumeValue = 50;

    public PreviewPanelViewModel()
    {
        Id = "Preview";
        Title = "Preview";
        CanClose = false;
        AssetInfoText = "Select an asset to preview";
    }
}