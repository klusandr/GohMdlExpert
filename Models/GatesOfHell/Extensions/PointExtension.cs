using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace GohMdlExpert.Models.GatesOfHell.Extensions {
    public static class PointExtension {
        public static IEnumerable<Point3D> SwapYZ(this IEnumerable<Point3D> points) {
            var pointArray = points.ToArray();

            for (int i = 0; i < pointArray.Length; i++) {
                var point = pointArray[i];
                (point.Y, point.Z) = (point.Z, point.Y);
                pointArray[i] = point;
            }

            return pointArray;
        }

        public static IEnumerable<Point3D> SwapXY(this IEnumerable<Point3D> points) {
            var pointArray = points.ToArray();

            for (int i = 0; i < pointArray.Length; i++) {
                var point = pointArray[i];
                (point.Y, point.X) = (point.X, point.Y);
                pointArray[i] = point;
            }

            return pointArray;
        }

        public static IEnumerable<Point3D> SwapXZ(this IEnumerable<Point3D> points) {
            var pointArray = points.ToArray();

            for (int i = 0; i < pointArray.Length; i++) {
                var point = pointArray[i];
                (point.X, point.Z) = (point.Z, point.X);
                pointArray[i] = point;
            }

            return pointArray;
        }

        public static Point3D GetCenterPoint(this IEnumerable<Point3D> points) {
            return new Point3D() {
                X = points.Average(x => x.X),
                Y = points.Average(x => x.Y),
                Z = points.Average(x => x.Z),
            };
        }
    }
}
