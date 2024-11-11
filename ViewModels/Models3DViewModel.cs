using GohMdlExpert.Extensions;
using GohMdlExpert.Models;
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

namespace GohMdlExpert.ViewModels {
    public sealed class Models3DViewModel : ViewModelBase {

        public Model3DCollection Models { get; set; }

        public Point3D? ModelsCenter => ((GeometryModel3D?)Models.FirstOrDefault())?.GetCentrPoint();

        public Models3DViewModel() {
            Models = [];
            //OpenPlyFile("F:\\SDK\\Content\\goh\\humanskin\\[germans]\\[ger_source]\\ger_tunic_pzwrap\\ger_ranks\\heer_ranks\\ger_rank_heer_pzwrap_em_03_unteroffizier.ply")
        }

        public GeometryModel3D OpenPlyFile(string fileName) {
            PlyReader.PlyRead(fileName);

            var mesh = new MeshGeometry3D {
                Positions = new Point3DCollection(PlyReader.Points),
                Normals = new Vector3DCollection(PlyReader.Normalizes),
                TriangleIndices = new Int32Collection(PlyReader.IndicesList),
                TextureCoordinates = new PointCollection(PlyReader.UVPoints)
            };

            GeometryModel3D model3D = new GeometryModel3D() {
                Geometry = mesh,
                Material = new DiffuseMaterial(
                    new ImageBrush(
                        new BitmapImage(
                            new Uri(@"F:\SDK\Content\goh\texture\common\_hum/ger_uniform/heer/ger_unif_pzwrap_43.dds", UriKind.Absolute)
                        )
                    ) { ViewportUnits = BrushMappingMode.Absolute }
                )
            };

            return model3D;
        }
    }
}
