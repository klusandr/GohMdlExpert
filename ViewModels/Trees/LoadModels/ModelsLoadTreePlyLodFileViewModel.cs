using System.ComponentModel;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Properties;

namespace GohMdlExpert.ViewModels.Trees.LoadModels {
    public class ModelsLoadTreePlyLodFileViewModel : ModelsLoadTreeItemViewModel {
        private readonly int _lodIndex;

        public ModelsLoadTreePlyLodFileViewModel(PlyFile lodPlyFile, int lodIndex, ModelsLoadTreeViewModel modelsTree) : base(lodPlyFile, modelsTree) {
            Icon = IconResources.Instance.GetIcon(nameof(Resources.PlyIcon));

            PropertyChangeHandler.AddHandler(nameof(IsSelected), SelectedHandler);
            _lodIndex = lodIndex;
        }

        private void SelectedHandler(object? sender, PropertyChangedEventArgs e) {
            if (IsSelected) {
                Tree.ModelsAdder.AddedModel?.SetLodIndex(_lodIndex);
            } else {
                Tree.ModelsAdder.AddedModel?.SetLodIndex(0);
            }
        }

        public override void LoadData() { }

        public override void Approve() { }
    }
}
