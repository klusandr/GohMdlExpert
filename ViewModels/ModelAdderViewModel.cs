using GohMdlExpert.Models.GatesOfHell.Resources;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using MvvmWpf.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;

namespace GohMdlExpert.ViewModels {
    public class ModelAdderViewModel : ViewModelBase {
        private readonly Dictionary<string, GeometryModel3D> _texturesGeometries;
        private Model3D? _addedModel;

        public Models3DViewModel Models3DView { get; }

        public ICommand EndAddCommand => CommandManager.GetCommand(EndAdd);

        public Model3D? AddedModel {
            get => _addedModel;
            set {
                _addedModel = value;
                OnPropertyChanged();
            }
        }

        public bool IsAddedInProgress => AddedModel != null;


        public ModelAdderViewModel(Models3DViewModel models3DView) {
            Models3DView = models3DView;
            _texturesGeometries = [];
            _addedModel = null;
        }

        public void SetModel(PlyFile plyFile) {
            _texturesGeometries.Clear();
            AddedModel = GetModel3D(plyFile.Data);
        }

        public void SetModelTexture(MtlFile mtlFile) {
            if (!IsAddedInProgress) {
                throw new InvalidOperationException("Error setting texture model. Model not added yet.");
            }
            //TODO Сделать проверку на отсутствие конфликтов текстур уже добавленных моделей.
            if (_texturesGeometries.TryGetValue(mtlFile.Name, out var geometry)) {
                geometry.Material = mtlFile.Data.Diffuse.Data;
            }
        }

        public void EndAdd() {
            if (_texturesGeometries.Values.Any(m => !CheckLoadingTexture(m.Material))) {
                throw new InvalidOperationException("Error end adding model. One or more texture is not select.");
            }

            var model = AddedModel;
            AddedModel = null;
            Models3DView.Models.Add(model);
            _texturesGeometries.Clear();
        }

        private Model3D GetModel3D(PlyModel plyModel) {
            var model = new Model3DGroup() {
                Transform = new Transform3DGroup() {
                    Children = [
                        new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), -90)),
                        new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), -90)),
                    ]
                }
            };

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
                    TextureCoordinates = new PointCollection(plyModel.UVPoints)
                };

                var geometry = new GeometryModel3D(mesh, GetRandomTexture());
                _texturesGeometries.Add(meshData.TextureFileName, geometry);

                model.Children.Add(geometry);
            }

            return model;
        }

        private static bool CheckLoadingTexture(Material material) {
            if (material is DiffuseMaterial diffuseMaterial) {
                return diffuseMaterial.Brush is not SolidColorBrush;
            } else {
                return false;
            }
        }

        private static Material GetRandomTexture() {
            return new DiffuseMaterial(new SolidColorBrush(
                new Color() {
                    A = 255,
                    R = (byte)Random.Shared.Next(0, 255),
                    G = (byte)Random.Shared.Next(0, 255),
                    B = (byte)Random.Shared.Next(0, 255),
                }
            ));
        }
    }
}
