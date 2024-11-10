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
using System.Windows.Media.Media3D;

namespace GohMdlExpert.ViewModesl {
    public sealed class Models3DViewModel : ViewModelBase {

        public Model3DCollection Models { get; set; }

        public Point3D? ModelsCenter => ((GeometryModel3D?)Models.FirstOrDefault())?.GetCentrPoint();

        public Models3DViewModel() {
            Models = [OpenPlyFile("F:\\SDK\\Content\\goh\\humanskin\\[germans]\\[ger_source]\\ger_tunic_pzwrap\\ger_ranks\\heer_ranks\\ger_rank_heer_pzwrap_em_03_unteroffizier.ply")];
        }

        private GeometryModel3D OpenPlyFile(string fileName) {
            PlyReader.PlyRead(fileName);

            var mesh = new MeshGeometry3D();

            foreach (var point in PlyReader.Points) {
                mesh.Positions.Add(point);
            }

            foreach (var normal in PlyReader.Normalizes) {
                mesh.Normals.Add(normal);
            }

            mesh.TriangleIndices = new Int32Collection(PlyReader.IndicesList);

            GeometryModel3D model3D = new GeometryModel3D() {
                Geometry = mesh,
                Material = new DiffuseMaterial(new SolidColorBrush(new System.Windows.Media.Color() { A = 255, R = 255, G = 0, B = 0 }))
            };

            return model3D;
        }
    }
}
