using GohMdlExpert.Models.GatesOfHell.Resources;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Views.ModelsTree;
using WpfMvvm.ViewModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Security.Policy;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;

namespace GohMdlExpert.ViewModels.ModelsTree {
    public abstract class ModelsTreeViewModel : BaseViewModel {
        private readonly HashSet<ModelsTreeItemViewModel> _approvedItems;

        private Dictionary<Type, Action<ModelsTreeItemViewModel>> _approveItemHandlers;
        private Dictionary<Type, Action<ModelsTreeItemViewModel>> _cancelApproveItemHandlers;

        
        public ObservableCollection<ModelsTreeItemViewModel> Items { get; }
        public IEnumerable<ModelsTreeItemViewModel> ApprovedItems => _approvedItems;

        public ModelsTreeViewModel() {
            Items = [];
            _approvedItems = [];
            _approveItemHandlers = [];
            _cancelApproveItemHandlers = [];
        }

        public abstract void LoadResources();

        public virtual void ClearResources() {
            CancelApproveItems();
            Items.Clear();
        }

        public void ApproveItem(ModelsTreeItemViewModel item) {
            if (item.IsApproved || _approvedItems.Contains(item)) {
                return;
            }

            if (_approveItemHandlers.TryGetValue(item.GetType(), out var handler)) {
                handler.Invoke(item);
            }

            _approvedItems.Add(item);
        }

        public void CancelApproveItem(ModelsTreeItemViewModel item) {
            if (_cancelApproveItemHandlers.TryGetValue(item.GetType(), out var handler)) {
                handler.Invoke(item);
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

        protected void SetApproveItemHandler<T>(Action<ModelsTreeItemViewModel> handler) where T : ModelsTreeItemViewModel {
            _approveItemHandlers[typeof(T)] = handler;
        }

        protected void SetCancelApproveItemHandler<T>(Action<ModelsTreeItemViewModel> handler) where T : ModelsTreeItemViewModel {
            _cancelApproveItemHandlers[typeof(T)] = handler;
        }
    }
}
