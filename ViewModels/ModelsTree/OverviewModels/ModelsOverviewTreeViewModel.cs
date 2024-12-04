using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using GohMdlExpert.Extensions;
using GohMdlExpert.Models.GatesOfHell.Media3D;
using GohMdlExpert.Models.GatesOfHell.Resources;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Properties;

namespace GohMdlExpert.ViewModels.ModelsTree.OverviewModels {
    public class ModelsOverviewTreeViewModel : ModelsTreeViewModel {
        private IEnumerable<MtlTexture>? _mtlTextures;

        public Models3DViewModel Models3DViewModel { get; }

        public ModelsOverviewTreeMdlViewModel MdlItem { get; }
        public ObservableCollection<ModelsTreeItemViewModel> PlyItems => MdlItem.Items;

        public IEnumerable<PlyAggregateMtlFile> MtlFiles => Models3DViewModel.AggregateMtlFiles.Values;
        public IEnumerable<MtlTexture>? MtlTextures {
            get => _mtlTextures;
            private set {
                _mtlTextures = value;
                OnPropertyChanged();
            }
        }

        public ModelsOverviewTreeViewModel(Models3DViewModel models3DViewModel) {
            Models3DViewModel = models3DViewModel;
            MtlTextures = [];

            MdlItem = new ModelsOverviewTreeMdlViewModel(new MdlFile("new_humanskin.mdl"), this);

            models3DViewModel.PlyModels.CollectionChanged += ModelsPlyChanged;
            models3DViewModel.AggregateMtlFiles.CollectionChanged += MtlFilesChanged;

            PropertyNotifyHandler.AddHandler(nameof(SelectedItem), () => {
                if (SelectedItem is ModelsOverviewTreeMtlViewModel mtlItem) {
                    SelectedMtlItem(mtlItem);
                }
            });

            LoadData();
        }

        private void SelectedMtlItem(ModelsOverviewTreeMtlViewModel mtlItem) {
            MtlTextures = mtlItem.MtlFile.Data;
        }

        private void MtlFilesChanged(object? sender, NotifyCollectionChangedEventArgs e) {
            switch (e.Action) {
                case NotifyCollectionChangedAction.Add:
                    var newMtlFile = ((KeyValuePair<string, PlyAggregateMtlFile>)e.NewItems![0]!).Value;
                    Items.Add(new ModelsOverviewTreeMtlViewModel(newMtlFile, this));
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldStartingIndex != -1) {
                        PlyItems.RemoveAt(e.OldStartingIndex + 1);
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    PlyItems?.Clear();
                    break;
                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Move:
                    break;
            }
        }

        private void ModelsPlyChanged(object? sender, NotifyCollectionChangedEventArgs e) {
            switch (e.Action) {
                case NotifyCollectionChangedAction.Add:
                    var newPlyModel = (PlyModel3D)e.NewItems![0]!;
                    PlyItems.Add(new ModelsOverviewTreePlyViewModel(newPlyModel, this));
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldStartingIndex != -1) {
                        PlyItems.RemoveAt(e.OldStartingIndex);
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    PlyItems?.Clear();
                    break;
                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Move:
                    break;
            }

        }

        public override void LoadData() {
            Items.Add(MdlItem);

            foreach (var item in MtlFiles) {
                Items.Add(new ModelsOverviewTreeMtlViewModel(item, this));
            }

        }
    }
}
