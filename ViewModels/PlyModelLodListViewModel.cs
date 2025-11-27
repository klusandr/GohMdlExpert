using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows.Input;
using GohMdlExpert.Extensions;
using GohMdlExpert.Models.GatesOfHell.Media3D;
using GohMdlExpert.Models.GatesOfHell.Resources;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Services;
using Microsoft.Win32;
using WpfMvvm.ViewModels;

namespace GohMdlExpert.ViewModels {
    public class PlyModelLodListViewModel : BaseViewModel {
        private PlyModel3D? _plyModel;
        private ObservableCollection<PlyFile>? _items;
        private int _selectedIndex;
        private Regex _lodFilefilterRegex;
        private readonly SelectResourceFileService _selectResourceFileService;

        public PlyModel3D? PlyModel {
            get => _plyModel;
            set {
                if (value != null) { 
                    Items = value.LodPlyFiles;
                } else {
                    _plyModel?.SetLodIndex(0);
                    Items = null;
                }

                _plyModel = value;
            }
        }

        public ObservableCollection<PlyFile>? Items {
            get => _items;
            private set {
                _items = value;
                OnPropertyChanged();
            }
        }

        public PlyFile? SelectedItem { get; set; }
        public int SelectedIndex {
            get => _selectedIndex;
            set {
                _selectedIndex = value;
                PlyModel?.Model.ClearSelectMaterial();
                PlyModel?.SetLodIndex(SelectedIndex + 1);
            }
        }

        public ICommand AddCommand => CommandManager.GetCommand(() => {
            if (Items == null || PlyModel == null) {
                return;
            }

            //var fileDialog = new OpenFileDialog {
            //    Filter = "Ply files (*.ply)|*.ply"
            //};

            //if (fileDialog.ShowDialog() ?? false) {
            //    AddLod(new PlyFile(fileDialog.FileName));
            //}

            //_lodFilefilterRegex ??= new Regex(string.Join("", GohResourceLoading.PlyFilesLoadFilters), RegexOptions.Compiled);
            //f => _lodFilefilterRegex.IsMatch(f.Name)

            _selectResourceFileService.SelectResourceFile<PlyFile>(initPath: PlyModel.PlyFile.GetDirectoryPath());
        });

        public ICommand RemoveCommand => CommandManager.GetCommand(RemoveSelectedLod);

        public PlyModelLodListViewModel(SelectResourceFileService selectResourceFileService) {
            _selectResourceFileService = selectResourceFileService;
        }

        public void AddLod(PlyFile plyFile) {
            Items?.Add(plyFile);
        }

        public void RemoveSelectedLod() {
            Items?.RemoveAt(SelectedIndex);
        }
    }
}