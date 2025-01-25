using GohMdlExpert.Models.GatesOfHell.Resources;
using WpfMvvm.ViewModels.Controls;

namespace GohMdlExpert.ViewModels.Trees.Materials {
    public class MaterialLoadTreeViewModel : TreeViewModel {
        private readonly GohTextureProvider _textureProvider;
        private MaterialLoadTreeMaterialItemViewModel? _selectedMaterialItem;

        public MaterialLoadTreeMaterialItemViewModel? SelectedMaterialItem {
            get => _selectedMaterialItem;
            private set {
                _selectedMaterialItem = value;
                OnPropertyChanged();
            }
        }

        public MaterialLoadTreeViewModel(GohTextureProvider textureProvider) {
            _textureProvider = textureProvider;

            textureProvider.ResourceUpdated += TextureResourceUpdatedHandler;
            PropertyChangeHandler.AddHandler(nameof(SelectedItem), (_, _) => SelectedMaterialItem = SelectedItem as MaterialLoadTreeMaterialItemViewModel);
        }

        public override void LoadData() {
            AddItem(new MaterialLoadTreeDirectoryItemViewModel(_textureProvider.TextureDirectory, this));
        }

        private void TextureResourceUpdatedHandler(object? sender, EventArgs e) {
            UpdateData();
        }
    }
}
