using GohMdlExpert.Models.GatesOfHell.Exceptions;
using GohMdlExpert.Models.GatesOfHell.Resources;
using System.Windows.Media.Media3D;

namespace GohMdlExpert.Models.GatesOfHell.Media3D {
    /// <summary>
    /// Предоставляет карту связывающую <see cref="PlyModel.Mesh"/> в <see cref="Resources.PlyModel"/> 
    /// и <see cref="GeometryModel3D"/> в <see cref="Model3DGroup"/> по <see cref="PlyModel.Mesh.TextureName"/>.
    /// </summary>
    public class Model3DGroupPlyFileMap {
        private readonly Dictionary<string, GeometryModel3D> _meshesGeometries;

        /// <summary>
        /// Связанная модель <see cref="Resources.PlyModel"/>.
        /// </summary>
        public PlyModel PlyModel { get; }
        /// <summary>
        /// Связанная модель <see cref="Model3DGroup"/>.
        /// </summary>
        public Model3DGroup Model { get; }

        /// <summary>
        /// Создаёт карту связывающую <see cref="PlyModel.Mesh"/> в <see cref="Resources.PlyModel"/> 
        /// и <see cref="GeometryModel3D"/> в <see cref="Model3DGroup"/> по <see cref="PlyModel.Mesh.TextureName"/>.
        /// </summary>
        /// <param name="plyModel">3D модель <see cref="Resources.PlyModel"/> которую необходимо связать с <see cref="Model3DGroup"/>.</param>
        /// <param name="model">3D модель <see cref="Model3DGroup"/> которую необходимо связать с <see cref="Resources.PlyModel"/>.</param>
        /// <exception cref="PlyModelException"></exception>
        public Model3DGroupPlyFileMap(PlyModel plyModel, Model3DGroup model) {
            if (plyModel.Meshes.Length != model.Children.Count) {
                throw new PlyModelException(message: "Create Model3DGroupPlyFileMap error. Count PlyModel meshes don't match to count Model3DGeometry");
            }

            PlyModel = plyModel;
            Model = model;

            _meshesGeometries = [];
            int meshIndex = 0;

            foreach (var modelMeshGeometry in model.Children) {
                _meshesGeometries.Add(plyModel.Meshes[meshIndex].TextureName, (GeometryModel3D)modelMeshGeometry);

                meshIndex++;
            }
        }

        /// <summary>
        /// Получает <see cref="GeometryModel3D"/> по имени текстуры меша.
        /// </summary>
        /// <param name="textureName">Имя текстуры меша <see cref="Resources.PlyModel"/>.</param>
        /// <returns>Экземпляр <see cref="GeometryModel3D"/>.</returns>
        /// <exception cref="PlyModelException"></exception>
        public GeometryModel3D GetMeshGeometry(string textureName) {
            if (_meshesGeometries.TryGetValue(textureName, out var value)) {
                return value;
            } else {
                throw new PlyModelException(message: $"Ply model don't contain mesh with \"{textureName}\" texture name.");
            }
        }
    }
}
