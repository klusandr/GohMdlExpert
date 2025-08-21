using System.IO;
using System.Text;
using SystemPath = System.IO.Path;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Files {
    public class DefFile : GohResourceFile {
        public static string Extension => ".def";

        public DefFile(string name, string? path = null, string? relativePathPoint = null) : base(name, path, relativePathPoint) { }

        public override string? GetExtension() {
            return Extension;
        }

        public override void SaveData() {
            using var streamWriter = new StreamWriter(GetStream());
            var buildString = new StringBuilder();

            buildString.AppendLine("{game_entity")
                .Append("\t{extension \"").Append(SystemPath.GetFileNameWithoutExtension(Name)).Append(".mdl").AppendLine("\"}{RimLight}")
            .Append('}');

            streamWriter.WriteLine(buildString.ToString());
        }
    }
}
