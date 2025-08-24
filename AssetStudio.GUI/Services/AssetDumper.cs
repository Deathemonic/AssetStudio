using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using AssetStudio.GUI.Models.Panels;

namespace AssetStudio.GUI.Services;

public class AssetDumper(AssetsManager assetsManager)
{
    private static readonly AssemblyLoader AssemblyLoader = new();

    public TreeNodeItem DumpAssetToTree(long pathId)
    {
        try
        {
            foreach (var asset in assetsManager.assetsFileList
                         .Select(assetsFile => assetsFile.Objects.FirstOrDefault(obj => obj.m_PathID == pathId))
                         .OfType<Object>())
            {
                var jsonDoc = DumpAssetToJsonDoc(asset);
                if (jsonDoc != null) return JsonToTreeNode(jsonDoc.RootElement, $"{asset.type} (PathID: {pathId})");
            }
        }
        catch (Exception ex)
        {
            return new TreeNodeItem
            {
                Name = $"Error: {ex.Message}",
                Children = []
            };
        }

        return new TreeNodeItem
        {
            Name = $"Asset not found (PathID: {pathId})",
            Children = []
        };
    }

    private static JsonDocument? DumpAssetToJsonDoc(Object? obj)
    {
        if (obj == null)
            return null;

        try
        {
            if (obj is not MonoBehaviour m_MonoBehaviour) return obj.ToJsonDoc();
            var type = obj.serializedType?.m_Type ?? MonoBehaviourToTypeTree(m_MonoBehaviour);
            return m_MonoBehaviour.ToJsonDoc(type);
        }
        catch
        {
            return null;
        }
    }

    private static TreeNodeItem JsonToTreeNode(JsonElement element, string name)
    {
        var node = new TreeNodeItem { Name = name };

        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                foreach (var childNode in element.EnumerateObject()
                             .Select(property => JsonToTreeNode(property.Value, property.Name)))
                    node.Children.Add(childNode);
                break;

            case JsonValueKind.Array:
                var index = 0;
                foreach (var childNode in element.EnumerateArray()
                             .Select(arrayElement => JsonToTreeNode(arrayElement, $"[{index++}]")))
                    node.Children.Add(childNode);
                break;

            case JsonValueKind.String:
                node.Name = $"{name}: \"{element.GetString()}\"";
                break;

            case JsonValueKind.Number:
                node.Name = $"{name}: {element.GetRawText()}";
                break;

            case JsonValueKind.True:
            case JsonValueKind.False:
                node.Name = $"{name}: {element.GetBoolean()}";
                break;

            case JsonValueKind.Null:
                node.Name = $"{name}: null";
                break;

            case JsonValueKind.Undefined:
            default:
                node.Name = $"{name}: {element.GetRawText()}";
                break;
        }

        return node;
    }

    private static TypeTree? MonoBehaviourToTypeTree(MonoBehaviour m_MonoBehaviour)
    {
        try
        {
            return m_MonoBehaviour.ConvertToTypeTree(AssemblyLoader);
        }
        catch
        {
            return null;
        }
    }
}