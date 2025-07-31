using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using GohMdlExpert.Extensions;
using GohMdlExpert.Models.GatesOfHell.Exceptions;
using GohMdlExpert.Models.GatesOfHell.Media3D;
using GohMdlExpert.Models.GatesOfHell.Resources.Data;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Models.GatesOfHell.Resources.Files.Loaders;
using GohMdlExpert.Models.GatesOfHell.Resources.Humanskins;
using GohMdlExpert.Models.GatesOfHell.Serialization;
using GohMdlExpert.Models.GatesOfHell.Сaches;

namespace GohMdlExpert.Models.GatesOfHell.Resources
{
    /// <summary>
    /// Предоставляет методы для загрузки различных ресурсов.
    /// </summary>
    public static class GohResourceLoading {
        
        private static GohMaterialCache _materialCache = new();

        public static MdlSerializer MdlSerializer { get; } = new MdlSerializer();

        public static string ResourceDirectoryName { get; } = "resource";

        public const char DIRECTORY_SEPARATE = '\\';

        /// <summary>
        /// Фильтр загрузки .ply файлов.
        /// </summary>
        public static IEnumerable<string> PlyFilesLoadFilters { get; } = [
            @"^(?!.*_lod\d*\.)",
            @"^(?!.*#)",
            @"^(?!.*null\.)",
        ];

        public static string HumanskinMdl { get; } = @"\Templates\humanskin_template.mdl";
        public static string MdlFileOpenFilter { get; } = "Mdl files (*.mdl)|*.mdl";
        public static IReadOnlyDictionary<string, Type> ResourcesFilesTypes { get; } = new Dictionary<string, Type>() {
            [".mdl"] = typeof(MdlFile),
            [".ply"] = typeof(PlyFile),
            [".mtl"] = typeof(MtlFile),
            [".dds"] = typeof(MaterialFile)
        };

        /// <summary>
        /// Возвращает материал текстуры из файла материала в виде изображения.
        /// </summary>
        /// <param name="materialFile">Файл материала.</param>
        /// <returns>Материал.</returns>
        public static DiffuseMaterial LoadMaterial(MaterialFile materialFile) {
            if (!_materialCache.TryGetMaterial(materialFile.Name, out DiffuseMaterial? diffuseMaterial)) {
                if (!materialFile.Exists()) {
                    throw GohResourceFileException.IsNotExists(materialFile);
                }

                using var stream = materialFile.GetStream();

                var bitmap = new BitmapImage();

                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = new MemoryStream();
                stream.CopyTo(bitmap.StreamSource);
                bitmap.EndInit();


                diffuseMaterial = new DiffuseMaterial(
                    new ImageBrush(bitmap) {
                        ViewportUnits = BrushMappingMode.Absolute
                    }
                );

                diffuseMaterial.Freeze();
                bitmap.Freeze();

                _materialCache.SetMaterial(materialFile.Name, diffuseMaterial);
            }

            return diffuseMaterial;
        }

        /// <summary>
        /// Фильтрует коллекцию <see cref="PlyFile"/>, файлов в соответствии с фильтром загрузки .ply файлов.
        /// </summary>
        /// <param name="plyFiles">Коллекция .ply файлов.</param>
        /// <returns>Коллекция .ply файлов, не имеющих файлов которые на соответствуют фильтру загрузки.</returns>
        public static IEnumerable<PlyFile> FilterPlyFiles(IEnumerable<PlyFile> plyFiles) {
            return plyFiles.Where(f => PlyFilesLoadFilters.All(ff => Regex.IsMatch(f.Name, ff)));
        }

        /// <summary>
        /// Возвращает коллекцию LOD файлов для указанного <see cref="PlyFile"/> файла.
        /// </summary>
        /// <param name="plyFile">Файл, для которого будут возвращены LOD файлы.</param>
        /// <returns>Коллекция <see cref="PlyFile"/> файлов, который являются LOD для указанного файла.</returns>
        public static IEnumerable<PlyFile> GetPlyLodFiles(PlyFile plyFile, IGohHumanskinResource humanskinResource, GohResourceProvider resourceProvider) {
            var directory = resourceProvider.GetResourceDirectory(plyFile);

            var lodFiles = directory
                .FindResourceElements<PlyFile>(searchPattern: @$"{plyFile.Name[..^4]}_lod\d*\.");

            if (!lodFiles.Any()) {
                lodFiles = [humanskinResource.GetNullPlyFile(plyFile)];
            }

            return lodFiles;
        }

        /// <summary>
        /// Загружает humanskin, прописывая для .ply файлов полные пути, а так же заполняет список .mtl файлов используемых в моделях.
        /// </summary>
        /// <param name="mdlFile">humanskin .mdl файл.</param>
        /// <param name="mtlFiles">Список .mtl файлов.</param>
        /// <param name="humanskinResourceProvider">Провайдер humanskin.</param>
        /// <param name="textureProvider">Провайдер текстур.</param>
        public static void LoadHumanskinFile(MdlFile mdlFile, out IEnumerable<MtlFile> mtlFiles, GohTextureProvider textureProvider) {
            var plyFiles = mdlFile.Data.PlyModel;
            var lodFiles = mdlFile.Data.PlyModelLods;
            var mtlFilesList = new List<MtlFile>();

            string? mdlFilePath = mdlFile.GetDirectoryPath();

            if (mdlFilePath != null) {
                foreach (var mtlFilePath in Directory.GetFiles(mdlFilePath, "*.mtl")) {
                    if (ResourceChecking.CheckMdlModelMeshTextureName(mdlFile.Data, Path.GetFileName(mtlFilePath))) {
                        mtlFilesList.Add(new MtlFile(mtlFilePath));
                    }
                }
            }

            textureProvider.SetTexturesMaterialsFullPath(mtlFilesList.Select(m => m.Data));

            mtlFiles = mtlFilesList;
        }

        public static ModelDataSerializer.ModelDataParameter GetHumanskinMdlParametersTemplate() {
            var templateFile = new StreamReader(Path.Join(Environment.CurrentDirectory, HumanskinMdl));

            return MdlSerializer.Deserialize(templateFile.ReadToEnd());
        }

        public static GohResourceFile CreateResourceFile(string fileName, string? path = null, IFileLoader? fileLoader = null) {
            GohResourceFile? file = null;

            if (ResourcesFilesTypes.TryGetValue(Path.GetExtension(fileName), out var fileType)) {
                file = (GohResourceFile)fileType.GetConstructors()[0].Invoke([fileName, path, null]);

                if (fileLoader != null) {
                    file.Loader = fileLoader;
                }
            }

            return file ?? new GohResourceFile(fileName, path);
        }

        public static GohResourceDirectory FilterResource(GohResourceDirectory resourceDirectory, Func<GohResourceFile, bool> predicate) {
            var files = resourceDirectory.GetFiles().Where(predicate);

            var virtualDirectory = new GohResourceVirtualDirectory(resourceDirectory);

            if (files.Any()) {
                foreach (var file in files) {
                    virtualDirectory.Items.Add(file);
                }
            }

            foreach (var directory in resourceDirectory.GetDirectories()) {
                var subVirtualDirectory = FilterResource(directory, predicate);

                if (subVirtualDirectory.Items.Count != 0) {
                    virtualDirectory.Items.Add(subVirtualDirectory);
                }
            }

            return virtualDirectory;
        }

        public static GohResourceDirectory GetResourceStructure(IEnumerable<string> paths, GohResourceProvider resourceProvider) {
            var rootDirectory = new GohResourceVirtualDirectory(resourceProvider.ResourceDirectory);

            foreach (var path in paths) {
                var file = resourceProvider.GetFile(path);

                if (file != null) {
                    var pathDirectory = rootDirectory.AlongPathOrCreate(file.GetDirectoryPath()!);
                    pathDirectory.Items.Add(file);
                }
            }

            return rootDirectory;
        }

        public static bool TryGetNextCompletedDirectory(GohResourceDirectory resourceDirectory, [NotNullWhen(true)] out GohResourceDirectory? nextDirectory, [NotNullWhen(true)] out string? path) {
            if (resourceDirectory.Items.Count == 1 && resourceDirectory.Items[0] is GohResourceDirectory directory) {
                if (!TryGetNextCompletedDirectory(directory, out nextDirectory, out path)) {
                    path = resourceDirectory.Name + DIRECTORY_SEPARATE + directory.Name;
                    nextDirectory = directory;
                }

                return true;
            }

            nextDirectory = null;
            path = null;

            return false;
        }
    }
}
