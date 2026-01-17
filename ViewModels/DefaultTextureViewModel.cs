using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using GohMdlExpert.Models.GatesOfHell.Resources.Data;
using GohMdlExpert.Services;
using WpfMvvm.Collections.ObjectModel;
using WpfMvvm.ViewModels;

namespace GohMdlExpert.ViewModels {
    public class DefaultTextureViewModel : BaseViewModel, INotifyDataErrorInfo {
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

        private readonly TextureLoadService _textureSelector;
        private readonly ObservableList<DefaultTextureItemViewModel> _textures;
        private readonly Dictionary<string, Exception?> _errors;
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
                ClearError();
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

        public ICommand AddTextureCommand => CommandManager.GetCommand(AddTexture);
        public ICommand EditTextureNameSelectedTextureCommand => CommandManager.GetCommand(EditTextureNameSelectedTexture);
        public ICommand EditSelectedTextureCommand => CommandManager.GetCommand(EditSelectedTexture);
        public ICommand RemoveSelectedTextureCommand => CommandManager.GetCommand(RemoveSelectedTexture);
        public ICommand ClearSelectCommand => CommandManager.GetCommand(ClearSelect);

        public bool HasErrors => _errors.Count != 0;

        public event EventHandler? TexturesUpdate;
        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        public DefaultTextureViewModel(TextureLoadService materialSelector) {
            _textureSelector = materialSelector;
            _textures = [];
            _errors = [];

            materialSelector.SelectedTextureChange += MaterialSelectorMaterialFileChangeHandler;
        }

        public MtlTexture? GetTextureFile(string textureName) {
            return _textures.FirstOrDefault(m => m.TextureName == textureName)?.Texture;
        }

        public void AddTexture() {
            try {
                if (TextureName != null) {
                    string textureName = TextureName;
                    SelectedTexture = null;
                    TextureName = textureName;

                    _textureSelector.SelectedTextureChange -= TextureSelectorTextureChangeHandlerWhenAdd;
                    _textureSelector.SelectedTextureChange += TextureSelectorTextureChangeHandlerWhenAdd;

                    var texture = _textureSelector.ShowDialog();

                    if (texture != null) {
                        if (SelectedTexture == null) {
                            AddDefaulttexture(texture);
                        }
                    } else if (SelectedTexture != null) {
                        RemoveSelectedTexture();
                    }
                } else {
                    SetError(new Exception(), nameof(TextureName));
                }
            } finally {
                _textureSelector.SelectedTextureChange -= TextureSelectorTextureChangeHandlerWhenAdd;
            }
        }

        private void TextureSelectorTextureChangeHandlerWhenAdd(object? sender, EventArgs e) {
            if (SelectedTexture != null) { return; }

            var texture = _textureSelector.SelectedTexture;

            if (texture != null) {
                SelectedTexture = AddDefaulttexture(texture);
            }
        }

        public void EditTextureNameSelectedTexture() {
            if (SelectedTexture != null && TextureName != null && CheckTextureNameUniqueness(TextureName)) {
                SelectedTexture.TextureName = TextureName;
                ClearSelect();
                OnTexturesUpdate();
            }
        }

        public void EditSelectedTexture() {
            if (SelectedTexture != null) {
                _textureSelector.SelectedTexture = SelectedTexture.Texture;

                var materialFile = _textureSelector.ShowDialog();

                if (materialFile != null) {
                    SelectedTexture.Texture = SelectedTexture.Texture = materialFile;
                    ClearSelect();
                    OnTexturesUpdate();
                }
            }
        }

        public void RemoveSelectedTexture() {
            if (SelectedTexture != null) {
                _textures.Remove(SelectedTexture);
                ClearSelect();
                OnTexturesUpdate();
            }
        }

        public IEnumerable GetErrors(string? propertyName) {
            if (propertyName != null) {
                if (_errors.TryGetValue(propertyName, out var error)) {
                    return (IEnumerable<Exception?>)[error];
                } else {
                    return Enumerable.Empty<Exception>();
                }
            } else {
                return _errors.Values;
            }
        }

        public void ClearSelect() {
            SelectedTexture = null;
        }

        private bool CheckTextureNameUniqueness(string textureName) {
            return !_textures.Any(dm => dm.TextureName == textureName);
        }

        private DefaultTextureItemViewModel? AddDefaulttexture(MtlTexture texture) {
            DefaultTextureItemViewModel? defaultTexture = null;

            if (TextureName != null && CheckTextureNameUniqueness(TextureName)) {
                defaultTexture = new DefaultTextureItemViewModel(TextureName, texture);

                _textures.Add(defaultTexture);

                TextureName = null;
                OnTexturesUpdate();
            }

            return defaultTexture;
        }

        private void SetError(Exception? exception, [CallerMemberName] string? propertyName = null) {
            if (propertyName != null) {

                _errors[propertyName] = exception;
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            }
        }

        private void ClearError([CallerMemberName] string? propertyName = null) {
            if (propertyName != null) {
                _errors.Remove(propertyName);
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            } else {
                _errors.Clear();
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(null));
            }
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
