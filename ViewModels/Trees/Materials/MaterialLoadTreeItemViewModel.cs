using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using WpfMvvm.ViewModels.Controls;

namespace GohMdlExpert.ViewModels.Trees.Materials
{
    public class MaterialLoadTreeItemViewModel : TreeItemViewModel
    {

        public new MaterialLoadTreeViewModel Tree => (MaterialLoadTreeViewModel)base.Tree;
        public GohResourceElement ResourceElement { get; }

        public MaterialLoadTreeItemViewModel(GohResourceElement resourceElement, TreeViewModel modelsTree) : base(modelsTree)
        {
            Text = resourceElement.Name;
            ResourceElement = resourceElement;
        }
    }
}
