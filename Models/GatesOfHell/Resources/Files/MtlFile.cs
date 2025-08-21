using System.IO;
using System.Windows.Media;
using GohMdlExpert.Models.GatesOfHell.Exceptions;
using GohMdlExpert.Models.GatesOfHell.Resources.Data;
using GohMdlExpert.Models.GatesOfHell.Serialization;
using static GohMdlExpert.Models.GatesOfHell.Serialization.ModelDataSerializer;
using static GohMdlExpert.Models.GatesOfHell.Serialization.MtlSerializer;
using SystemPath = System.IO.Path;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Files
{
    public class MtlFile : GohResourceFile {
        private static MtlSerializer? s_serializer;

        protected static MtlSerializer Serializer => s_serializer ??= new MtlSerializer();

        public static string Extension => ".mtl";

        public new MtlTexture Data { get => (MtlTexture)base.Data; set => base.Data = value; }

        public MtlFile(string name, string? path = null, string? relativePathPoint = null)
            : base(name, path, relativePathPoint) { }

        public override string? GetExtension() {
            return Extension;
        }

        public override void LoadData() {
            var parameters = Serializer.Deserialize(GetAllText());
            var textureFilesPath = new List<string?>();

            foreach (var type in (IEnumerable<Types>)[Types.Diffuse, Types.Bump, Types.Specular]) {
                string? value = FindParameter(parameters, type.ToString())?.Data as string;

                if (value != null) {
                    value = value.Replace("$", "");
                }

                textureFilesPath.Add(value);
            }

            var color = FindParameter(parameters, Types.Color.ToString())?.Data as Color?;

            if (textureFilesPath[0] == null) {
                throw TextureException.TextureDiffuseMaterialIsNotDefine(this);
            }

            Data = new MtlTexture(textureFilesPath[0]!, textureFilesPath[1], textureFilesPath[2]) {
                Color = color
            };
        }

        public override void SaveData() {
            string str = Serializer.Serialize(new ModelDataParameter() {
                Type = Types.MaterialBump.ToString(),
                Data = new ModelDataParameter[] {
                    new() {
                       Type = Types.Diffuse.ToString(),
                       Data = '$' + SystemPath.Join(Data.Diffuse.Path, SystemPath.GetFileNameWithoutExtension(Data.Diffuse.Name)),
                    },
                    new() {
                       Type = Types.Bump.ToString(),
                       Data = '$' + SystemPath.Join(Data.Bump?.Path, SystemPath.GetFileNameWithoutExtension(Data.Bump?.Name)),
                    },
                    new() {
                       Type = Types.Specular.ToString(),
                       Data = '$' + SystemPath.Join(Data.Specular?.Path, SystemPath.GetFileNameWithoutExtension(Data.Specular?.Name)),
                    },
                    new() {
                       Type = Types.Color.ToString(),
                       Data = Data.Color,
                    },
                    new() {
                       Type = Types.Blend.ToString(),
                       Data = "none",
                    },
                }
            });

            using var stream = new StreamWriter(GetStream());
            stream.Write(str);
        }
    }
}
