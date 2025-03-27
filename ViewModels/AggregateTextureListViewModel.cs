using System.Collections.Specialized;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media.TextFormatting;
using GohMdlExpert.Models.GatesOfHell.Exceptions;
using GohMdlExpert.Models.GatesOfHell.Resources.Data;
using GohMdlExpert.Models.GatesOfHell.Resources.Files.Aggregates;
using GohMdlExpert.Services;
using WpfMvvm.Collections.ObjectModel;
using WpfMvvm.Data;
using WpfMvvm.ViewModels;

namespace GohMdlExpert.ViewModels
{
    public class AggregateTextureListViewModel : BaseViewModel {
        private readonly TextureLoadService _materialSelector;
        private readonly ObservableTargetList<MtlTexture> _items;
        private AggregateMtlFile? _mtlFile;
        private int _selectedTextureIndex;

        public AggregateMtlFile? MtlFile {
            get => _mtlFile;
            set {

                _mtlFile = value;
                _items.Target = value?.Data;

                OnPropertyChanged();
            }
        }

        public IObservableEnumerable<MtlTexture> Items => _items;

        public int SelectedTextureIndex {
            get => _selectedTextureIndex;
            set {
                _selectedTextureIndex = value;
                OnPropertyChanged();
                OnSelectedTextureUpdate();
            }
        }

        public ICommand AddCommand => CommandManager.GetCommand(AddTexture);
        public ICommand RemoveCommand => CommandManager.GetCommand(RemoveSelectedTexture);
        public ICommand EditCommand => CommandManager.GetCommand(EditSelectedTexture);

        public event EventHandler? SelectedTextureUpdate;

        public AggregateTextureListViewModel(TextureLoadService materialSelector) {
            _materialSelector = materialSelector;
            _items = [];
        }

        public void AddTexture() {
            var texture = _materialSelector.GetMaterialDialog();

            if (texture != null) {
                if (_items.Any(t => t.Equals(texture))) {
                    throw TextureException.TextureAlreadyContained(texture);
                }

                _items.Add(texture);
                SelectedTextureIndex = _items.Count - 1;
            }
        }

        public void EditSelectedTexture() {
            var texture = _items.ElementAtOrDefault(SelectedTextureIndex);

            if (texture != null) {
                try {
                    _materialSelector.SelectedTextureChange -= TextureSelectorChangeHandler;
                    _materialSelector.SelectedTextureChange += TextureSelectorChangeHandler;
                    _materialSelector.SelectedTexture = texture.Clone();

                    texture = _materialSelector.GetMaterialDialog();

                    if (texture != null) {
                        SetTexture(texture, SelectedTextureIndex);
                    }
                } catch {
_materialSelector.SelectedTextureChange -= TextureSelectorChangeHandler;
                }                
            }
        }

        public void RemoveSelectedTexture() {
            var index = SelectedTextureIndex;

            if (index != -1) {
                _items.RemoveAt(SelectedTextureIndex);
                SelectedTextureIndex = index - 1;
            }
        }

        private void SetTexture(MtlTexture texture, int index) {
            _items[index] = texture;
            SelectedTextureIndex = index;
        }

        private void TextureSelectorChangeHandler(object? sender, EventArgs e) {
            if (_materialSelector.SelectedTexture != null) {
                SetTexture(_materialSelector.SelectedTexture.Clone(), SelectedTextureIndex);
            }
        }

        private void OnSelectedTextureUpdate() {
            SelectedTextureUpdate?.Invoke(this, EventArgs.Empty);
        }
    }
}