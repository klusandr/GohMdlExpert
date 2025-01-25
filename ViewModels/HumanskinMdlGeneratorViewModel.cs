using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GohMdlExpert.Models.GatesOfHell.Resources;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Models.GatesOfHell.Resources.Humanskins;
using GohMdlExpert.Models.GatesOfHell.Serialization;
using GohMdlExpert.Properties;
using Microsoft.Win32;
using WpfMvvm.ViewModels;
using WpfMvvm.Views.Dialogs;

namespace GohMdlExpert.ViewModels {
    public class HumanskinMdlGeneratorViewModel : BaseViewModel {
        private readonly SaveFileDialog _fileDialog;
        private readonly GohHumanskinResourceProvider _humanskinResourceProvider;
        private readonly IUserDialogProvider _userDialog;

        public HumanskinMdlGeneratorViewModel(GohHumanskinResourceProvider humanskinResourceProvider, IUserDialogProvider userDialog) {
            _humanskinResourceProvider = humanskinResourceProvider;
            _userDialog = userDialog;
            _fileDialog = new SaveFileDialog() {
                Filter = ResourceLoading.MdlFileOpenFilter,
                AddExtension = true,
                DefaultExt = "mdl"
            };
        }

        public string? CreateMtlFile(MdlFile? mdlFile, IEnumerable<PlyFile> plyFiles, Dictionary<string, MtlTexture> mtlTextures, Dictionary<PlyFile, PlyFile[]> lodFiles) {
            _fileDialog.InitialDirectory = Path.GetDirectoryName(Settings.Default.LastSavedFile);
            _fileDialog.FileName = mdlFile?.Name ?? "";

            string fileName;
            ModelDataSerializer.ModelDataParameter parameters;

            if (mdlFile?.Exists() ?? false) {
                parameters = mdlFile.Data.Parameters;
                fileName = mdlFile.GetFullPath();

                if (_userDialog.Ask(string.Format("Do you want to save \"{0}\" file?", mdlFile.GetFullPath()), "Save file") != QuestionResult.OK) {
                    return null;
                }
            } else {
                if (_fileDialog.ShowDialog() == true) {
                    fileName = _fileDialog.FileName;
                } else {
                    return null;
                }
                    
                parameters = ResourceLoading.GetHumanskinMdlParametersTemplate();
            }

            var newMdlFile = new MdlFile(fileName);
            var newDefFile = new DefFile(Path.GetFileNameWithoutExtension(newMdlFile.Name) + ".def", newMdlFile.Path);

            var mtlFiles = new List<MtlFile>();
            var refPlyFiles = new List<PlyFile>();
            var refLodFiles = new Dictionary<PlyFile, PlyFile[]>();

            foreach (var mtlTexture in mtlTextures) {
                mtlFiles.Add(new MtlFile(mtlTexture.Key, newMdlFile.Path) { Data = mtlTexture.Value });
            }

            foreach (var plyFile in plyFiles) {
                var refPlyFile = new PlyFile(Path.Join("..", Path.GetRelativePath(_humanskinResourceProvider.Current.Root.GetFullPath(), plyFile.GetFullPath())));

                refPlyFiles.Add(refPlyFile);

                if (lodFiles.TryGetValue(plyFile, out var plyLogFiles)) {
                    var refPlyLodFiles = new List<PlyFile>();
                    foreach (var lodFile in plyLogFiles) {
                        refPlyLodFiles.Add(new PlyFile(Path.Join("..", Path.GetRelativePath(_humanskinResourceProvider.Current.Root.GetFullPath(), lodFile.GetFullPath()))));
                    }

                    refLodFiles.Add(refPlyFile, [.. refPlyLodFiles]);
                }
            }

            newMdlFile.Data = new MdlModel(parameters, refPlyFiles, refLodFiles);

            newMdlFile.SaveData();

            foreach (var mtlFile in mtlFiles) {
                mtlFile.SaveData();
            }

            if (!newDefFile.Exists()) {
                newDefFile.SaveData();
            }

            Settings.Default.LastSavedFile = fileName;
            return fileName;
        }
    }
}
