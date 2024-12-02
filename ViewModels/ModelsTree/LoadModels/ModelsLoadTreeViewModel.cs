using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Net.Sockets;
using System.Windows.Input;
using GohMdlExpert.Models.GatesOfHell.Resources;
using GohMdlExpert.Models.GatesOfHell.Resources.Humanskins;
using WpfMvvm.Collections.ObjectModel;

namespace GohMdlExpert.ViewModels.ModelsTree.LoadModels {
    public sealed class ModelsLoadTreeViewModel : ModelsTreeViewModel {
        private ModelsTreePlyFileViewModel? _approvedPlyItem;
        private readonly ObservableCollection<ModelsTreeTextureViewModel> _approvedTexturesItems;

        public ModelAdderViewModel ModelsAdder { get; }
        public GohHumanskinResourceProvider SkinResourceProvider { get; }
        public GohTextureProvider TextureProvider { get; }

        public GohFactionHumanskinResource? HumanskinResource => SkinResourceProvider.Current;

        public ModelsTreePlyFileViewModel? ApprovedPlyItem {
            get => _approvedPlyItem;
            set {
                _approvedPlyItem?.CancelApprove();
                _approvedPlyItem = value;

                OnPropertyChanged();
            }
        }

        public ObservableCollection<ModelsTreeTextureViewModel> ApprovedTextureItems => _approvedTexturesItems;

        public ICommand NextModelCommand => CommandManager.GetCommand(NextPly);
        public ICommand PastModelCommand => CommandManager.GetCommand(PastPly);

        public ModelsLoadTreeViewModel(ModelAdderViewModel modelsAdder, GohHumanskinResourceProvider skinResourceProvider, GohTextureProvider textureProvider) {
            ModelsAdder = modelsAdder;
            SkinResourceProvider = skinResourceProvider;
            TextureProvider = textureProvider;
            _approvedTexturesItems = [];

            ModelsAdder.ModelAdded += ModelAdded; ;
            SkinResourceProvider.ResourceUpdated += HumanskinResourceUpdated;
            _approvedTexturesItems.CollectionChanged += ApprovedTexturesItemsChanged; ;
        }

        public override void LoadData() {
            if (Items.Count != 0) {
                return;
            }

            if (HumanskinResource != null) {
                Items.Add(new ModelsTreeDirectoryViewModel(HumanskinResource.Source, this));
            }
        }

        public void MovePlyApprove(int offset) {
            if (ApprovedPlyItem != null && ApprovedPlyItem.Parent != null) {
                var currentList = ApprovedPlyItem.Parent.Items;

                int newIndex = currentList.IndexOf(ApprovedPlyItem) + offset;

                var newApproveItem = currentList.ElementAtOrDefault(newIndex) as ModelsTreePlyFileViewModel;
                newApproveItem?.Approve();
            }
        }

        public void NextPly() {
            MovePlyApprove(1);
        }

        public void PastPly() {
            MovePlyApprove(-1);
        }

        public void CancelApproveItems(IEnumerable<ModelsLoadTreeItemViewModel>? items = null) {
            foreach (var item in items ?? ApprovedTextureItems) {
                item.CancelApprove();
            }

            if (items == null) {
                ApprovedPlyItem = null;
                _approvedTexturesItems.Clear();
            }
        }

        public void CreatedItem(object? sender, EventArgs e) {
            if (sender is ModelsLoadTreeItemViewModel item) {
                if (item.Tree == this) {
                    item.PropertyChanged += ItemPropertyChanged;
                }
            }
        }

        private void ItemPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e) {
            var item = (ModelsLoadTreeItemViewModel)sender!;

            if (e.PropertyName == nameof(ModelsLoadTreeItemViewModel.IsApproved)) {
                if (item.IsApproved) {
                    ApprovedItem(item);
                } else {
                    CancelApprovedItem(item);
                }
            }
        }

        private void ApprovedItem(ModelsLoadTreeItemViewModel item) {
            switch (item) {
                case ModelsTreePlyFileViewModel plyItem:
                    ApprovedPlyItem = plyItem;
                    break;
                case ModelsTreeTextureViewModel textureItem:
                    _approvedTexturesItems.Add(textureItem);
                    break;
            }
        }

        private void CancelApprovedItem(ModelsLoadTreeItemViewModel item) {
            switch (item) {
                case ModelsTreePlyFileViewModel _:
                    ApprovedPlyItem = null;
                    break;
                case ModelsTreeTextureViewModel textureItem:
                    _approvedTexturesItems.Remove(textureItem);
                    break;
            }
        }

        private void HumanskinResourceUpdated(object? sender, EventArgs e) {
            UpdateData();
        }

        private void ModelAdded(object? sender, EventArgs e) {
            CancelApproveItems();
        }

        private void ApprovedTexturesItemsChanged(object? sender, NotifyCollectionChangedEventArgs e) {
            switch (e.Action) {
                case NotifyCollectionChangedAction.Add:
                    ((ModelsTreeTextureViewModel)e.NewItems![0]!).Approve();
                    break;
                case NotifyCollectionChangedAction.Remove:
                    ((ModelsTreeTextureViewModel)e.OldItems![0]!).CancelApprove();
                    break;
                case NotifyCollectionChangedAction.Replace:
                    ((ModelsTreeTextureViewModel)e.OldItems![0]!).CancelApprove();
                    ((ModelsTreeTextureViewModel)e.NewItems![0]!).Approve();
                    break;
            }
        }

    }
}
