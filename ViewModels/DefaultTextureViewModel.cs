using System.Windows.Input;
using GohMdlExpert.Models.GatesOfHell.Resources.Data;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Services;
using WpfMvvm.Collections.ObjectModel;
using WpfMvvm.ViewModels;

namespace GohMdlExpert.ViewModels {
    public class DefaultTextureViewModel : BaseViewModel {
        public class DefaultTextureItemViewModel : BaseViewModel {
            private string _textureName;
            private MtlTexture _texture;

            public string TextureName {
                get => _textureName;
                set {
                    _textureName = value;
                    OnPropertyChanged();
                }
            }

            public MtlTexture Texture {
                get => _texture;
                set {
                    _texture = value;
                    OnPropertyChanged();
                }
            }

            public DefaultTextureItemViewModel(string textureName, MtlTexture texture) {
                _textureName = textureName;
                _texture = texture;
            }
        }

        private readonly TextureSelectorService _textureSelector;
        private readonly ObservableList<DefaultTextureItemViewModel> _textures;
        private DefaultTextureItemViewModel? _selectedMaterial;
        private bool _isUse = true;
        private bool _isUseAlways;
        private string? _textureName;

        public IObservableEnumerable<DefaultTextureItemViewModel> Textures => _textures;

        public DefaultTextureItemViewModel? SelectedTexture {
            get => _selectedMaterial;
            set {
                _selectedMaterial = value;
                TextureName = value?.TextureName;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsShowEditButtons));
                OnPropertyChanged(nameof(IsShowAddButtons));
            }
        }

        public bool IsShowEditButtons => SelectedTexture != null;
        public bool IsShowAddButtons => SelectedTexture == null;

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

        public ICommand AddTextureCommand => CommandManager.GetCommand(AddMaterial);
        public ICommand EditTextureNameSelectedTextureCommand => CommandManager.GetCommand(EditTextureNameSelectedTexture);
        public ICommand EditSelectedTextureCommand => CommandManager.GetCommand(EditSelectedTexture);
        public ICommand RemoveSelectedTextureCommand => CommandManager.GetCommand(RemoveSelectedTexture);
        public ICommand ClearSelectCommand => CommandManager.GetCommand(ClearSelect);

        public event EventHandler? TexturesUpdate;

        public DefaultTextureViewModel(TextureSelectorService materialSelector) {
            _textureSelector = materialSelector;
            _textures = [];

            materialSelector.SelectedTextureChange += MaterialSelectorMaterialFileChangeHandler;
        }

        public MtlTexture? GetTextureFile(string textureName) {
            return _textures.FirstOrDefault(m => m.TextureName == textureName)?.Texture;
        }

        private void AddMaterial() {
            if (TextureName != null) {
                var materialFile = _textureSelector.GetMaterialDialog();

                if (materialFile != null && CheckTextureNameUniqueness(TextureName)) {
                    _textures.Add(new DefaultTextureItemViewModel(TextureName, materialFile));
                    TextureName = null;
                    OnTexturesUpdate();
                }
            }
        }

        private void EditTextureNameSelectedTexture() {
            if (SelectedTexture != null && TextureName != null && CheckTextureNameUniqueness(TextureName)) {
                SelectedTexture.TextureName = TextureName;
                ClearSelect();
                OnTexturesUpdate();
            }
        }

        private void EditSelectedTexture() {
            if (SelectedTexture != null) {
                _textureSelector.SelectedTexture = SelectedTexture.Texture;

                var materialFile = _textureSelector.GetMaterialDialog();

                if (materialFile != null) {
                    SelectedTexture.Texture = SelectedTexture.Texture = materialFile;
                    ClearSelect();
                    OnTexturesUpdate();
                }
            }
        }

        private void RemoveSelectedTexture() {
            if (SelectedTexture != null) {
                _textures.Remove(SelectedTexture);
                ClearSelect();
                OnTexturesUpdate();
            }
        }

        private void ClearSelect() {
            SelectedTexture = null;
        }

        private bool CheckTextureNameUniqueness(string textureName) {
            return !_textures.Any(dm => dm.TextureName == textureName);
        }

        private void OnTexturesUpdate() {
            TexturesUpdate?.Invoke(this, EventArgs.Empty);
        }

        private void MaterialSelectorMaterialFileChangeHandler(object? sender, EventArgs e) {
            if (SelectedTexture != null && _textureSelector.SelectedTexture != null) {
                SelectedTexture.Texture = _textureSelector.SelectedTexture;
                OnTexturesUpdate();
            }
        }
    }
}
