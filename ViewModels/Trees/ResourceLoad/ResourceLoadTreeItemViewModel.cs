using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using WpfMvvm.ViewModels.Controls;
using WpfMvvm.Views.Controls;

namespace GohMdlExpert.ViewModels.Trees.ResourceLoad {
    public abstract class ResourceLoadTreeItemViewModel : TreeItemViewModel {

        public new ResourceLoadTreeViewModel Tree => (ResourceLoadTreeViewModel)base.Tree;

        public ICommand ApproveCommand => CommandManager.GetCommand(Approve);

        public ResourceLoadTreeItemViewModel(TreeViewModel modelsTree) : base(modelsTree) {
            
        }

        protected virtual void Approve() { }

    }
}
