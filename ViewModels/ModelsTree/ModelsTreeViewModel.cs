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
using GohMdlExpert.ViewModels.ModelsTree.LoadModels;

namespace GohMdlExpert.ViewModels.ModelsTree {
    public abstract class ModelsTreeViewModel : BaseViewModel {
        private ModelsLoadTreeItemViewModel? _selectedItem;

        public ObservableCollection<ModelsTreeItemViewModel> Items { get; }

        public ModelsLoadTreeItemViewModel? SelectedItem {
            get => _selectedItem;
            set {
                _selectedItem = value;
                OnPropertyChanged();
            }
        }

        public ModelsTreeViewModel() {
            Items = [];
        }

        public abstract void LoadData();

        public virtual void ClearData() {
            Items.Clear();
        }

        public virtual void UpdateData() {
            ClearData();
            LoadData();
        }
    }
}
