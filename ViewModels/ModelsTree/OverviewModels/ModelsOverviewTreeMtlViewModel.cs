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
    public class ModelsOverviewTreeMtlViewModel : ModelsOverviewTreeItemViewModel {
        private static readonly ImageSource s_iconSource = new BitmapImage().FromByteArray(Resources.TextureIcon);
        private static readonly ImageSource s_plyIconSource = new BitmapImage().FromByteArray(Resources.PlyIcon);

        public PlyAggregateMtlFile MtlFile { get; }

        public ModelsOverviewTreeMtlViewModel(PlyAggregateMtlFile aggregateMtlFile, ModelsOverviewTreeViewModel modelsTree) : base(modelsTree) {
            HeaderText = aggregateMtlFile.Name;
            IconSource = s_iconSource;
            MtlFile = aggregateMtlFile;
            IsEnableActive = false;
            IsVisibleActive = false;

            foreach (var plyModel in Tree.Models3DViewModel.GetMtlFilePlyModels(aggregateMtlFile.Name)) {
                AddNextNode(new ModelsOverviewTreeItemViewModel(Tree) {
                    HeaderText = plyModel.PlyFile.Name,
                    IconSource = s_plyIconSource,
                });
            }
        }
    }
} 
