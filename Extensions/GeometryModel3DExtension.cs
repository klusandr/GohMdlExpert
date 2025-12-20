using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using static GohMdlExpert.Models.GatesOfHell.Resources.Data.PlyModel;

namespace GohMdlExpert.Extensions {
    public static class GeometryModel3DExtension {
        private static readonly Material s_selectMaterial = new DiffuseMaterial(new SolidColorBrush(new Color() { A = 25, R = 255 }));

        public static Point3D GetCenterPoint(this GeometryModel3D model3D) {
            var mesh = (MeshGeometry3D)model3D.Geometry;

            return new Point3D() {
                X = mesh.Positions.Average(x => x.X),
                Y = mesh.Positions.Average(x => x.Y),
                Z = mesh.Positions.Average(x => x.Z),
            };
        }

        public static void SetSelectMaterial(this GeometryModel3D model3D) {
            ((SolidColorBrush)((DiffuseMaterial)s_selectMaterial).Brush).Color = new Color() { A = 50, R = 200 };

            if (model3D.Material is MaterialGroup materialGroup) {
                if (!materialGroup.Children.Contains(s_selectMaterial)) {
                    materialGroup.Children.Add(s_selectMaterial);
                }
            } else {
                var material = model3D.Material;

                model3D.Material = new MaterialGroup() { 
                    Children = { material, s_selectMaterial } 
                };
            }

            model3D.BackMaterial = model3D.Material;
        }

        public static void ClearSelectMaterial(this GeometryModel3D model3D) {
            if (model3D.Material is MaterialGroup materialGroup) {
                if (materialGroup.Children.Contains(s_selectMaterial)) {
                    for (int i = 0; i < materialGroup.Children.Count(m => m == s_selectMaterial); i++) {
                        materialGroup.Children.Remove(s_selectMaterial);
                    }

                    if (materialGroup.Children.Count == 1) {
                        model3D.Material = materialGroup.Children.First();
                    }
                }

                model3D.BackMaterial = model3D.Material;
            }
        }
    }
}
