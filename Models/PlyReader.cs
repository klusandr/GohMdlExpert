using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;

namespace GohMdlExpert.Models {
    public class PlyReader {
        private const int INDEX_SIZE = 6;

        private static readonly string[] s_chunkNames = { "SKIN", "MESH", "VERT", "INDX" };
        private static readonly int[] s_mashFormats = { 0x0644, 0x0604, 0x0404, 0x0704, 0x0744, 0x0C14 };

        public static int Offset = 0;

        public static List<Point3D> Points { get; set; } = new();
        public static List<Vector3D> Normalizes { get; set; } = new();
        public static List<Point> UVPoints { get; set; } = new();
        public static List<int> IndicesList { get; set; } = new();

        //public string Material = 

        public static void PlyRead (string fileName) {
            Points.Clear();
            Normalizes.Clear();
            IndicesList.Clear();
            UVPoints.Clear();

            using var modelFileStream = new FileStream(fileName, FileMode.Open);
            byte[] readBytes = new byte[128];

            modelFileStream.Read(readBytes, 0, 8);
            var fileHeader = Encoding.ASCII.GetString(readBytes, 0, 8);

            float[] readFloat = new float[6];
            modelFileStream.Read(readBytes, 0, 24);
            for (int i = 0; i < readFloat.Length; i++) {
                readFloat[i] = BitConverter.ToSingle(readBytes, i * 4);
            }

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
                            string skinName = Encoding.ASCII.GetString(readBytes, 0, skinNameSize);
                        }
                        break;
                    case "MESH":
                        modelFileStream.Read(readBytes, 0, 8);

                        modelFileStream.Read(readBytes, 0, 8);
                        int triangelsCount = BitConverter.ToInt32(readBytes, 0);
                        int materialInfo = BitConverter.ToInt32(readBytes, 4);

                        if (materialInfo != 0x0404 && materialInfo != 0x0C14) {
                            //modelFileStream.Read(readBytes, 0, 4);
                        }

                        int materialNameSize = modelFileStream.ReadByte();

                        modelFileStream.Read(readBytes, 0, materialNameSize);
                        string materialName = Encoding.ASCII.GetString(readBytes, 0, materialNameSize);

                        for (int i = 0; i < 100; i++) {
                            if (modelFileStream.ReadByte() == 'V') {
                                break;
                            }
                        }

                        modelFileStream.Position--;

                        break;
                    case "VERT":
                        modelFileStream.Read(readBytes, 0, 8);
                        int vertexCount = BitConverter.ToInt32(readBytes, 0);
                        int vertexDataSize = BitConverter.ToInt16(readBytes, 4);
                        int vertexFlags = BitConverter.ToInt16(readBytes, 6);

                        byte[] vertexesData = new byte[vertexCount * vertexDataSize];
                        modelFileStream.Read(vertexesData, 0, vertexesData.Length); 

                        for (int i = 0; i < vertexCount; i++) {
                            byte[] vertexData = new byte[vertexDataSize];
                            Array.Copy(vertexesData, i * vertexDataSize, vertexData, 0, vertexDataSize);

                            var point = new Point3D {
                                X = BitConverter.ToSingle(vertexData, 0 * 4) * - 1,
                                Y = BitConverter.ToSingle(vertexData, 2 * 4),
                                Z = BitConverter.ToSingle(vertexData, 1 * 4)
                            };

                            var normal = new Vector3D {
                                X = BitConverter.ToSingle(vertexData, 20),
                                Y = BitConverter.ToSingle(vertexData, 28),
                                Z = BitConverter.ToSingle(vertexData, 24)
                            };

                            var d = BitConverter.ToSingle(vertexData, 3 * 4);
                            var d1 = vertexData[16..20];

                            var uvPoint = new Point {
                                X = BitConverter.ToSingle(vertexData, 32),
                                Y = BitConverter.ToSingle(vertexData, 36),
                            };

                            Points.Add(point);
                            Normalizes.Add(normal);
                            UVPoints.Add(uvPoint);
                        }

                        break;
                    case "INDX":
                        modelFileStream.Read(readBytes, 0, 4);
                        int indicesCount = BitConverter.ToInt32(readBytes, 0);

                        byte[] indicesData = new byte[indicesCount * 2];
                        modelFileStream.Read(indicesData, 0, indicesData.Length);

                        for (int i = 0; i < indicesCount; i++) {
                            IndicesList.Add(BitConverter.ToInt16(indicesData, i * 2));
                        }

                        for (int i = 0; i < IndicesList.Count; i += 3) {
                            var temp = IndicesList[i + 1];
                            IndicesList[i + 1] = IndicesList[i + 2];
                            IndicesList[i + 2] = temp;
                        }

                        break;
                    default: 
                        break;
                }
            }


        }
    }

}