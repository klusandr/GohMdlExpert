using System.Text;
using System.Windows.Media.Media3D;

namespace GohMdlExpert.Models.GatesOfHell.Serialization {
    public class MdlSerializer : ModelDataSerializer {
        public enum MdlTypes {
            Skeleton,
            Bone,
            BoneRevolute,
            Limits,
            Speed,
            Orientation,
            Matrix34,
            LODView,
            Position,
            VolumeView,
        }

        public MdlSerializer() {
            AddType("Skeleton", "skeleton");
            AddType("Bone", "bone");
            AddType("BoneRevolute", "bone revolute");
            AddType("Limits", "limits",
                (t) => t.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(d => Convert.ToInt32(d.Trim())),
                (d) => string.Join(' ', (IEnumerable<int>)d)
            );
            AddType("Speed", "speed", (t) => Convert.ToSingle(t), (d) => d.ToString()!);
            AddType("Orientation", "orientation",
                (t) => {
                    var data = t.Replace('\n', ' ').Replace('\r', ' ').Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    var matrix = new float[3, 3];

                    for (int i = 0; i < 3; i++) {
                        for (int j = 0; j < 3; j++) {
                            matrix[i, j] = Convert.ToSingle(data.ElementAt(i * 3 + j));
                        }
                    }

                    return matrix;
                },
                (d) => {
                    var str = new StringBuilder();
                    var matrix = (float[,])d;

                    for (int i = 0; i < 3; i++) {
                        for (int j = 0; j < 3; j++) {
                            str.Append(matrix[i, j]);
                            if (j != 2) { str.Append(" \t "); }
                        }
                        if (i != 2) { str.AppendLine(); }
                    }

                    return str.ToString();
                }
            );
            AddType("Matrix34", "matrix34",
                (t) => {
                    var data = t.Replace('\n', ' ').Replace('\r', ' ').Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    var matrix = new float[4, 3];

                    for (int i = 0; i < 4; i++) {
                        for (int j = 0; j < 3; j++) {
                            matrix[i, j] = Convert.ToSingle(data.ElementAt(i * 3 + j));
                        }
                    }

                    return matrix;
                },
                (d) => {
                    var str = new StringBuilder();
                    var matrix = (float[,])d;

                    for (int i = 0; i < 4; i++) {
                        for (int j = 0; j < 3; j++) {
                            str.Append(matrix[i, j]);
                            if (j != 2) { str.Append(" \t "); }
                        }
                        if (i != 3) { str.AppendLine(); }
                    }

                    return str.ToString();
                }
            );
            AddType("Position", "Position",
                (t) => {
                    var d = t.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(d => Convert.ToDouble(d)).ToArray();

                    return new Point3D(d[0], d[1], d[2]);
                },
                (d) => {
                    var point = (Point3D)d;

                    return string.Join(" \t ", point.X, point.Y, point.Z);
                }
            );
            AddType("LODView", "LODView");
            AddType("VolumeView", "VolumeView");
        }
    }
}
