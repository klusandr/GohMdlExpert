using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GohMdlExpert.Extensions;
using GohMdlExpert.Models.GatesOfHell.Exceptions;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Models.GatesOfHell.Resources.Files.Aggregates;
using GohMdlExpert.Properties;
using WpfMvvm.ViewModels.Controls.Menu;

namespace GohMdlExpert.ViewModels.Trees.LoadModels {
    public class ModelsLoadTreePlyFileViewModel : ModelsLoadTreeItemViewModel {
        private static readonly ImageSource s_icon = new BitmapImage().FromByteArray(Resources.PlyIcon);
        private static readonly ImageSource s_icon_m = new BitmapImage().FromByteArray(Resources.PlyIcon);

        public PlyFile PlyFile { get; }
        public AggregateMtlFiles? AggregateMtlFiles { get; private set; }

        public override ICommand LoadCommand => Tree.ModelsAdder.AddModelCommand;
        public override ICommand DeleteCommand => Tree.ModelsAdder.ClearModelCommand;
        public ICommand AddCommand => CommandManager.GetCommand(AddPlyModel);

        private void AddPlyModel() {
            try {
                Tree.ModelsAdder.AddModel(PlyFile, new AggregateMtlFiles(PlyFile, Tree.SkinResourceProvider.Resource, Tree.TextureProvider));
            } catch (OperationCanceledException) { }
        }

        public ModelsLoadTreePlyFileViewModel(PlyFile plyFile, ModelsLoadTreeViewModel modelsTree) : base(plyFile, modelsTree) {
            PlyFile = plyFile;
            Icon = s_icon;
            ContextMenuViewModel
                .InsertItemBuilder(0, new MenuItemViewModel("Add", AddCommand) { Icon = new Image() { Source = s_icon } })
                .InsertItemBuilder(1, new MenuItemViewModel("Cancel", DeleteCommand) { VisibleFromEnable = true });
        }

        public override void LoadData() {
            if (Items.Any() || Tree.HumanskinResource == null) {
                return;
            }

            AggregateMtlFiles = new AggregateMtlFiles(PlyFile, Tree.HumanskinResource, Tree.TextureProvider);

            foreach (var mtlFile in AggregateMtlFiles) {
                AddItem(new ModelsLoadTreeMeshViewModel(mtlFile, Tree));
            }
        }

        public override void Approve() {
            if (!IsApproved) {
                try {
                    LoadData();
                    Tree.ModelsAdder.SetModel(PlyFile, AggregateMtlFiles);
                    SelectTextures();
                    IsButtonActive = true;
                    IsExpanded = true;
                } catch (GohResourceFileException) {
                    throw;
                } finally {
                    base.Approve();
                }
            }
        }

        public override void CancelApprove() {
            if (IsApproved) {
                ClearData();
                IsButtonActive = false;
                base.CancelApprove();
            }
        }

        private void SelectTextures() {
            foreach (var meshItem in Items) {
                (meshItem.Items.FirstOrDefault() as ModelsLoadTreeTextureViewModel)?.Approve();
            }
        }
    }
}
