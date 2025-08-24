using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AssetStudio.GUI.Models.Documents;
using AssetStudio.GUI.Models.Panels;

namespace AssetStudio.GUI.Services;

public class AssetDataBuilder
{
    private readonly AssetsManager _assetsManager;
    private readonly SceneHierarchyBuilder _sceneHierarchyBuilder;

    public AssetDataBuilder(AssetsManager assetsManager, SceneHierarchyBuilder sceneHierarchyBuilder)
    {
        _assetsManager = assetsManager;
        _sceneHierarchyBuilder = sceneHierarchyBuilder;
    }

    public async Task<(List<AssetItem> Assets, List<ClassItem> Classes, List<TreeNodeItem> SceneHierarchy)>
        BuildAssetDataAsync(bool displayAllAssets)
    {
        return await Task.Run(() =>
        {
            var assets = new List<AssetItem>();
            var classes = new Dictionary<int, ClassItem>();
            var sceneHierarchy = new List<TreeNodeItem>();
            var containers = new List<(PPtr<Object>, string)>();
            var objectAssetItemDict = new Dictionary<Object, AssetItem>();

            foreach (var assetsFile in _assetsManager.assetsFileList)
            {
                var preloadTable = Array.Empty<PPtr<Object>>();

                foreach (var asset in assetsFile.Objects)
                {
                    switch (asset.type)
                    {
                        case ClassIDType.AssetBundle:
                        {
                            var assetBundle = (AssetBundle)asset;
                            var isStreamedSceneAssetBundle = assetBundle.m_IsStreamedSceneAssetBundle;
                            if (!isStreamedSceneAssetBundle)
                            {
                                preloadTable = assetBundle.m_PreloadTable;
                            }

                            foreach (var container in assetBundle.m_Container)
                            {
                                var preloadIndex = container.Value.preloadIndex;
                                var preloadSize = isStreamedSceneAssetBundle
                                    ? preloadTable.Length
                                    : container.Value.preloadSize;
                                var preloadEnd = preloadIndex + preloadSize;
                                for (var k = preloadIndex; k < preloadEnd; k++)
                                {
                                    if (k < preloadTable.Length)
                                    {
                                        containers.Add((preloadTable[k], container.Key));
                                    }
                                }
                            }
                            continue;
                        }
                        case ClassIDType.ResourceManager:
                        {
                            var resourceManager = (ResourceManager)asset;
                            containers.AddRange(resourceManager.m_Container.Select(container => (container.Value, container.Key)));
                            continue;
                        }
                    }

                    var exportable = AssetExportabilityChecker.IsExportableAsset(asset.type);

                    if (!displayAllAssets && !exportable)
                        continue;

                    var assetItem = new AssetItem
                    {
                        Name = AssetNameResolver.GetAssetName(asset),
                        Container = Path.GetFileName(assetsFile.originalPath ?? assetsFile.fileName),
                        Type = asset.type.ToString(),
                        PathId = asset.m_PathID,
                        Size = asset.byteSize
                    };
                    assets.Add(assetItem);
                    objectAssetItemDict.Add(asset, assetItem);

                    // Only add class if the asset is being displayed
                    var classId = (int)asset.type;
                    if (!classes.ContainsKey(classId))
                        classes[classId] = new ClassItem
                        {
                            Id = classId,
                            ClassName = asset.type.ToString()
                        };
                }
            }

            foreach (var (pptr, containerPath) in containers)
            {
                if (!pptr.TryGet(out var obj) || !objectAssetItemDict.TryGetValue(obj, out var assetItem)) continue;
                assetItem.Container = containerPath;

                if (obj is not MonoBehaviour) continue;
                var fileName = Path.GetFileNameWithoutExtension(containerPath);

                if (!string.IsNullOrEmpty(fileName))
                {
                    assetItem.Name = fileName;
                }
            }

            sceneHierarchy.AddRange(_sceneHierarchyBuilder.BuildSceneHierarchy());

            return (assets, classes.Values.ToList(), sceneHierarchy);
        });
    }
}
