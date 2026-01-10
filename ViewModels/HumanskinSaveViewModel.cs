using System.IO;
using System.Windows.Input;
using GohMdlExpert.Models.GatesOfHell.Resources;
using GohMdlExpert.Models.GatesOfHell.Resources.Data;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Models.GatesOfHell.Resources.Humanskins;
using GohMdlExpert.Models.GatesOfHell.Resources.Mods;
using GohMdlExpert.Services;
using GohMdlExpert.ViewModels.Lists;
using GohMdlExpert.ViewModels.Trees.ResourceLoad;
using WpfMvvm.Collections.ObjectModel;
using WpfMvvm.Data;
using WpfMvvm.ViewModels;
using WpfMvvm.ViewModels.Controls.Menu;
using WpfMvvm.Views.Dialogs;

namespace GohMdlExpert.ViewModels {
    public class HumanskinSaveViewModel : BaseViewModel {
        public ResourceLoadTreeViewModel Tree { get; }

        private readonly GohHumanskinResourceProvider _humanskinResourceProvider;
        private readonly GohOutputModProvider _gohOutputModProvider;
        private readonly IUserDialogProvider _userDialog;
        private readonly RequestTextService _requestTextService;
        private bool _isLoadOnlyMod;
        private GohResourceDirectory? _currentDirectory;
        private string? _newDirectoryName;

        public OutputModResource Mod => _gohOutputModProvider.Mod;

        public bool ModIsLoaded => _gohOutputModProvider.ModIsLoaded;

        public GohResourceDirectory? CurrentDirectory {
            get => _currentDirectory;
            set {
                _currentDirectory = value;
                OnPropertyChanged();
            }
        }

        public ObservableList<ResourceSaveListItemViewModel> CurrentDirectoryItems { get; }

        public ObservableList<GohResourceFile> HumanskinFiles { get; }

        public string? HumanskinName {
            get => HumanskinFiles.FirstOrDefault(f => f is MdlFile)?.Name;
            set {
                if (value == null) {
                    OnPropertyChanged();
                    return;
                }

                var mdlFile = HumanskinFiles.FirstOrDefault(f => f is MdlFile);
                var defFile = HumanskinFiles.FirstOrDefault(f => f is DefFile);

                if (mdlFile != null) {
                    if (!value.EndsWith(MdlFile.Extension)) {
                        value = value.Trim();
                        value += MdlFile.Extension;
                    }

                    mdlFile.Name = value;

                    if (defFile != null) {
                        defFile.Name = PathUtils.GetPathWithoutExtension(value) + DefFile.Extension;
                    }
                }

                OnPropertyChanged();
            }
        }

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
                TreeUpdate();
                OnPropertyChanged();
            }
        }

        public ICommand AddDirectoryCommand => CommandManager.GetCommand(AddDirectory, canExecute: (_) => CurrentDirectory != null);
        public ICommand RemoveDirectoryCommand => CommandManager.GetCommand(RemoveDirectory, canExecute: (_) => CurrentDirectory != null);
        public ICommand SaveCommand => CommandManager.GetCommand(Seve, canExecute: (_) => CurrentDirectory != null);
        public ICommand CancelCommand => CommandManager.GetCommand(Cancel);

        public event EventHandler? Saved;
        public event EventHandler? Canceled;

        public HumanskinSaveViewModel(GohHumanskinResourceProvider humanskinResourceProvider, GohOutputModProvider gohOutputModProvider, IUserDialogProvider userDialog, RequestTextService requestTextService) {
            _humanskinResourceProvider = humanskinResourceProvider;
            _gohOutputModProvider = gohOutputModProvider;
            _userDialog = userDialog;
            _requestTextService = requestTextService;

            Tree = new ResourceLoadTreeViewModel() { Filter = (f) => false };
            CurrentDirectoryItems = [];
            HumanskinFiles = [];

            Tree.PropertyChangeHandler.AddHandler(nameof(Tree.SelectedItem), (_, _) => {
                if (Tree.SelectedItem != null) {
                    if (Tree.SelectedItem is ResourceLoadTreeDirectoryViewModel treeDirectory) {
                        CurrentDirectory = treeDirectory.ResourceDirectory;
                    }
                } else {
                    CurrentDirectory = null;
                }
            });

            Tree.ItemAdded += (s, e) => {
                e.Item.ContextMenuViewModel.AddItem(new MenuItemViewModel("Add directory", AddDirectoryCommand));
                e.Item.ContextMenuViewModel.AddItem(new MenuItemViewModel("Remove directory", RemoveDirectoryCommand));
            };

            PropertyChangeHandler
                .AddHandlerBuilder(nameof(CurrentDirectory), (_, _) => {
                    CurrentDirectoryItemsUpdate();
                    CommandManager.OnCommandCanExecuteChanged(nameof(SaveCommand));
                    CommandManager.OnCommandCanExecuteChanged(nameof(AddDirectoryCommand));
                })
                .AddHandlerBuilder(nameof(HumanskinName), (_, _) => {
                    CurrentDirectoryItemsUpdate();
                });

            _humanskinResourceProvider.ResourceUpdated += (_, _) => {
                TreeUpdate();
            };
        }

        public void SetHumanskin(MdlFile mdlFile, Dictionary<string, MtlTexture> mtlTextures) {
            HumanskinFiles.Clear();
            HumanskinFiles.Add(mdlFile);
            HumanskinFiles.Add(new DefFile(Path.GetFileNameWithoutExtension(mdlFile.Name) + DefFile.Extension));
            HumanskinFiles.AddRange(mtlTextures.Select(m => new MtlFile(m.Key) { Data = m.Value }));

            Tree.ClearSelect();
            Tree.ExpandToResourceElement(mdlFile.GetDirectoryPath()?.Replace(GohResourceLocations.Humanskin, null) ?? "");
            OnPropertyChanged(nameof(HumanskinName));
        }

        private void Seve() {
            if (CurrentDirectory == null) {
                return;
            }

            var savingItems = CurrentDirectoryItems.Where(i => i.Status != Statuses.None && !i.Ignore);

            if (savingItems.Any(i => i.Status == Statuses.Error)) {
                if (_userDialog.Ask("One or many exist textures have different meterial. \nIf save your textures, other human skin may be replaced self textures.\n" +
                    "Contine saving?", "Saving", QuestionType.OKCancel) != QuestionResult.OK) {
                    return;
                }
            }

            if (savingItems.Any(i => i.Status == Statuses.Warning)) {
                if (_userDialog.Ask("One or many files will be repleces. Contine saving?", "Saving", QuestionType.OKCancel) != QuestionResult.OK) {
                    return;
                }
            }

            foreach (var file in HumanskinFiles) {
                if (savingItems.Any(i => i.ResourceElement == file)) {
                    Mod.AddFile(file, CurrentDirectory);
                }
            }

            CurrentDirectory.UpdateData();
            Saved?.Invoke(this, EventArgs.Empty);
        }

        private void Cancel() {
            Canceled?.Invoke(this, EventArgs.Empty);
        }

        private void AddDirectory() {
            if (CurrentDirectory != null) {
                string? text = _requestTextService.Request(string.Format("Creating new directory\n{0}{1}", CurrentDirectory.GetFullPath(), GohResourceLoading.DIRECTORY_SEPARATE), "Directory creating", "NewDirectory");

                if (text != null) {
                    Mod.AddDirectory(PathUtils.GetPathFromComponents([CurrentDirectory.GetFullPath(), text]));
                    CurrentDirectory.UpdateData();
                    Tree.ExpandToResourceElement(CurrentDirectory.GetFullPath());
                }
            }
        }

        private void RemoveDirectory() {
            if (CurrentDirectory != null) {
                bool isEmpty = CurrentDirectory.Items.Count == 0;

                if (_userDialog.Ask(string.Format("Do you want remove \"{0}\" directory?{1}", CurrentDirectory.GetFullPath(), !isEmpty ? "\nDirectory is not empty!" : string.Empty), "Directory removing") == QuestionResult.OK) {
                    Mod.RemoveDirectory(CurrentDirectory.GetFullPath(), true);
                    OpenParentDirectory(CurrentDirectory);
                    CurrentDirectory.UpdateData();
                }
            }
        }

        private void TreeUpdate() {
            Tree.ClearSelect();

            if (IsLoadOnlyMod) {
                Tree.Root = Mod.Root.AlongPath(GohResourceLocations.Humanskin);
            } else {
                Tree.Root = _humanskinResourceProvider.Resource.Root;
            }

            Tree.UpdateData();
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
                    var newItem = new ResourceSaveListItemViewModel(item) { Status = Statuses.Good };

                    CurrentDirectoryItems.Add(newItem);

                    if (directoryFileItem != null) {
                        if (item is MtlFile mtlFile) {
                            var directoryMtlFile = (MtlFile)directoryFileItem.ResourceElement;

                            if (directoryMtlFile.Data.DiffusePath != mtlFile.Data.DiffusePath) {
                                newItem.Status = Statuses.Error;
                                newItem.Message = "Texture file have different material. If save your textures, other humanskin may be replaced self textures.";
                            } else {
                                newItem.Status = Statuses.Warning;
                            }
                        } else {
                            newItem.Status = Statuses.Warning;
                        }

                        CurrentDirectoryItems.Remove(directoryFileItem);
                    }
                }
            }
        }

        private void ListItemApproveEventHandler(object? sender, EventArgs e) {
            var listItem = (ResourceLoadListItemViewModel)sender!;

            if (listItem.ResourceElement is GohResourceDirectory resourceDirectory) {
                OpenDirectory(resourceDirectory);
            }
        }

        private void OpenDirectory(GohResourceDirectory resourceDirectory) {
            if (resourceDirectory.Name == "..") {
                OpenParentDirectory(CurrentDirectory);
            } else {
                OpenDirectory(resourceDirectory.GetFullPath().Replace(GohResourceLocations.Humanskin, null));
            }
        }

        private void OpenDirectory(string directoryPath) {
            Tree.ExpandToResourceElement(directoryPath);
        }

        private void OpenParentDirectory(GohResourceDirectory? resourceDirectory = null) {
            string directoryPath = resourceDirectory?.GetFullPath().Replace(GohResourceLocations.Humanskin, null) ?? GohResourceLoading.DIRECTORY_SEPARATE.ToString();
            directoryPath = PathUtils.GetPathWithoutLastElments(directoryPath, 1);
            OpenDirectory(directoryPath);
        }
    }
}
