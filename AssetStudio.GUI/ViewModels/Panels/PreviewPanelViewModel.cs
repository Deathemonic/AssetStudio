using CommunityToolkit.Mvvm.ComponentModel;
using Dock.Model.Mvvm.Controls;

namespace AssetStudio.GUI.ViewModels.Panels;

public partial class PreviewPanelViewModel : Tool
{
    [ObservableProperty]
    private string _assetInfoText = "No asset selected";

    [ObservableProperty]
    private bool _isAssetInfoVisible = true;

    [ObservableProperty]
    private bool _isImagePreviewVisible = false;

    [ObservableProperty]
    private bool _isTextPreviewVisible = false;

    [ObservableProperty]
    private string _textPreviewContent = string.Empty;

    [ObservableProperty]
    private bool _isFontPreviewVisible = false;

    [ObservableProperty]
    private string _fontPreviewContent = string.Empty;

    [ObservableProperty]
    private bool _isAudioPanelVisible = false;

    [ObservableProperty]
    private string _audioInfoText = string.Empty;

    [ObservableProperty]
    private bool _isLoopEnabled = false;

    [ObservableProperty]
    private double _progressValue = 0;

    [ObservableProperty]
    private string _timeText = "00:00 / 00:00";

    [ObservableProperty]
    private double _volumeValue = 50;

    [ObservableProperty]
    private bool _isModelPreviewVisible = false;

    [ObservableProperty]
    private bool _isClassTextPreviewVisible = false;

    [ObservableProperty]
    private string _classTextContent = string.Empty;

    public PreviewPanelViewModel()
    {
        Id = "Preview";
        Title = "Preview";
        CanClose = false;
        AssetInfoText = "Select an asset to preview";
    }
}