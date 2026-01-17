using System.Collections.ObjectModel;
using System.Windows.Input;
using GohMdlExpert.Extensions;
using GohMdlExpert.Models.GatesOfHell.Media3D;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Services;
using WpfMvvm.ViewModels;

namespace GohMdlExpert.ViewModels {
    public class PlyModelLodListViewModel : BaseViewModel {
        private PlyModel3D? _plyModel;
        private ObservableCollection<PlyFile>? _items;
        private PlyFile? _selectedItem;
        private readonly SelectResourceFileService _selectResourceFileService;

        public PlyModel3D? PlyModel {
            get => _plyModel;
            set {
                if (value == null) {
                    _plyModel?.SetLodIndex(0);
                }

                _plyModel = value;
                OnPropertyChanged();
            }
        }

        public PlyFile? MainLod => PlyModel?.PlyFile;

        public ObservableCollection<PlyFile>? Items {
            get => _items;
            private set {
                _items = value;
                OnPropertyChanged();
            }
        }

        public PlyFile? SelectedItem {
            get => _selectedItem;
            set {
                _selectedItem = value;
                PlyModel?.Model.ClearSelectMaterial();

                var idndex = value != null && Items != null ? Items.IndexOf(value) : -1;
                PlyModel?.SetLodIndex(idndex + 1);
                OnPropertyChanged();
            }
        }

        public ICommand UpSelectLodCommand => CommandManager.GetCommand(UpSelectLod, canExecute: (_) => SelectedItem != null && Items?.IndexOf(SelectedItem) > 0);

        public ICommand DownSelectLodCommand => CommandManager.GetCommand(DownSelectLod, canExecute: (_) => SelectedItem != null && Items?.IndexOf(SelectedItem) < Items?.Count - 1);

        public ICommand AddCommand => CommandManager.GetCommand(() => {
            if (Items == null || PlyModel == null) {
                return;
            }

            var lodFile = _selectResourceFileService.SelectResourceFile<PlyFile>(initPath: PlyModel.PlyFile.GetDirectoryPath());

            if (lodFile != null) {
                Items.Add(lodFile);
            }
        });

        public ICommand RemoveCommand => CommandManager.GetCommand(RemoveSelectedLod);

        public PlyModelLodListViewModel(SelectResourceFileService selectResourceFileService) {
            _selectResourceFileService = selectResourceFileService;

            PropertyChangeHandler
                .AddHandlerBuilder(nameof(SelectedItem), (_, _) => {
                    if (SelectedItem == null) {
                        PlyModel?.SetLodIndex(0);
                    }

                    CommandManager.OnCommandCanExecuteChanged(nameof(UpSelectLodCommand));
                    CommandManager.OnCommandCanExecuteChanged(nameof(DownSelectLodCommand));
                })
                .AddHandlerBuilder(nameof(PlyModel), (_, _) => {
                    Items = PlyModel?.LodPlyFiles;
                    SelectedItem = PlyModel?.PlyFile;

                    OnPropertyChanged(nameof(MainLod));
                })
            ;
        }

        public void AddLod(PlyFile plyFile) {
            Items?.Add(plyFile);
        }

        public void RemoveSelectedLod() {
            if (SelectedItem != null) {
                Items?.Remove(SelectedItem);
            }
        }

        private void UpSelectLod() {
            if (SelectedItem != null && Items != null) {
                var index = Items.IndexOf(SelectedItem);
                Items.Move(index, index - 1);
            }
        }

        private void DownSelectLod() {
            if (SelectedItem != null && Items != null) {
                var index = Items.IndexOf(SelectedItem);
                Items.Move(index, index + 1);
            }
        }
    }
}