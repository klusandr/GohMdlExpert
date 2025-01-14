using WpfMvvm.ViewModels.Controls;

namespace GohMdlExpert.ViewModels.ModelsTree.OverviewModels {
    public class ModelsOverviewTreeItemViewModel : TreeItemViewModel {
        private bool _isVisible;
        private bool _isEnableCheck;
        private bool _edit;

        public new ModelsOverviewTreeViewModel Tree => (ModelsOverviewTreeViewModel)base.Tree;

        public bool IsVisible {
            get => _isVisible;
            set {
                _isVisible = value;
                OnPropertyChanged();
            }
        }

        public bool IsEnableCheck {
            get => _isEnableCheck;
            set {
                _isEnableCheck = value;
                OnPropertyChanged();
            }
        }


        public bool IsEnableCheckActive { get; init; }
        public bool IsVisibleActive { get; init; }
        public bool IsEdit {
            get => _edit; 
            set {
                _edit = value;
                OnPropertyChanged();
            }
        }

        public ModelsOverviewTreeItemViewModel(ModelsOverviewTreeViewModel modelsTree) : base(modelsTree) {
            _isEnableCheck = true;
            _isVisible = true;

            IsVisibleActive = false;
            IsEnableCheckActive = false;
        }
    }
}
