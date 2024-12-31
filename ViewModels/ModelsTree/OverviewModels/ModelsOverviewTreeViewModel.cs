using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using GohMdlExpert.Models.GatesOfHell.Media3D;
using GohMdlExpert.Models.GatesOfHell.Resources;
using GohMdlExpert.Models.GatesOfHell.Resources.Files.Aggregates;
using WpfMvvm.Data;
using WpfMvvm.ViewModels.Controls;

namespace GohMdlExpert.ViewModels.ModelsTree.OverviewModels {
    public class ModelsOverviewTreeViewModel : TreeViewModel {
        private IEnumerable<MtlTexture>? _mtlTextures;
        private AggregateMtlFile? _selectedMtlFile;

        public HumanskinMdlOverviewViewModel Models3DViewModel { get; }
        public TextureMaterialListViewModel MaterialListViewModel { get; }
        public PlyModelLodListViewModel LodListViewModel { get; }

        public ModelsOverviewTreeMdlViewModel? MdlItem {
            get => Items.ElementAtOrDefault(0) as ModelsOverviewTreeMdlViewModel;
            private set {
                ClearData();
                if (value != null) {
                    Items.Insert(0, value);
                    LoadData();
                }
            }
        }
        public ObservableCollection<TreeItemViewModel>? PlyItems => MdlItem?.Items;

        public IEnumerable<MtlTexture>? MtlTextures {
            get => _mtlTextures;
            private set {
                _mtlTextures = value;
                OnPropertyChanged();
            }
        }

        public ModelsOverviewTreeViewModel(HumanskinMdlOverviewViewModel models3DViewModel) {
            Models3DViewModel = models3DViewModel;
            MaterialListViewModel = new TextureMaterialListViewModel();
            LodListViewModel = new PlyModelLodListViewModel();
            MtlTextures = [];

            models3DViewModel.PropertyChangeHandler.AddHandler(nameof(Models3DViewModel.MdlFile), MdlFileChangeHandler);
            models3DViewModel.PlyModels.CollectionChanged += ModelsPlyChanged;
            models3DViewModel.AggregateMtlFiles.CollectionChanged += MtlFilesChanged;
        }

        public override void LoadData() {
            foreach (var mtlFile in Models3DViewModel.AggregateMtlFiles.Values) {
                AddItem(new ModelsOverviewTreeMtlViewModel(mtlFile, this));
            }
        }

        private void MdlFileChangeHandler(object? sender, PropertyChangedEventArgs e) {
            if (Models3DViewModel.MdlFile != MdlItem?.MdlFile) {
                MdlItem = Models3DViewModel.MdlFile != null ? new ModelsOverviewTreeMdlViewModel(Models3DViewModel.MdlFile, this) : null;
            }
        }

        private void ModelsPlyChanged(object? sender, NotifyCollectionChangedEventArgs e) {
            if (PlyItems != null) {
                switch (e.Action) {
                    case NotifyCollectionChangedAction.Add: PlyItems.Add(new ModelsOverviewTreePlyViewModel(e.GetItem<PlyModel3D>()!, this)); break;
                    case NotifyCollectionChangedAction.Remove: PlyItems.RemoveAt(e.GetIndex()); break;
                    case NotifyCollectionChangedAction.Reset: PlyItems.Clear(); break;
                }
            }
        }

        private void MtlFilesChanged(object? sender, NotifyCollectionChangedEventArgs e) {
            switch (e.Action) {
                case NotifyCollectionChangedAction.Add: Items.Add(new ModelsOverviewTreeMtlViewModel(e.GetItem<KeyValuePair<string, AggregateMtlFile>>().Value, this)); break;
                case NotifyCollectionChangedAction.Remove: Items.RemoveAt(e.GetIndex() + 1); break;
                case NotifyCollectionChangedAction.Reset:
                    while (Items.Count > 1) {
                        Items.RemoveAt(1);
                    }
                break;
            }
        }
    }
}
