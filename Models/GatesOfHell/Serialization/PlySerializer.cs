using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media.Media3D;
using GohMdlExpert.Models.GatesOfHell.Exceptions;
using GohMdlExpert.Models.GatesOfHell.Resources.Data;
using static GohMdlExpert.Models.GatesOfHell.Resources.Data.PlyModel;
using Mesh = GohMdlExpert.Models.GatesOfHell.Resources.Data.PlyModel.Mesh;

namespace GohMdlExpert.Models.GatesOfHell.Serialization
{
    public class PlySerializer {
        private enum MashFlags {
            TwoSided = 0x01,     // render this mesh without culling
            useAlpha = 0x02,     // (unused, compatibility mode)
            Light = 0x04,     // use realtime scene light (unused, compatibility mode)
            PlayerColor = 0x08,     // use player color light
            Skinned = 0x10,     // skinned mesh
            Shadow = 0x20,     // shadow volume mesh
            Mirrored = 0x40,     // has negative scaling
            BlendTexture = 0x80,     // blend by second texture alpha
            Bump = 0x100,    // bump-mapped
            Specular = 0x200,    // specular color stored
            Material = 0x400,    // format with material, not textures
            SubSkin = 0x800,    // sub-skin feature
            TwoTexture = 0x1000,   // two textures with one texcoord
            UseInGvd = 0x2000,   // using vertex declaration instead of fvf
            Lightmap = 0x4000,   // has lightmap
        }

        private const int READ_BUFFER_SIZE = 64;

        public PlyModel Deserialize(string fileName) {
            if (File.Exists(fileName)) {
                throw GohResourceFileException.IsNotExists(fileName);
            }

            if (Path.GetExtension(fileName) != ".ply") {
                throw GohResourceFileException.InvalidExtension(fileName, ".ply");
            }

            return Deserialize(new FileStream(fileName, FileMode.Open));
        }

        public PlyModel Deserialize(Stream modelFileStream) {
            try {
                Point3D minPoint, maxPoint;
                List<Mesh> meshes = [];
                List<string> skins = [];
                List<Point3D> points = [];
                List<Vector3D> normalizes = [];
                List<Point> uvPoints = [];
                List<Face> indicesList = [];

                byte[] readBytes = new byte[READ_BUFFER_SIZE];

                modelFileStream.Read(readBytes, 0, 8);
                var fileHeader = Encoding.ASCII.GetString(readBytes, 0, 8);

                if (!fileHeader.Contains("EPLY")) {
#warning Вынасти проверку формата файла куда-нибудь.
                    throw GohResourceFileException.InvalidFormat("!!!", "EPLY");
                }

                modelFileStream.Read(readBytes, 0, 24);

                minPoint = new Point3D() {
                    X = BitConverter.ToSingle(readBytes, 0 * 4),
                    Y = BitConverter.ToSingle(readBytes, 1 * 4),
                    Z = BitConverter.ToSingle(readBytes, 2 * 4),
                };

                maxPoint = new Point3D() {
                    X = BitConverter.ToSingle(readBytes, 3 * 4),
                    Y = BitConverter.ToSingle(readBytes, 4 * 4),
                    Z = BitConverter.ToSingle(readBytes, 5 * 4),
                };

                while (modelFileStream.Position < modelFileStream.Length) {
                    modelFileStream.Read(readBytes, 0, 4);

                    string chunkName = Encoding.ASCII.GetString(readBytes, 0, 4);

                    switch (chunkName) {
                        case "SKIN":
                            modelFileStream.Read(readBytes, 0, 4);
                            int skinCount = BitConverter.ToInt32(readBytes, 0);

                            for (int i = 0; i < skinCount; i++) {
                                int skinNameSize = modelFileStream.ReadByte();
                                modelFileStream.Read(readBytes, 0, skinNameSize);
                                skins.Add(Encoding.ASCII.GetString(readBytes, 0, skinNameSize));
                            }
                            break;
                        case "MESH":
                            modelFileStream.Read(readBytes, 0, 16);

                            int fvf = BitConverter.ToInt32(readBytes, 0);
                            int firstFace = BitConverter.ToInt32(readBytes, 4);
                            int faceCount = BitConverter.ToInt32(readBytes, 8);
                            int? specular = null;

                            byte[] bitMask = readBytes[12..16];

                            int flagsBitMask = BitConverter.ToInt32(readBytes, 12);

                            if ((flagsBitMask & (int)MashFlags.Specular) != 0) {
                                modelFileStream.Read(readBytes, 0, 4);
                                specular = BitConverter.ToInt32(readBytes, 0);
                            }

                            int materialNameSize = modelFileStream.ReadByte();

                            modelFileStream.Read(readBytes, 0, materialNameSize);
                            string materialName = Encoding.ASCII.GetString(readBytes, 0, materialNameSize);


                            if ((flagsBitMask & (int)MashFlags.SubSkin) != 0) {
                                int subSkinCount = modelFileStream.ReadByte();

                                for (int i = 0; i < subSkinCount; i++) {
                                    modelFileStream.ReadByte();
                                }
                            }

                            meshes.Add(new Mesh() { FirstFace = firstFace, FaceCount = faceCount, TextureName = materialName });

                            break;
                        case "VERT":
                            modelFileStream.Read(readBytes, 0, 8);
                            int vertexCount = BitConverter.ToInt32(readBytes, 0);
                            int vertexDataSize = BitConverter.ToInt16(readBytes, 4);
                            int vertexFlags = BitConverter.ToInt16(readBytes, 6);

                            bool someFlags = vertexDataSize == 60; //It's unclear why. For UV.

                            byte[] vertexesData = new byte[vertexCount * vertexDataSize];
                            modelFileStream.Read(vertexesData, 0, vertexesData.Length);

                            for (int i = 0; i < vertexCount; i++) {
                                byte[] vertexData = new byte[vertexDataSize];
                                Array.Copy(vertexesData, i * vertexDataSize, vertexData, 0, vertexDataSize);

                                var point = new Point3D {
                                    X = BitConverter.ToSingle(vertexData, 0 * 4) * -1,
                                    Y = BitConverter.ToSingle(vertexData, 1 * 4),
                                    Z = BitConverter.ToSingle(vertexData, 2 * 4)
                                };

                                var normal = new Vector3D {
                                    X = BitConverter.ToSingle(vertexData, 20),
                                    Y = BitConverter.ToSingle(vertexData, 24),
                                    Z = BitConverter.ToSingle(vertexData, 28)
                                };

                                //var d = BitConverter.ToSingle(vertexData, 3 * 4);
                                //var d1 = vertexData[16..20];

                                var uvPoint = new Point {
                                    X = BitConverter.ToSingle(vertexData, 32 + (someFlags ? 4 : 0)),
                                    Y = BitConverter.ToSingle(vertexData, 36 + (someFlags ? 4 : 0)),
                                };

                                points.Add(point);
                                normalizes.Add(normal);
                                uvPoints.Add(uvPoint);
                            }

                            break;
                        case "INDX":
                            modelFileStream.Read(readBytes, 0, 4);
                            int indicesCount = BitConverter.ToInt32(readBytes, 0);
                            byte[] faceReadBytes = new byte[6];

                            int readFaceCount = indicesCount / 3;


                            for (int i = 0; i < readFaceCount; i++) {
                                modelFileStream.Read(faceReadBytes, 0, faceReadBytes.Length);

                                var face = new Face();

                                face.PointIndices[0] = BitConverter.ToInt16(faceReadBytes, 2 * 0);
                                face.PointIndices[1] = BitConverter.ToInt16(faceReadBytes, 2 * 1);
                                face.PointIndices[2] = BitConverter.ToInt16(faceReadBytes, 2 * 2);

                                indicesList.Add(face);
                            }

                            break;
                        default:
                            break;
                    }
                }
                return new PlyModel(points, indicesList, normalizes, uvPoints, meshes, minPoint, maxPoint);
            } finally {
                modelFileStream.Dispose();
            }

        }
    }
}