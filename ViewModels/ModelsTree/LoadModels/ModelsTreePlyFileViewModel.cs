using GohMdlExpert.Extensions;
using GohMdlExpert.Models.GatesOfHell.Exceptions;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Properties;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GohMdlExpert.ViewModels.ModelsTree.LoadModels {
    public class ModelsTreePlyFileViewModel : ModelsLoadTreeItemViewModel {
        private static readonly ImageSource s_iconSource = new BitmapImage().FromByteArray(Resources.PlyIcon);

        public PlyFile PlyFile { get; }
        public PlyAggregateMtlFiles? AggregateMtlFiles { get; private set; }

        public override ICommand DoubleClickCommand => CommandManager.GetCommand(Approve);
        public override ICommand LoadCommand => Tree.ModelsAdder.AddModelCommand;
        public override ICommand DeleteCommand => Tree.ModelsAdder.ClearModelCommand;
        public ICommand AddCommand => CommandManager.GetCommand(AddPlyModel);

        private void AddPlyModel() {
            if (Tree.HumanskinResource != null) {
                try {
                    Tree.ModelsAdder.AddModel(PlyFile, new PlyAggregateMtlFiles(PlyFile, Tree.HumanskinResource, Tree.TextureProvider));
                } catch (OperationCanceledException) { }
            }
        }

        public ModelsTreePlyFileViewModel(PlyFile plyFile, ModelsLoadTreeViewModel modelsTree) : base(plyFile, modelsTree) {
            PlyFile = plyFile;
            IconSource = s_iconSource;
            ContextMenuCommands.Add("Add", AddCommand);
            ContextMenuCommands.Add("Delete", DeleteCommand);
        }

        public override void LoadData() {
            if (Items.Count != 0 || Tree.HumanskinResource == null) {
                return;
            }

            AggregateMtlFiles = new PlyAggregateMtlFiles(PlyFile, Tree.HumanskinResource, Tree.TextureProvider);

            foreach (var mtlFile in AggregateMtlFiles) {
                AddNextNode(new ModelsTreeMeshViewModel(mtlFile, Tree));
            }
        }

        public override void Approve() {
            if (!IsApproved) {
                try {
                    LoadData();
                    Tree.ModelsAdder.SetModel(PlyFile, AggregateMtlFiles);
                    SelectTextures();
                    IsButtonActive = true;
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
                (meshItem.Items.FirstOrDefault() as ModelsTreeTextureViewModel)?.Approve();
            }
        }
    }
}
