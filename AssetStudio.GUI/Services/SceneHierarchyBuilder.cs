using System.Collections.Generic;
using System.Linq;
using AssetStudio.GUI.Models.Panels;

namespace AssetStudio.GUI.Services;

public class SceneHierarchyBuilder(AssetsManager assetsManager)
{
    public List<TreeNodeItem> BuildSceneHierarchy()
    {
        var hierarchy = new List<TreeNodeItem>();
        var gameObjectDict = new Dictionary<long, TreeNodeItem>();
        var transformDict = new Dictionary<long, Transform>();
        var fileGameObjectDict = new Dictionary<string, List<TreeNodeItem>>();

        foreach (var assetsFile in assetsManager.assetsFileList)
        {
            var fileName = assetsFile.fileName;
            fileGameObjectDict[fileName] = new List<TreeNodeItem>();

            foreach (var asset in assetsFile.Objects)
            {
                switch (asset)
                {
                    case GameObject gameObject:
                        var goItem = new TreeNodeItem
                        {
                            Name = gameObject.m_Name
                        };
                        gameObjectDict[gameObject.m_PathID] = goItem;
                        fileGameObjectDict[fileName].Add(goItem);
                        break;

                    case Transform transform:
                        transformDict[transform.m_PathID] = transform;
                        break;
                }
            }
        }

        foreach (var assetsFile in assetsManager.assetsFileList)
        {
            var fileName = assetsFile.fileName;
            var fileGameObjects = fileGameObjectDict[fileName];

            if (fileGameObjects.Count == 0) continue;

            var fileNode = new TreeNodeItem
            {
                Name = fileName
            };

            var rootGameObjects = new List<TreeNodeItem>();

            foreach (var gameObjectItem in fileGameObjects)
            {
                var gameObject = assetsFile.Objects
                    .OfType<GameObject>()
                    .FirstOrDefault(go => go.m_Name == gameObjectItem.Name);

                if (gameObject?.m_Transform != null &&
                    transformDict.TryGetValue(gameObject.m_Transform.m_PathID, out var transform))
                {
                    if (transform.m_Father?.TryGet(out var parentTransform) == true &&
                        parentTransform.m_GameObject?.TryGet(out var parentGameObject) == true &&
                        gameObjectDict.TryGetValue(parentGameObject.m_PathID, out var parentItem) &&
                        fileGameObjects.Contains(parentItem))
                    {
                        parentItem.Children.Add(gameObjectItem);
                    }
                    else
                    {
                        rootGameObjects.Add(gameObjectItem);
                    }
                }
                else
                {
                    rootGameObjects.Add(gameObjectItem);
                }
            }

            foreach (var rootGo in rootGameObjects)
            {
                fileNode.Children.Add(rootGo);
            }

            if (fileNode.Children.Count > 0)
            {
                hierarchy.Add(fileNode);
            }
        }

        return hierarchy;
    }
}
