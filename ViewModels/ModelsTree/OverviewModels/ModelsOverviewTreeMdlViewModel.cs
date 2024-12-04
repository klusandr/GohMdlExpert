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

namespace GohMdlExpert.ViewModels.ModelsTree.OverviewModels {
    public class ModelsOverviewTreeMdlViewModel : ModelsOverviewTreeItemViewModel {
        private static readonly ImageSource s_iconSource = new BitmapImage().FromByteArray(Resources.MdlIcon);

        public ModelsOverviewTreeMdlViewModel(MdlFile mdlFile, ModelsOverviewTreeViewModel modelsTree) : base(modelsTree) {
            HeaderText = mdlFile.Name;
            IconSource = s_iconSource;
            IsEnableActive = false;
            IsVisibleActive = false;
        }
    }
} 
