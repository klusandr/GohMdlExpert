using System.Windows.Media;
using System.Windows.Media.Media3D;
using GohMdlExpert.Models.GatesOfHell.Resources.Data;

namespace GohMdlExpert.Models.GatesOfHell.Media3D {
    /// <summary>
    /// Предоставляет методы преобразования различных ресурсов GoH в объекты используемые в .Net.
    /// </summary>
    public static class ResourceConverts {
        /// <summary>
        /// Конвертирует <see cref="PlyModel"/> в модель <see cref="Model3DGroup"/>, содержащую одну или несколько <see cref="GeometryModel3D"/>,
        /// представляющие <see cref="PlyModel.Mesh"/> в исходной модели, с возможностью указания использованных материалов.
        /// </summary>
        /// <remarks>
        /// Ply Модели в GoH имеют несколько другие установки по осям, и для правильного отображения применяются трансформации вращения.
        /// </remarks>
        /// <param name="plyModel">Модель <see cref="PlyModel"/>, которую необходимо конвертировать в <see cref="Model3DGroup"/>.</param>
        /// <param name="meshesTextures">Перечисление материалов <see cref="Material"/>, используемых в текстуре. Если будет равно <see langword="null"/>, 
        /// будут использованы <see cref="SolidColorBrush"/> со случайными цветами.</param>
        /// <returns>Экземпляр <see cref="Model3DGroup"/>, содержащий один или несколько <see cref="GeometryModel3D"/>, 
        /// соответствующим порядку <see cref="PlyModel.Mesh"/> в исходной модели.</returns>
        public static Model3DGroup PlyModelToModel3D(PlyModel plyModel, Dictionary<string, MtlTexture?>? meshesTextures = null) {
            var model = new Model3DGroup() {
                Transform = new Transform3DGroup() {
                    Children = [
                        new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), -90)),
                        new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), -90)),
                    ]
                }
            };

            var meshesEnumerator = PlyModelToMeshesGeometry3D(plyModel).GetEnumerator();
            meshesEnumerator.MoveNext();

            foreach (var meshData in plyModel.Meshes) {
                var material = GetMeshMaterialOrRandomColor(meshData.TextureName, meshesTextures);
                var geometry = new GeometryModel3D(meshesEnumerator.Current, material) { BackMaterial = material };

                model.Children.Add(geometry);

                meshesEnumerator.MoveNext();
            }

            return model;
        }

        /// <summary>
        /// Конвертирует <see cref="PlyModel"/> в несколько <see cref="MeshGeometry3D"/>, представляющие <see cref="PlyModel.Mesh"/> в исходной модели.
        /// </summary>
        /// <param name="plyModel">Модель <see cref="PlyModel"/>, которую конвертировать в несколько <see cref="MeshGeometry3D"/>.</param>
        /// <returns>Перечисление <see cref="MeshGeometry3D"/>, соответствующего порядку <see cref="PlyModel.Mesh"/> в исходной модели.</returns>
        public static IEnumerable<MeshGeometry3D> PlyModelToMeshesGeometry3D(PlyModel plyModel) {
            var meshes = new List<MeshGeometry3D>();

            foreach (var meshData in plyModel.Meshes) {
                int meshTriangleStartIndex = meshData.FirstFace * 3;
                int meshTriangleEndIndex = meshTriangleStartIndex + meshData.FaceCount * 3;

                var points = plyModel.Points.Select((p) => new Point3D(p.X * -1, p.Y, p.Z));
                var triangleIndices = plyModel.Faces[meshData.FirstFace..(meshData.FirstFace + meshData.FaceCount)]
                    .SelectMany(f =>
                        new int[] {
                            f.PointIndices[0],
                            f.PointIndices[2],
                            f.PointIndices[1]
                        }
                    );

                var mesh = new MeshGeometry3D {
                    Positions = new Point3DCollection(points),
                    Normals = new Vector3DCollection(plyModel.Normalizes),
                    TriangleIndices = new Int32Collection(triangleIndices),
                    TextureCoordinates = new PointCollection(plyModel.UVPoints),
                };

                meshes.Add(mesh);
            }

            return meshes;
        }

        /// <summary>
        /// Проверяет, является ли материал загруженной текстурой.
        /// </summary>
        /// <param name="material"></param>
        /// <returns></returns>
        public static bool CheckLoadingTexture(Material material) {
            if (material is DiffuseMaterial diffuseMaterial) {
                return diffuseMaterial.Brush is ImageBrush;
            } else {
                return false;
            }
        }

        /// <summary>
        /// Возвращает материал со случайным цветом. 
        /// </summary>
        public static DiffuseMaterial GetRandomTexture() {
            return new DiffuseMaterial(new SolidColorBrush(
                new Color() {
                    A = 255,
                    R = (byte)Random.Shared.Next(0, 255),
                    G = (byte)Random.Shared.Next(0, 255),
                    B = (byte)Random.Shared.Next(0, 255),
                }
            ));
        }

        private static Material GetMeshMaterialOrRandomColor(string meshTextureName, Dictionary<string, MtlTexture?>? meshesTextures) {
            if (meshesTextures != null) {
                if (meshesTextures.TryGetValue(meshTextureName, out var texture) && texture != null) {
                    return texture.Diffuse.Data;
                }
            }

            return GetRandomTexture();
        }
    }
}
