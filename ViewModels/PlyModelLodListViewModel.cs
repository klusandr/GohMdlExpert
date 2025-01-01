using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;
using GohMdlExpert.Models.GatesOfHell.Resources;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Views;
using Microsoft.Win32;
using WpfMvvm.ViewModels;
using WpfMvvm.Views.Dialogs;

namespace GohMdlExpert.ViewModels {
    public class PlyModelLodListViewModel : BaseViewModel {
        private ObservableCollection<PlyFile>? _items;

        public ObservableCollection<PlyFile>? Items {
            get => _items;
            set {
                _items = value;
                OnPropertyChanged();
            }
        }
        public PlyFile? SelectedItem { get; set; }
        public int SelectedIndex { get; set; }

        public ICommand AddCommand => CommandManager.GetCommand(() => {
            if (Items == null) {
                return;
            }

            var fileDialog = new OpenFileDialog {
                Filter = "Ply files (*.ply)|*.ply"
            };

            if (fileDialog.ShowDialog() ?? false) {
                AddLod(new PlyFile(fileDialog.FileName));
            }
        });

        public ICommand RemoveCommand => CommandManager.GetCommand(RemoveSelectedLod);

        public PlyModelLodListViewModel() { }

        public void AddLod(PlyFile plyFile) {
            Items?.Add(plyFile);
        }

        public void RemoveSelectedLod() {
            Items?.RemoveAt(SelectedIndex);
        }
    }
}