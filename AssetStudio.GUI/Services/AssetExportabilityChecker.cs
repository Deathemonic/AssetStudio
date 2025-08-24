namespace AssetStudio.GUI.Services;

public static class AssetExportabilityChecker
{
    public static bool IsExportableAsset(ClassIDType type)
    {
        return type switch
        {
            ClassIDType.Texture2D => true,
            ClassIDType.Texture2DArray => true,
            ClassIDType.AudioClip => true,
            ClassIDType.VideoClip => true,
            ClassIDType.Shader => true,
            ClassIDType.Mesh => true,
            ClassIDType.TextAsset => true,
            ClassIDType.AnimationClip => true,
            ClassIDType.Font => true,
            ClassIDType.MovieTexture => true,
            ClassIDType.Sprite => true,
            ClassIDType.Animator => true,
            ClassIDType.MonoBehaviour => true,
            _ => false
        };
    }
}
