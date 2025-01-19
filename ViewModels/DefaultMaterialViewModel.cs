using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Services;
using WpfMvvm.ViewModels;

namespace GohMdlExpert.ViewModels {
    public class DefaultMaterialViewModel : BaseViewModel {
        private MaterialFile? _materialFile;
        private bool _isUse = true;
        private bool _isUseAlways;
        private readonly MaterialSelector _materialSelector;

        public MaterialFile? MaterialFile {
            get => _materialFile;
            set {
                _materialFile = value;
                OnPropertyChanged();
            }
        }

        public bool IsUse {
            get => _isUse;
            set {
                _isUse = value;
                OnPropertyChanged();
            }
        }

        public bool IsUseAlways {
            get => _isUseAlways;
            set {
                _isUseAlways = value;
                OnPropertyChanged();
            }
        }

        public ICommand SelectMaterialCommand => CommandManager.GetCommand(SelectMaterial);

        public DefaultMaterialViewModel(MaterialSelector materialSelector) {
            _materialSelector = materialSelector;

            materialSelector.SelectedMaterialFileChange += (_, _) => MaterialFile = materialSelector.SelectedMaterialFile;
        }

        private void SelectMaterial() {
            MaterialFile = _materialSelector.GetMaterialDialog();
        }
    }
}
