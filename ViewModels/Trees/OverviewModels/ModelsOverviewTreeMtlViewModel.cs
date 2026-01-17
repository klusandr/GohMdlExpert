using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GohMdlExpert.Extensions;
using GohMdlExpert.Models.GatesOfHell.Resources.Files.Aggregates;
using GohMdlExpert.Properties;
using WpfMvvm.ViewModels.Controls.Menu;

namespace GohMdlExpert.ViewModels.Trees.OverviewModels {
    public class ModelsOverviewTreeMtlViewModel : ModelsOverviewTreeItemViewModel {
        private static readonly ImageSource s_icon = new BitmapImage().FromByteArray(Resources.TextureIcon);
        private static readonly ImageSource s_plyIcon = new BitmapImage().FromByteArray(Resources.PlyIcon);
        private readonly HumanskinMdlOverviewViewModel _models3DView;
        private readonly AggregateTextureListViewModel _aggregateTextureList;

        public AggregateMtlFile MtlFile { get; }

        public ICommand AddTextureCommand => CommandManager.GetCommand(Tree.AggregateTextureListViewModel.AddTexture);

        public ModelsOverviewTreeMtlViewModel(AggregateMtlFile aggregateMtlFile, ModelsOverviewTreeViewModel modelsTree) : base(modelsTree) {
            _models3DView = Tree.Models3DViewModel;
            _aggregateTextureList = Tree.AggregateTextureListViewModel;

            Icon = s_icon;
            MtlFile = aggregateMtlFile;

            Text = GetFullText();

            ContextMenuViewModel.AddItemBuilder(new MenuItemViewModel("Add texture", AddTextureCommand));

            PropertyChangeHandler.AddHandler(nameof(IsSelected), SelectedChangeHandler);
            _models3DView.PlyModels.CollectionChanged += PlyModelsChanged;
            WeakEventManager<HumanskinMdlOverviewViewModel, EventArgs>.AddHandler(_models3DView, nameof(HumanskinMdlOverviewViewModel.UpdatedTextures), Models3DViewModelUpdatedTextures);
        }

        public void LoadData() {
            if (!Items.Any()) {
                foreach (var plyModel in Tree.Models3DViewModel.GetMtlFilePlyModels(MtlFile.Name)) {
                    AddItem(new ModelsOverviewTreeItemViewModel(Tree) {
                        Text = plyModel.PlyFile.Name,
                        Icon = s_plyIcon,
                        IsEnableCheckActive = false,
                        IsVisibleActive = false
                    });
                }
            }
        }

        public void ClearData() {
            _items.Clear();
        }

        public void UpdateData() {
            ClearData();
            LoadData();
        }

        private string GetFullText() {
            return $"{MtlFile.Name} [{Tree.Models3DViewModel.GetCurrentMtlFileTexture(MtlFile.Name)?.Diffuse?.Name ?? "null"}]";
        }

        private void Models3DViewModelUpdatedTextures(object? sender, EventArgs e) {
            if (Tree.Models3DViewModel.AggregateMtlFiles.ContainsKey(MtlFile.Name)) {
                Text = GetFullText();
            }
        }

        private void PlyModelsChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
            UpdateData();
        }

        private void SelectedChangeHandler(object? sender, PropertyChangedEventArgs e) {
            if (IsSelected) {
                _aggregateTextureList.MtlFile = MtlFile;
                _aggregateTextureList.SelectedTextureIndex = _models3DView.GetMtlFileMaterialIndex(MtlFile.Name);
                _aggregateTextureList.SelectedTextureUpdate -= TextureListSelectedTextureUpdateHandler;
                _aggregateTextureList.SelectedTextureUpdate += TextureListSelectedTextureUpdateHandler;

            } else {
                _aggregateTextureList.SelectedTextureUpdate -= TextureListSelectedTextureUpdateHandler;
                _aggregateTextureList.MtlFile = null;
            }
        }

        private void TextureListSelectedTextureUpdateHandler(object? sender, EventArgs e) {
            _models3DView.SetMtlFileTextureByIndex(MtlFile.Name, _aggregateTextureList.SelectedTextureIndex);
        }
    }
}
