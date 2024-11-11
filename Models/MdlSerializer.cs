using System;
using System.Collections;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace GohMdlExpert.Models {
    public class MdlSerialize {
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
            VolumeView
        }

        public struct MdlParameter {
            public MdlTypes Type { get; set; }
            public string? Name { get; set; }
            public object? Data { get; set; }

            public override string ToString() {
                string dataText = Data is IEnumerable<MdlParameter> dataCollection ? $"[{dataCollection.Count()}]" : Data.ToString();

                return $"{Type} {Name ?? ""} {dataText}";
            }
        }

        private static readonly Dictionary<string, MdlTypes> s_typeNames = new() {
            ["skeleton"] = MdlTypes.Skeleton,
            ["bone"] = MdlTypes.Bone,
            ["bone revolute"] = MdlTypes.BoneRevolute,
            ["limits"] = MdlTypes.Limits,
            ["speed"] = MdlTypes.Speed,
            ["orientation"] = MdlTypes.Orientation,
            ["matrix34"] = MdlTypes.Matrix34,
            ["Position"] = MdlTypes.Position,
            ["LODView"] = MdlTypes.LODView,
            ["VolumeView"] = MdlTypes.VolumeView,
        };

        private static readonly Dictionary<MdlTypes, Func<string, object?>> s_typesDataParse = new() {
            [MdlTypes.Skeleton] = ParseParameterCollection
        };
        

        public MdlSerialize() { }

        public static MdlParameter Deserialize(string text) {
            var buildText = new StringBuilder(text);

            ClearComments(buildText);
            SetSimpleType(buildText);

            return ParseParameter(buildText.ToString().Trim('{', '}'));
        }

        private static MdlParameter ParseParameter(string text) {
            var parameter = GetTypeNameData(text);

            string textData = (string)parameter.Data!;

            if (s_typesDataParse.TryGetValue(parameter.Type, out var parse)) {
                parameter.Data = parse(textData);
            } else {
                if (textData.Contains('{')) {
                    parameter.Data = ParseParameterCollection(textData);
                } else {
                    parameter.Data = textData;
                }
            }

            return parameter;
        }

        private static MdlParameter GetTypeNameData(string text) {
            var parameter = new MdlParameter {
                Type = (MdlTypes)int.Parse(text[(text.IndexOf('|') + 1)..text.IndexOf('|', text.IndexOf('|') + 1)])
            };

            int separatorIndex = text.IndexOf('{');

            if (separatorIndex != -1) {
                int nameIndex = text.IndexOf('"', 0, separatorIndex);

                if (nameIndex != -1) {
                    parameter.Name = text[(nameIndex + 1)..text.IndexOf('"', nameIndex + 1)];
                }

                parameter.Data = text[separatorIndex..];
            } else {
                parameter.Data = text[((text.IndexOf('|', text.IndexOf('|') + 1)) + 1)..].Trim(' ', '"');
            }


            return parameter;
        }

        private static IEnumerable<MdlParameter> ParseParameterCollection(string text) {
            List<MdlParameter> parameterlist = new();

            int open = 0;
            int close = 0;

            for (int i = 0; i < text.Length; i++) {
                if (text[i] == '{') {
                    open++;
                    for (int j = i + 1; j < text.Length; j++) {
                        if (text[j] == '{') { open++; }

                        if (text[j] == '}') {
                            close++;
                            if (open == close) {
                                parameterlist.Add(ParseParameter(text[(i + 1)..j]));
                                i = j + 1;
                                break;
                            }
                        }
                    }
                }
            }

            return parameterlist;
        }


        private static void SetSimpleType(StringBuilder buildText) {
            foreach (var doubleNameType in s_typeNames.Where((kv) => kv.Key.Contains(' '))) {
                buildText.Replace(doubleNameType.Key, $"|{(int)doubleNameType.Value}|");
            }

            foreach (var typeName in s_typeNames) {
                buildText.Replace(typeName.Key, $"|{(int)typeName.Value}|");
            }
        }

        private static void ClearComments(StringBuilder buildText) {
            for (int i = buildText.Length - 1; i >= 0; i--) {
                if (buildText[i] == ';') {
                    for (int j = i; j < buildText.Length; j++) {
                        if (buildText[j] == '\n' || j == buildText.Length - 1) {
                            buildText.Remove(i, j - i);
                            break;
                        }
                    }
                }  
            }
        }

        private static MdlTypes GetType(string type) => Enum.Parse<MdlTypes>(type, true);
    }
}
