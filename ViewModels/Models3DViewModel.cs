using GohMdlExpert.Extensions;
using GohMdlExpert.Models.GatesOfHell.Resources;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Models.GatesOfHell.Serialization;
using Microsoft.Win32;
using MvvmWpf.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using static GohMdlExpert.Models.GatesOfHell.Resources.PlyModel;

namespace GohMdlExpert.ViewModels
{
    public sealed class Models3DViewModel : ViewModelBase {
        private readonly Dictionary<string, MtlFile> _texturesCache;

        public Model3DCollection Models { get; set; }

        public Point3D? ModelsCenter => ((GeometryModel3D?)Models.FirstOrDefault())?.GetCentrPoint();

        public Models3DViewModel() {
            Models = [];
            _texturesCache = [];
        }

        public void OpenMdlFile(string fileName) {
            var mdlFile = new MdlFile(fileName);

            foreach (var plyModels in mdlFile.Data!.PlyModelFiles.Where(pf => !pf.Name.Contains("lod")).Select(pf => pf.Data!)) {

                List<(Geometry3D mesh, string texture)> meshes = new();

                foreach (var meshData in plyModels.Meshes!) {
                    int meshTriangleStartIndex = meshData.FirstFace * 3;
                    int meshTriangleEndIndex = meshTriangleStartIndex + meshData.FaceCount * 3;

                    var points = plyModels.Points.Select((p) => new Point3D(p.X * -1, p.Y, p.Z));
                    var triangleIndices = plyModels.Faces[meshData.FirstFace..(meshData.FirstFace + meshData.FaceCount)]
                        .SelectMany(f =>
                            new int[] {
                            f.PointIndices[0],
                            f.PointIndices[2],
                            f.PointIndices[1]
                            }
                        );

            var mesh = new MeshGeometry3D {
                        Positions = new Point3DCollection(points),
                        Normals = new Vector3DCollection(plyModels.Normalizes),
                        TriangleIndices = new Int32Collection(triangleIndices),
                        TextureCoordinates = new PointCollection(plyModels.UVPoints)
            };

                    meshes.Add((mesh, meshData.TextureFileName));
                }

                foreach (var mesh in meshes) {
                    Models.Add(new GeometryModel3D() {
                        Geometry = mesh.mesh,
                        Material =  mdlFile.Data.Textures.First(t => t.Name == mesh.texture).Data!.Diffuse!.Data!,
                        Transform = new Transform3DGroup() {
                            Children = [
                                new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), -90)),
                                new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), -90)),
                            ]
                        }
                    });
                }
            }
        }

        public void OpenPlyFile(string fileName) {
            var plyModel = new PlyFile(fileName).Data;

            List<(Geometry3D mesh, string texture)> meshes = new();

            foreach (var meshData in plyModel.Meshes!) {
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

                meshes.Add((mesh, meshData.TextureFileName));
            }
            

            foreach (var mesh in meshes) {
                if (!_texturesCache.ContainsKey(mesh.texture)) {
                    var d = Directory.GetFiles(ResourceLocations.Instance.GetLocationPath("ger_humanskin"), mesh.texture, SearchOption.AllDirectories);

                    var textureFileName = d.First();
                    _texturesCache.Add(mesh.texture, new MtlFile(textureFileName));
                }
            }

            foreach (var mesh in meshes) {
                Models.Add(new GeometryModel3D() {
                    Geometry = mesh.mesh,
                    Material = _texturesCache[mesh.texture].Data.Diffuse.Data,
                    Transform = new Transform3DGroup() {
                        Children = [
                            new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), -90)),
                            new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), -90)),
                        ]
                    }
                });
            }
        }

        private Point3D GetPointsCenter(params Point3D[] points3D) {
            return new Point3D() {
                X = points3D.Average(p => p.X),
                Y = points3D.Average(p => p.Y),
                Z = points3D.Average(p => p.Z),
            };
        }
    }
}
