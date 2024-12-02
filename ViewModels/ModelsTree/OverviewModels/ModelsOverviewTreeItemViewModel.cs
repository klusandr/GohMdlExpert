using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using WpfMvvm.ViewModels.Commands;

namespace GohMdlExpert.ViewModels.ModelsTree.OverviewModels {
    public class ModelsOverviewTreeItemViewModel : ModelsTreeItemViewModel {

        public new ModelsOverviewTreeViewModel Tree => (ModelsOverviewTreeViewModel)base.Tree;

        public Action<ModelsOverviewTreeItemViewModel> Action { get; init; }

        public string MtlFileName { get; set; }

        public override ICommand? DoubleClickCommand => CommandManager.GetCommand(() => Action(this));

        public ModelsOverviewTreeItemViewModel(ModelsOverviewTreeViewModel modelsTree) : base(modelsTree) {
            Action = (e) => { };
        }
    }
}
