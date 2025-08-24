using System;
using System.Collections.Generic;
using AssetStudio.GUI.Models.Documents;
using AssetStudio.GUI.Models.Panels;
using AssetStudio.GUI.ViewModels.Documents;
using AssetStudio.GUI.ViewModels.Panels;
using Dock.Avalonia.Controls;
using Dock.Model.Controls;
using Dock.Model.Core;
using Dock.Model.Mvvm;
using Dock.Model.Mvvm.Controls;

namespace AssetStudio.GUI.ViewModels;

internal class MainDockFactory : Factory
{
    private AssetListDocumentViewModel? _assetListDocument;
    private ClassListDocumentViewModel? _classListDocument;
    private DumpPanelViewModel? _dumpPanel;
    private IDocumentDock? _fileDocumentDock;
    private PreviewPanelViewModel? _previewPanel;
    private IRootDock? _rootDock;
    private SceneHierarchyPanelViewModel? _sceneHierarchyPanel;

    public Func<long, string, TreeNodeItem?>? AssetDumpFunction { get; set; }

    public override IRootDock CreateLayout()
    {
        _sceneHierarchyPanel = new SceneHierarchyPanelViewModel();
        _assetListDocument = new AssetListDocumentViewModel();
        _classListDocument = new ClassListDocumentViewModel();

        _previewPanel = new PreviewPanelViewModel();
        _dumpPanel = new DumpPanelViewModel();

        // Setup asset selection event
        _assetListDocument.AssetSelected += OnAssetSelected;

        var documentDock = _fileDocumentDock = new DocumentDock
        {
            ActiveDockable = _assetListDocument,
            VisibleDockables = CreateList<IDockable>
            (
                _assetListDocument,
                _classListDocument
            ),
            CanCreateDocument = false,
            IsCollapsable = false,
            Proportion = double.NaN
        };

        var leftToolDock = new ToolDock
        {
            ActiveDockable = _sceneHierarchyPanel,
            VisibleDockables = CreateList<IDockable>
            (
                _sceneHierarchyPanel
            ),
            Alignment = Alignment.Left,
            GripMode = GripMode.Visible,
            Proportion = 0.25
        };

        var rightToolDock = new ToolDock
        {
            ActiveDockable = _previewPanel,
            VisibleDockables = CreateList<IDockable>
            (
                _previewPanel,
                _dumpPanel
            ),
            Alignment = Alignment.Right,
            GripMode = GripMode.Visible,
            Proportion = 0.25
        };

        var mainPane = new ProportionalDock
        {
            Orientation = Orientation.Horizontal,
            IsCollapsable = false,
            VisibleDockables = CreateList<IDockable>
            (
                leftToolDock,
                new ProportionalDockSplitter(),
                documentDock,
                new ProportionalDockSplitter(),
                rightToolDock
            )
        };

        var windowLayout = CreateRootDock();
        windowLayout.Title = "AssetStudio";
        windowLayout.IsCollapsable = false;
        windowLayout.VisibleDockables = CreateList<IDockable>(mainPane);
        windowLayout.ActiveDockable = mainPane;

        _rootDock = CreateRootDock();
        _rootDock.IsCollapsable = false;
        _rootDock.VisibleDockables = CreateList<IDockable>(windowLayout);
        _rootDock.ActiveDockable = windowLayout;
        _rootDock.DefaultDockable = windowLayout;

        return _rootDock;
    }

    public override void InitLayout(IDockable layout)
    {
        ContextLocator = new Dictionary<string, Func<object?>>();

        DockableLocator = new Dictionary<string, Func<IDockable?>>
        {
            ["Root"] = () => _rootDock,
            ["Files"] = () => _fileDocumentDock,
            ["SceneHierarchy"] = () => _sceneHierarchyPanel,
            ["AssetList"] = () => _assetListDocument,
            ["ClassList"] = () => _classListDocument,
            ["Preview"] = () => _previewPanel,
            ["Dump"] = () => _dumpPanel
        };

        HostWindowLocator = new Dictionary<string, Func<IHostWindow?>>
        {
            [nameof(IDockWindow)] = () => new HostWindow()
        };

        base.InitLayout(layout);
    }

    public void UpdateAssetData(List<AssetItem> assets, List<ClassItem> classes, List<TreeNodeItem> sceneHierarchy)
    {
        _assetListDocument?.UpdateData(assets);
        _classListDocument?.UpdateData(classes);
        _sceneHierarchyPanel?.UpdateData(sceneHierarchy);
    }

    private void OnAssetSelected(AssetItem asset)
    {
        if (_dumpPanel == null || AssetDumpFunction == null) return;
        var dumpTree = AssetDumpFunction(asset.PathId, asset.Container);
        _dumpPanel.UpdateDump(dumpTree);
    }
}