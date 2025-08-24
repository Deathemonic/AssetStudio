using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using AssetStudio.GUI.Logic;
using AssetStudio.GUI.Models.Documents;
using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using Dock.Model.Mvvm.Controls;

namespace AssetStudio.GUI.ViewModels.Documents;

public partial class AssetListDocumentViewModel : Document
{
    [ObservableProperty] private DataGridCollectionView _collectionView;

    [ObservableProperty] private string _searchText = string.Empty;

    [ObservableProperty] private int _selectedSearchFilterMode;

    public AssetListDocumentViewModel()
    {
        Id = "AssetList";
        Title = "Assets";
        CanClose = false;
        InitializeSampleData();
        CollectionView = new DataGridCollectionView(Assets);

        PropertyChanged += OnPropertyChanged;
    }

    public ObservableCollection<AssetItem> Assets { get; } = new();
    public ObservableCollection<string> SearchHistory { get; } = new();

    public MainWindowViewModel? MainWindow { get; set; }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(SearchText):
                PerformSearch(SearchText, (SearchMethod)SelectedSearchFilterMode, IncludeExcludeMode.Include);
                break;
            case nameof(SelectedSearchFilterMode):
            {
                if (!string.IsNullOrWhiteSpace(SearchText))
                    PerformSearch(SearchText, (SearchMethod)SelectedSearchFilterMode, IncludeExcludeMode.Include);
                break;
            }
        }
    }

    private void InitializeSampleData()
    {
        // Sample asset data - simulating a loaded AssetBundle

        // GameObjects
        Assets.Add(new AssetItem
            { Name = "Player", Container = "sharedassets0.assets", Type = "GameObject", PathId = 1001, Size = 2048 });
        Assets.Add(new AssetItem
        {
            Name = "Enemy_Goblin", Container = "sharedassets0.assets", Type = "GameObject", PathId = 1002, Size = 1856
        });
        Assets.Add(new AssetItem
        {
            Name = "Weapon_Sword", Container = "sharedassets0.assets", Type = "GameObject", PathId = 1003, Size = 1024
        });
        Assets.Add(new AssetItem
        {
            Name = "UI_MainMenu", Container = "sharedassets0.assets", Type = "GameObject", PathId = 1004, Size = 3072
        });

        // Textures
        Assets.Add(new AssetItem
        {
            Name = "player_diffuse", Container = "sharedassets1.assets", Type = "Texture2D", PathId = 2001,
            Size = 1048576
        });
        Assets.Add(new AssetItem
        {
            Name = "player_normal", Container = "sharedassets1.assets", Type = "Texture2D", PathId = 2002, Size = 524288
        });
        Assets.Add(new AssetItem
        {
            Name = "enemy_atlas", Container = "sharedassets1.assets", Type = "Texture2D", PathId = 2003, Size = 2097152
        });
        Assets.Add(new AssetItem
        {
            Name = "ui_background", Container = "sharedassets1.assets", Type = "Texture2D", PathId = 2004, Size = 262144
        });
        Assets.Add(new AssetItem
        {
            Name = "weapon_icons", Container = "sharedassets1.assets", Type = "Texture2D", PathId = 2005, Size = 131072
        });

        // Materials
        Assets.Add(new AssetItem
        {
            Name = "PlayerMaterial", Container = "sharedassets2.assets", Type = "Material", PathId = 3001, Size = 4096
        });
        Assets.Add(new AssetItem
        {
            Name = "EnemyMaterial", Container = "sharedassets2.assets", Type = "Material", PathId = 3002, Size = 3584
        });
        Assets.Add(new AssetItem
        {
            Name = "WeaponMaterial", Container = "sharedassets2.assets", Type = "Material", PathId = 3003, Size = 2048
        });

        // Audio
        Assets.Add(new AssetItem
        {
            Name = "bgm_menu", Container = "sharedassets3.assets", Type = "AudioClip", PathId = 4001, Size = 5242880
        });
        Assets.Add(new AssetItem
        {
            Name = "sfx_sword_swing", Container = "sharedassets3.assets", Type = "AudioClip", PathId = 4002,
            Size = 98304
        });
        Assets.Add(new AssetItem
        {
            Name = "sfx_enemy_hit", Container = "sharedassets3.assets", Type = "AudioClip", PathId = 4003, Size = 65536
        });
        Assets.Add(new AssetItem
        {
            Name = "voice_player_hurt", Container = "sharedassets3.assets", Type = "AudioClip", PathId = 4004,
            Size = 131072
        });

        // Meshes
        Assets.Add(new AssetItem
            { Name = "PlayerMesh", Container = "sharedassets4.assets", Type = "Mesh", PathId = 5001, Size = 45632 });
        Assets.Add(new AssetItem
            { Name = "SwordMesh", Container = "sharedassets4.assets", Type = "Mesh", PathId = 5002, Size = 12288 });
        Assets.Add(new AssetItem
            { Name = "GoblinMesh", Container = "sharedassets4.assets", Type = "Mesh", PathId = 5003, Size = 38912 });

        // Scripts
        Assets.Add(new AssetItem
        {
            Name = "PlayerController", Container = "sharedassets5.assets", Type = "MonoBehaviour", PathId = 6001,
            Size = 8192
        });
        Assets.Add(new AssetItem
        {
            Name = "EnemyAI", Container = "sharedassets5.assets", Type = "MonoBehaviour", PathId = 6002, Size = 6144
        });
        Assets.Add(new AssetItem
        {
            Name = "GameManager", Container = "sharedassets5.assets", Type = "MonoBehaviour", PathId = 6003, Size = 4096
        });
        Assets.Add(new AssetItem
        {
            Name = "UIManager", Container = "sharedassets5.assets", Type = "MonoBehaviour", PathId = 6004, Size = 7168
        });

        // Animations
        Assets.Add(new AssetItem
        {
            Name = "player_idle", Container = "sharedassets6.assets", Type = "AnimationClip", PathId = 7001,
            Size = 16384
        });
        Assets.Add(new AssetItem
        {
            Name = "player_walk", Container = "sharedassets6.assets", Type = "AnimationClip", PathId = 7002,
            Size = 24576
        });
        Assets.Add(new AssetItem
        {
            Name = "player_attack", Container = "sharedassets6.assets", Type = "AnimationClip", PathId = 7003,
            Size = 20480
        });
        Assets.Add(new AssetItem
        {
            Name = "enemy_death", Container = "sharedassets6.assets", Type = "AnimationClip", PathId = 7004,
            Size = 18432
        });

        // Sample search history
        SearchHistory.Add("Texture2D");
        SearchHistory.Add("GameObject");
        SearchHistory.Add("AudioClip");
        SearchHistory.Add("player");
        SearchHistory.Add("enemy");
    }

    public bool PerformSearch(string searchText, SearchMethod searchMethod, IncludeExcludeMode includeMode)
    {
        if (string.IsNullOrWhiteSpace(searchText))
        {
            ClearSearch();
            return true;
        }

        try
        {
            var filteredAssets = AssetSearch.FilterAssets(Assets, searchText, searchMethod, includeMode);
            CollectionView = new DataGridCollectionView(new ObservableCollection<AssetItem>(filteredAssets));

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public void ClearSearch()
    {
        CollectionView = new DataGridCollectionView(Assets);
    }
}