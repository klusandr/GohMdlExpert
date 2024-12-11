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
using System.ComponentModel;

namespace GohMdlExpert.ViewModels.ModelsTree.OverviewModels
{
    public class ModelsOverviewTreeMtlViewModel : ModelsOverviewTreeItemViewModel {
        private static readonly ImageSource s_iconSource = new BitmapImage().FromByteArray(Resources.TextureIcon);
        private static readonly ImageSource s_plyIconSource = new BitmapImage().FromByteArray(Resources.PlyIcon);
        private readonly Models3DViewModel _models3DView;
        private readonly TextureMaterialListViewModel _materialList;

        public PlyAggregateMtlFile MtlFile { get; }

        public ModelsOverviewTreeMtlViewModel(PlyAggregateMtlFile aggregateMtlFile, ModelsOverviewTreeViewModel modelsTree) : base(modelsTree) {
            _models3DView = Tree.Models3DViewModel;
            _materialList = Tree.MaterialList;

            IconSource = s_iconSource;
            MtlFile = aggregateMtlFile;
            IsEnableActive = false;
            IsVisibleActive = false;

            HeaderText = GetFullHeaderText();

            PropertyChangeHandler.AddHandler(nameof(IsSelected), SelectedChangeHandler);
            _models3DView.PlyModels.CollectionChanged += PlyModelsChanged;
            _models3DView.UpdatedTextures += Models3DViewModelUpdatedTextures;
        }

        public void LoadData() {
            if (Items.Count == 0) {
                foreach (var plyModel in Tree.Models3DViewModel.GetMtlFilePlyModels(MtlFile.Name)) {
                    AddNextNode(new ModelsOverviewTreeItemViewModel(Tree) {
                        HeaderText = plyModel.PlyFile.Name,
                        IconSource = s_plyIconSource,
                        IsEnableActive = false,
                        IsVisibleActive = false
                    });
                }
            }
        }

        public void ClearData() {
            Items.Clear();
        }

        public void UpdateData() {
            ClearData();
            LoadData();
        }

        private string GetFullHeaderText() {
            return $"{MtlFile.Name} [{Tree.Models3DViewModel.GetCurrentMtlFileTexture(MtlFile.Name).Diffuse.Name}]";
        }

        private void Models3DViewModelUpdatedTextures(object? sender, EventArgs e) {
            HeaderText = GetFullHeaderText();
        }

        private void PlyModelsChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
            UpdateData();
        }

        private void SelectedChangeHandler(object? sender, PropertyChangedEventArgs e) {
            if (IsSelected) {
                _materialList.MtlFile = MtlFile;
                _materialList.SelectedMaterialIndex = _models3DView.GetMtlFileMaterialIndex(MtlFile.Name);
                _materialList.PropertyChangeHandler.AddHandler(nameof(_materialList.SelectedMaterialIndex), SelectedMaterialIndexChangeHandeler);
            } else {
                _materialList.MtlFile = null;
                _materialList.PropertyChangeHandler.RemoveHandler(nameof(_materialList.SelectedMaterialIndex), SelectedMaterialIndexChangeHandeler);
            }
        }

        private void SelectedMaterialIndexChangeHandeler(object? sender, PropertyChangedEventArgs e) {
            if (_materialList.SelectedMaterialIndex != -1 && _materialList.SelectedMaterialIndex != _models3DView.GetMtlFileMaterialIndex(MtlFile.Name)) {
                _models3DView.SetMtlFileTextureByIndex(MtlFile.Name, _materialList.SelectedMaterialIndex);
            }
        }
    }
} 
