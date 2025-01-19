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
using System.ComponentModel;
using GohMdlExpert.Models.GatesOfHell.Resources.Files.Aggregates;

namespace GohMdlExpert.ViewModels.Trees.OverviewModels
{
    public class ModelsOverviewTreeMtlViewModel : ModelsOverviewTreeItemViewModel {
        private static readonly ImageSource s_icon = new BitmapImage().FromByteArray(Resources.TextureIcon);
        private static readonly ImageSource s_plyIcon = new BitmapImage().FromByteArray(Resources.PlyIcon);
        private readonly HumanskinMdlOverviewViewModel _models3DView;
        private readonly TextureMaterialListViewModel _materialList;

        public AggregateMtlFile MtlFile { get; }

        public ModelsOverviewTreeMtlViewModel(AggregateMtlFile aggregateMtlFile, ModelsOverviewTreeViewModel modelsTree) : base(modelsTree) {
            _models3DView = Tree.Models3DViewModel;
            _materialList = Tree.MaterialListViewModel;

            Icon = s_icon;
            MtlFile = aggregateMtlFile;

            Text = GetFullText();

            PropertyChangeHandler.AddHandler(nameof(IsSelected), SelectedChangeHandler);
            _models3DView.PlyModels.CollectionChanged += PlyModelsChanged;
            _models3DView.UpdatedTextures += Models3DViewModelUpdatedTextures;
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
            return $"{MtlFile.Name} [{Tree.Models3DViewModel.GetCurrentMtlFileTexture(MtlFile.Name).Diffuse.Name}]";
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
                _materialList.MtlFile = MtlFile;
                _materialList.SelectedMaterialIndex = _models3DView.GetMtlFileMaterialIndex(MtlFile.Name);
                _materialList.PropertyChangeHandler.AddHandler(nameof(_materialList.SelectedMaterialIndex), SelectedMaterialIndexChangeHandler);
            } else {
                _materialList.MtlFile = null;
                _materialList.PropertyChangeHandler.RemoveHandler(nameof(_materialList.SelectedMaterialIndex), SelectedMaterialIndexChangeHandler);
            }
        }

        private void SelectedMaterialIndexChangeHandler(object? sender, PropertyChangedEventArgs e) {
            if (_materialList.SelectedMaterialIndex != -1 && _materialList.SelectedMaterialIndex != _models3DView.GetMtlFileMaterialIndex(MtlFile.Name)) {
                _models3DView.SetMtlFileTextureByIndex(MtlFile.Name, _materialList.SelectedMaterialIndex);
            }
        }
    }
} 
