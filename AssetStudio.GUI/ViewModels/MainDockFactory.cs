using Dock.Avalonia.Controls;
using Dock.Model.Controls;
using Dock.Model.Core;
using Dock.Model.Mvvm;
using Dock.Model.Mvvm.Controls;
using System;
using System.Collections.Generic;
using AssetStudio.GUI.ViewModels.Panels;
using AssetStudio.GUI.ViewModels.Documents;
using AssetStudio.GUI.Views.Panels;
using AssetStudio.GUI.Views.Documents;

namespace AssetStudio.GUI.ViewModels;

internal class MainDockFactory : Factory
{
    private readonly MainWindowViewModel _mainWindow;
    private IRootDock? _rootDock;
    private IDocumentDock? _fileDocumentDock;
    private SceneHierarchyPanelViewModel? _sceneHierarchyPanel;
    private AssetListDocumentViewModel? _assetListDocument;
    private ClassListDocumentViewModel? _classListDocument;
    private PreviewPanelViewModel? _previewPanel;
    private DumpPanelViewModel? _dumpPanel;

    public MainDockFactory(MainWindowViewModel mainWindow)
    {
        _mainWindow = mainWindow;
    }

    public override IRootDock CreateLayout()
    {
        _sceneHierarchyPanel = new SceneHierarchyPanelViewModel();
        _assetListDocument = new AssetListDocumentViewModel { MainWindow = _mainWindow };
        _classListDocument = new ClassListDocumentViewModel();
        
        _assetListDocument.UpdateAvailableAssetTypes();
        _previewPanel = new PreviewPanelViewModel();
        _dumpPanel = new DumpPanelViewModel();



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
        ContextLocator = new Dictionary<string, Func<object?>>
        {
        };

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
}