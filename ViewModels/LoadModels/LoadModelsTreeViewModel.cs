using GohMdlExpert.Models.GatesOfHell.Resources;
using GohMdlExpert.ViewModels.ModelsTree;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Input;
using WpfMvvm.DependencyInjection;

namespace GohMdlExpert.ViewModels.LoadModels {
    public sealed class LoadModelsTreeViewModel : ModelsTreeViewModel {
        public GohResourceLoader ResourceLoader { get; }
        public ModelAdderViewModel ModelsAdder { get; }

        public ICommand LoadModelsCommand => CommandManager.GetCommand(LoadResources);
        public ICommand NextModelCommand => CommandManager.GetCommand(NextModel);
        public ICommand PastModelCommand => CommandManager.GetCommand(PastModel);

        public LoadModelsTreeViewModel(ModelAdderViewModel modelsAdder) {
            ResourceLoader = AppDependencyInjection.Instant.ServiceProvider.GetRequiredService<GohResourceLoader>();
            ModelsAdder = modelsAdder;

            ModelsAdder.ModelAdded += (_, _) => CancelApproveItems();

            SetApproveItemHandler<ModelsTreePlyViewModel>(ApprovePLyItem);
            SetApproveItemHandler<ModelsTreeTextureViewModel>(ApproveTextureItem);
            SetCancelApproveItemHandler<ModelsTreePlyViewModel>(CancelApprovePlyItem);
        }

        public override void LoadResources() {
            Items.Add(new ModelsTreeDirectoryViewModel(ResourceLoader.GetResourceDirectory("ger_humanskin_source"), this));
        }

        public void NextModel() {
            var selectModel = ApprovedItems.First();

            var currentList = selectModel.Parent!;

            int index = currentList.Items.IndexOf(selectModel);

            var nextModel = currentList.Items.ElementAtOrDefault(index + 1);

            nextModel?.Approve();
        }

        public void PastModel() {
            var selectModel = ApprovedItems.First();

            var currentList = selectModel.Parent!;

            int index = currentList.Items.IndexOf(selectModel);

            var nextModel = currentList.Items.ElementAtOrDefault(index - 1);

            nextModel?.Approve();
        }

        private void ApprovePLyItem(ModelsTreeItemViewModel item) {
            var plyItem = (ModelsTreePlyViewModel)item;

            ModelsAdder.SetModel(plyItem.PlyFile);

            plyItem.LoadData();

            CancelApproveItems();

            foreach (var meshItem in item.Items) {
                var textureItem = meshItem.Items.FirstOrDefault();

                textureItem?.Approve();
            }
        }

        private void CancelApprovePlyItem(ModelsTreeItemViewModel item) {
            item.Items.Clear();
        }

        private void ApproveTextureItem(ModelsTreeItemViewModel item) {
            var meshItem = (ModelsTreeMashViewModel)item.Parent!;

            ModelsAdder.SelectModelMeshTextureByIndex(meshItem.Mesh, meshItem.Items.IndexOf(item));
            CancelApproveItems(meshItem.Items);
        }
    }
}
