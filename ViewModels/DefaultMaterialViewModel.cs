using System.Windows.Input;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Services;
using WpfMvvm.Collections.ObjectModel;
using WpfMvvm.ViewModels;

namespace GohMdlExpert.ViewModels {
    public class DefaultMaterialViewModel : BaseViewModel {
        public class DefaultMaterialItemViewModel : BaseViewModel {
            private string _textureName;
            private MaterialFile _materialFile;

            public string TextureName {
                get => _textureName;
                set {
                    _textureName = value;
                    OnPropertyChanged();
                }
            }

            public MaterialFile MaterialFile {
                get => _materialFile;
                set {
                    _materialFile = value;
                    OnPropertyChanged();
                }
            }

            public DefaultMaterialItemViewModel(string textureName, MaterialFile materialFile) {
                _textureName = textureName;
                _materialFile = materialFile;
            }
        }

        private readonly MaterialSelector _materialSelector;
        private readonly ObservableList<DefaultMaterialItemViewModel> _materialFiles;
        private DefaultMaterialItemViewModel? _selectedMaterial;
        private bool _isUse = true;
        private bool _isUseAlways;
        private string? _textureName;

        public IObservableEnumerable<DefaultMaterialItemViewModel> MaterialFiles => _materialFiles;

        public DefaultMaterialItemViewModel? SelectedMaterial {
            get => _selectedMaterial;
            set {
                _selectedMaterial = value;
                TextureName = value?.TextureName;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsShowEditButtons));
                OnPropertyChanged(nameof(IsShowAddButtons));
            }
        }

        public bool IsShowEditButtons => SelectedMaterial != null;
        public bool IsShowAddButtons => SelectedMaterial == null;

        public string? TextureName {
            get => _textureName;
            set {
                _textureName = value;
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

        public ICommand AddMaterialCommand => CommandManager.GetCommand(AddMaterial);
        public ICommand EditTextureNameSelectedMaterialCommand => CommandManager.GetCommand(EditTextureNameSelectedMaterial);
        public ICommand EditMaterialFileSelectedMaterialCommand => CommandManager.GetCommand(EditMaterialFileTextSelectedMaterial);
        public ICommand RemoveSelectedMaterialCommand => CommandManager.GetCommand(RemoveSelectedMaterial);
        public ICommand ClearSelectCommand => CommandManager.GetCommand(ClearSelect);

        public event EventHandler? MaterialsUpdate;

        public DefaultMaterialViewModel(MaterialSelector materialSelector) {
            _materialSelector = materialSelector;
            _materialFiles = [];

            materialSelector.SelectedMaterialFileChange += MaterialSelectorMaterialFileChangeHandler;
        }

        public MaterialFile? GetMaterialFileForTexture(string textureName) {
            return _materialFiles.FirstOrDefault(m => m.TextureName == textureName)?.MaterialFile;
        }

        private void AddMaterial() {
            if (TextureName != null) {
                var materialFile = _materialSelector.GetMaterialDialog();

                if (materialFile != null && CheckTextureNameUniqueness(TextureName)) {
                    _materialFiles.Add(new DefaultMaterialItemViewModel(TextureName, materialFile));
                    OnMaterialUpdate();
                    TextureName = null;
                }
            }
        }

        private void EditTextureNameSelectedMaterial() {
            if (SelectedMaterial != null && TextureName != null && CheckTextureNameUniqueness(TextureName)) {
                SelectedMaterial.TextureName = TextureName;
                OnMaterialUpdate();
                ClearSelect();
            }
        }

        private void EditMaterialFileTextSelectedMaterial() {
            if (SelectedMaterial != null) {
                _materialSelector.SelectedMaterialFile = SelectedMaterial.MaterialFile;

                var materialFile = _materialSelector.GetMaterialDialog();

                if (materialFile != null) {
                    SelectedMaterial.MaterialFile = SelectedMaterial.MaterialFile = materialFile;
                    OnMaterialUpdate();
                    ClearSelect();
                }
            }
        }

        private void RemoveSelectedMaterial() {
            if (SelectedMaterial != null) {
                _materialFiles.Remove(SelectedMaterial);
                OnMaterialUpdate();
                ClearSelect();
            }
        }

        private void ClearSelect() {
            SelectedMaterial = null;
        }

        private bool CheckTextureNameUniqueness(string textureName) {
            return !_materialFiles.Any(dm => dm.TextureName == textureName);
        }

        private void OnMaterialUpdate() {
            MaterialsUpdate?.Invoke(this, EventArgs.Empty);
        }

        private void MaterialSelectorMaterialFileChangeHandler(object? sender, EventArgs e) {
            if (SelectedMaterial != null && _materialSelector.SelectedMaterialFile != null) {
                SelectedMaterial.MaterialFile = _materialSelector.SelectedMaterialFile;
                OnMaterialUpdate();
            }
        }
    }
}
