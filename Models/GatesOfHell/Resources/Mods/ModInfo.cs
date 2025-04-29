using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GohMdlExpert.Models.GatesOfHell.Serialization;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Mods {
    public class ModInfo {
        private static readonly ModelDataSerializer s_dataSerializer = new ModelDataSerializer();

        public static readonly string FileName = "mod.info";

        public string Name { get; set; }
        public IEnumerable<string>? Tags { get; set; }
        public string? Description { get; set; }
        public string? MinGameVersion { get; set; } 
        public string? MaxGameVersion { get; set; }
        public IEnumerable<string>? Require { get; set; }

        private ModInfo(string name) {
            Name = name;
        }

        public static ModInfo Parse(string modInfoFileText) {
            var parameters = s_dataSerializer.Deserialize(modInfoFileText);

            var modInfo = new ModInfo((string)ModelDataSerializer.FindParameter(parameters, "name")!.Value.Data!) { 
                Tags = ((string?)ModelDataSerializer.FindParameter(parameters, "tags")?.Data)?.Split('"', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries),
                Description = (string?)ModelDataSerializer.FindParameter(parameters, "desc")?.Data,
                MinGameVersion = (string?)ModelDataSerializer.FindParameter(parameters, "minGameVersion")?.Data,
                MaxGameVersion = (string?)ModelDataSerializer.FindParameter(parameters, "maxGameVersion")?.Data,
                Require = ((string?)ModelDataSerializer.FindParameter(parameters, "require")?.Data)?.Split('"', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            };

            return modInfo;
        }
    }
}
