using GohMdlExpert.Models.GatesOfHell.Resources;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Views.ModelsTree;
using MvvmWpf.ViewModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Security.Policy;
using System.Windows.Controls;
using System.Windows.Input;

namespace GohMdlExpert.ViewModels.ModelsTree {
    public class ModelsTreeViewModel : ViewModelBase {
        private readonly HashSet<ModelsTreeItemViewModel> _approvedItems;

        public Models3DViewModel Models3DView { get; }
        public GohResourceLoader ResourceLoader { get; }
        public ObservableCollection<ModelsTreeItemViewModel> Items { get; }
        public IEnumerable<ModelsTreeItemViewModel> ApprovedItems => _approvedItems;

        public ICommand LoadModelsCommand => CommandManager.GetCommand(LoadResources);
        public ICommand NextModelCommand => CommandManager.GetCommand(NextModel); 
        public ICommand PastModelCommand => CommandManager.GetCommand(PastModel); 

        public ModelsTreeViewModel() {
            _approvedItems = [];
            Items = [];
            ResourceLoader = GohResourceLoader.Instance;
            Models3DView = ViewModelManager.GetViewModel<Models3DViewModel>()!;
        }

        public void LoadResources() {
            Items.Add(new ModelsTreeDirectoryViewModel(ResourceLoader.GetResourceDirectory("ger_humanskin_source"), this));
            OnPropertyChanged(nameof(Items));
        }

        public void ApproveItem(ModelsTreeItemViewModel item) {
            if (item.IsApproved || _approvedItems.Contains(item)) {
                return;
            }

            switch (item) {
                case ModelsTreePlyViewModel plyItem:
                    ApprovePLyItem(plyItem);
                    break;
                case ModelsTreeTextureViewModel textureItem:
                    ApproveTextureItem(textureItem);
                    break;
                default:
                    break;
            }

            _approvedItems.Add(item);
        }

        public void CancelApproveItem(ModelsTreeItemViewModel item) {
            switch (item) {
                case ModelsTreePlyViewModel plyItem:
                    CancelApprovePlyItem(plyItem);
                    break;
            }

            _approvedItems.Remove(item);
            item.CancelApprove();
        }

        public void CancelApproveItems(IEnumerable<ModelsTreeItemViewModel>? items = null) {
            items ??= _approvedItems;

            foreach (var item in items) {
                if (item.IsApproved) {
                    CancelApproveItem(item);
                }
            }
        }

        public void CancelApproveItems<T>(IEnumerable<ModelsTreeItemViewModel>? items = null) where T : ModelsTreeItemViewModel {
            items ??= _approvedItems;

            foreach (var item in items) {
                if (item is T && item.IsApproved) {
                    CancelApproveItem(item);
                }
            }
        }

        public void NextModel() {
            var selectModel = _approvedItems.First();

            var currentList = selectModel.Parent!;

            int index = currentList.Items.IndexOf(selectModel);

            var nextModel = currentList.Items.ElementAtOrDefault(index + 1);

            nextModel?.Approve();
        }

        public void PastModel() {
            var selectModel = _approvedItems.First();

            var currentList = selectModel.Parent!;

            int index = currentList.Items.IndexOf(selectModel);

            var nextModel = currentList.Items.ElementAtOrDefault(index - 1);

            nextModel?.Approve();
        }

        private void ApprovePLyItem(ModelsTreePlyViewModel item) {
            Models3DView.Adder.SetModel(item.PlyFile);

            item.LoadData();

            CancelApproveItems();

            foreach (var meshItem in item.Items) {
                var textureItem = meshItem.Items.FirstOrDefault();

                textureItem?.Approve();
            }
        }

        private void CancelApprovePlyItem(ModelsTreePlyViewModel item) {
            item.Items.Clear();
        }

        private void ApproveTextureItem(ModelsTreeTextureViewModel item) {
            if (item.Parent is ModelsTreeMashViewModel meshItem) {
                Models3DView.Adder.SelectModelMeshTextureByIndex(meshItem.Mesh, meshItem.Items.IndexOf(item));
                CancelApproveItems(meshItem.Items);
            }   
        }
    }
}
