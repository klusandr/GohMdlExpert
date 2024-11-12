using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GohMdlExpert.Models.GatesOfHell.Serialization.MdlSerialize;
using static System.Net.Mime.MediaTypeNames;

namespace GohMdlExpert.Models.GatesOfHell.Serialization {
    public abstract class ModelDataSerializer {
        public struct ModelDataParameter {
            public string Type { get; set; }
            public string? Name { get; set; }
            public object? Data { get; set; }

            public override string ToString() {
                string? dataText = Data is IEnumerable<ModelDataParameter> dataCollection ? $"[{dataCollection.Count()}]" : Data?.ToString();

                return $"{Type} {Name ?? ""} {dataText ?? "null"}";
            }
        }

        private readonly Dictionary<string, (string? nameInText, Func<string, object?>? parser)> _types;

       
        public ModelDataSerializer() {
            _types = new();
        }

        protected void AddType(string typeName, string? typeInText = null) {
            if (_types.ContainsKey(typeName)) {
                throw new ArgumentException("Error add type. Model data type already exists!", nameof(typeName));
            } 

            _types.Add(typeName, (typeInText, null));
        }

        protected void AddTypeDataParser(string typeName, Func<string, object?> parse) {
            if (!_types.TryGetValue(typeName, out (string? nameInText, Func<string, object?>? parser) value)) {
                throw new ArgumentException("Error add type parser. Model data type is not exists!", nameof(typeName));
            }

            _types[typeName] = (value.nameInText, parse);
        }

        public virtual string Serialize(ModelDataParameter modelDataParameter) {
            throw new NotImplementedException();
        }

        public virtual ModelDataParameter Deserialize(string text) {
            var buildText = new StringBuilder(text);

            ClearComments(buildText);
            SetSimpleType(buildText);

            return ParseParameter(buildText.ToString().Trim('{', '}'));
        }

        private ModelDataParameter ParseParameter(string text) {
            var parameter = GetTypeNameData(text);

            string textData = (string)parameter.Data!;

            if (_types.TryGetValue(parameter.Type, out var type) && type.parser != null) {
                parameter.Data = type.parser(textData);
            } else {
                if (textData.Contains('{')) {
                    parameter.Data = ParseParameterCollection(textData);
                } else {
                    parameter.Data = textData;
                }
            }

            return parameter;
        }

        private IEnumerable<ModelDataParameter> ParseParameterCollection(string text) {
            List<ModelDataParameter> parameterList = new();

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
                                parameterList.Add(ParseParameter(text[(i + 1)..j]));
                                i = j + 1;
                                break;
                            }
                        }
                    }
                }
            }

            return parameterList;
        }


        private ModelDataParameter GetTypeNameData(string text) {
            var parameter = new ModelDataParameter {
                Type = GetTypeByIndex(int.Parse(text[(text.IndexOf('|') + 1)..text.IndexOf('|', text.IndexOf('|') + 1)]))
            };

            int separatorIndex = text.IndexOf('{');

            if (separatorIndex != -1) {
                int nameIndex = text.IndexOf('"', 0, separatorIndex);

                if (nameIndex != -1) {
                    parameter.Name = text[(nameIndex + 1)..text.IndexOf('"', nameIndex + 1)];
                }

                parameter.Data = text[separatorIndex..];
            } else {
                parameter.Data = text[(text.IndexOf('|', text.IndexOf('|') + 1) + 1)..].Trim(' ', '"');
            }


            return parameter;
        }

        private void SetSimpleType(StringBuilder buildText) {
            foreach (var doubleNameType in _types.Where((kw) => kw.Key.Contains(' '))) {
                buildText.Replace(doubleNameType.Value.nameInText ?? doubleNameType.Key, $"|{GetTypeIndex(doubleNameType.Key)}|");
            }

            foreach (var typeName in _types) {
                buildText.Replace(typeName.Value.nameInText ?? typeName.Key, $"|{GetTypeIndex(typeName.Key)}|");
            }
        }

        private int GetTypeIndex(string typeName) {
            int index = 0;
            foreach (var key in _types.Keys) {
                if (key == typeName) { break; }
                index++;
            }

            return index;
        }

        private string GetTypeByIndex(int index) {
            return _types.Keys.ElementAt(index);
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
    }
}
