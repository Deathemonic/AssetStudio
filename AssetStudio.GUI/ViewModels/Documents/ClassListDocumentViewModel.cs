using System.Collections.ObjectModel;
using System.ComponentModel;
using AssetStudio.GUI.Logic;
using AssetStudio.GUI.Models.Documents;
using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using Dock.Model.Mvvm.Controls;

namespace AssetStudio.GUI.ViewModels.Documents;

public partial class ClassListDocumentViewModel : Document
{
    [ObservableProperty] private DataGridCollectionView _collectionView;
    [ObservableProperty] private string _searchText = string.Empty;
    [ObservableProperty] private int _selectedIncludeExcludeMode;
    [ObservableProperty] private int _selectedSearchFilterMode;

    public ClassListDocumentViewModel()
    {
        Id = "ClassList";
        Title = "Classes";
        CanClose = false;
        InitializeSampleData();
        RefreshView();

        PropertyChanged += OnPropertyChanged;
    }

    private ObservableCollection<ClassItem> Classes { get; } = [];

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(SearchText):
            case nameof(SelectedSearchFilterMode):
            case nameof(SelectedIncludeExcludeMode):
                RefreshView();
                break;
        }
    }

    private void InitializeSampleData()
    {
        Classes.Add(new ClassItem { Id = 1, ClassName = "GameObject" });
        Classes.Add(new ClassItem { Id = 2, ClassName = "Component" });
        Classes.Add(new ClassItem { Id = 3, ClassName = "LevelGameManager" });
        Classes.Add(new ClassItem { Id = 4, ClassName = "Transform" });
        Classes.Add(new ClassItem { Id = 5, ClassName = "TimeManager" });
        Classes.Add(new ClassItem { Id = 8, ClassName = "Behaviour" });
        Classes.Add(new ClassItem { Id = 9, ClassName = "GameManager" });
        Classes.Add(new ClassItem { Id = 11, ClassName = "AudioManager" });
        Classes.Add(new ClassItem { Id = 12, ClassName = "ParticleAnimator" });
        Classes.Add(new ClassItem { Id = 13, ClassName = "EllipsoidParticleEmitter" });
        Classes.Add(new ClassItem { Id = 15, ClassName = "MeshParticleEmitter" });
        Classes.Add(new ClassItem { Id = 17, ClassName = "Cubemap" });
        Classes.Add(new ClassItem { Id = 18, ClassName = "Avatar" });
        Classes.Add(new ClassItem { Id = 19, ClassName = "AnimatorController" });
        Classes.Add(new ClassItem { Id = 20, ClassName = "GUILayer" });
        Classes.Add(new ClassItem { Id = 21, ClassName = "Material" });
        Classes.Add(new ClassItem { Id = 23, ClassName = "MeshRenderer" });
        Classes.Add(new ClassItem { Id = 25, ClassName = "Renderer" });
        Classes.Add(new ClassItem { Id = 26, ClassName = "ParticleRenderer" });
        Classes.Add(new ClassItem { Id = 27, ClassName = "Texture" });
        Classes.Add(new ClassItem { Id = 28, ClassName = "Texture2D" });
        Classes.Add(new ClassItem { Id = 29, ClassName = "SceneSettings" });
        Classes.Add(new ClassItem { Id = 30, ClassName = "GraphicsSettings" });
        Classes.Add(new ClassItem { Id = 33, ClassName = "MeshFilter" });
        Classes.Add(new ClassItem { Id = 41, ClassName = "OcclusionPortal" });
        Classes.Add(new ClassItem { Id = 43, ClassName = "Mesh" });
        Classes.Add(new ClassItem { Id = 45, ClassName = "Skybox" });
        Classes.Add(new ClassItem { Id = 47, ClassName = "QualitySettings" });
        Classes.Add(new ClassItem { Id = 48, ClassName = "Shader" });
        Classes.Add(new ClassItem { Id = 49, ClassName = "TextAsset" });
        Classes.Add(new ClassItem { Id = 50, ClassName = "Rigidbody2D" });
        Classes.Add(new ClassItem { Id = 54, ClassName = "Rigidbody" });
        Classes.Add(new ClassItem { Id = 56, ClassName = "Collider" });
        Classes.Add(new ClassItem { Id = 57, ClassName = "Joint" });
        Classes.Add(new ClassItem { Id = 58, ClassName = "CircleCollider2D" });
        Classes.Add(new ClassItem { Id = 59, ClassName = "HingeJoint" });
        Classes.Add(new ClassItem { Id = 60, ClassName = "PolygonCollider2D" });
        Classes.Add(new ClassItem { Id = 61, ClassName = "BoxCollider2D" });
        Classes.Add(new ClassItem { Id = 62, ClassName = "PhysicsMaterial2D" });
        Classes.Add(new ClassItem { Id = 64, ClassName = "MeshCollider" });
        Classes.Add(new ClassItem { Id = 65, ClassName = "BoxCollider" });
        Classes.Add(new ClassItem { Id = 68, ClassName = "EdgeCollider2D" });
        Classes.Add(new ClassItem { Id = 70, ClassName = "CapsuleCollider2D" });
        Classes.Add(new ClassItem { Id = 72, ClassName = "ComputeShader" });
        Classes.Add(new ClassItem { Id = 74, ClassName = "AnimationClip" });
        Classes.Add(new ClassItem { Id = 75, ClassName = "ConstantForce" });
        Classes.Add(new ClassItem { Id = 76, ClassName = "WorldParticleCollider" });
        Classes.Add(new ClassItem { Id = 78, ClassName = "TagManager" });
        Classes.Add(new ClassItem { Id = 81, ClassName = "AudioListener" });
        Classes.Add(new ClassItem { Id = 82, ClassName = "AudioSource" });
        Classes.Add(new ClassItem { Id = 83, ClassName = "AudioClip" });
        Classes.Add(new ClassItem { Id = 84, ClassName = "RenderTexture" });
        Classes.Add(new ClassItem { Id = 87, ClassName = "MeshParticleEmitter" });
        Classes.Add(new ClassItem { Id = 88, ClassName = "ParticleEmitter" });
        Classes.Add(new ClassItem { Id = 89, ClassName = "Cubemap" });
        Classes.Add(new ClassItem { Id = 90, ClassName = "Avatar" });
        Classes.Add(new ClassItem { Id = 91, ClassName = "AnimatorController" });
        Classes.Add(new ClassItem { Id = 92, ClassName = "GUILayer" });
        Classes.Add(new ClassItem { Id = 93, ClassName = "RuntimeAnimatorController" });
        Classes.Add(new ClassItem { Id = 94, ClassName = "ScriptMapper" });
        Classes.Add(new ClassItem { Id = 95, ClassName = "Animator" });
        Classes.Add(new ClassItem { Id = 96, ClassName = "TrailRenderer" });
        Classes.Add(new ClassItem { Id = 98, ClassName = "DelayedCallManager" });
        Classes.Add(new ClassItem { Id = 102, ClassName = "TextMesh" });
        Classes.Add(new ClassItem { Id = 104, ClassName = "RenderSettings" });
        Classes.Add(new ClassItem { Id = 108, ClassName = "Light" });
        Classes.Add(new ClassItem { Id = 109, ClassName = "CGProgram" });
        Classes.Add(new ClassItem { Id = 110, ClassName = "BaseAnimationTrack" });
        Classes.Add(new ClassItem { Id = 111, ClassName = "Animation" });
        Classes.Add(new ClassItem { Id = 114, ClassName = "MonoBehaviour" });
        Classes.Add(new ClassItem { Id = 115, ClassName = "MonoScript" });
        Classes.Add(new ClassItem { Id = 116, ClassName = "MonoManager" });
        Classes.Add(new ClassItem { Id = 117, ClassName = "Texture3D" });
        Classes.Add(new ClassItem { Id = 118, ClassName = "NewAnimationTrack" });
        Classes.Add(new ClassItem { Id = 119, ClassName = "Projector" });
        Classes.Add(new ClassItem { Id = 120, ClassName = "LineRenderer" });
        Classes.Add(new ClassItem { Id = 121, ClassName = "Flare" });
        Classes.Add(new ClassItem { Id = 122, ClassName = "Halo" });
        Classes.Add(new ClassItem { Id = 123, ClassName = "LensFlare" });
        Classes.Add(new ClassItem { Id = 124, ClassName = "FlareLayer" });
        Classes.Add(new ClassItem { Id = 125, ClassName = "HaloLayer" });
        Classes.Add(new ClassItem { Id = 126, ClassName = "NavMeshAreas" });
        Classes.Add(new ClassItem { Id = 127, ClassName = "HaloManager" });
        Classes.Add(new ClassItem { Id = 128, ClassName = "Font" });
        Classes.Add(new ClassItem { Id = 129, ClassName = "PlayerSettings" });
        Classes.Add(new ClassItem { Id = 130, ClassName = "NamedObject" });
        Classes.Add(new ClassItem { Id = 131, ClassName = "GUITexture" });
        Classes.Add(new ClassItem { Id = 132, ClassName = "GUIText" });
        Classes.Add(new ClassItem { Id = 133, ClassName = "GUIElement" });
        Classes.Add(new ClassItem { Id = 134, ClassName = "PhysicMaterial" });
        Classes.Add(new ClassItem { Id = 135, ClassName = "SphereCollider" });
        Classes.Add(new ClassItem { Id = 136, ClassName = "CapsuleCollider" });
        Classes.Add(new ClassItem { Id = 137, ClassName = "SkinnedMeshRenderer" });
        Classes.Add(new ClassItem { Id = 138, ClassName = "FixedJoint" });
        Classes.Add(new ClassItem { Id = 141, ClassName = "BuildSettings" });
        Classes.Add(new ClassItem { Id = 142, ClassName = "AssetBundle" });
        Classes.Add(new ClassItem { Id = 143, ClassName = "CharacterController" });
        Classes.Add(new ClassItem { Id = 144, ClassName = "CharacterJoint" });
        Classes.Add(new ClassItem { Id = 145, ClassName = "SpringJoint" });
        Classes.Add(new ClassItem { Id = 146, ClassName = "WheelCollider" });
        Classes.Add(new ClassItem { Id = 147, ClassName = "ResourceManager" });
        Classes.Add(new ClassItem { Id = 148, ClassName = "NetworkView" });
        Classes.Add(new ClassItem { Id = 149, ClassName = "NetworkManager" });
        Classes.Add(new ClassItem { Id = 150, ClassName = "EllipsoidParticleEmitter" });
        Classes.Add(new ClassItem { Id = 152, ClassName = "ParticleAnimator" });
        Classes.Add(new ClassItem { Id = 153, ClassName = "ParticleRenderer" });
        Classes.Add(new ClassItem { Id = 154, ClassName = "Shader" });
        Classes.Add(new ClassItem { Id = 156, ClassName = "TerrainCollider" });
        Classes.Add(new ClassItem { Id = 157, ClassName = "TerrainData" });
        Classes.Add(new ClassItem { Id = 158, ClassName = "LightmapSettings" });
        Classes.Add(new ClassItem { Id = 159, ClassName = "WebCamTexture" });
        Classes.Add(new ClassItem { Id = 160, ClassName = "EditorSettings" });
        Classes.Add(new ClassItem { Id = 162, ClassName = "EditorUserSettings" });
        Classes.Add(new ClassItem { Id = 164, ClassName = "AudioReverbFilter" });
        Classes.Add(new ClassItem { Id = 165, ClassName = "AudioHighPassFilter" });
        Classes.Add(new ClassItem { Id = 166, ClassName = "AudioChorusFilter" });
        Classes.Add(new ClassItem { Id = 167, ClassName = "AudioReverbZone" });
        Classes.Add(new ClassItem { Id = 168, ClassName = "AudioEchoFilter" });
        Classes.Add(new ClassItem { Id = 169, ClassName = "AudioLowPassFilter" });
        Classes.Add(new ClassItem { Id = 170, ClassName = "AudioDistortionFilter" });
        Classes.Add(new ClassItem { Id = 171, ClassName = "SparseTexture" });
        Classes.Add(new ClassItem { Id = 180, ClassName = "AudioBehaviour" });
        Classes.Add(new ClassItem { Id = 181, ClassName = "AudioFilter" });
        Classes.Add(new ClassItem { Id = 182, ClassName = "WindZone" });
        Classes.Add(new ClassItem { Id = 183, ClassName = "Cloth" });
        Classes.Add(new ClassItem { Id = 184, ClassName = "SubstanceArchive" });
        Classes.Add(new ClassItem { Id = 185, ClassName = "ProceduralMaterial" });
        Classes.Add(new ClassItem { Id = 186, ClassName = "ProceduralTexture" });
        Classes.Add(new ClassItem { Id = 187, ClassName = "Texture2DArray" });
        Classes.Add(new ClassItem { Id = 188, ClassName = "CubemapArray" });
        Classes.Add(new ClassItem { Id = 191, ClassName = "OffMeshLink" });
        Classes.Add(new ClassItem { Id = 192, ClassName = "OcclusionArea" });
        Classes.Add(new ClassItem { Id = 193, ClassName = "Tree" });
        Classes.Add(new ClassItem { Id = 194, ClassName = "NavMeshObstacle" });
        Classes.Add(new ClassItem { Id = 195, ClassName = "NavMeshAgent" });
        Classes.Add(new ClassItem { Id = 196, ClassName = "NavMeshSettings" });
        Classes.Add(new ClassItem { Id = 197, ClassName = "LightProbesLegacy" });
        Classes.Add(new ClassItem { Id = 198, ClassName = "ParticleSystem" });
        Classes.Add(new ClassItem { Id = 199, ClassName = "ParticleSystemRenderer" });
        Classes.Add(new ClassItem { Id = 200, ClassName = "ShaderVariantCollection" });
        Classes.Add(new ClassItem { Id = 205, ClassName = "LODGroup" });
        Classes.Add(new ClassItem { Id = 206, ClassName = "BlendTree" });
        Classes.Add(new ClassItem { Id = 207, ClassName = "Motion" });
        Classes.Add(new ClassItem { Id = 208, ClassName = "NavMeshObstacle" });
        Classes.Add(new ClassItem { Id = 210, ClassName = "SortingGroup" });
        Classes.Add(new ClassItem { Id = 212, ClassName = "Sprite" });
        Classes.Add(new ClassItem { Id = 213, ClassName = "CachedSpriteAtlas" });
        Classes.Add(new ClassItem { Id = 214, ClassName = "ReflectionProbe" });
        Classes.Add(new ClassItem { Id = 215, ClassName = "ReflectionProbes" });
        Classes.Add(new ClassItem { Id = 218, ClassName = "Terrain" });
        Classes.Add(new ClassItem { Id = 220, ClassName = "LightProbeGroup" });
        Classes.Add(new ClassItem { Id = 221, ClassName = "AnimatorOverrideController" });
        Classes.Add(new ClassItem { Id = 222, ClassName = "CanvasRenderer" });
        Classes.Add(new ClassItem { Id = 223, ClassName = "Canvas" });
        Classes.Add(new ClassItem { Id = 224, ClassName = "RectTransform" });
        Classes.Add(new ClassItem { Id = 225, ClassName = "CanvasGroup" });
        Classes.Add(new ClassItem { Id = 226, ClassName = "BillboardAsset" });
        Classes.Add(new ClassItem { Id = 227, ClassName = "BillboardRenderer" });
        Classes.Add(new ClassItem { Id = 228, ClassName = "SpeedTreeWindAsset" });
        Classes.Add(new ClassItem { Id = 229, ClassName = "AnchoredJoint2D" });
        Classes.Add(new ClassItem { Id = 230, ClassName = "Joint2D" });
        Classes.Add(new ClassItem { Id = 231, ClassName = "SpringJoint2D" });
        Classes.Add(new ClassItem { Id = 232, ClassName = "DistanceJoint2D" });
        Classes.Add(new ClassItem { Id = 233, ClassName = "HingeJoint2D" });
        Classes.Add(new ClassItem { Id = 234, ClassName = "SliderJoint2D" });
        Classes.Add(new ClassItem { Id = 235, ClassName = "WheelJoint2D" });
        Classes.Add(new ClassItem { Id = 236, ClassName = "ClusterInputManager" });
        Classes.Add(new ClassItem { Id = 237, ClassName = "BaseVideoTexture" });
        Classes.Add(new ClassItem { Id = 238, ClassName = "NavMeshData" });
        Classes.Add(new ClassItem { Id = 240, ClassName = "AudioMixer" });
        Classes.Add(new ClassItem { Id = 241, ClassName = "AudioMixerController" });
        Classes.Add(new ClassItem { Id = 243, ClassName = "AudioMixerGroupController" });
        Classes.Add(new ClassItem { Id = 244, ClassName = "AudioMixerEffectController" });
        Classes.Add(new ClassItem { Id = 245, ClassName = "AudioMixerSnapshotController" });
        Classes.Add(new ClassItem { Id = 246, ClassName = "PhysicsUpdateBehaviour2D" });
        Classes.Add(new ClassItem { Id = 247, ClassName = "ConstantForce2D" });
        Classes.Add(new ClassItem { Id = 248, ClassName = "Effector2D" });
        Classes.Add(new ClassItem { Id = 249, ClassName = "AreaEffector2D" });
        Classes.Add(new ClassItem { Id = 250, ClassName = "PointEffector2D" });
        Classes.Add(new ClassItem { Id = 251, ClassName = "PlatformEffector2D" });
        Classes.Add(new ClassItem { Id = 252, ClassName = "SurfaceEffector2D" });
        Classes.Add(new ClassItem { Id = 253, ClassName = "BuoyancyEffector2D" });
        Classes.Add(new ClassItem { Id = 254, ClassName = "RelativeJoint2D" });
        Classes.Add(new ClassItem { Id = 255, ClassName = "FixedJoint2D" });
        Classes.Add(new ClassItem { Id = 256, ClassName = "FrictionJoint2D" });
        Classes.Add(new ClassItem { Id = 257, ClassName = "TargetJoint2D" });
        Classes.Add(new ClassItem { Id = 258, ClassName = "LightProbes" });
        Classes.Add(new ClassItem { Id = 259, ClassName = "LightProbeProxyVolume" });
    }

    private void RefreshView()
    {
        var filteredClasses = ClassSearch.PerformSearch(
            Classes,
            SearchText,
            (SearchMethod)SelectedSearchFilterMode,
            (IncludeExcludeMode)SelectedIncludeExcludeMode);

        CollectionView = new DataGridCollectionView(new ObservableCollection<ClassItem>(filteredClasses));
    }
}