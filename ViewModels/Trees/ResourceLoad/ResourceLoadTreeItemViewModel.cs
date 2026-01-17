using System.Windows.Input;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using WpfMvvm.ViewModels.Controls;
using WpfMvvm.ViewModels.Controls.Menu;

namespace GohMdlExpert.ViewModels.Trees.ResourceLoad {
    public abstract class ResourceLoadTreeItemViewModel : TreeItemViewModel {

        public new ResourceLoadTreeViewModel Tree => (ResourceLoadTreeViewModel)base.Tree;

        public ICommand ApproveCommand => CommandManager.GetCommand(Approve);
        public ICommand OpenInExplorerCommand => CommandManager.GetCommand(OpenInExplorer);

        public GohResourceElement ResourceElement { get; protected init; }

        public ResourceLoadTreeItemViewModel(GohResourceElement resourceElement, TreeViewModel modelsTree) : base(modelsTree) {
            Text = resourceElement.Name;
            ResourceElement = resourceElement;

            ContextMenuViewModel.AddItem(new MenuItemViewModel("Open in explorer", OpenInExplorerCommand));
        }

        protected virtual void Approve() { }

        private void OpenInExplorer() {
            ViewModelUtils.OpenInExplorer(ResourceElement);
        }
    }
}
