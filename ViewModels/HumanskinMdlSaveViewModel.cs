using System.Diagnostics;
using System.IO;
using GohMdlExpert.Models.GatesOfHell.Exceptions;
using GohMdlExpert.Models.GatesOfHell.Resources;
using GohMdlExpert.Models.GatesOfHell.Resources.Data;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Models.GatesOfHell.Resources.Files.Loaders;
using GohMdlExpert.Models.GatesOfHell.Resources.Files.Loaders.Files;
using GohMdlExpert.Models.GatesOfHell.Resources.Humanskins;
using GohMdlExpert.Models.GatesOfHell.Resources.Loaders;
using GohMdlExpert.Models.GatesOfHell.Resources.Mods;
using GohMdlExpert.Models.GatesOfHell.Serialization;
using GohMdlExpert.Properties;
using Microsoft.Win32;
using WpfMvvm.ViewModels;
using WpfMvvm.Views.Dialogs;

namespace GohMdlExpert.ViewModels
{
    public class HumanskinMdlSaveViewModel : BaseViewModel {
        private readonly SaveFileDialog _fileDialog;
        private readonly GohHumanskinResourceProvider _humanskinResourceProvider;
        private readonly OutputModProvider _gohOutputModProvider;
        private readonly IUserDialogProvider _userDialog;
        private string _currentModDirectory;

        private OutputModResource Mod => _gohOutputModProvider.Mod;

        public HumanskinMdlSaveViewModel(GohHumanskinResourceProvider humanskinResourceProvider, OutputModProvider gohOutputModProvider, IUserDialogProvider userDialog) {
            _humanskinResourceProvider = humanskinResourceProvider;
            _gohOutputModProvider = gohOutputModProvider;
            _userDialog = userDialog;
            _fileDialog = new SaveFileDialog() {
                Filter = GohResourceLoading.MdlFileOpenFilter,
                AddExtension = true,
                DefaultExt = "mdl"
            };

            _gohOutputModProvider.Mod = new OutputModResource("F:\\Steam Game\\steamapps\\common\\Call to Arms - Gates of Hell\\mods\\divisions");
            _currentModDirectory = "entity\\humanskin\\[germans]\\ger_test";
            Mod.AddDirectory("entity\\humanskin\\[germans]\\ger_test");
        }

        public void Save(MdlFile mdlFile, Dictionary<string, MtlTexture> mtlTextures) {
            var currentDirectory = Mod.Root.AlongPath(_currentModDirectory);
            var files = currentDirectory!.GetFiles();

            var oldFile = files.FirstOrDefault(f => f.Name == mdlFile.Name);

            if (oldFile != null) {
                if (true /*ask*/) {
                    currentDirectory.Items.Remove(oldFile);
                } else {
                    return;
                }
            }

            if (mdlFile.IsLoaderInitialize) {
                if (mdlFile.Loader != Mod.FileLoader) {
                    mdlFile = new MdlFile(mdlFile.Name) {
                        Data = mdlFile.Data,
                        Loader = Mod.FileLoader
                    };
                }
            } else {
                mdlFile.Loader = Mod.FileLoader;
            }

            mdlFile.Path = _currentModDirectory;

            Mod.AddFile(mdlFile);

            string defFileName = Path.GetFileNameWithoutExtension(mdlFile.Name) + ".def";

            DefFile? defFile = null;

            if (!files.Any(f => f.Name == defFileName)) {
                defFile = new DefFile(defFileName, currentDirectory.GetFullPath()) { 
                    Loader = Mod.FileLoader 
                };
            }

            var mtlFiles = new List<MtlFile>();
            var currentMtlFiles = currentDirectory.GetFiles().OfType<MtlFile>();

            foreach (var mtlTexture in mtlTextures) {
                var mtlFile = currentMtlFiles.FirstOrDefault(f => f.Name == mtlTexture.Key);

                if (mtlFile == null) {
                    mtlFiles.Add(new MtlFile(mtlTexture.Key, currentDirectory.GetFullPath()) {
                        Data = mtlTexture.Value,
                        Loader = Mod.FileLoader
                    });
                } else {
                    if (mtlFile.Data.DiffusePath != mtlTexture.Value.DiffusePath) {
                        var replaceDialogResult = _userDialog.Ask(
                            "The humanskin output directory contain another texture which your humanskin use. " +
                            "Replace this texture on your texture?" +
                            "\n Warning. Your new texture will be used by other humanskin.",
                            "Replace texture", QuestionType.YesNoCancel, QuestionResult.Cancel
                        );

                        if (replaceDialogResult == QuestionResult.Cancel) {
                            return;
                        } else if (replaceDialogResult == QuestionResult.Yes) {
                            mtlFile.Data = mtlTexture.Value;
                            mtlFiles.Add(mtlFile);
                        }
                    }
                }
            }

            Mod.CreateModDirectories();
            mdlFile.SaveData();
            defFile?.SaveData();
            mtlFiles.ForEach(f => f.SaveData());
        }

        public string? CreateMtlFile(MdlFile? mdlFile, IEnumerable<PlyFile> plyFiles, Dictionary<string, MtlTexture> mtlTextures) {
            _fileDialog.InitialDirectory = Path.GetDirectoryName(Settings.Default.LastSavedFile);
            _fileDialog.FileName = mdlFile?.Name ?? "";
            string fileName;

            if (_fileDialog.ShowDialog() == true) {
                var mod = _gohOutputModProvider.Mod;
                

                if (mdlFile != null && mdlFile.Loader is FileSystemFileLoader && mdlFile.Exists()) {
                                        
                } else {

                }
                
            }

            //if (pathElements[^3] != _humanskinResourceProvider.Current.Root.Name) {
            //    _userDialog.ShowWarning("Humanskin is saved incorrectly and may not work in the game. The correct way to save is \"resource\\entity\\humanskin\\[fraction_name]\\your_folder\\humanskin.mdl\".");
            //}
            //            ModelDataSerializer.ModelDataParameter parameters;

            //            if (/*mdlFile?.Exists() ??*/ false) {
            //                //parameters = mdlFile.Data.Parameters;
            //                //fileName = mdlFile.GetFullPath();

            //                //if (_userDialog.Ask(string.Format("Do you want to save \"{0}\" file?", mdlFile.GetFullPath()), "Save file") != QuestionResult.OK) {
            //                //    return null;
            //                //}
            //            } else {
            //                if (_fileDialog.ShowDialog() == true) {
            //                    fileName = _fileDialog.FileName;

            //                    var pathElements = fileName.Split('\\', StringSplitOptions.RemoveEmptyEntries);

            //                    //if (pathElements[^3] != _humanskinResourceProvider.Current.Root.Name) {
            //                    //    _userDialog.ShowWarning("Humanskin is saved incorrectly and may not work in the game. The correct way to save is \"resource\\entity\\humanskin\\[fraction_name]\\your_folder\\humanskin.mdl\".");
            //                    //}
            //#warning Upgrade humanskin saving.
            //                } else {
            //                    return null;
            //                }

            //                parameters = GohResourceLoading.GetHumanskinMdlParametersTemplate();
            //            }

            //            var newMdlFile = new MdlFile(fileName);
            //            var newDefFile = new DefFile(Path.GetFileNameWithoutExtension(newMdlFile.Name) + ".def", newMdlFile.Path);

            //            var mtlFiles = new List<MtlFile>();
            //            var refPlyFiles = new List<PlyFile>();
            //            var refLodFiles = new Dictionary<PlyFile, PlyFile[]>();

            //            foreach (var mtlTexture in mtlTextures) {
            //                mtlFiles.Add(new MtlFile(mtlTexture.Key, newMdlFile.Path) { Data = mtlTexture.Value });
            //            }

            //            foreach (var plyFile in plyFiles) {


            //                //var refPlyFile = new PlyFile(Path.Join("..", Path.GetRelativePath(_humanskinResourceProvider.Current.Root.GetFullPath(), plyFile.GetFullPath())));

            //                //refPlyFiles.Add(refPlyFile);

            //                //if (lodFiles.TryGetValue(plyFile, out var plyLogFiles)) {
            //                //    var refPlyLodFiles = new List<PlyFile>();
            //                //    foreach (var lodFile in plyLogFiles) {
            //                //        refPlyLodFiles.Add(new PlyFile(Path.Join("..", Path.GetRelativePath(_humanskinResourceProvider.Current.Root.GetFullPath(), lodFile.GetFullPath()))));
            //                //    }

            //                //    refLodFiles.Add(refPlyFile, [.. refPlyLodFiles]);
            //}
#warning Upgrade humanskin saving.


            //newMdlFile.Data = new MdlModel(parameters, refPlyFiles, refLodFiles);

            //newMdlFile.SaveData();

            //foreach (var mtlFile in mtlFiles) {
            //    mtlFile.SaveData();
            //}

            //if (!newDefFile.Exists()) {
            //    newDefFile.SaveData();
            //}

            //Settings.Default.LastSavedFile = fileName;
            //return fileName;
            return "";
        }
    }
}
