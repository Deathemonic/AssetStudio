using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using static AssetStudio.ImportHelper;

namespace AssetStudio
{
    public class AssetsManager
    {
        public bool LoadingViaTypeTreeEnabled = true;
        public CompressionType CustomBlockCompression = CompressionType.Auto;
        public CompressionType CustomBlockInfoCompression = CompressionType.Auto;
        public List<SerializedFile> assetsFileList = new List<SerializedFile>();

        internal Dictionary<string, int> assetsFileIndexCache = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        internal ConcurrentDictionary<string, BinaryReader> resourceFileReaders = new ConcurrentDictionary<string, BinaryReader>(StringComparer.OrdinalIgnoreCase);

        private UnityVersion specifiedUnityVersion;
        private List<string> importFiles = new List<string>();
        private HashSet<ClassIDType> filteredAssetTypesList = new HashSet<ClassIDType>();
        private HashSet<string> importFilesHash = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private HashSet<string> noexistFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private HashSet<string> assetsFileListHash = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        public UnityVersion SpecifyUnityVersion
        {
            get => specifiedUnityVersion;
            set
            {
                if (specifiedUnityVersion == value)
                {
                    return;
                }
                if (value == null)
                {
                    specifiedUnityVersion = null;
                    Logger.Info("Specified Unity version: None");
                    return;
                }

                if (string.IsNullOrEmpty(value.BuildType))
                {
                    throw new NotSupportedException("Specified Unity version is not in a correct format.\n" +
                        "Specify full Unity version, including letters at the end.\n" +
                        "Example: 2017.4.39f1");
                }

                specifiedUnityVersion = value;
                Logger.Info($"Specified Unity version: {specifiedUnityVersion}");
            }
        }

        public void SetAssetFilter(params ClassIDType[] classIDTypes)
        {
            filteredAssetTypesList.UnionWith(new[]
            {
                ClassIDType.AssetBundle,
                ClassIDType.ResourceManager,
                ClassIDType.GameObject,
                ClassIDType.Transform,
            });

            if (classIDTypes.Contains(ClassIDType.MonoBehaviour))
            {
                filteredAssetTypesList.Add(ClassIDType.MonoScript);
            }
            if (classIDTypes.Contains(ClassIDType.Sprite))
            {
                filteredAssetTypesList.Add(ClassIDType.Texture2D);
                filteredAssetTypesList.Add(ClassIDType.SpriteAtlas);
            }
            if (classIDTypes.Contains(ClassIDType.Animator))
            {
                filteredAssetTypesList.Add(ClassIDType.AnimatorController);
                filteredAssetTypesList.Add(ClassIDType.AnimatorOverrideController);
            }

            filteredAssetTypesList.UnionWith(classIDTypes);
        }

        public void SetAssetFilter(List<ClassIDType> classIDTypeList)
        {
            SetAssetFilter(classIDTypeList.ToArray());
        }

        public void LoadFilesAndFolders(params string[] path)
        {
            LoadFilesAndFolders(out _, path);
        }

        public void LoadFilesAndFolders(out string parentPath, params string[] path)
        {
            var pathList = new List<string>();
            pathList.AddRange(path);
            LoadFilesAndFolders(out parentPath, pathList);
        }

        public void LoadFilesAndFolders(out string parentPath, List<string> pathList)
        {
            var fileList = new List<string>();
            var filesInPath = false;
            parentPath = "";
            foreach (var path in pathList)
            {
                var fullPath = Path.GetFullPath(path);
                if (Directory.Exists(fullPath))
                {
                    var parent = Directory.GetParent(fullPath)?.FullName;
                    if (!filesInPath && (parentPath == "" || parentPath?.Length > parent?.Length))
                    {
                        parentPath = parent;
                    }
                    MergeSplitAssets(fullPath, true);
                    fileList.AddRange(Directory.GetFiles(fullPath, "*.*", SearchOption.AllDirectories));
                }
                else if (File.Exists(fullPath))
                {
                    parentPath = Path.GetDirectoryName(fullPath);
                    fileList.Add(fullPath);
                    filesInPath = true;
                }
            }
            if (filesInPath)
            {
                MergeSplitAssets(parentPath);
            }
            var toReadFile = ProcessingSplitFiles(fileList);
            fileList.Clear();
            pathList.Clear();

            Load(toReadFile);
        }

        private void Load(string[] files)
        {
            foreach (var file in files)
            {
                importFiles.Add(file);
                importFilesHash.Add(Path.GetFileName(file));
            }

            Progress.Reset();
            //use a for loop because list size can change
            for (var i = 0; i < importFiles.Count; i++)
            {
                if (LoadFile(importFiles[i]))
                {
                    Progress.Report(i + 1, importFiles.Count);
                }
                else
                {
                    break;
                }
            }

            importFiles.Clear();
            importFilesHash.Clear();
            noexistFiles.Clear();
            assetsFileListHash.Clear();

            ReadAssets();
            ProcessAssets();
        }

        private bool LoadFile(string fullName)
        {
            var reader = new FileReader(fullName);
            return LoadFile(reader);
        }

        private bool LoadFile(FileReader reader)
        {
            if (reader == null)
                return false;

            switch (reader.FileType)
            {
                case FileType.AssetsFile:
                    return LoadAssetsFile(reader);
                case FileType.BundleFile:
                    return LoadBundleFile(reader);
                case FileType.WebFile:
                    LoadWebFile(reader);
                    break;
                case FileType.GZipFile:
                    LoadFile(DecompressGZip(reader));
                    break;
                case FileType.BrotliFile:
                    LoadFile(DecompressBrotli(reader));
                    break;
                case FileType.ZipFile:
                    LoadZipFile(reader);
                    break;
            }
            return true;
        }

        private bool LoadAssetsFile(FileReader reader)
        {
            if (!assetsFileListHash.Contains(reader.FileName))
            {
                Logger.Info($"Loading \"{reader.FullPath}\"");
                try
                {
                    var assetsFile = new SerializedFile(reader, this);
                    var dirName = Path.GetDirectoryName(reader.FullPath);
                    CheckStrippedVersion(assetsFile);
                    assetsFileList.Add(assetsFile);
                    assetsFileListHash.Add(assetsFile.fileName);

                    foreach (var sharedFile in assetsFile.m_Externals)
                    {
                        var sharedFileName = sharedFile.fileName;

                        if (!importFilesHash.Contains(sharedFileName))
                        {
                            var sharedFilePath = Path.Combine(dirName, sharedFileName);
                            if (!noexistFiles.Contains(sharedFilePath))
                            {
                                if (!File.Exists(sharedFilePath))
                                {
                                    var findFiles = Directory.GetFiles(dirName, sharedFileName, SearchOption.AllDirectories);
                                    if (findFiles.Length > 0)
                                    {
                                        sharedFilePath = findFiles[0];
                                    }
                                }
                                if (File.Exists(sharedFilePath))
                                {
                                    importFiles.Add(sharedFilePath);
                                    importFilesHash.Add(sharedFileName);
                                }
                                else
                                {
                                    noexistFiles.Add(sharedFilePath);
                                    Logger.Warning($"Dependency wasn't found: {sharedFilePath}");
                                }
                            }
                        }
                    }
                }
                catch (NotSupportedException e)
                {
                    Logger.Error(e.Message);
                    reader.Dispose();
                    return false;
                }
                catch (Exception e)
                {
                    Logger.Warning($"Failed to read assets file \"{reader.FullPath}\"\n{e}");
                    reader.Dispose();
                }
            }
            else
            {
                Logger.Info($"Skipping \"{reader.FullPath}\"");
                reader.Dispose();
            }
            return true;
        }

        private bool LoadAssetsFromMemory(FileReader reader, string originalPath, UnityVersion assetBundleUnityVer = null)
        {
            if (!assetsFileListHash.Contains(reader.FileName))
            {
                try
                {
                    var assetsFile = new SerializedFile(reader, this);
                    assetsFile.originalPath = originalPath;
                    if (assetBundleUnityVer != null && assetsFile.header.m_Version < SerializedFileFormatVersion.Unknown_7)
                    {
                        assetsFile.version = assetBundleUnityVer;
                    }
                    CheckStrippedVersion(assetsFile, assetBundleUnityVer);
                    assetsFileList.Add(assetsFile);
                    assetsFileListHash.Add(assetsFile.fileName);
                }
                catch (NotSupportedException e)
                {
                    Logger.Error(e.Message);
                    resourceFileReaders.TryAdd(reader.FileName, reader);
                    return false;
                }
                catch (Exception e)
                {
                    Logger.Warning($"Failed to read assets file \"{reader.FullPath}\" from {Path.GetFileName(originalPath)}\n{e}");
                    resourceFileReaders.TryAdd(reader.FileName, reader);
                }
            }
            else
            {
                Logger.Info($"Skipping \"{originalPath}\" ({reader.FileName})");
            }
            return true;
        }

        private bool LoadBundleFile(FileReader reader, string originalPath = null)
        {
            Logger.Info($"Loading \"{reader.FullPath}\"");
            Logger.Debug($"Bundle offset: {reader.Position}");
            var bundleStream = new OffsetStream(reader);
            var bundleReader = new FileReader(reader.FullPath, bundleStream);
            
            try
            {
                var bundleFile = new BundleFile(bundleReader, CustomBlockInfoCompression, CustomBlockCompression, specifiedUnityVersion);
                var isLoaded = LoadBundleFiles(bundleReader, bundleFile, originalPath);
                if (!isLoaded)
                    return false;

                while (bundleFile.IsMultiBundle && isLoaded)
                {
                    bundleStream.Offset = reader.Position;
                    bundleReader = new FileReader($"{reader.FullPath}_0x{bundleStream.Offset:X}", bundleStream);
                    if (bundleReader.Position > 0)
                    {
                        bundleStream.Offset += bundleReader.Position;
                        bundleReader.FullPath = $"{reader.FullPath}_0x{bundleStream.Offset:X}";
                        bundleReader.FileName = $"{reader.FileName}_0x{bundleStream.Offset:X}";
                    }
                    Logger.Info($"[MultiBundle] Loading \"{reader.FileName}\" from offset: 0x{bundleStream.Offset:X}");
                    bundleFile = new BundleFile(bundleReader, CustomBlockInfoCompression, CustomBlockCompression, specifiedUnityVersion);
                    isLoaded = LoadBundleFiles(bundleReader, bundleFile, originalPath ?? reader.FullPath);
                }
                return isLoaded;
            }
            catch (NotSupportedException e)
            {
                Logger.Error(e.Message);
                return false;
            }
            catch (Exception e)
            {
                var str = $"Error while reading bundle file \"{bundleReader.FullPath}\"";
                if (originalPath != null)
                {
                    str += $" from {Path.GetFileName(originalPath)}";
                }
                Logger.Warning($"{str}\n{e}");
                return true;
            }
            finally
            {
                bundleReader.Dispose();
            }
        }

        private bool LoadBundleFiles(FileReader reader, BundleFile bundleFile, string originalPath = null)
        {
            foreach (var file in bundleFile.fileList)
            {
                var dummyPath = Path.Combine(Path.GetDirectoryName(reader.FullPath), file.fileName);
                var subReader = new FileReader(dummyPath, file.stream);
                if (subReader.FileType == FileType.AssetsFile)
                {
                    if (!LoadAssetsFromMemory(subReader, originalPath ?? reader.FullPath, bundleFile.m_Header.unityRevision))
                        return false;
                }
                else
                {
                    resourceFileReaders.TryAdd(file.fileName, subReader);
                }
            }
            return true;
        }

        private void LoadWebFile(FileReader reader)
        {
            Logger.Info($"Loading \"{reader.FullPath}\"");
            try
            {
                var webFile = new WebFile(reader);
                foreach (var file in webFile.fileList)
                {
                    var dummyPath = Path.Combine(Path.GetDirectoryName(reader.FullPath), file.fileName);
                    var subReader = new FileReader(dummyPath, file.stream);
                    switch (subReader.FileType)
                    {
                        case FileType.AssetsFile:
                            LoadAssetsFromMemory(subReader, reader.FullPath);
                            break;
                        case FileType.BundleFile:
                            LoadBundleFile(subReader, reader.FullPath);
                            break;
                        case FileType.WebFile:
                            LoadWebFile(subReader);
                            break;
                        case FileType.ResourceFile:
                            resourceFileReaders.TryAdd(file.fileName, subReader);
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error($"Error while reading web file \"{reader.FullPath}\"", e);
            }
            finally
            {
                reader.Dispose();
            }
        }

        private void LoadZipFile(FileReader reader)
        {
            Logger.Info("Reading " + reader.FileName);
            try
            {
                using (ZipArchive archive = new ZipArchive(reader.BaseStream, ZipArchiveMode.Read))
                {
                    List<string> splitFiles = new List<string>();
                    // register all files before parsing the assets so that the external references can be found
                    // and find split files
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        if (entry.Name.Contains(".split"))
                        {
                            string baseName = Path.GetFileNameWithoutExtension(entry.Name);
                            string basePath = Path.Combine(Path.GetDirectoryName(entry.FullName), baseName);
                            if (!splitFiles.Contains(basePath))
                            {
                                splitFiles.Add(basePath);
                                importFilesHash.Add(baseName);
                            }
                        }
                        else
                        {
                            importFilesHash.Add(entry.Name);
                        }
                    }

                    // merge split files and load the result
                    foreach (string basePath in splitFiles)
                    {
                        try
                        {
                            Stream splitStream = new MemoryStream();
                            int i = 0;
                            while (true)
                            {
                                string path = $"{basePath}.split{i++}";
                                ZipArchiveEntry entry = archive.GetEntry(path);
                                if (entry == null)
                                    break;
                                using (Stream entryStream = entry.Open())
                                {
                                    entryStream.CopyTo(splitStream);
                                }
                            }
                            splitStream.Seek(0, SeekOrigin.Begin);
                            FileReader entryReader = new FileReader(basePath, splitStream);
                            LoadFile(entryReader);
                        }
                        catch (Exception e)
                        {
                            Logger.Warning($"Error while reading zip split file \"{basePath}\"\n{e}");
                        }
                    }

                    // load all entries
                    var progressCount = archive.Entries.Count;
                    int k = 0;
                    Progress.Reset();
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        try
                        {
                            string dummyPath = Path.Combine(Path.GetDirectoryName(reader.FullPath), reader.FileName, entry.FullName);
                            // create a new stream
                            // - to store the deflated stream in
                            // - to keep the data for later extraction
                            Stream streamReader = new MemoryStream();
                            using (Stream entryStream = entry.Open())
                            {
                                entryStream.CopyTo(streamReader);
                            }
                            streamReader.Position = 0;

                            FileReader entryReader = new FileReader(dummyPath, streamReader);
                            LoadFile(entryReader);
                            if (entryReader.FileType == FileType.ResourceFile)
                            {
                                entryReader.Position = 0;
                                resourceFileReaders.TryAdd(entry.Name, entryReader);
                            }
                            Progress.Report(++k, progressCount);
                        }
                        catch (Exception e)
                        {
                            Logger.Warning($"Error while reading zip entry \"{entry.FullName}\"\n{e}");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error($"Error while reading zip file {reader.FileName}", e);
            }
            finally
            {
                reader.Dispose();
            }
        }

        public void CheckStrippedVersion(SerializedFile assetsFile, UnityVersion bundleUnityVer = null)
        {
            if (assetsFile.version.IsStripped && specifiedUnityVersion == null)
            {
                var msg = "The asset's Unity version has been stripped, please set the version in the options.";
                if (bundleUnityVer != null && !bundleUnityVer.IsStripped)
                    msg += $"\n\nAssumed Unity version based on asset bundle: {bundleUnityVer}";
                throw new NotSupportedException(msg);
            }
            if (specifiedUnityVersion != null)
            {
                assetsFile.version = SpecifyUnityVersion;
            }
        }

        public void Clear()
        {
            foreach (var assetsFile in assetsFileList)
            {
                assetsFile.Objects.Clear();
                assetsFile.reader.Close();
            }
            assetsFileList.Clear();

            foreach (var resourceFileReader in resourceFileReaders)
            {
                resourceFileReader.Value.Close();
            }
            resourceFileReaders.Clear();

            assetsFileIndexCache.Clear();
        }

        private void ReadAssets()
        {
            Logger.Info("Read assets...");

            var jsonOptions = new JsonSerializerOptions
            {
                Converters = { new JsonConverterHelper.ByteArrayConverter(), new JsonConverterHelper.PPtrConverter(), new JsonConverterHelper.KVPConverter() },
                NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
                PropertyNameCaseInsensitive = true,
                IncludeFields = true,
            };

            var progressCount = assetsFileList.Sum(x => x.m_Objects.Count);
            var i = 0;
            Progress.Reset();
            foreach (var assetsFile in assetsFileList)
            {
                JsonConverterHelper.AssetsFile = assetsFile;
                foreach (var objectInfo in assetsFile.m_Objects)
                {
                    var objectReader = new ObjectReader(assetsFile.reader, assetsFile, objectInfo);
                    if (filteredAssetTypesList.Count > 0 && !filteredAssetTypesList.Contains(objectReader.type))
                    {
                        continue;
                    }
                    try
                    {
                        Object obj = null;
                        switch (objectReader.type)
                        {
                            case ClassIDType.Animation:
                                obj = new Animation(objectReader);
                                break;
                            case ClassIDType.AnimationClip:
                                obj = objectReader.serializedType?.m_Type != null && LoadingViaTypeTreeEnabled
                                    ? new AnimationClip(objectReader, TypeTreeHelper.ReadTypeByteArray(objectReader.serializedType.m_Type, objectReader), jsonOptions, objectInfo)
                                    : new AnimationClip(objectReader);
                                break;
                            case ClassIDType.Animator:
                                obj = new Animator(objectReader);
                                break;
                            case ClassIDType.AnimatorController:
                                obj = new AnimatorController(objectReader);
                                break;
                            case ClassIDType.AnimatorOverrideController:
                                obj = new AnimatorOverrideController(objectReader);
                                break;
                            case ClassIDType.AssetBundle:
                                obj = new AssetBundle(objectReader);
                                break;
                            case ClassIDType.AudioClip:
                                obj = new AudioClip(objectReader);
                                break;
                            case ClassIDType.Avatar:
                                obj = new Avatar(objectReader);
                                break;
                            case ClassIDType.BuildSettings:
                                obj = new BuildSettings(objectReader);
                                break;
                            case ClassIDType.Font:
                                obj = new Font(objectReader);
                                break;
                            case ClassIDType.GameObject:
                                obj = new GameObject(objectReader);
                                break;
                            case ClassIDType.Material:
                                obj = objectReader.serializedType?.m_Type != null && LoadingViaTypeTreeEnabled
                                    ? new Material(objectReader, TypeTreeHelper.ReadTypeByteArray(objectReader.serializedType.m_Type, objectReader), jsonOptions)
                                    : new Material(objectReader);
                                break;
                            case ClassIDType.Mesh:
                                obj = new Mesh(objectReader);
                                break;
                            case ClassIDType.MeshFilter:
                                obj = new MeshFilter(objectReader);
                                break;
                            case ClassIDType.MeshRenderer:
                                obj = new MeshRenderer(objectReader);
                                break;
                            case ClassIDType.MonoBehaviour:
                                obj = new MonoBehaviour(objectReader);
                                break;
                            case ClassIDType.MonoScript:
                                obj = new MonoScript(objectReader);
                                break;
                            case ClassIDType.MovieTexture:
                                obj = new MovieTexture(objectReader);
                                break;
                            case ClassIDType.PlayerSettings:
                                obj = new PlayerSettings(objectReader);
                                break;
                            case ClassIDType.PreloadData:
                                obj = new PreloadData(objectReader);
                                break;
                            case ClassIDType.RectTransform:
                                obj = new RectTransform(objectReader);
                                break;
                            case ClassIDType.Shader:
                                if (objectReader.version < 2021)
                                    obj = new Shader(objectReader);
                                break;
                            case ClassIDType.SkinnedMeshRenderer:
                                obj = new SkinnedMeshRenderer(objectReader);
                                break;
                            case ClassIDType.Sprite:
                                obj = new Sprite(objectReader);
                                break;
                            case ClassIDType.SpriteAtlas:
                                obj = new SpriteAtlas(objectReader);
                                break;
                            case ClassIDType.TextAsset:
                                obj = new TextAsset(objectReader);
                                break;
                            case ClassIDType.Texture2D:
                                obj = objectReader.serializedType?.m_Type != null && LoadingViaTypeTreeEnabled
                                    ? new Texture2D(objectReader, TypeTreeHelper.ReadTypeByteArray(objectReader.serializedType.m_Type, objectReader), jsonOptions)
                                    : new Texture2D(objectReader);
                                break;
                            case ClassIDType.Texture2DArray:
                                obj = objectReader.serializedType?.m_Type != null && LoadingViaTypeTreeEnabled
                                    ? new Texture2DArray(objectReader, TypeTreeHelper.ReadTypeByteArray(objectReader.serializedType.m_Type, objectReader), jsonOptions)
                                    : new Texture2DArray(objectReader);
                                break;
                            case ClassIDType.Transform:
                                obj = new Transform(objectReader);
                                break;
                            case ClassIDType.VideoClip:
                                obj = new VideoClip(objectReader);
                                break;
                            case ClassIDType.ResourceManager:
                                obj = new ResourceManager(objectReader);
                                break;
                            default:
                                obj = new Object(objectReader);
                                break;
                        }
                        if (obj != null)
                        {
                            assetsFile.AddObject(obj);
                        }
                    }
                    catch (Exception e)
                    {
                        var sb = new StringBuilder();
                        sb.AppendLine("Unable to load object")
                            .AppendLine($"Assets {assetsFile.fileName}")
                            .AppendLine($"Path {assetsFile.originalPath}")
                            .AppendLine($"Type {objectReader.type}")
                            .AppendLine($"PathID {objectInfo.m_PathID}")
                            .Append(e);
                        Logger.Warning(sb.ToString());
                    }

                    Progress.Report(++i, progressCount);
                }
            }
        }

        private void ProcessAssets()
        {
            Logger.Info("Process assets...");

            foreach (var assetsFile in assetsFileList)
            {
                foreach (var obj in assetsFile.Objects)
                {
                    if (obj is GameObject m_GameObject)
                    {
                        foreach (var pptr in m_GameObject.m_Components)
                        {
                            if (pptr.TryGet(out var m_Component))
                            {
                                switch (m_Component)
                                {
                                    case Transform m_Transform:
                                        m_GameObject.m_Transform = m_Transform;
                                        break;
                                    case MeshRenderer m_MeshRenderer:
                                        m_GameObject.m_MeshRenderer = m_MeshRenderer;
                                        break;
                                    case MeshFilter m_MeshFilter:
                                        m_GameObject.m_MeshFilter = m_MeshFilter;
                                        break;
                                    case SkinnedMeshRenderer m_SkinnedMeshRenderer:
                                        m_GameObject.m_SkinnedMeshRenderer = m_SkinnedMeshRenderer;
                                        break;
                                    case Animator m_Animator:
                                        m_GameObject.m_Animator = m_Animator;
                                        break;
                                    case Animation m_Animation:
                                        m_GameObject.m_Animation = m_Animation;
                                        break;
                                    case MonoBehaviour m_MonoBehaviour:
                                        if (m_MonoBehaviour.m_Script.TryGet(out var m_Script))
                                        {
                                            switch (m_Script.m_ClassName)
                                            {
                                                case "CubismModel":
                                                    if (m_GameObject.m_Transform == null)
                                                        break;
                                                    m_GameObject.CubismModel = new CubismModel(m_GameObject)
                                                    {
                                                        CubismModelMono = m_MonoBehaviour
                                                    };
                                                    break;
                                                case "CubismPhysicsController":
                                                    if (m_GameObject.CubismModel != null)
                                                        m_GameObject.CubismModel.PhysicsController = m_MonoBehaviour;
                                                    break;
                                                case "CubismFadeController":
                                                    if (m_GameObject.CubismModel != null)
                                                        m_GameObject.CubismModel.FadeController = m_MonoBehaviour;
                                                    break;
                                                case "CubismExpressionController":
                                                    if (m_GameObject.CubismModel != null)
                                                        m_GameObject.CubismModel.ExpressionController = m_MonoBehaviour;
                                                    break;
                                            }
                                        }
                                        break;
                                }
                            }
                        }
                    }
                    else if (obj is SpriteAtlas m_SpriteAtlas)
                    {
                        foreach (var m_PackedSprite in m_SpriteAtlas.m_PackedSprites)
                        {
                            if (m_PackedSprite.TryGet(out var m_Sprite))
                            {
                                if (m_Sprite.m_SpriteAtlas.IsNull)
                                {
                                    m_Sprite.m_SpriteAtlas.Set(m_SpriteAtlas);
                                }
                                else if (m_Sprite.m_SpriteAtlas.TryGet(out var m_SpriteAtlasOld))
                                {
                                    if (m_SpriteAtlasOld.m_IsVariant)
                                    {
                                        m_Sprite.m_SpriteAtlas.Set(m_SpriteAtlas);
                                    }
                                }
                                else
                                {
                                    Logger.Debug($"\"{m_Sprite.m_Name}\": The actual SpriteAtlas PathID \"{m_SpriteAtlas.m_PathID}\" does not match the specified one \"{m_Sprite.m_SpriteAtlas.m_PathID}\".");
                                    m_Sprite.m_SpriteAtlas.Set(m_SpriteAtlas);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
