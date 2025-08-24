using System.Collections.Generic;
using System.Threading.Tasks;
using AssetStudio.GUI.Models.Documents;
using AssetStudio.GUI.Models.Panels;

namespace AssetStudio.GUI.Services;

public class AssetManager
{
    private readonly AssetsManager _assetsManager = new();
    private readonly AssetLoader _assetLoader;
    private readonly AssetDataBuilder _assetDataBuilder;
    private readonly AssetDumper _assetDumper;

    public bool DisplayAllAssets { get; set; }
    public bool HasLoadedAssets { get; private set; }

    public AssetManager()
    {
        _assetLoader = new AssetLoader(_assetsManager);
        var sceneHierarchyBuilder = new SceneHierarchyBuilder(_assetsManager);
        _assetDataBuilder = new AssetDataBuilder(_assetsManager, sceneHierarchyBuilder);
        _assetDumper = new AssetDumper(_assetsManager);
    }

    public async Task<bool> LoadFileAsync(string filePath)
    {
        var result = await _assetLoader.LoadFileAsync(filePath);
        HasLoadedAssets = result;
        return result;
    }

    public async Task<bool> LoadFilesAsync(string[] filePaths)
    {
        var result = await _assetLoader.LoadFilesAsync(filePaths);
        HasLoadedAssets = result;
        return result;
    }

    public async Task<(List<AssetItem> Assets, List<ClassItem> Classes, List<TreeNodeItem> SceneHierarchy)>
        BuildAssetDataAsync()
    {
        return await _assetDataBuilder.BuildAssetDataAsync(DisplayAllAssets);
    }

    public TreeNodeItem? DumpAsset(long pathId, string containerName)
    {
        return _assetDumper.DumpAssetToTree(pathId);
    }

    public void Clear()
    {
        _assetsManager.Clear();
        HasLoadedAssets = false;
    }
}