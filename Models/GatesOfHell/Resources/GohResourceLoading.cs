using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
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
using GohMdlExpert.Models.GatesOfHell.Resources.Loaders;
using GohMdlExpert.Models.GatesOfHell.Serialization;
using GohMdlExpert.Models.GatesOfHell.Сaches;

namespace GohMdlExpert.Models.GatesOfHell.Resources
{
    /// <summary>
    /// Предоставляет методы для загрузки различных ресурсов.
    /// </summary>
    public static class GohResourceLoading {
        private static GohMaterialCache? s_materialCache;
        private static ModelDataSerializer.ModelDataParameter? s_mdlTemplateParameters;
        private static AppResourceLoader? s_appResourceLoader;

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

        public static string HumanskinTemplateMdl { get; } = @"\Templates\humanskin_template.mdl";
        public static string MdlFileOpenFilter { get; } = "Mdl files (*.mdl)|*.mdl";
        public static IReadOnlyDictionary<string, Type> ResourcesFilesTypes { get; } = new Dictionary<string, Type>() {
            [MdlFile.Extension] = typeof(MdlFile),
            [PlyFile.Extension] = typeof(PlyFile),
            [MtlFile.Extension] = typeof(MtlFile),
            [MaterialFile.Extension] = typeof(MaterialFile),
            [MaterialFile.Extension2] = typeof(MaterialFile),
            [DefFile.Extension] = typeof(DefFile)
        };

        public static string PakExtension => ".pak";

        public static AppResourceLoader AppResourceLoader => s_appResourceLoader ??= new AppResourceLoader(AppDomain.CurrentDomain.BaseDirectory);

        public static ModelDataSerializer.ModelDataParameter MdlTemplateParameters => s_mdlTemplateParameters ??= new MdlFile(HumanskinTemplateMdl) {
            Loader = AppResourceLoader.FileLoader
        }.Data.Parameters;

        /// <summary>
        /// Возвращает материал текстуры из файла материала в виде изображения.
        /// </summary>
        /// <param name="materialFile">Файл материала.</param>
        /// <returns>Материал.</returns>
        public static DiffuseMaterial LoadMaterial(MaterialFile materialFile) {
            s_materialCache ??= new();

            if (!s_materialCache.TryGetMaterial(materialFile.Name, out DiffuseMaterial? diffuseMaterial)) {
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

                s_materialCache.SetMaterial(materialFile.Name, diffuseMaterial);
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
        /// Загружает humanskin, прописывая для .ply файлов полные пути, а так же заполняет список .mtl файлов используемых в моделях.
        /// </summary>
        /// <param name="mdlFile">humanskin .mdl файл.</param>
        /// <param name="mtlFiles">Список .mtl файлов.</param>
        /// <param name="humanskinResourceProvider">Провайдер humanskin.</param>
        /// <param name="textureProvider">Провайдер текстур.</param>
        public static void LoadHumanskinFile(MdlFile mdlFile, out IEnumerable<MtlFile> mtlFiles, GohResourceProvider resourceProvider, GohTextureProvider textureProvider) {
            var plyFiles = mdlFile.Data.PlyModel;
            var lodFiles = mdlFile.Data.PlyModelLods;
            var mtlFilesList = new List<MtlFile>();

            string? mdlFilePath = mdlFile.GetDirectoryPath();


            if (mdlFilePath != null) {
                var mdlResourceDirectory = resourceProvider.GetDirectory(mdlFilePath);

                if (mdlResourceDirectory != null) {
                    foreach (var mtlFile in mdlResourceDirectory.GetFiles().OfType<MtlFile>()) {
                        if (ResourceChecking.CheckMdlModelMeshTextureName(mdlFile.Data, mtlFile.Name)) {
                            mtlFilesList.Add(mtlFile);
                        }
                    }
                }
            }

            textureProvider.SetTexturesMaterialsFullPath(mtlFilesList.Select(m => m.Data));

            mtlFiles = mtlFilesList;
        }

        public static ModelDataSerializer.ModelDataParameter GetHumanskinMdlParametersTemplate() {
            var templateFile = new StreamReader(Path.Join(Environment.CurrentDirectory, HumanskinTemplateMdl));

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


        /// <summary>
        /// Filters resource files in resource directory and subdirectories by predicate and return virtual directory with only select files.
        /// </summary>
        /// <param name="resourceDirectory">Directory where need filter resource.</param>
        /// <param name="predicate">Condition for filtering files.</param>
        /// <returns>New virtual directory with structure input directory, but without directories don't have filtered files.</returns>
        public static GohResourceVirtualDirectory FilterResource(GohResourceDirectory resourceDirectory, Func<GohResourceFile, bool> predicate) {
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

        /// <summary>
        /// Create virtual directories structure on files list with full path.
        /// </summary>
        /// <param name="paths">List files with full path.</param>
        /// <param name="resourceProvider">GoH resource provider.</param>
        /// <returns>New virtual directories structure with files from list.</returns>
        public static GohResourceVirtualDirectory GetResourceStructure(IEnumerable<string> paths, GohResourceProvider resourceProvider) {
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

        /// <summary>
        /// Trying get next subdirectory have more than one directory or any files.
        /// </summary>
        /// <param name="resourceDirectory">Directory where start find next completed directory.</param>
        /// <param name="nextDirectory">Completed directory, have more one than directory or any files.</param>
        /// <param name="path">Path from resource directory to completed directory.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Return relatively path from first path to second path with one root.
        /// </summary>
        /// <returns>Relatively path from first path to second path.</returns>
        public static string GetRelativelyPath(string firstPath, string secondPath) {
            if (firstPath.Equals(secondPath, StringComparison.OrdinalIgnoreCase)) {
                return "";
            }

            var firstPathElements = firstPath.Split(DIRECTORY_SEPARATE, StringSplitOptions.RemoveEmptyEntries);
            var secondPathElements = secondPath.Split(DIRECTORY_SEPARATE, StringSplitOptions.RemoveEmptyEntries);

            if (firstPath[0] != secondPath[0]) {
                throw GohResourceLoadException.ResourcesPathNotHaveOneRoot(firstPath, secondPath);
            }

            int lastSharedPathElementIndex = Math.Min(firstPathElements.Length, secondPathElements.Length) - 1;
            for (var i = 1; i < firstPathElements.Length && i < secondPathElements.Length; i++) {
                if (!firstPathElements[i].Equals(secondPathElements[i], StringComparison.OrdinalIgnoreCase)) {
                    lastSharedPathElementIndex = i - 1;
                    break;
                }
            }

            int upStep = (firstPathElements.Length - 1) - lastSharedPathElementIndex;
            int startIndexDownSteap = lastSharedPathElementIndex + 1;

            string relativelyPath = string.Join(DIRECTORY_SEPARATE, [.. Enumerable.Repeat("..", upStep) ,..secondPathElements[(lastSharedPathElementIndex + 1)..]]);

            return relativelyPath;
        }
    }
}
