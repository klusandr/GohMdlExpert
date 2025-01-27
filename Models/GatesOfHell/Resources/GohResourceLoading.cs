using System.Collections.Immutable;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using GohMdlExpert.Extensions;
using GohMdlExpert.Models.GatesOfHell.Exceptions;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Models.GatesOfHell.Resources.Files.Loaders;
using GohMdlExpert.Models.GatesOfHell.Resources.Humanskins;
using GohMdlExpert.Models.GatesOfHell.Serialization;

namespace GohMdlExpert.Models.GatesOfHell.Resources {
    /// <summary>
    /// Предоставляет методы для загрузки различных ресурсов.
    /// </summary>
    public static class GohResourceLoading {
        public static MdlSerializer MdlSerializer { get; } = new MdlSerializer();

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
            DiffuseMaterial diffuseMaterial;

            if (!materialFile.Exists()) {
                throw GohResourceFileException.IsNotExists(materialFile);
            }

            using var stream = materialFile.GetStream();
            byte[] buffer = new byte[stream.Length];

            stream.Read(buffer, 0, buffer.Length);

            diffuseMaterial = new DiffuseMaterial(
                new ImageBrush(new BitmapImage().FromByteArray(buffer)) {
                    ViewportUnits = BrushMappingMode.Absolute
                }
            );

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
        public static IEnumerable<PlyFile> GetPlyLodFiles(PlyFile plyFile, GohFactionHumanskinResource humanskinResource, GohResourceProvider resourceProvider) {
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
        public static void LoadHumanskinFile(MdlFile mdlFile, out IEnumerable<MtlFile> mtlFiles, GohHumanskinResourceProvider humanskinResourceProvider, GohTextureProvider textureProvider) {
            var plyFiles = mdlFile.Data.PlyModel;
            var lodFiles = mdlFile.Data.PlyModelLods;
            var mtlFilesList = new List<MtlFile>();

            foreach (var plyFile in plyFiles) {
                humanskinResourceProvider.Current.SetPlyFileFullPath(plyFile);

                foreach (var lodFile in lodFiles[plyFile]) {
                    humanskinResourceProvider.Current.SetPlyFileFullPath(lodFile);
                }

                string? mdlFilePath = mdlFile.GetDirectoryPath();

                if (mdlFilePath != null) {
                    foreach (var mtlFilePath in Directory.GetFiles(mdlFilePath, "*.mtl")) {
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

        public static GohResourceFile GetResourceFile(string fileName, string? path = null, IFileLoader? fileLoader = null) {
            GohResourceFile? file = null;

            if (ResourcesFilesTypes.TryGetValue(Path.GetExtension(fileName), out var fileType)) {
                file = (GohResourceFile)fileType.GetConstructors()[0].Invoke([fileName, path, null]);

                if (fileLoader != null) {
                    file.Loader = fileLoader;
                }
            }

            return file ?? new GohResourceFile(fileName, path);
        }

    }
}
