using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;

namespace GohMdlExpert.Models.GatesOfHell.Resources {
    public class PlyModel {
        public struct Mesh {
            public int FirstFace { get; set; }
            public int FaceCount { get; set; }
            public string TextureFileName { get; set; }
        }

        public readonly struct Face {
            public int[] PointIndices { get; } = new int[3];

            public Face() { }
        }

        public Point3D[] Points { get; init; }
        public Face[] Faces { get; init; }
        public Vector3D[]? Normalizes { get; init; }
        public Point[]? UVPoints { get; init; }
        public Mesh[]? Meshes { get; init; }
        public Point3D? MinPoint {  get; init; }
        public Point3D? MaxPoint {  get; init; }

        public PlyModel(IEnumerable<Point3D> points, IEnumerable<Face> indicesList,
            IEnumerable<Vector3D>? normalizes = null, IEnumerable<Point>? uvPoints = null,
            IEnumerable<Mesh>? meshes = null, Point3D? minPoint = null, Point3D? maxPoint = null) {
            Points = points.ToArray();
            Faces = indicesList.ToArray();
            Normalizes = normalizes?.ToArray();
            UVPoints = uvPoints?.ToArray();
            Meshes = meshes?.ToArray();
            MinPoint = minPoint;
            MaxPoint = maxPoint;
        }
    }
}
