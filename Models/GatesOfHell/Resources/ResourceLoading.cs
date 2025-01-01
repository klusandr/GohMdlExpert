using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GohMdlExpert.Models.GatesOfHell.Exceptions;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.Text.RegularExpressions;
using GohMdlExpert.Models.GatesOfHell.Resources.Humanskins;

namespace GohMdlExpert.Models.GatesOfHell.Resources {
    /// <summary>
    /// Предоставляет методы для загрузки различных ресурсов.
    /// </summary>
    public static class ResourceLoading {
        /// <summary>
        /// Фильтр загрузки .ply файлов.
        /// </summary>
        public static IEnumerable<string> PlyFilesLoadFilters { get; } = [
            @"^(?!.*_lod\d*\.)",
            @"^(?!.*#)",
            @"^(?!.*null\.)",
        ];


        /// <summary>
        /// Возвращает материал текстуры из файла материала в виде изображения.
        /// </summary>
        /// <param name="materialFile">Файл материала.</param>
        /// <returns>Материал.</returns>
        public static Material LoadMaterial(MaterialFile materialFile) {
            string fullPath = materialFile.GetFullPath();
            DiffuseMaterial diffuseMaterial;

            if (!materialFile.Exists()) {
                throw GohResourceFileException.IsNotExists(materialFile);
            }

            diffuseMaterial = new DiffuseMaterial(
                new ImageBrush(new BitmapImage(new Uri(fullPath))) {
                    ViewportUnits = BrushMappingMode.Absolute,
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
    }
}
