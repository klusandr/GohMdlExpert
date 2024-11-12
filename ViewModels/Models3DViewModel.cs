using GohMdlExpert.Extensions;
using GohMdlExpert.Models.GatesOfHell.Serialization;
using MvvmWpf.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace GohMdlExpert.ViewModels
{
    public sealed class Models3DViewModel : ViewModelBase {
        private PlyFileSerialize _plyFileSerialize;

        private Dictionary<string, Uri> _textures = new() {
            ["ger_heads.mtl"] = new Uri(@"F:\SDK\Content\goh\texture\common\_hum\ger_uniform\ger_heads.dds", UriKind.Absolute),
            ["ger_uniform.mtl"] = new Uri(@"F:\SDK\Content\goh\texture\common\_hum/ger_uniform/heer/ger_unif_pzwrap_43.dds", UriKind.Absolute)
        };

        public Model3DCollection Models { get; set; }

        public Point3D? ModelsCenter => ((GeometryModel3D?)Models.FirstOrDefault())?.GetCentrPoint();

        public Models3DViewModel() {
            Models = [];
            //OpenPlyFile("F:\\SDK\\Content\\goh\\humanskin\\[germans]\\[ger_source]\\ger_tunic_pzwrap\\ger_ranks\\heer_ranks\\ger_rank_heer_pzwrap_em_03_unteroffizier.ply")
            _plyFileSerialize = new PlyFileSerialize();
        }

        public void OpenPlyFile(string fileName) {
            var ply = _plyFileSerialize.Deserialize(fileName);

            List<(Geometry3D mesh, string texture)> meshes = new();

            foreach (var meshData in ply.Meshes!) {
                int meshTriangleStartIndex = meshData.FirstFace * 3;
                int meshTriangleEndIndex = meshTriangleStartIndex + meshData.FaceCount * 3;

                var points = ply.Points.Select((p) => new Point3D(p.X * -1, p.Y, p.Z));
                var triangleIndices = ply.Faces[meshData.FirstFace..(meshData.FirstFace + meshData.FaceCount)]
                    .SelectMany(f =>
                        new int[] {
                            f.PointIndices[0],
                            f.PointIndices[2],
                            f.PointIndices[1]
                        }
                    );

                var mesh = new MeshGeometry3D {
                    Positions = new Point3DCollection(points),
                    Normals = new Vector3DCollection(ply.Normalizes),
                    TriangleIndices = new Int32Collection(triangleIndices),
                    TextureCoordinates = new PointCollection(ply.UVPoints)
                };

                meshes.Add((mesh, meshData.TextureFileName));
            }

            List<GeometryModel3D> models3D = new();

            foreach (var mesh in meshes) {
                Models.Add(new GeometryModel3D() {
                    Geometry = mesh.mesh,
                    Material = new DiffuseMaterial(
                        new ImageBrush(new BitmapImage(_textures[mesh.texture])
                        ) { ViewportUnits = BrushMappingMode.Absolute }
                    ),
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
