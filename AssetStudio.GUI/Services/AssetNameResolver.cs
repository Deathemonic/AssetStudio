namespace AssetStudio.GUI.Services;

public static class AssetNameResolver
{
    public static string GetAssetName(Object asset)
    {
        var name = asset switch
        {
            Animator animator when animator.m_GameObject.TryGet(out var gameObject) && !string.IsNullOrEmpty(gameObject.m_Name) => gameObject.m_Name,
            NamedObject namedObject when !string.IsNullOrEmpty(namedObject.m_Name) => namedObject.m_Name,
            Texture2D texture2D when !string.IsNullOrEmpty(texture2D.m_Name) => texture2D.m_Name,
            AudioClip audioClip when !string.IsNullOrEmpty(audioClip.m_Name) => audioClip.m_Name,
            Mesh mesh when !string.IsNullOrEmpty(mesh.m_Name) => mesh.m_Name,
            Material material when !string.IsNullOrEmpty(material.m_Name) => material.m_Name,
            _ => string.Empty
        };

        if (!string.IsNullOrEmpty(name)) return name;
        var isExportable = AssetExportabilityChecker.IsExportableAsset(asset.type);
        name = isExportable 
            ? $"Unnamed {asset.type}" 
            : $"{asset.type} #{asset.m_PathID}";

        return name;
    }
}
