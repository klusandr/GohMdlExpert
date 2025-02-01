using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media;
using GohMdlExpert.Models.GatesOfHell.Resources;
using GohMdlExpert.Models.GatesOfHell.Resources.Data;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.ViewModels.Trees.Materials;
using WpfMvvm.ViewModels;

namespace GohMdlExpert.ViewModels {
    public class TextureLoadViewModel : BaseViewModel {
        private static readonly PropertyInfo[] s_fieldBindingProperties = [
            typeof(TextureLoadViewModel).GetProperty(nameof(TextureDiffuse))!,
            typeof(TextureLoadViewModel).GetProperty(nameof(TextureBump))!,
            typeof(TextureLoadViewModel).GetProperty(nameof(TextureSpecular))!
        ];

        private readonly GohTextureProvider _textureProvider;
        private readonly SolidColorBrush _textureColorBrush;
        private MtlTexture? _texture;
        private PropertyInfo? _selectFieldBindingProperty;
        private MaterialFile? _selectedMaterialFile;

        public MaterialFile? SelectedMaterialFile {
            get => _selectedMaterialFile;
            set {
                _selectedMaterialFile = value;
                OnPropertyChanged();
            }
        }

        public MtlTexture? Texture {
            get => _texture;
            set {
                _texture = value;
                OnPropertyChanged();
            }
        }

        public MaterialFile? TextureDiffuse {
            get => Texture?.Diffuse;
            set {
                if (value != null) {
                    if (Texture == null) {
                        Texture = new MtlTexture(value);
                    } else {
                        Texture.Diffuse = value;
                    }
                    OnPropertyChanged();
                }
            }
        }

        public MaterialFile? TextureSpecular {
            get => Texture?.Specular;
            set {
                if (Texture != null && value != null) {
                    Texture.Specular = value;
                }
                OnPropertyChanged();
            }
        }

        public MaterialFile? TextureBump {
            get => Texture?.Bump;
            set {
                if (Texture != null && value != null) {
                    Texture.Bump = value;
                }
                OnPropertyChanged();
            }
        }

        public Color? TextureColor {
            get => Texture?.Color;
            set {
                if (Texture != null && value.HasValue) {
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

        public ICommand ApproveCommand => CommandManager.GetCommand(Approve);
        public ICommand ApplyCommand => CommandManager.GetCommand(Apply);
        public ICommand CancelCommand => CommandManager.GetCommand(Cancel);

        public event EventHandler? TextureApprove;
        public event EventHandler? TextureApply;

        public TextureLoadViewModel(GohTextureProvider textureProvider) {
            _textureProvider = textureProvider;
            MaterialTree = new MaterialLoadTreeViewModel(textureProvider);
            _textureColorBrush = new SolidColorBrush();

            _selectFieldBindingProperty = s_fieldBindingProperties[0];

            if (textureProvider.IsResourceLoad) {
                MaterialTree.LoadData();
            }
            
            MaterialTree.PropertyChangeHandler.AddHandler(nameof(MaterialLoadTreeViewModel.SelectedMaterialItem), SelectedMaterialHandler);
            PropertyChangeHandler
                .AddHandlerBuilder(nameof(TextureColor), (_, _) => {
                    OnPropertyChanged(nameof(TextureColorRedValue));
                    OnPropertyChanged(nameof(TextureColorGreenValue));
                    OnPropertyChanged(nameof(TextureColorBlueValue));
                    OnPropertyChanged(nameof(TextureColorAlphaValue));
                })
                .AddHandlerBuilder(nameof(Texture), (_, _) => {
                    OnPropertyChanged(nameof(TextureDiffuse));
                    OnPropertyChanged(nameof(TextureBump));
                    OnPropertyChanged(nameof(TextureSpecular));
                    OnPropertyChanged(nameof(TextureColor));
                });

        }

        public void SetSelectFieldIndex(int index) {
            if (index != -1) {
                _selectFieldBindingProperty = s_fieldBindingProperties[index];
            } else {
                _selectFieldBindingProperty = null;
            }
        }

        private void Approve() {
            TextureApprove?.Invoke(this, EventArgs.Empty);
        }

        private void Apply() {
            TextureApply?.Invoke(this, EventArgs.Empty);
        }

        private void Cancel() {
            Texture = null;
            Approve();
        }

        private void SelectedMaterialHandler(object? sender, PropertyChangedEventArgs e) {
            _selectFieldBindingProperty?.SetValue(this, MaterialTree.SelectedMaterialItem?.MaterialFile);

            SelectedMaterialFile = MaterialTree.SelectedMaterialItem?.MaterialFile;
        }
    }
}
