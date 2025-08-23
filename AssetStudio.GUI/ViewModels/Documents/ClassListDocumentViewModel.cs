using System.Collections.ObjectModel;
using AssetStudio.GUI.Models.Documents;
using AssetStudio.GUI.Logic;
using CommunityToolkit.Mvvm.ComponentModel;
using Dock.Model.Mvvm.Controls;
using Avalonia.Collections;
using System.Linq;
using System;
using System.Collections.Generic;

namespace AssetStudio.GUI.ViewModels.Documents;

public partial class ClassListDocumentViewModel : Document
{
    [ObservableProperty]
    private DataGridCollectionView _collectionView;

    [ObservableProperty]
    private string _searchText = string.Empty;

    public ObservableCollection<ClassItem> Classes { get; } = new();

    public ClassListDocumentViewModel()
    {
        Id = "ClassList";
        Title = "Classes";
        CanClose = false;
        InitializeSampleData();
        CollectionView = new DataGridCollectionView(Classes);
        
        PropertyChanged += OnPropertyChanged;
    }
    
    private void OnPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(SearchText))
        {
            PerformSearch(SearchText, SearchMethod.Contains, IncludeExcludeMode.Include);
        }
    }

    private void InitializeSampleData()
    {
        Classes.Add(new ClassItem { ID = 1, ClassName = "GameObject" });
        Classes.Add(new ClassItem { ID = 2, ClassName = "Component" });
        Classes.Add(new ClassItem { ID = 3, ClassName = "LevelGameManager" });
        Classes.Add(new ClassItem { ID = 4, ClassName = "Transform" });
        Classes.Add(new ClassItem { ID = 5, ClassName = "TimeManager" });
        Classes.Add(new ClassItem { ID = 8, ClassName = "Behaviour" });
        Classes.Add(new ClassItem { ID = 9, ClassName = "GameManager" });
        Classes.Add(new ClassItem { ID = 11, ClassName = "AudioManager" });
        Classes.Add(new ClassItem { ID = 12, ClassName = "ParticleAnimator" });
        Classes.Add(new ClassItem { ID = 13, ClassName = "EllipsoidParticleEmitter" });
        Classes.Add(new ClassItem { ID = 15, ClassName = "MeshParticleEmitter" });
        Classes.Add(new ClassItem { ID = 17, ClassName = "Cubemap" });
        Classes.Add(new ClassItem { ID = 18, ClassName = "Avatar" });
        Classes.Add(new ClassItem { ID = 19, ClassName = "AnimatorController" });
        Classes.Add(new ClassItem { ID = 20, ClassName = "GUILayer" });
        Classes.Add(new ClassItem { ID = 21, ClassName = "Material" });
        Classes.Add(new ClassItem { ID = 23, ClassName = "MeshRenderer" });
        Classes.Add(new ClassItem { ID = 25, ClassName = "Renderer" });
        Classes.Add(new ClassItem { ID = 26, ClassName = "ParticleRenderer" });
        Classes.Add(new ClassItem { ID = 27, ClassName = "Texture" });
        Classes.Add(new ClassItem { ID = 28, ClassName = "Texture2D" });
        Classes.Add(new ClassItem { ID = 29, ClassName = "SceneSettings" });
        Classes.Add(new ClassItem { ID = 30, ClassName = "GraphicsSettings" });
        Classes.Add(new ClassItem { ID = 33, ClassName = "MeshFilter" });
        Classes.Add(new ClassItem { ID = 41, ClassName = "OcclusionPortal" });
        Classes.Add(new ClassItem { ID = 43, ClassName = "Mesh" });
        Classes.Add(new ClassItem { ID = 45, ClassName = "Skybox" });
        Classes.Add(new ClassItem { ID = 47, ClassName = "QualitySettings" });
        Classes.Add(new ClassItem { ID = 48, ClassName = "Shader" });
        Classes.Add(new ClassItem { ID = 49, ClassName = "TextAsset" });
        Classes.Add(new ClassItem { ID = 50, ClassName = "Rigidbody2D" });
        Classes.Add(new ClassItem { ID = 54, ClassName = "Rigidbody" });
        Classes.Add(new ClassItem { ID = 56, ClassName = "Collider" });
        Classes.Add(new ClassItem { ID = 57, ClassName = "Joint" });
        Classes.Add(new ClassItem { ID = 58, ClassName = "CircleCollider2D" });
        Classes.Add(new ClassItem { ID = 59, ClassName = "HingeJoint" });
        Classes.Add(new ClassItem { ID = 60, ClassName = "PolygonCollider2D" });
        Classes.Add(new ClassItem { ID = 61, ClassName = "BoxCollider2D" });
        Classes.Add(new ClassItem { ID = 62, ClassName = "PhysicsMaterial2D" });
        Classes.Add(new ClassItem { ID = 64, ClassName = "MeshCollider" });
        Classes.Add(new ClassItem { ID = 65, ClassName = "BoxCollider" });
        Classes.Add(new ClassItem { ID = 68, ClassName = "EdgeCollider2D" });
        Classes.Add(new ClassItem { ID = 70, ClassName = "CapsuleCollider2D" });
        Classes.Add(new ClassItem { ID = 72, ClassName = "ComputeShader" });
        Classes.Add(new ClassItem { ID = 74, ClassName = "AnimationClip" });
        Classes.Add(new ClassItem { ID = 75, ClassName = "ConstantForce" });
        Classes.Add(new ClassItem { ID = 76, ClassName = "WorldParticleCollider" });
        Classes.Add(new ClassItem { ID = 78, ClassName = "TagManager" });
        Classes.Add(new ClassItem { ID = 81, ClassName = "AudioListener" });
        Classes.Add(new ClassItem { ID = 82, ClassName = "AudioSource" });
        Classes.Add(new ClassItem { ID = 83, ClassName = "AudioClip" });
        Classes.Add(new ClassItem { ID = 84, ClassName = "RenderTexture" });
        Classes.Add(new ClassItem { ID = 87, ClassName = "MeshParticleEmitter" });
        Classes.Add(new ClassItem { ID = 88, ClassName = "ParticleEmitter" });
        Classes.Add(new ClassItem { ID = 89, ClassName = "Cubemap" });
        Classes.Add(new ClassItem { ID = 90, ClassName = "Avatar" });
        Classes.Add(new ClassItem { ID = 91, ClassName = "AnimatorController" });
        Classes.Add(new ClassItem { ID = 92, ClassName = "GUILayer" });
        Classes.Add(new ClassItem { ID = 93, ClassName = "RuntimeAnimatorController" });
        Classes.Add(new ClassItem { ID = 94, ClassName = "ScriptMapper" });
        Classes.Add(new ClassItem { ID = 95, ClassName = "Animator" });
        Classes.Add(new ClassItem { ID = 96, ClassName = "TrailRenderer" });
        Classes.Add(new ClassItem { ID = 98, ClassName = "DelayedCallManager" });
        Classes.Add(new ClassItem { ID = 102, ClassName = "TextMesh" });
        Classes.Add(new ClassItem { ID = 104, ClassName = "RenderSettings" });
        Classes.Add(new ClassItem { ID = 108, ClassName = "Light" });
        Classes.Add(new ClassItem { ID = 109, ClassName = "CGProgram" });
        Classes.Add(new ClassItem { ID = 110, ClassName = "BaseAnimationTrack" });
        Classes.Add(new ClassItem { ID = 111, ClassName = "Animation" });
        Classes.Add(new ClassItem { ID = 114, ClassName = "MonoBehaviour" });
        Classes.Add(new ClassItem { ID = 115, ClassName = "MonoScript" });
        Classes.Add(new ClassItem { ID = 116, ClassName = "MonoManager" });
        Classes.Add(new ClassItem { ID = 117, ClassName = "Texture3D" });
        Classes.Add(new ClassItem { ID = 118, ClassName = "NewAnimationTrack" });
        Classes.Add(new ClassItem { ID = 119, ClassName = "Projector" });
        Classes.Add(new ClassItem { ID = 120, ClassName = "LineRenderer" });
        Classes.Add(new ClassItem { ID = 121, ClassName = "Flare" });
        Classes.Add(new ClassItem { ID = 122, ClassName = "Halo" });
        Classes.Add(new ClassItem { ID = 123, ClassName = "LensFlare" });
        Classes.Add(new ClassItem { ID = 124, ClassName = "FlareLayer" });
        Classes.Add(new ClassItem { ID = 125, ClassName = "HaloLayer" });
        Classes.Add(new ClassItem { ID = 126, ClassName = "NavMeshAreas" });
        Classes.Add(new ClassItem { ID = 127, ClassName = "HaloManager" });
        Classes.Add(new ClassItem { ID = 128, ClassName = "Font" });
        Classes.Add(new ClassItem { ID = 129, ClassName = "PlayerSettings" });
        Classes.Add(new ClassItem { ID = 130, ClassName = "NamedObject" });
        Classes.Add(new ClassItem { ID = 131, ClassName = "GUITexture" });
        Classes.Add(new ClassItem { ID = 132, ClassName = "GUIText" });
        Classes.Add(new ClassItem { ID = 133, ClassName = "GUIElement" });
        Classes.Add(new ClassItem { ID = 134, ClassName = "PhysicMaterial" });
        Classes.Add(new ClassItem { ID = 135, ClassName = "SphereCollider" });
        Classes.Add(new ClassItem { ID = 136, ClassName = "CapsuleCollider" });
        Classes.Add(new ClassItem { ID = 137, ClassName = "SkinnedMeshRenderer" });
        Classes.Add(new ClassItem { ID = 138, ClassName = "FixedJoint" });
        Classes.Add(new ClassItem { ID = 141, ClassName = "BuildSettings" });
        Classes.Add(new ClassItem { ID = 142, ClassName = "AssetBundle" });
        Classes.Add(new ClassItem { ID = 143, ClassName = "CharacterController" });
        Classes.Add(new ClassItem { ID = 144, ClassName = "CharacterJoint" });
        Classes.Add(new ClassItem { ID = 145, ClassName = "SpringJoint" });
        Classes.Add(new ClassItem { ID = 146, ClassName = "WheelCollider" });
        Classes.Add(new ClassItem { ID = 147, ClassName = "ResourceManager" });
        Classes.Add(new ClassItem { ID = 148, ClassName = "NetworkView" });
        Classes.Add(new ClassItem { ID = 149, ClassName = "NetworkManager" });
        Classes.Add(new ClassItem { ID = 150, ClassName = "EllipsoidParticleEmitter" });
        Classes.Add(new ClassItem { ID = 152, ClassName = "ParticleAnimator" });
        Classes.Add(new ClassItem { ID = 153, ClassName = "ParticleRenderer" });
        Classes.Add(new ClassItem { ID = 154, ClassName = "Shader" });
        Classes.Add(new ClassItem { ID = 156, ClassName = "TerrainCollider" });
        Classes.Add(new ClassItem { ID = 157, ClassName = "TerrainData" });
        Classes.Add(new ClassItem { ID = 158, ClassName = "LightmapSettings" });
        Classes.Add(new ClassItem { ID = 159, ClassName = "WebCamTexture" });
        Classes.Add(new ClassItem { ID = 160, ClassName = "EditorSettings" });
        Classes.Add(new ClassItem { ID = 162, ClassName = "EditorUserSettings" });
        Classes.Add(new ClassItem { ID = 164, ClassName = "AudioReverbFilter" });
        Classes.Add(new ClassItem { ID = 165, ClassName = "AudioHighPassFilter" });
        Classes.Add(new ClassItem { ID = 166, ClassName = "AudioChorusFilter" });
        Classes.Add(new ClassItem { ID = 167, ClassName = "AudioReverbZone" });
        Classes.Add(new ClassItem { ID = 168, ClassName = "AudioEchoFilter" });
        Classes.Add(new ClassItem { ID = 169, ClassName = "AudioLowPassFilter" });
        Classes.Add(new ClassItem { ID = 170, ClassName = "AudioDistortionFilter" });
        Classes.Add(new ClassItem { ID = 171, ClassName = "SparseTexture" });
        Classes.Add(new ClassItem { ID = 180, ClassName = "AudioBehaviour" });
        Classes.Add(new ClassItem { ID = 181, ClassName = "AudioFilter" });
        Classes.Add(new ClassItem { ID = 182, ClassName = "WindZone" });
        Classes.Add(new ClassItem { ID = 183, ClassName = "Cloth" });
        Classes.Add(new ClassItem { ID = 184, ClassName = "SubstanceArchive" });
        Classes.Add(new ClassItem { ID = 185, ClassName = "ProceduralMaterial" });
        Classes.Add(new ClassItem { ID = 186, ClassName = "ProceduralTexture" });
        Classes.Add(new ClassItem { ID = 187, ClassName = "Texture2DArray" });
        Classes.Add(new ClassItem { ID = 188, ClassName = "CubemapArray" });
        Classes.Add(new ClassItem { ID = 191, ClassName = "OffMeshLink" });
        Classes.Add(new ClassItem { ID = 192, ClassName = "OcclusionArea" });
        Classes.Add(new ClassItem { ID = 193, ClassName = "Tree" });
        Classes.Add(new ClassItem { ID = 194, ClassName = "NavMeshObstacle" });
        Classes.Add(new ClassItem { ID = 195, ClassName = "NavMeshAgent" });
        Classes.Add(new ClassItem { ID = 196, ClassName = "NavMeshSettings" });
        Classes.Add(new ClassItem { ID = 197, ClassName = "LightProbesLegacy" });
        Classes.Add(new ClassItem { ID = 198, ClassName = "ParticleSystem" });
        Classes.Add(new ClassItem { ID = 199, ClassName = "ParticleSystemRenderer" });
        Classes.Add(new ClassItem { ID = 200, ClassName = "ShaderVariantCollection" });
        Classes.Add(new ClassItem { ID = 205, ClassName = "LODGroup" });
        Classes.Add(new ClassItem { ID = 206, ClassName = "BlendTree" });
        Classes.Add(new ClassItem { ID = 207, ClassName = "Motion" });
        Classes.Add(new ClassItem { ID = 208, ClassName = "NavMeshObstacle" });
        Classes.Add(new ClassItem { ID = 210, ClassName = "SortingGroup" });
        Classes.Add(new ClassItem { ID = 212, ClassName = "Sprite" });
        Classes.Add(new ClassItem { ID = 213, ClassName = "CachedSpriteAtlas" });
        Classes.Add(new ClassItem { ID = 214, ClassName = "ReflectionProbe" });
        Classes.Add(new ClassItem { ID = 215, ClassName = "ReflectionProbes" });
        Classes.Add(new ClassItem { ID = 218, ClassName = "Terrain" });
        Classes.Add(new ClassItem { ID = 220, ClassName = "LightProbeGroup" });
        Classes.Add(new ClassItem { ID = 221, ClassName = "AnimatorOverrideController" });
        Classes.Add(new ClassItem { ID = 222, ClassName = "CanvasRenderer" });
        Classes.Add(new ClassItem { ID = 223, ClassName = "Canvas" });
        Classes.Add(new ClassItem { ID = 224, ClassName = "RectTransform" });
        Classes.Add(new ClassItem { ID = 225, ClassName = "CanvasGroup" });
        Classes.Add(new ClassItem { ID = 226, ClassName = "BillboardAsset" });
        Classes.Add(new ClassItem { ID = 227, ClassName = "BillboardRenderer" });
        Classes.Add(new ClassItem { ID = 228, ClassName = "SpeedTreeWindAsset" });
        Classes.Add(new ClassItem { ID = 229, ClassName = "AnchoredJoint2D" });
        Classes.Add(new ClassItem { ID = 230, ClassName = "Joint2D" });
        Classes.Add(new ClassItem { ID = 231, ClassName = "SpringJoint2D" });
        Classes.Add(new ClassItem { ID = 232, ClassName = "DistanceJoint2D" });
        Classes.Add(new ClassItem { ID = 233, ClassName = "HingeJoint2D" });
        Classes.Add(new ClassItem { ID = 234, ClassName = "SliderJoint2D" });
        Classes.Add(new ClassItem { ID = 235, ClassName = "WheelJoint2D" });
        Classes.Add(new ClassItem { ID = 236, ClassName = "ClusterInputManager" });
        Classes.Add(new ClassItem { ID = 237, ClassName = "BaseVideoTexture" });
        Classes.Add(new ClassItem { ID = 238, ClassName = "NavMeshData" });
        Classes.Add(new ClassItem { ID = 240, ClassName = "AudioMixer" });
        Classes.Add(new ClassItem { ID = 241, ClassName = "AudioMixerController" });
        Classes.Add(new ClassItem { ID = 243, ClassName = "AudioMixerGroupController" });
        Classes.Add(new ClassItem { ID = 244, ClassName = "AudioMixerEffectController" });
        Classes.Add(new ClassItem { ID = 245, ClassName = "AudioMixerSnapshotController" });
        Classes.Add(new ClassItem { ID = 246, ClassName = "PhysicsUpdateBehaviour2D" });
        Classes.Add(new ClassItem { ID = 247, ClassName = "ConstantForce2D" });
        Classes.Add(new ClassItem { ID = 248, ClassName = "Effector2D" });
        Classes.Add(new ClassItem { ID = 249, ClassName = "AreaEffector2D" });
        Classes.Add(new ClassItem { ID = 250, ClassName = "PointEffector2D" });
        Classes.Add(new ClassItem { ID = 251, ClassName = "PlatformEffector2D" });
        Classes.Add(new ClassItem { ID = 252, ClassName = "SurfaceEffector2D" });
        Classes.Add(new ClassItem { ID = 253, ClassName = "BuoyancyEffector2D" });
        Classes.Add(new ClassItem { ID = 254, ClassName = "RelativeJoint2D" });
        Classes.Add(new ClassItem { ID = 255, ClassName = "FixedJoint2D" });
        Classes.Add(new ClassItem { ID = 256, ClassName = "FrictionJoint2D" });
        Classes.Add(new ClassItem { ID = 257, ClassName = "TargetJoint2D" });
        Classes.Add(new ClassItem { ID = 258, ClassName = "LightProbes" });
        Classes.Add(new ClassItem { ID = 259, ClassName = "LightProbeProxyVolume" });
    }
    
    public string SearchableDisplayName => "Class List";
    
    public bool PerformSearch(string searchText, SearchMethod searchMethod, IncludeExcludeMode includeMode)
    {
        if (string.IsNullOrWhiteSpace(searchText))
        {
            ClearSearch();
            return true;
        }
        
        try
        {
            var filteredClasses = Classes.Where(c => 
                searchMethod switch
                {
                    SearchMethod.Contains => c.ClassName.Contains(searchText, StringComparison.OrdinalIgnoreCase),
                    SearchMethod.Exact => c.ClassName.Equals(searchText, StringComparison.OrdinalIgnoreCase),
                    SearchMethod.Fuzzy => c.ClassName.Contains(searchText, StringComparison.OrdinalIgnoreCase), // Simple fuzzy for now
                    SearchMethod.Regex => System.Text.RegularExpressions.Regex.IsMatch(c.ClassName, searchText, System.Text.RegularExpressions.RegexOptions.IgnoreCase),
                    _ => c.ClassName.Contains(searchText, StringComparison.OrdinalIgnoreCase)
                }
            );
            
            var finalResult = includeMode == IncludeExcludeMode.Include ? filteredClasses : Classes.Except(filteredClasses);
            
            CollectionView = new DataGridCollectionView(new ObservableCollection<ClassItem>(finalResult));
            
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
    
    public void ClearSearch()
    {
        CollectionView = new DataGridCollectionView(Classes);
    }
}
