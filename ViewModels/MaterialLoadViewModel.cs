using System.ComponentModel;
using System.Windows.Input;
using GohMdlExpert.Models.GatesOfHell.Resources;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.ViewModels.Trees.Materials;
using WpfMvvm.ViewModels;

namespace GohMdlExpert.ViewModels {
    public class MaterialLoadViewModel : BaseViewModel {
        private readonly GohTextureProvider _textureProvider;
        private MaterialFile? _selectedMaterialFile;

        public MaterialFile? SelectedMaterialFile {
            get => _selectedMaterialFile;
            set {
                _selectedMaterialFile = value;
                OnPropertyChanged();
            }
        }

        public MaterialLoadTreeViewModel MaterialTree { get; }

        public ICommand ApproveCommand => CommandManager.GetCommand(Approve);
        public ICommand ApplyCommand => CommandManager.GetCommand(Apply);
        public ICommand CancelCommand => CommandManager.GetCommand(Cancel);

        public event EventHandler? MaterialApprove;
        public event EventHandler? MaterialApply;

        public MaterialLoadViewModel(GohTextureProvider textureProvider) {
            _textureProvider = textureProvider;
            MaterialTree = new MaterialLoadTreeViewModel(textureProvider);

            if (textureProvider.IsResourceLoad) {
                MaterialTree.LoadData();
            }

            MaterialTree.PropertyChangeHandler.AddHandler(nameof(MaterialLoadTreeViewModel.SelectedMaterialItem), SelectedMaterialHandler);
        }

        private void Approve() {
            MaterialApprove?.Invoke(this, EventArgs.Empty);
        }

        private void Apply() {
            MaterialApply?.Invoke(this, EventArgs.Empty);
        }

        private void Cancel() {
            SelectedMaterialFile = null;
            Approve();
        }

        private void SelectedMaterialHandler(object? sender, PropertyChangedEventArgs e) {
            SelectedMaterialFile = MaterialTree.SelectedMaterialItem?.MaterialFile;
        }
    }
}
