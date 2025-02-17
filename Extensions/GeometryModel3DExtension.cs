using System.Windows.Media.Media3D;
using static GohMdlExpert.Models.GatesOfHell.Resources.Data.PlyModel;

namespace GohMdlExpert.Extensions {
    public static class GeometryModel3DExtension {
        public static Point3D GetCenterPoint(this GeometryModel3D model3D) {
            var mesh = (MeshGeometry3D)model3D.Geometry;

            return new Point3D() {
                X = mesh.Positions.Average(x => x.X),
                Y = mesh.Positions.Average(x => x.Y),
                Z = mesh.Positions.Average(x => x.Z),
            };
        }
    }
}
