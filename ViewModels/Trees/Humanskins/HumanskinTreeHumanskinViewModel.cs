using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Properties;
using WpfMvvm.ViewModels.Controls;

namespace GohMdlExpert.ViewModels.Trees.Humanskins {
    public class HumanskinTreeHumanskinViewModel : HumanskinTreeItemViewModel {
        private readonly MdlFile _mdlFile;

        public HumanskinTreeHumanskinViewModel(MdlFile mdlFile, TreeViewModel modelsTree) : base(mdlFile, modelsTree) {
            Icon = IconResources.Instance.GetIcon(nameof(Resources.MdlIcon));
            _mdlFile = mdlFile;
        }

        protected override void Approve() {
            Tree.HumanskinOverview.SetMtlFile(_mdlFile);
        }
    }
}
