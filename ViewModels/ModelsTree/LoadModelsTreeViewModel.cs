using GohMdlExpert.Models.GatesOfHell.Resources;
using GohMdlExpert.Models.GatesOfHell.Resources.Humanskins;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Input;
using WpfMvvm.DependencyInjection;

namespace GohMdlExpert.ViewModels.ModelsTree
{
    public sealed class LoadModelsTreeViewModel : ModelsTreeViewModel
    {
        public ModelAdderViewModel ModelsAdder { get; }
        public GohHumanskinResourceProvider SkinResourceProvider { get; }
        public GohFactionHumanskinResource? HumanskinResource => SkinResourceProvider.Current;

        public ICommand LoadModelsCommand => CommandManager.GetCommand(LoadResources);
        public ICommand NextModelCommand => CommandManager.GetCommand(NextModel);
        public ICommand PastModelCommand => CommandManager.GetCommand(PastModel);

        public LoadModelsTreeViewModel(ModelAdderViewModel modelsAdder, GohHumanskinResourceProvider skinResourceProvider)
        {
            ModelsAdder = modelsAdder;
            SkinResourceProvider = skinResourceProvider;
            ModelsAdder.ModelAdded += (_, _) => CancelApproveItems();

            SetApproveItemHandler<ModelsTreePlyViewModel>(ApprovePlyItem);
            SetApproveItemHandler<ModelsTreeTextureViewModel>(ApproveTextureItem);
            SetCancelApproveItemHandler<ModelsTreePlyViewModel>(CancelApprovePlyItem);

            SkinResourceProvider.HumanskinsResourceUpdate += OnHumanskinResourceUpdate;
        }

        public override void LoadResources()
        {
            if (Items.Count != 0)
            {
                ClearResources();
            }

            if (HumanskinResource != null)
            {
                Items.Add(new ModelsTreeDirectoryViewModel(HumanskinResource.Source, this));
            }
        }

        public void NextModel()
        {
            var selectModel = ApprovedItems.First();

            var currentList = selectModel.Parent!;

            int index = currentList.Items.IndexOf(selectModel);

            var nextModel = currentList.Items.ElementAtOrDefault(index + 1);

            nextModel?.Approve();
        }

        public void PastModel()
        {
            var selectModel = ApprovedItems.First();

            var currentList = selectModel.Parent!;

            int index = currentList.Items.IndexOf(selectModel);

            var nextModel = currentList.Items.ElementAtOrDefault(index - 1);

            nextModel?.Approve();
        }

        private void OnHumanskinResourceUpdate(object? sender, EventArgs e)
        {
            LoadResources();
        }

        private void ApprovePlyItem(ModelsTreeItemViewModel item)
        {
            var plyItem = (ModelsTreePlyViewModel)item;

            ModelsAdder.SetModel(plyItem.PlyFile);

            plyItem.LoadData();

            CancelApproveItems();

            foreach (var meshItem in item.Items.Select(i => (ModelsTreeMashViewModel)i))
            {
                var meshTextures = HumanskinResource?.GetPlyMeshMtlTextures(plyItem.PlyFile, meshItem.Mesh);

                if (meshTextures != null)
                {

                    foreach (var texture in meshTextures)
                    {
                        meshItem.AddNextNode(new ModelsTreeTextureViewModel(texture, this, meshItem));
                    }

                    meshItem.Items.FirstOrDefault()?.Approve();
                }
            }
        }

        private void CancelApprovePlyItem(ModelsTreeItemViewModel item)
        {
            item.Items.Clear();
        }

        private void ApproveTextureItem(ModelsTreeItemViewModel item)
        {
            var meshItem = (ModelsTreeMashViewModel)item.Parent!;

            ModelsAdder.SelectModelMeshTextureByIndex(meshItem.Mesh, meshItem.Items.IndexOf(item));
            CancelApproveItems(meshItem.Items);
        }
    }
}
