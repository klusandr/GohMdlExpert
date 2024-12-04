using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using WpfMvvm.ViewModels.Commands;

namespace GohMdlExpert.ViewModels.ModelsTree.OverviewModels {
    public class ModelsOverviewTreeItemViewModel : ModelsTreeItemViewModel {
        private bool _isVisible;
        private bool _isEnable;

        public new ModelsOverviewTreeViewModel Tree => (ModelsOverviewTreeViewModel)base.Tree;

        public bool IsEnable {
            get => _isEnable;
            set {
                _isEnable = value;
                OnPropertyChanged();
            }
        }

        public bool IsVisible {
            get => _isVisible;
            set {
                _isVisible = value;
                OnPropertyChanged();
            }
        }

        public bool IsEnableActive { get; init; }
        public bool IsVisibleActive { get; init; }

        public ModelsOverviewTreeItemViewModel(ModelsOverviewTreeViewModel modelsTree) : base(modelsTree) {
            _isEnable = true;
            _isVisible = true;

            IsVisibleActive = true;
            IsEnableActive = true;
        }
    }
}
