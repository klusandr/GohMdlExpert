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
        public IEnumerable<PlyFile>? Items { get; private set; }
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

        public event NotifyCollectionChangedEventHandler? CollectionChange;

        public PlyModelLodListViewModel() { }

        public void SetItems(IEnumerable<PlyFile>? plyFiles) {
            Items = plyFiles;
            OnPropertyChanged(nameof(Items));
        }

        public void AddLod(PlyFile plyFile) {
            CollectionChange?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, plyFile));
            OnPropertyChanged(nameof(Items));
        }

        public void RemoveSelectedLod() {
            if (SelectedItem != null) {
                CollectionChange?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, SelectedItem, SelectedIndex));
                OnPropertyChanged(nameof(Items));
            }
        }
    }
}