using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AssetStudio.GUI.Logic;
using AssetStudio.GUI.Models.Panels;
using CommunityToolkit.Mvvm.ComponentModel;
using Dock.Model.Mvvm.Controls;

namespace AssetStudio.GUI.ViewModels.Panels;

public partial class SceneHierarchyPanelViewModel : Tool
{
    [ObservableProperty] private string _searchText = string.Empty;

    public SceneHierarchyPanelViewModel()
    {
        Id = "SceneHierarchy";
        Title = "Scene Hierarchy";
        CanClose = false;
        InitializeSampleData();

        PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(SearchText)) PerformSearch();
        };
    }

    public ObservableCollection<TreeNodeItem> SceneHierarchy { get; } = [];

    private void InitializeSampleData()
    {
        // Sample scene hierarchy data - simulating a loaded Unity scene
        var sceneRoot = new TreeNodeItem { Name = "SampleScene" };

        // Main Camera
        var mainCamera = new TreeNodeItem { Name = "Main Camera" };

        // Directional Light
        var directionalLight = new TreeNodeItem { Name = "Directional Light" };

        // Player GameObject with components
        var player = new TreeNodeItem { Name = "Player" };
        var playerModel = new TreeNodeItem { Name = "PlayerModel" };
        var playerWeapon = new TreeNodeItem { Name = "Weapon" };
        var sword = new TreeNodeItem { Name = "Sword" };
        playerWeapon.Children.Add(sword);
        player.Children.Add(playerModel);
        player.Children.Add(playerWeapon);

        // Enemies folder
        var enemies = new TreeNodeItem { Name = "Enemies" };
        var goblin1 = new TreeNodeItem { Name = "Goblin_001" };
        var goblin2 = new TreeNodeItem { Name = "Goblin_002" };
        var goblin3 = new TreeNodeItem { Name = "Goblin_003" };
        var orc = new TreeNodeItem { Name = "Orc_Boss" };
        var orcWeapon = new TreeNodeItem { Name = "OrcAxe" };
        orc.Children.Add(orcWeapon);
        enemies.Children.Add(goblin1);
        enemies.Children.Add(goblin2);
        enemies.Children.Add(goblin3);
        enemies.Children.Add(orc);

        // Environment
        var environment = new TreeNodeItem { Name = "Environment" };

        // Terrain
        var terrain = new TreeNodeItem { Name = "Terrain" };
        var terrainMesh = new TreeNodeItem { Name = "TerrainMesh" };
        terrain.Children.Add(terrainMesh);

        // Buildings
        var buildings = new TreeNodeItem { Name = "Buildings" };
        var castle = new TreeNodeItem { Name = "Castle" };
        var castleTower1 = new TreeNodeItem { Name = "Tower_01" };
        var castleTower2 = new TreeNodeItem { Name = "Tower_02" };
        var castleWalls = new TreeNodeItem { Name = "Walls" };
        var gate = new TreeNodeItem { Name = "Gate" };
        castle.Children.Add(castleTower1);
        castle.Children.Add(castleTower2);
        castle.Children.Add(castleWalls);
        castle.Children.Add(gate);

        var village = new TreeNodeItem { Name = "Village" };
        var house1 = new TreeNodeItem { Name = "House_01" };
        var house2 = new TreeNodeItem { Name = "House_02" };
        var house3 = new TreeNodeItem { Name = "House_03" };
        var well = new TreeNodeItem { Name = "Well" };
        village.Children.Add(house1);
        village.Children.Add(house2);
        village.Children.Add(house3);
        village.Children.Add(well);

        buildings.Children.Add(castle);
        buildings.Children.Add(village);

        // Props
        var props = new TreeNodeItem { Name = "Props" };
        var barrels = new TreeNodeItem { Name = "Barrels" };
        var barrel1 = new TreeNodeItem { Name = "Barrel_01" };
        var barrel2 = new TreeNodeItem { Name = "Barrel_02" };
        var barrel3 = new TreeNodeItem { Name = "Barrel_03" };
        barrels.Children.Add(barrel1);
        barrels.Children.Add(barrel2);
        barrels.Children.Add(barrel3);

        var crates = new TreeNodeItem { Name = "Crates" };
        var crate1 = new TreeNodeItem { Name = "WoodCrate_01" };
        var crate2 = new TreeNodeItem { Name = "WoodCrate_02" };
        crates.Children.Add(crate1);
        crates.Children.Add(crate2);

        props.Children.Add(barrels);
        props.Children.Add(crates);

        environment.Children.Add(terrain);
        environment.Children.Add(buildings);
        environment.Children.Add(props);

        // UI Canvas
        var uiCanvas = new TreeNodeItem { Name = "UI Canvas" };
        var mainMenu = new TreeNodeItem { Name = "MainMenu" };
        var titleText = new TreeNodeItem { Name = "TitleText" };
        var startButton = new TreeNodeItem { Name = "StartButton" };
        var optionsButton = new TreeNodeItem { Name = "OptionsButton" };
        var exitButton = new TreeNodeItem { Name = "ExitButton" };
        mainMenu.Children.Add(titleText);
        mainMenu.Children.Add(startButton);
        mainMenu.Children.Add(optionsButton);
        mainMenu.Children.Add(exitButton);

        var gameHud = new TreeNodeItem { Name = "GameHUD" };
        var healthBar = new TreeNodeItem { Name = "HealthBar" };
        var manaBar = new TreeNodeItem { Name = "ManaBar" };
        var minimap = new TreeNodeItem { Name = "Minimap" };
        var inventory = new TreeNodeItem { Name = "InventoryPanel" };
        gameHud.Children.Add(healthBar);
        gameHud.Children.Add(manaBar);
        gameHud.Children.Add(minimap);
        gameHud.Children.Add(inventory);

        uiCanvas.Children.Add(mainMenu);
        uiCanvas.Children.Add(gameHud);

        // Audio Sources
        var audioSources = new TreeNodeItem { Name = "Audio" };
        var bgmSource = new TreeNodeItem { Name = "BackgroundMusic" };
        var ambientSource = new TreeNodeItem { Name = "AmbientSounds" };
        var sfxSource = new TreeNodeItem { Name = "SFXSource" };
        audioSources.Children.Add(bgmSource);
        audioSources.Children.Add(ambientSource);
        audioSources.Children.Add(sfxSource);

        // Particle Systems
        var particleSystems = new TreeNodeItem { Name = "ParticleSystems" };
        var fireEffect = new TreeNodeItem { Name = "FireEffect" };
        var magicEffect = new TreeNodeItem { Name = "MagicSparkles" };
        var dustEffect = new TreeNodeItem { Name = "DustParticles" };
        particleSystems.Children.Add(fireEffect);
        particleSystems.Children.Add(magicEffect);
        particleSystems.Children.Add(dustEffect);

        // Add all to scene root
        sceneRoot.Children.Add(mainCamera);
        sceneRoot.Children.Add(directionalLight);
        sceneRoot.Children.Add(player);
        sceneRoot.Children.Add(enemies);
        sceneRoot.Children.Add(environment);
        sceneRoot.Children.Add(uiCanvas);
        sceneRoot.Children.Add(audioSources);
        sceneRoot.Children.Add(particleSystems);

        SceneHierarchy.Add(sceneRoot);
    }

    private void PerformSearch()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            CollapseAllNodes(SceneHierarchy);
            return;
        }

        try
        {
            TreeNodeSearch.ExpandToMatches(SceneHierarchy, SearchText);
            OnPropertyChanged(nameof(SceneHierarchy));
        }
        catch (Exception)
        {
            // Continue
        }
    }

    private void CollapseAllNodes(IEnumerable<TreeNodeItem> nodes)
    {
        foreach (var node in nodes)
        {
            node.IsExpanded = false;
            if (node.Children.Any()) CollapseAllNodes(node.Children);
        }
    }
}