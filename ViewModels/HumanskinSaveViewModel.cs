using System.Diagnostics;
using System.IO;
using System.Windows.Documents;
using System.Windows.Input;
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
using GohMdlExpert.ViewModels.Lists;
using GohMdlExpert.ViewModels.Trees.ResourceLoad;
using GohMdlExpert.Views;
using Microsoft.Win32;
using WpfMvvm.Collections.ObjectModel;
using WpfMvvm.ViewModels;
using WpfMvvm.Views.Dialogs;

namespace GohMdlExpert.ViewModels
{
    public class HumanskinSaveViewModel : BaseViewModel {
        private readonly SaveFileDialog _fileDialog;

        public ResourceLoadTreeViewModel Tree { get; }

        private readonly GohHumanskinResourceProvider _humanskinResourceProvider;
        private readonly GohOutputModProvider _gohOutputModProvider;
        private readonly IUserDialogProvider _userDialog;

        private bool _isLoadOnlyMod;
        private GohResourceDirectory? _currentDirectory;
        private string? _newDirectoryName;

        private OutputModResource Mod => _gohOutputModProvider.Mod;

        public GohResourceDirectory? CurrentDirectory {
            get => _currentDirectory;
            set {
                _currentDirectory = value;
                OnPropertyChanged();
            }
        }

        public ObservableList<ResourceSaveListItemViewModel> CurrentDirectoryItems { get; }

        public ObservableList<GohResourceFile> HumanskinFiles { get; }

        public string? NewDirectoryName {
            get => _newDirectoryName;
            set {
                _newDirectoryName = value;
                OnPropertyChanged();
            }
        }

        public bool IsLoadOnlyMod {
            get => _isLoadOnlyMod;
            set {
                _isLoadOnlyMod = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddDirectoryCommand => CommandManager.GetCommand(AddDirectory);

        private void AddDirectory() {
            if (Tree.SelectedItem != null && Tree.SelectedItem.ResourceElement is GohResourceDirectory resourceDirectory) {
                if (NewDirectoryName != null) {
                    Mod.AddDirectory(resourceDirectory.GetFullPath(), NewDirectoryName);
                }   
            }
        }

        public HumanskinSaveViewModel(GohHumanskinResourceProvider humanskinResourceProvider, GohOutputModProvider gohOutputModProvider, IUserDialogProvider userDialog) {
            _humanskinResourceProvider = humanskinResourceProvider;
            _gohOutputModProvider = gohOutputModProvider;
            _userDialog = userDialog;
            _fileDialog = new SaveFileDialog() {
                Filter = GohResourceLoading.MdlFileOpenFilter,
                AddExtension = true,
                DefaultExt = "mdl"
            };

            Tree = new ResourceLoadTreeViewModel() { Filter = (f) => false };
            CurrentDirectoryItems = [];
            HumanskinFiles = [];

            //_gohOutputModProvider.Mod = new OutputModResource("F:\\Steam Game\\steamapps\\common\\Call to Arms - Gates of Hell\\mods\\divisions");
            //_currentModDirectory = "entity\\humanskin\\[germans]\\ger_test";
            //Mod.AddDirectory("entity\\humanskin\\[germans]\\ger_test");

            Tree.PropertyChangeHandler.AddHandler(nameof(Tree.SelectedItem), (_, _) => {
                if (Tree.SelectedItem != null) {
                    if (Tree.SelectedItem is ResourceLoadTreeDirectoryViewModel treeDirectory) {
                        CurrentDirectory = treeDirectory.ResourceDirectory;
                    }
                } else {
                    CurrentDirectory = null;
                }
            });

            PropertyChangeHandler.AddHandler(nameof(CurrentDirectory), (_, _) => {
                CurrentDirectoryItemsUpdate();
            });

            _humanskinResourceProvider.ResourceUpdated += (_, _) => {
                TreeUpdate();
            };
        }

        private void CurrentDirectoryItemsUpdate() {
            CurrentDirectoryItems.Clear();

            if (CurrentDirectory != null) {
                var listItem = new ResourceSaveListItemViewModel(new GohResourceVirtualDirectory(".."));
                listItem.ApproveEvent += ListItemApproveEventHandler;
                CurrentDirectoryItems.Add(listItem);

                foreach (var item in CurrentDirectory.Items) {
                    listItem = new ResourceSaveListItemViewModel(item);

                    if (item is GohResourceDirectory) {
                        listItem.ApproveEvent += ListItemApproveEventHandler;
                    }

                    CurrentDirectoryItems.Add(listItem);
                }

                var currentDirectoryFilesItems = CurrentDirectoryItems.Where(i => i.ResourceElement is GohResourceFile);

                foreach (var item in HumanskinFiles) {
                    var directoryFileItem = currentDirectoryFilesItems.FirstOrDefault(f => f.ResourceElement.Name == item.Name);

                    if (directoryFileItem != null) {
                        if (item is MtlFile mtlFile) {
                            var directoryMtlFile = (MtlFile)directoryFileItem.ResourceElement;

                            if (directoryMtlFile.Data.DiffusePath != mtlFile.Data.DiffusePath) {
                                directoryFileItem.Status = WpfMvvm.Data.Statuses.Error;
                            } else {
                                directoryFileItem.Status = WpfMvvm.Data.Statuses.Warning;
                            }
                        } else {
                            directoryFileItem.Status = WpfMvvm.Data.Statuses.Warning;
                        }
                    } else {
                        CurrentDirectoryItems.Add(new ResourceSaveListItemViewModel(item) { Status = WpfMvvm.Data.Statuses.Good });
                    }
                }
            }
        }

        private void ListItemApproveEventHandler(object? sender, EventArgs e) {
            var listItem = (ResourceLoadListItemViewModel)sender!;

            if (listItem.ResourceElement is GohResourceDirectory resourceDirectory) {
                string directoryPath; 

                if (resourceDirectory.Name != "..") {
                    directoryPath = resourceDirectory.GetFullPath().Replace(GohResourceLocations.Humanskin, null);
                } else {
                    directoryPath = CurrentDirectory?.GetFullPath().Replace(GohResourceLocations.Humanskin, null) ?? GohResourceLoading.DIRECTORY_SEPARATE.ToString();
                    directoryPath = PathUtils.GetPathWithoutLastElments(directoryPath, 1);
                }

                Tree.ExpandetDirectory(directoryPath);
            }
        }

        public void SetHumanskin(MdlFile mdlFile, Dictionary<string, MtlTexture> mtlTextures) {
            HumanskinFiles.Clear();

            HumanskinFiles.Add(mdlFile);
            HumanskinFiles.Add(new DefFile(Path.GetFileNameWithoutExtension(mdlFile.Name) + DefFile.Extension) { Loader = Mod.FileLoader});
            HumanskinFiles.AddRange(mtlTextures.Select(m => new MtlFile(m.Key) { Data = m.Value, Loader = Mod.FileLoader }));
        }

        public void Save(MdlFile mdlFile, Dictionary<string, MtlTexture> mtlTextures) {

            var d = new ChildWindow() {
                Content = new HumanskinSaveView() { ViewModel = this }
            };

            d.ShowDialog();

            return;

            var currentDirectory = _currentDirectory;
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

            //mdlFile.Path = _currentModDirectory;

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

        private void TreeUpdate() {
            if (IsLoadOnlyMod) {
                Tree.Root = Mod.Root.AlongPath(GohResourceLocations.Humanskin);
            } else {
                Tree.Root = _humanskinResourceProvider.Resource.Root;
            }

            Tree.UpdateData();
        }
    }
}
