using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Net.Sockets;
using System.Windows.Input;
using GohMdlExpert.Models.GatesOfHell.Resources;
using GohMdlExpert.Models.GatesOfHell.Resources.Humanskins;
using WpfMvvm.Collections.ObjectModel;

namespace GohMdlExpert.ViewModels.ModelsTree.LoadModels {
    public sealed class ModelsLoadTreeViewModel : ModelsTreeViewModel {
        private ModelsTreePlyFileViewModel? _approvedPlyItem;

        public ModelAdderViewModel ModelsAdder { get; }
        public GohHumanskinResourceProvider SkinResourceProvider { get; }
        public GohTextureProvider TextureProvider { get; }

        public GohFactionHumanskinResource? HumanskinResource => SkinResourceProvider.Current;

        public ModelsTreePlyFileViewModel? ApprovedPlyItem {
            get => _approvedPlyItem;
            private set {
                _approvedPlyItem = value;
                OnPropertyChanged();
            }
        }

        public ICommand NextModelCommand => CommandManager.GetCommand(NextPly);
        public ICommand PastModelCommand => CommandManager.GetCommand(PastPly);

        public ModelsLoadTreeViewModel(ModelAdderViewModel modelsAdder, GohHumanskinResourceProvider skinResourceProvider, GohTextureProvider textureProvider) {
            ModelsAdder = modelsAdder;
            SkinResourceProvider = skinResourceProvider;
            TextureProvider = textureProvider;

            ModelsAdder.ModelAdded += ModelAddedHandler;
            ModelsAdder.CancelModelAdded += CancelModelAddedHandler;
            SkinResourceProvider.ResourceUpdated += HumanskinResourceUpdatedHandler;
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

        public void CreatedItemHandler(object? sender, EventArgs e) {
            if (sender is ModelsTreePlyFileViewModel item) {
                if (item.Tree == this) {
                    item.PropertyNotifyHandler.AddHandler(nameof(item.IsApproved), PlyFileApprovedChangeHandler);
                }
            }
        }

        private void PlyFileApprovedChangeHandler(object? sender, PropertyChangedEventArgs e) {
            var plyItem = (ModelsTreePlyFileViewModel)sender!;

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
