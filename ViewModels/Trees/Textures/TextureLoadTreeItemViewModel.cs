using System.Windows.Input;
using WpfMvvm.ViewModels.Controls;

namespace GohMdlExpert.ViewModels.Trees.Textures {
    public abstract class TextureLoadTreeItemViewModel : TreeItemViewModel {
        public new TextureLoadTreeViewModel Tree => (TextureLoadTreeViewModel)base.Tree;

        public ICommand? DoubleClickCommand { get; set; }

        public TextureLoadTreeItemViewModel(TreeViewModel modelsTree) : base(modelsTree) { }
    }
}
