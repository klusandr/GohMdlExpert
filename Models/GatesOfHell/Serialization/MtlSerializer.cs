using System.Windows.Media;

namespace GohMdlExpert.Models.GatesOfHell.Serialization {
    public class MtlSerializer : ModelDataSerializer {
        public enum Types {
            MaterialBump,
            Diffuse,
            Bump,
            Specular,
            Color,
            Blend,
            FullSpecular,
        }

        public MtlSerializer() {
            AddType("MaterialBump", "material bump");
            AddType("Diffuse", "diffuse");
            AddType("Bump", "bump");
            AddType("Specular", "specular");
            AddType("Color", "color",
                (str) => {
                    var values = str.Split(' ').Select(v => Convert.ToByte(v)).ToArray();
                    return new Color() { A = values[0], R = values[1], G = values[2], B = values[3] };
                },
                (value) => {
                    var color = (Color)value;

                    return $"\"{color.A} {color.R} {color.G} {color.B}\"";
                }
            );
            AddType("Blend", "blend",
                (str) => {
                    return null;
                },
                (value) => {
                    return "none";
                }
            );
            AddType("FullSpecular", "full_specular");
        }
    }
}
