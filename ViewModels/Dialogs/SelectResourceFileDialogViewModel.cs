using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GohMdlExpert.Models.GatesOfHell.Resources;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Models.GatesOfHell.Resources.Mods;
using GohMdlExpert.ViewModels.Trees.ResourceLoad;
using WpfMvvm.Data;
using WpfMvvm.ViewModels;

namespace GohMdlExpert.ViewModels.Dialogs {
    public class SelectResourceFileDialogViewModel : BaseViewModel {
        private readonly GohResourceProvider _gohResource;
        private GohResourceFile? _selectedResource;

        public ResourceLoadTreeViewModel Tree { get; }

        private readonly PropertyChangeHandler _treePropertyChangeHandler;

        public Type? ResourceFileType { get; set; }

        public GohResourceFile? SelectedResource {
            get => _selectedResource;
            private set {
                _selectedResource = value;
                OnPropertyChanged();
            }
        }

        public string? SelectedResourceFullName => SelectedResource?.GetFullPath();

        public ICommand SelectCommand => CommandManager.GetCommand(Select, canExecute: (_) => SelectedResource != null);

        public ICommand CancelCommand => CommandManager.GetCommand(Cancel);

        public event EventHandler? SelectEvent;
        public event EventHandler? CancelEvent;

        public SelectResourceFileDialogViewModel(GohResourceProvider gohResource) {
            _gohResource = gohResource;

            Tree = new ResourceLoadTreeViewModel();

            PropertyChangeHandler.AddHandler(nameof(SelectedResource), (_, _) => {
                OnPropertyChanged(nameof(SelectedResourceFullName));
                CommandManager.OnCommandCanExecuteChanged(nameof(SelectCommand));
            });

            _treePropertyChangeHandler = new PropertyChangeHandler(Tree)
                .AddHandlerBuilder(nameof(Tree.SelectedItem), (_, _) => {
                    if (Tree.SelectedItem is ResourceLoadTreeFileViewModel fileItem) {
                        SelectedResource = fileItem.ResourceFile; 
                    }
                });

            gohResource.ResourceUpdated += GohResourceUpdatedHandler;
        }

        public void Update() {
            if (_gohResource.IsResourceLoaded && _gohResource.ResourceLoader.Root != null) {
                GohResourceDirectory root;

                if (ResourceFileType != null) {
                    root = GohResourceLoading.FilterResource(_gohResource.ResourceLoader.Root, (f) => f.GetType() == ResourceFileType);
                } else {
                    root = _gohResource.ResourceLoader.Root;
                }

                Tree.Root = root;
                Tree.UpdateData();
            } else {
                Tree.ClearData();
            }
        }

        public void Clear() {
            SelectedResource = null;
            Tree.SelectedItem = null;
        }

        private void Select() {
            SelectEvent?.Invoke(this, EventArgs.Empty);
        }

        private void Cancel() {
            SelectedResource = null;
            CancelEvent?.Invoke(this, EventArgs.Empty);
        }

        private void GohResourceUpdatedHandler(object? sender, EventArgs e) {
            Update();
        }
    }
}

