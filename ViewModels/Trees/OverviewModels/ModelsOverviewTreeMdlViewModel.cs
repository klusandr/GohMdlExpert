using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using GohMdlExpert.Models.GatesOfHell.Media3D;
using GohMdlExpert.Properties;
using GohMdlExpert.Extensions;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using System.Windows.Input;
using WpfMvvm.ViewModels.Controls.Menu;

namespace GohMdlExpert.ViewModels.Trees.OverviewModels {
    public class ModelsOverviewTreeMdlViewModel : ModelsOverviewTreeItemViewModel {
        private static readonly ImageSource s_icon = new BitmapImage().FromByteArray(Resources.MdlIcon);
        
        public MdlFile MdlFile { get; }

        public ICommand EditNameCommand => CommandManager.GetCommand(() => IsEdit = true);

        public ModelsOverviewTreeMdlViewModel(MdlFile mdlFile, ModelsOverviewTreeViewModel modelsTree) : base(modelsTree) {
            Text = mdlFile.Name;
            Icon = s_icon;
            MdlFile = mdlFile;
            ContextMenuViewModel.AddItem(new MenuItemViewModel("Edit", EditNameCommand));

            PropertyChangeHandler.AddHandler(nameof(Text), (_, _) => MdlFile.Name = Text);
        }

    }
} 
