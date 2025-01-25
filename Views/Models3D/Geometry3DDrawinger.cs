using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace GohMdlExpert.Views.Models3D {
    public static class Geometry3DDrawinger {

        public static GeometryModel3D DrowCube(double sadeSize) {
            sadeSize /= 2;

            return new GeometryModel3D() {
                Geometry = new MeshGeometry3D() {
                    Positions = new Point3DCollection([
                        new Point3D(-sadeSize, -sadeSize, -sadeSize),
                        new Point3D(sadeSize, -sadeSize, -sadeSize),
                        new Point3D(-sadeSize, sadeSize,-sadeSize),
                        new Point3D(sadeSize, sadeSize, -sadeSize),
                        new Point3D(-sadeSize, -sadeSize, sadeSize),
                        new Point3D(sadeSize, -sadeSize, sadeSize),
                        new Point3D(-sadeSize, sadeSize, sadeSize),
                        new Point3D(sadeSize, sadeSize, sadeSize)
                    ]),
                    TriangleIndices = new Int32Collection([0, 2, 1, 1, 2, 3, 0, 4, 2, 2, 4, 6, 0, 1, 4, 1, 5, 4, 1, 7, 5, 1, 3, 7, 4, 5, 6, 7, 6, 5, 2, 6, 3, 3, 6, 7])
                },
                Material = new DiffuseMaterial(new SolidColorBrush(new Color() { A = 255, R = 255, G = 0, B = 0 }))
            };
        }
    }
}