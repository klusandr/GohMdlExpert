using System.Windows.Input;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using WpfMvvm.ViewModels.Controls;
using WpfMvvm.ViewModels.Controls.Menu;

namespace GohMdlExpert.ViewModels.Trees.Humanskins {
    public abstract class HumanskinTreeItemViewModel : TreeItemViewModel {
        public new HumanskinTreeViewModel Tree => (HumanskinTreeViewModel)base.Tree;

        public ICommand ApproveCommand => CommandManager.GetCommand(Approve);
        public ICommand OpenInExplorerCommand => CommandManager.GetCommand(OpenInExplorer);

        public GohResourceElement ResourceElement { get; }

        public HumanskinTreeItemViewModel(GohResourceElement resourceElement, TreeViewModel modelsTree) : base(modelsTree) {
            Text = resourceElement.Name;
            ToolTip = resourceElement.GetFullPath();
            ContextMenuViewModel.AddItem(new MenuItemViewModel("Open in explorer", OpenInExplorerCommand));
            ResourceElement = resourceElement;
        }

        protected virtual void Approve() { }

        private void OpenInExplorer() {
            ViewModelUtils.OpenInExplorer(ResourceElement);
        }
    }
}
