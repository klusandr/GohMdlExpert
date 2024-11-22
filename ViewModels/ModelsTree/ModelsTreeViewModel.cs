using GohMdlExpert.Models.GatesOfHell.Resources;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Views.ModelsTree;
using MvvmWpf.ViewModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            if (item.IsApproved || !_approvedItems.Contains(item)) {
                return;
            }

            bool tryApproved = item switch {
                ModelsTreePlyViewModel plyItem => TryApprovePLyItem(plyItem),
                ModelsTreeTextureViewModel textureItem => TryApproveTextureItem(textureItem),
                _ => true
            };

            if (tryApproved) {
                _approvedItems.Add(item);
                item.IsApproved = true;
            }
        }

        public void CancelApproveItem(ModelsTreeItemViewModel item) {
            _approvedItems.Remove(item);
            item.IsApproved = false;
        }

        public void CancelApproveAllItems(ModelsTreeItemViewModel? item) {
            var items = item?.Items ?? (IEnumerable<ModelsTreeItemViewModel>)_approvedItems;

            foreach (var cItem in items) {
                CancelApproveItem(cItem);
            }
        }

        private bool TryApprovePLyItem(ModelsTreePlyViewModel item) {
            Models3DView.Adder.SetModel(item.PlyFile);

            foreach (var meshItem in item.Items) {
                
            }

            return true;
        }

        private bool TryApproveTextureItem(ModelsTreeTextureViewModel item) {
            return true;
        }
    }
}
