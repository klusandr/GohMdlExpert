using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using WpfMvvm.ViewModels.Controls;

namespace GohMdlExpert.ViewModels.Trees.Materials {
    public class MaterialLoadTreeItemViewModel : TreeItemViewModel {

        public new MaterialLoadTreeViewModel Tree => (MaterialLoadTreeViewModel)base.Tree;
        public GohResourceElement ResourceElement { get; }

        public MaterialLoadTreeItemViewModel(GohResourceElement resourceElement, TreeViewModel modelsTree) : base(modelsTree) {
            Text = resourceElement.Name;
            ResourceElement = resourceElement;
        }
    }
}
