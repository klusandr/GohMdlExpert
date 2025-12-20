using System.ComponentModel;
using System.Windows.Input;
using GohMdlExpert.Models.GatesOfHell.Resources;
using GohMdlExpert.Models.GatesOfHell.Resources.Humanskins;
using WpfMvvm.Collections;
using WpfMvvm.Extensions;
using WpfMvvm.ViewModels.Controls;

namespace GohMdlExpert.ViewModels.Trees.LoadModels {
    public sealed class ModelsLoadTreeViewModel : TreeViewModel {
        private ModelsLoadTreePlyFileViewModel? _approvedPlyItem;

        public PlyModelAdderViewModel ModelsAdder { get; }
        public DefaultTextureViewModel DefaultTexture { get; }
        public GohHumanskinResourceProvider SkinResourceProvider { get; }
        public GohTextureProvider TextureProvider { get; }

        public IGohHumanskinResource? HumanskinResource => SkinResourceProvider.Resource;

        public ModelsLoadTreePlyFileViewModel? ApprovedPlyItem {
            get => _approvedPlyItem;
            private set {
                _approvedPlyItem = value;
                OnPropertyChanged();
            }
        }

        public ICommand NextModelCommand => CommandManager.GetCommand(NextPly);
        public ICommand PastModelCommand => CommandManager.GetCommand(PastPly);

        public ModelsLoadTreeViewModel(PlyModelAdderViewModel modelsAdder, DefaultTextureViewModel defaultTexture, GohHumanskinResourceProvider skinResourceProvider, GohTextureProvider textureProvider) {
            ModelsAdder = modelsAdder;
            DefaultTexture = defaultTexture;
            SkinResourceProvider = skinResourceProvider;
            TextureProvider = textureProvider;
            
            ModelsAdder.ModelAdded += ModelAddedHandler;
            ModelsAdder.CancelModelAdded += CancelModelAddedHandler;
            SkinResourceProvider.ResourceUpdated += HumanskinResourceUpdatedHandler;
        }

        public override void LoadData() {
            if (Items.Any()) {
                return;
            }

            if (HumanskinResource != null) {
                App.Current.Synchronize(() => _items.Add(new ModelsLoadTreeDirectoryViewModel(HumanskinResource.Source, this)));  
            }
        }

        public void MovePlyApprove(int offset) {
            if (ApprovedPlyItem != null && ApprovedPlyItem.Parent != null) {
                var currentList = ApprovedPlyItem.Parent.Items;

                int newIndex = currentList.FindIndex(ApprovedPlyItem) + offset;

                var newApproveItem = currentList.ElementAtOrDefault(newIndex) as ModelsLoadTreePlyFileViewModel;
                newApproveItem?.Approve();
            }
        }

        public void NextPly() {
            MovePlyApprove(1);
        }

        public void PastPly() {
            MovePlyApprove(-1);
        }

        public void CreatedItemHandler(object? sender, EventArgs e) {
            if (sender is ModelsLoadTreePlyFileViewModel item) {
                if (item.Tree == this) {
                    item.PropertyChangeHandler.AddHandler(nameof(item.IsApproved), PlyFileApprovedChangeHandler);
                }
            }
        }

        private void PlyFileApprovedChangeHandler(object? sender, PropertyChangedEventArgs e) {
            var plyItem = (ModelsLoadTreePlyFileViewModel)sender!;

            if (plyItem.IsApproved) {
                ApprovedPlyItem?.CancelApprove();
                ApprovedPlyItem = plyItem;
            } else {
                ApprovedPlyItem = null;
            }
        }

        private void HumanskinResourceUpdatedHandler(object? sender, EventArgs e) {
            UpdateData();
        }

        private void ModelAddedHandler(object? sender, EventArgs e) {
            ApprovedPlyItem?.CancelApprove();
        }


        private void CancelModelAddedHandler(object? sender, EventArgs e) {
            ApprovedPlyItem?.CancelApprove();
        }
    }
}
