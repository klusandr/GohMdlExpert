using System.Text;

namespace GohMdlExpert.Models.GatesOfHell.Serialization {
    public abstract class ModelDataSerializer {
        private struct ParameterType {
            public string Name;
            public string? NameInText;
            public Func<string, object?>? Parser;
            public Func<object, string>? Writer;

            public ParameterType(string name, string? nameInText = null, Func<string, object?>? parser = null, Func<object, string>? writer = null) : this() {
                Name = name;
                NameInText = nameInText;
                Parser = parser;
                Writer = writer;
            }
        }

        public struct ModelDataParameter {
            public string Type { get; set; }
            public string? Name { get; set; }
            public object? Data { get; set; }

            public ModelDataParameter(string type, string? name = null, object? data = null) : this() {
                Type = type;
                Name = name;
                Data = data;
            }


            public override string ToString() {
                string? dataText = Data is IEnumerable<ModelDataParameter> dataCollection ? $"[{dataCollection.Count()}]" : Data?.ToString();

                return $"{Type} {Name ?? ""} {dataText ?? "null"}";
            }
        }

        private readonly Dictionary<string, ParameterType> _types;


        public ModelDataSerializer() {
            _types = [];
        }

        public static ModelDataParameter? FindParameterByName(IEnumerable<ModelDataParameter> parameters, string name) {
            return FindParameter(parameters, null, name);
        }

        public static ModelDataParameter? FindParameterByName(ModelDataParameter parameter, string name) {
            return FindParameter(parameter, null, name);
        }

        public static ModelDataParameter? FindParameter(IEnumerable<ModelDataParameter> parameters, string? type, string? name = null) {
            ModelDataParameter? result = null;

            foreach (var parameter in parameters) {
                result = FindParameter(parameter, type, name);
                if (result != null) {
                    break;
                }
            }

            return result;
        }

        public static ModelDataParameter? FindParameter(ModelDataParameter parameter, string? type, string? name = null) {
            if ((type == null || parameter.Type == type) && (name == null || parameter.Name == name)) {
                return parameter;
            }

            if (parameter.Data is IEnumerable<ModelDataParameter> parametersCollection) {
                return FindParameter(parametersCollection, type, name);
            }

            return null;
        }

        public virtual string Serialize(ModelDataParameter modelDataParameter) {
            var buildString = WriteParameter(modelDataParameter);

            buildString.Replace(',', '.');
            buildString.Replace('\\', '/');

            return buildString.ToString();
        }

        public virtual ModelDataParameter Deserialize(string text) {
            var buildText = new StringBuilder(text);

            buildText.Replace('\t', ' ');
            ClearComments(buildText);
            SetSimpleType(buildText);

            return ParseParameter(buildText.ToString().Trim('{', '}'));
        }

        public ModelDataParameter DesirializeParameter(string text, string parameterName) {
            int bracketIndex = text.LastIndexOf('{', text.IndexOf(parameterName));

            return Deserialize(text[bracketIndex..(GetIndexOfClosingBracket(text, bracketIndex) + 1)]);
        }

        protected void AddType(string typeName, string? typeInText = null, Func<string, object?>? parse = null, Func<object, string>? writer = null) {
            if (_types.ContainsKey(typeName)) {
                throw new ArgumentException("Error add type. Model data type already exists!", nameof(typeName));
            }

            _types.Add(typeName, new ParameterType(typeName, typeInText, parse, writer));
        }

        private static int GetIndexOfClosingBracket(string text, int indexOpen, char openBracket = '{', char closeBracket = '}') {
            int open = 0;
            int close = 0;

            for (int i = indexOpen; i < text.Length; i++) {
                if (text[i] == openBracket) {
                    open++;
                }

                if (text[i] == closeBracket) {
                    close++;
                }

                if (close == open) {
                    return i;
                }
            }

            return -1;
        }

        private StringBuilder WriteParameter(ModelDataParameter parameter, StringBuilder? buildString = null, int level = 0) {
            buildString ??= new StringBuilder();
            var type = _types[parameter.Type];

            buildString.Append('\t', level).Append('{').Append(type.NameInText);

            if (parameter.Name != null) {
                buildString.Append($" \"{parameter.Name}\"");
            }

            if (parameter.Data is IEnumerable<ModelDataParameter> parameters) {
                buildString.AppendLine();

                foreach (var subParameter in parameters) {
                    WriteParameter(subParameter, buildString, level + 1);
                }

                buildString.Append('\t', level).Append('}');

            } else {
                var dataText = new StringBuilder();

                if (parameter.Data != null) {
                    if (type.Writer != null) {
                        dataText.Append(type.Writer(parameter.Data));

                        if (dataText.ToString().Contains('\n')) {
                            dataText
                                .Insert(0, '\n').Append('\n')
                                .Replace("\n", new StringBuilder().Append('\n').Append('\t', level + 1).ToString())
                                .Remove(dataText.Length - 1, 1);
                        } else {
                            dataText.Insert(0, ' ');
                        }
                    } else {
                        dataText.Append(" \"").Append(parameter.Data).Append('\"');
                    }
                }

                buildString.Append(dataText).Append('}');
            }

            return buildString.AppendLine();
        }



        private ModelDataParameter ParseParameter(string text) {
            var parameter = GetTypeNameData(text);

            string textData = (string)parameter.Data!;

            if (_types.TryGetValue(parameter.Type, out var type) && type.Parser != null) {
                parameter.Data = type.Parser(textData.Replace('.', ','));
            } else {
                if (textData.Contains('{')) {
                    parameter.Data = ParseParameterCollection(textData);
                } else {
                    parameter.Data = textData;
                }
            }

            return parameter;
        }

        private List<ModelDataParameter> ParseParameterCollection(string text) {
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
            var parameter = new ModelDataParameter(GetTypeByIndex(int.Parse(text[(text.IndexOf('|') + 1)..text.IndexOf('|', text.IndexOf('|') + 1)])));

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
            foreach (var typeName in _types.Values.OrderBy(pt => pt.NameInText).Reverse()) {
                buildText.Replace('{' + (typeName.NameInText ?? typeName.Name), $"{{|{GetTypeIndex(typeName.Name)}|");
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
