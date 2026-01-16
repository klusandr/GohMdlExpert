using System.ComponentModel;
using System.Reflection;
using System.Windows.Input;
using System.Windows.Media;
using GohMdlExpert.Models.GatesOfHell.Resources;
using GohMdlExpert.Models.GatesOfHell.Resources.Data;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.ViewModels.Trees.Materials;
using GohMdlExpert.ViewModels.Trees.Textures;
using WpfMvvm.ViewModels;

namespace GohMdlExpert.ViewModels {
    public class TextureLoadViewModel : BaseViewModel {
        private static readonly PropertyInfo[] s_fieldBindingProperties = [
            typeof(TextureLoadViewModel).GetProperty(nameof(TextureDiffuse))!,
            typeof(TextureLoadViewModel).GetProperty(nameof(TextureBump))!,
            typeof(TextureLoadViewModel).GetProperty(nameof(TextureSpecular))!
        ];

        private readonly SolidColorBrush _textureColorBrush;
        private MtlTexture? _texture;
        private PropertyInfo? _selectFieldBindingProperty;

        public MaterialFile? SelectedMaterialFile { get; private set; }

        public MtlTexture Texture {
            get => _texture ??= MtlTexture.NullTexture.Clone();
            set {
                MaterialTree.ClearSelect();

                _texture = value;

                OnPropertyChanged();
            }
        }

        public MaterialFile? TextureDiffuse {
            get => Texture?.Diffuse;
            set {
                if (value != null) {
                    Texture.Diffuse = value;
                    OnPropertyChanged();
                }
            }
        }

        public MaterialFile? TextureSpecular {
            get => Texture?.Specular;
            set {
                if (value != null) {
                    Texture.Specular = value;
                }
                OnPropertyChanged();
            }
        }

        public MaterialFile? TextureBump {
            get => Texture?.Bump;
            set {
                if (value != null) {
                    Texture.Bump = value;
                }
                OnPropertyChanged();
            }
        }

        public Color? TextureColor {
            get => Texture?.Color;
            set {
                if (value.HasValue) {
                    Texture.Color = value.Value;
                    _textureColorBrush.Color = value.Value;
                }
            }
        }

        public byte TextureColorRedValue {
            get => TextureColor?.R ?? 0;
            set {
                if (TextureColor.HasValue) {
                    var color = TextureColor.Value;
                    color.R = value;
                    TextureColor = color;
                }
                OnPropertyChanged();
            }
        }

        public byte TextureColorGreenValue {
            get => TextureColor?.G ?? 0;
            set {
                if (TextureColor.HasValue) {
                    var color = TextureColor.Value;
                    color.G = value;
                    TextureColor = color;
                }
                OnPropertyChanged();
            }
        }

        public byte TextureColorBlueValue {
            get => TextureColor?.B ?? 0;
            set {
                if (TextureColor.HasValue) {
                    var color = TextureColor.Value;
                    color.B = value;
                    TextureColor = color;
                }
                OnPropertyChanged();
            }
        }

        public byte TextureColorAlphaValue {
            get => TextureColor?.A ?? 0;
            set {
                if (TextureColor.HasValue) {
                    var color = TextureColor.Value;
                    color.A = value;
                    TextureColor = color;
                }
                OnPropertyChanged();
            }
        }

        public SolidColorBrush? TextureColorBrush => _textureColorBrush;

        public MaterialLoadTreeViewModel MaterialTree { get; }
        public TextureLoadTreeViewModel TextureTree { get; }

        public ICommand OkCommand => CommandManager.GetCommand(Ok);
        public ICommand ApplyCommand => CommandManager.GetCommand(Apply);
        public ICommand CancelCommand => CommandManager.GetCommand(Cancel);

        public event EventHandler? CancelEvent;
        public event EventHandler? ApplyEvent;
        public event EventHandler? OkEvent;

        public TextureLoadViewModel(GohResourceProvider resourceProvider, GohTextureProvider textureProvider) {
            MaterialTree = new MaterialLoadTreeViewModel(textureProvider);
            TextureTree = new TextureLoadTreeViewModel(resourceProvider, textureProvider);
            _textureColorBrush = new SolidColorBrush();

            _selectFieldBindingProperty = s_fieldBindingProperties[0];

            TextureTree.TextureApply += TextureTreeApplyTextureHandler;
            MaterialTree.PropertyChangeHandler.AddHandler(nameof(MaterialLoadTreeViewModel.SelectedMaterialItem), SelectedMaterialHandler);
            TextureTree.PropertyChangeHandler.AddHandler(nameof(TextureLoadTreeViewModel.SelectedTextureItem), SelectedTextureHandler);
            PropertyChangeHandler
                .AddHandlerBuilder(nameof(TextureColor), (_, _) => {
                    OnPropertyChanged(nameof(TextureColorRedValue));
                    OnPropertyChanged(nameof(TextureColorGreenValue));
                    OnPropertyChanged(nameof(TextureColorBlueValue));
                    OnPropertyChanged(nameof(TextureColorAlphaValue));
                    UpdateColorBrush();
                })
                .AddHandlerBuilder(nameof(Texture), (_, _) => {
                    OnPropertyChanged(nameof(TextureDiffuse));
                    OnPropertyChanged(nameof(TextureBump));
                    OnPropertyChanged(nameof(TextureSpecular));
                    OnPropertyChanged(nameof(TextureColor));
                    SelectedMaterialFile = _texture?.Diffuse;
                    OnPropertyChanged(nameof(SelectedMaterialFile));
                });

            MaterialTree.LoadData();
            TextureTree.LoadData();
        }

        public void SetSelectFieldIndex(int index) {
            if (index != -1) {
                _selectFieldBindingProperty = s_fieldBindingProperties[index];
            } else {
                _selectFieldBindingProperty = null;
            }
        }

        public void ClearTexture() {
            _texture = null;
            MaterialTree.ClearSelect();
            OnPropertyChanged(nameof(Texture));
        }

        private void Cancel() {
            ClearTexture();
            CancelEvent?.Invoke(this, EventArgs.Empty);
        }

        private void Apply() {
            ApplyEvent?.Invoke(this, EventArgs.Empty);
        }

        private void Ok() {
            OkEvent?.Invoke(this, EventArgs.Empty);
        }

        private void UpdateColorBrush() {
            if (TextureColor.HasValue) {
                _textureColorBrush.Color = TextureColor.Value;
            }
        }

        private void TextureTreeApplyTextureHandler(object? sender, EventArgs e) {
            if (TextureTree.SelectedTextureItem != null) {
                Texture = TextureTree.SelectedTextureItem.MtlFile.Data;
                Apply();
            }
        }

        private void SelectedTextureHandler(object? sender, PropertyChangedEventArgs e) {
            _texture = TextureTree.SelectedTextureItem?.MtlFile.Data.Clone();
            SelectedMaterialFile = _texture?.Diffuse;
            OnPropertyChanged(nameof(SelectedMaterialFile));
            OnPropertyChanged(nameof(Texture));
        }

        private void SelectedMaterialHandler(object? sender, PropertyChangedEventArgs e) {
            _selectFieldBindingProperty?.SetValue(this, MaterialTree.SelectedMaterialItem?.MaterialFile);
            SelectedMaterialFile = MaterialTree.SelectedMaterialItem?.MaterialFile;
            OnPropertyChanged(nameof(SelectedMaterialFile));
        }
    }
}
