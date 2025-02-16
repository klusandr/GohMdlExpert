using System.Collections.Specialized;
using System.ComponentModel;
using GohMdlExpert.Models.GatesOfHell.Media3D;
using GohMdlExpert.Models.GatesOfHell.Resources.Files.Aggregates;
using GohMdlExpert.Services;
using WpfMvvm.Collections.ObjectModel;
using WpfMvvm.Data;
using WpfMvvm.ViewModels.Controls;

namespace GohMdlExpert.ViewModels.Trees.OverviewModels {
    public class ModelsOverviewTreeViewModel : TreeViewModel {
        public HumanskinMdlOverviewViewModel Models3DViewModel { get; }
        public AggregateTextureListViewModel AggregateTextureListViewModel { get; }
        public PlyModelLodListViewModel LodListViewModel { get; }

        public ModelsOverviewTreeMdlViewModel? MdlItem {
            get => Items.ElementAtOrDefault(0) as ModelsOverviewTreeMdlViewModel;
            private set {
                ClearData();
                if (value != null) {
                    _items.Insert(0, value);
                    LoadData();
                }
            }
        }
        public IObservableEnumerable<TreeItemViewModel>? PlyItems => MdlItem?.Items;

        public ModelsOverviewTreeViewModel(HumanskinMdlOverviewViewModel models3DViewModel, TextureLoadService textureSelector) {
            Models3DViewModel = models3DViewModel;
            AggregateTextureListViewModel = new AggregateTextureListViewModel(textureSelector);
            LodListViewModel = new PlyModelLodListViewModel();

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
            if (MdlItem == null) {
                return;
            }

            if (PlyItems != null) {
                switch (e.Action) {
                    case NotifyCollectionChangedAction.Add: MdlItem.AddItem(new ModelsOverviewTreePlyViewModel(e.GetItem<PlyModel3D>()!, this)); break;
                    case NotifyCollectionChangedAction.Remove: MdlItem.RemoveItemAt(e.GetIndex()); break;
                    case NotifyCollectionChangedAction.Reset: MdlItem.ClearItems(); break;
                }
            }
        }

        private void MtlFilesChanged(object? sender, NotifyCollectionChangedEventArgs e) {
            if (MdlItem == null) {
                return;
            }

            switch (e.Action) {
                case NotifyCollectionChangedAction.Add: _items.Add(new ModelsOverviewTreeMtlViewModel(e.GetItem<KeyValuePair<string, AggregateMtlFile>>().Value, this)); break;
                case NotifyCollectionChangedAction.Remove: _items.RemoveAt(e.GetIndex() + 1); break;
                case NotifyCollectionChangedAction.Reset:
                    while (Items.Count() > 1) {
                        _items.RemoveAt(1);
                    }
                    break;
            }
        }
    }
}
