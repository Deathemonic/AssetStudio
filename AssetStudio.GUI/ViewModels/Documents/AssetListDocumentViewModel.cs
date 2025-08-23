using System.Collections.ObjectModel;
using AssetStudio.GUI.Models.Documents;
using CommunityToolkit.Mvvm.ComponentModel;
using Dock.Model.Mvvm.Controls;
using Avalonia.Collections;
using System.Linq;

namespace AssetStudio.GUI.ViewModels.Documents;

public partial class AssetListDocumentViewModel : Document
{
    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private int _selectedSearchFilterMode = 0;

    [ObservableProperty]
    private DataGridCollectionView _collectionView;

    public ObservableCollection<AssetItem> Assets { get; } = new();
    public ObservableCollection<string> SearchHistory { get; } = new();
    
    public MainWindowViewModel? MainWindow { get; set; }

    public AssetListDocumentViewModel()
    {
        Id = "AssetList";
        Title = "Assets";
        CanClose = false;
        InitializeSampleData();
        CollectionView = new DataGridCollectionView(Assets);
    }

    private void InitializeSampleData()
    {
        // Sample asset data - simulating a loaded AssetBundle
        
        // GameObjects
        Assets.Add(new AssetItem { Name = "Player", Container = "sharedassets0.assets", Type = "GameObject", PathID = 1001, Size = 2048 });
        Assets.Add(new AssetItem { Name = "Enemy_Goblin", Container = "sharedassets0.assets", Type = "GameObject", PathID = 1002, Size = 1856 });
        Assets.Add(new AssetItem { Name = "Weapon_Sword", Container = "sharedassets0.assets", Type = "GameObject", PathID = 1003, Size = 1024 });
        Assets.Add(new AssetItem { Name = "UI_MainMenu", Container = "sharedassets0.assets", Type = "GameObject", PathID = 1004, Size = 3072 });
        
        // Textures
        Assets.Add(new AssetItem { Name = "player_diffuse", Container = "sharedassets1.assets", Type = "Texture2D", PathID = 2001, Size = 1048576 });
        Assets.Add(new AssetItem { Name = "player_normal", Container = "sharedassets1.assets", Type = "Texture2D", PathID = 2002, Size = 524288 });
        Assets.Add(new AssetItem { Name = "enemy_atlas", Container = "sharedassets1.assets", Type = "Texture2D", PathID = 2003, Size = 2097152 });
        Assets.Add(new AssetItem { Name = "ui_background", Container = "sharedassets1.assets", Type = "Texture2D", PathID = 2004, Size = 262144 });
        Assets.Add(new AssetItem { Name = "weapon_icons", Container = "sharedassets1.assets", Type = "Texture2D", PathID = 2005, Size = 131072 });
        
        // Materials
        Assets.Add(new AssetItem { Name = "PlayerMaterial", Container = "sharedassets2.assets", Type = "Material", PathID = 3001, Size = 4096 });
        Assets.Add(new AssetItem { Name = "EnemyMaterial", Container = "sharedassets2.assets", Type = "Material", PathID = 3002, Size = 3584 });
        Assets.Add(new AssetItem { Name = "WeaponMaterial", Container = "sharedassets2.assets", Type = "Material", PathID = 3003, Size = 2048 });
        
        // Audio
        Assets.Add(new AssetItem { Name = "bgm_menu", Container = "sharedassets3.assets", Type = "AudioClip", PathID = 4001, Size = 5242880 });
        Assets.Add(new AssetItem { Name = "sfx_sword_swing", Container = "sharedassets3.assets", Type = "AudioClip", PathID = 4002, Size = 98304 });
        Assets.Add(new AssetItem { Name = "sfx_enemy_hit", Container = "sharedassets3.assets", Type = "AudioClip", PathID = 4003, Size = 65536 });
        Assets.Add(new AssetItem { Name = "voice_player_hurt", Container = "sharedassets3.assets", Type = "AudioClip", PathID = 4004, Size = 131072 });
        
        // Meshes
        Assets.Add(new AssetItem { Name = "PlayerMesh", Container = "sharedassets4.assets", Type = "Mesh", PathID = 5001, Size = 45632 });
        Assets.Add(new AssetItem { Name = "SwordMesh", Container = "sharedassets4.assets", Type = "Mesh", PathID = 5002, Size = 12288 });
        Assets.Add(new AssetItem { Name = "GoblinMesh", Container = "sharedassets4.assets", Type = "Mesh", PathID = 5003, Size = 38912 });
        
        // Scripts
        Assets.Add(new AssetItem { Name = "PlayerController", Container = "sharedassets5.assets", Type = "MonoBehaviour", PathID = 6001, Size = 8192 });
        Assets.Add(new AssetItem { Name = "EnemyAI", Container = "sharedassets5.assets", Type = "MonoBehaviour", PathID = 6002, Size = 6144 });
        Assets.Add(new AssetItem { Name = "GameManager", Container = "sharedassets5.assets", Type = "MonoBehaviour", PathID = 6003, Size = 4096 });
        Assets.Add(new AssetItem { Name = "UIManager", Container = "sharedassets5.assets", Type = "MonoBehaviour", PathID = 6004, Size = 7168 });
        
        // Animations
        Assets.Add(new AssetItem { Name = "player_idle", Container = "sharedassets6.assets", Type = "AnimationClip", PathID = 7001, Size = 16384 });
        Assets.Add(new AssetItem { Name = "player_walk", Container = "sharedassets6.assets", Type = "AnimationClip", PathID = 7002, Size = 24576 });
        Assets.Add(new AssetItem { Name = "player_attack", Container = "sharedassets6.assets", Type = "AnimationClip", PathID = 7003, Size = 20480 });
        Assets.Add(new AssetItem { Name = "enemy_death", Container = "sharedassets6.assets", Type = "AnimationClip", PathID = 7004, Size = 18432 });

        // Sample search history
        SearchHistory.Add("Texture2D");
        SearchHistory.Add("GameObject");
        SearchHistory.Add("AudioClip");
        SearchHistory.Add("player");
        SearchHistory.Add("enemy");
    }
    
    public void UpdateAvailableAssetTypes()
    {
        if (MainWindow != null)
        {
            var assetTypes = Assets.Select(a => a.Type).Distinct();
            MainWindow.UpdateAvailableAssetTypes(assetTypes);
        }
    }
}
