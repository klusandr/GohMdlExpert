using System.Text;

namespace ConsoleApp4 {
    internal class Program {
        private class Rank {
            public string Name;
            public string File;
        }
        // schutze;
        // oberschutze;
        // gefreiter;
        // obergefreiter;
        // oberstabsgefreiter;
        // stabsgefreiter;

        // unteroffizier;
        // unterfeldwebel;
        // feldwebel;
        // hauptfeldwebel;
        // oberfeldwebel;
        // stabsfeldwebel;


        // leutnant;
        // oberleutnant;
        // hauptmann;
        // major;
        // oberstleutnant;
        // oberst; 
        private const string MODEL_DIRECTORY = "F:\\SDK\\Content\\goh\\humanskin\\[germans]\\ger_heer_43_panzer\\";
        private const string RANK_DIRECTORY = "F:\\SDK\\Content\\goh\\humanskin\\[germans]\\[ger_source]\\ger_tunic_pzwrap\\ger_ranks\\heer_ranks\\";

        private const string RESULT_DIRECTORY = "F:\\Steam Game\\steamapps\\common\\Call to Arms - Gates of Hell\\mods\\divisions\\resource\\entity\\humanskin\\[germans]\\ger_heer_43_panzer\\";

        private const string SOURCE_SOLDER_FILE = MODEL_DIRECTORY + "ger_heer_43_pzwaffe_1.mdl";
        private const string SOURCE_UNTEROFFIZIER_FILE = MODEL_DIRECTORY + "ger_heer_43_pzwaffe_nco.mdl";
        private const string SOURCE_OFFIZIER_1_FILE = MODEL_DIRECTORY + "ger_heer_43_pzwaffe_1_co.mdl";
        private const string SOURCE_OFFIZIER_2_FILE = MODEL_DIRECTORY + "ger_heer_43_pzwaffe_2_co.mdl";

        private const string SOURCE_DEF_FILE_TEXT = "{game_entity\r\n\t{extension \"gg\"}{RimLight}\r\n}";

        private const string SURCE_RANK_NAME_PART = "ger_rank_heer_pzwrap";

        private static Rank[] _solderRanks = {
            new() { Name = "schutze" },
            new() { Name = "oberschutze" },
            new() { Name = "gefreiter" },
            new() { Name = "stabsgefreiter" },
            new() { Name = "obergefreiter" },
            new() { Name = "oberstabsgefreiter" }
        };

        private static Rank[] _unterOffizierRanks = {
            new() { Name = "unteroffizier" },
            new() { Name = "unterfeldwebel" },
            new() { Name = "feldwebel" },
            new() { Name = "hauptfeldwebel" },
            new() { Name = "oberfeldwebel" },
            new() { Name = "stabsfeldwebel" },
        };

        private static Rank[] _offizierRanks = {
            new() { Name = "leutnant" },
            new() { Name = "oberleutnant" },
            new() { Name = "hauptmann" },
            new() { Name = "major" },
            new() { Name = "oberstleutnant" },
            new() { Name = "oberst" }
        };

        static void Main(string[] args) {

            PlyReader.PlyRead("F:\\SDK\\Content\\goh\\humanskin\\[germans]\\[ger_source]\\ger_tunic_pzwrap\\ger_ranks\\heer_ranks\\ger_rank_heer_pzwrap_em_00_schutze.ply");
            return;
            var ranksFiles = Directory.GetFiles(RANK_DIRECTORY);

            foreach (var rank in _solderRanks.Concat(_unterOffizierRanks).Concat(_offizierRanks)) {
                var rankFile = ranksFiles.First(rf => {
                    var a = rf.IndexOf(rank.Name);

                    return a != -1 && rf[a - 1] == '_' && rf[a + rank.Name.Length] == '.';
                });
                rank.File = rankFile[(rankFile.LastIndexOf('\\') + 1)..];
            }

            int rankIndex = 0;
            var dd = new Dictionary<string, string>();

            var sourceFileText = File.ReadAllText(SOURCE_SOLDER_FILE);



            foreach (var rank in _solderRanks) {
                int rankModelNameIndex = sourceFileText.IndexOf(SURCE_RANK_NAME_PART);
                int rankModelNameLength = sourceFileText.IndexOf(".ply", rankModelNameIndex) + 4 - rankModelNameIndex;
                string newFileText = sourceFileText.Remove(rankModelNameIndex, rankModelNameLength);
                newFileText = newFileText.Insert(rankModelNameIndex, rank.File);

                string newFileName = $"ger_heer_43_pzwaffe_{rankIndex:d2}a_{rank.Name}.mdl";
                rankIndex++;

                dd.Add(newFileName, newFileText);
            }

            sourceFileText = File.ReadAllText(SOURCE_UNTEROFFIZIER_FILE);

            foreach (var rank in _unterOffizierRanks) {
                int rankModelNameIndex = sourceFileText.IndexOf(SURCE_RANK_NAME_PART);
                int rankModelNameLength = sourceFileText.IndexOf(".ply", rankModelNameIndex) + 4 - rankModelNameIndex;
                string newFileText = sourceFileText.Remove(rankModelNameIndex, rankModelNameLength);
                newFileText = newFileText.Insert(rankModelNameIndex, rank.File);

                string newFileName = $"ger_heer_43_pzwaffe_{rankIndex:d2}a_{rank.Name}.mdl";
                rankIndex++;

                dd.Add(newFileName, newFileText);
            }

            sourceFileText = File.ReadAllText(SOURCE_OFFIZIER_1_FILE);

            foreach (var rank in _offizierRanks[..2]) {
                int rankModelNameIndex = sourceFileText.IndexOf(SURCE_RANK_NAME_PART);
                int rankModelNameLength = sourceFileText.IndexOf(".ply", rankModelNameIndex) + 4 - rankModelNameIndex;
                string newFileText = sourceFileText.Remove(rankModelNameIndex, rankModelNameLength);
                newFileText = newFileText.Insert(rankModelNameIndex, rank.File);

                string newFileName = $"ger_heer_43_pzwaffe_{rankIndex:d2}a_{rank.Name}.mdl";
                rankIndex++;

                dd.Add(newFileName, newFileText);
            }

            sourceFileText = File.ReadAllText(SOURCE_OFFIZIER_2_FILE);

            foreach (var rank in _offizierRanks[2..]) {
                int rankModelNameIndex = sourceFileText.IndexOf(SURCE_RANK_NAME_PART);
                int rankModelNameLength = sourceFileText.IndexOf(".ply", rankModelNameIndex) + 4 - rankModelNameIndex;
                string newFileText = sourceFileText.Remove(rankModelNameIndex, rankModelNameLength);
                newFileText = newFileText.Insert(rankModelNameIndex, rank.File);

                string newFileName = $"ger_heer_43_pzwaffe_{rankIndex:d2}a_{rank.Name}.mdl";
                rankIndex++;

                dd.Add(newFileName, newFileText);
            }

            foreach (var model in dd) {
                File.WriteAllText(Path.Combine(RESULT_DIRECTORY, model.Key), model.Value);
                File.WriteAllText(Path.Combine(RESULT_DIRECTORY, model.Key.Replace("mdl", "def")), SOURCE_DEF_FILE_TEXT.Replace("gg", model.Key));
            }
        }
    }
}