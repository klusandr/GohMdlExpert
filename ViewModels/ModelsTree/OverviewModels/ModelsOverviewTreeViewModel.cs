using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using GohMdlExpert.Extensions;
using GohMdlExpert.Models.GatesOfHell.Media3D;
using GohMdlExpert.Models.GatesOfHell.Resources;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Properties;

namespace GohMdlExpert.ViewModels.ModelsTree.OverviewModels {
    public class ModelsOverviewTreeViewModel : ModelsTreeViewModel {
        public Models3DViewModel Models3DViewModel { get; }

        public ModelsOverviewTreeItemViewModel MdlFile { get; }

        public ICollection<ModelsTreeItemViewModel> PlyItems => MdlFile.Items;

        public IEnumerable<PlyAggregateMtlFile> MtlFiles => Models3DViewModel.AggregateMtlFiles.Values;
        public ObservableCollection<MtlTexture> MtlTextures { get; } 

        public ModelsOverviewTreeViewModel(Models3DViewModel models3DViewModel) {
            Models3DViewModel = models3DViewModel;
            MtlTextures = [];

            MdlFile = new ModelsOverviewTreeItemViewModel(this) {
                HeaderText = "new_humanskin.mdl",
                IconSource = new BitmapImage().FromByteArray(Resources.MdlIcon)
            };

            Items.Add(MdlFile);

            models3DViewModel.PlyModels.CollectionChanged += ModelsPlyChanged;
            models3DViewModel.AggregateMtlFiles.CollectionChanged += MtlFilesChanged;
        }

        public void UpdateMtlFiles() {
            for (int i = 1; i < MtlFiles.Count(); i++) {
                Items.RemoveAt(i);
            }

            foreach (var mtlFile in MtlFiles) {
                string materialName = Models3DViewModel.GetCurrentMtlFileTexture(mtlFile.Name).Diffuse.Name;

                var item = new ModelsOverviewTreeItemViewModel(this) {
                    HeaderText = $"{mtlFile.Name} [{materialName}]",
                    IconSource = new BitmapImage().FromByteArray(Resources.TextureIcon),
                    MtlFileName = mtlFile.Name,
                    Action = (e) => SelectMtlFile(e.MtlFileName)
                };

                Items.Add(item);
            }
        }

        private void SelectMtlFile(string mtlFileName) {
            var textures = MtlFiles.FirstOrDefault(mf => mf.Name == mtlFileName)?.Data;

            MtlTextures.Clear();

            if (textures != null) {
                foreach (var mtlTexture in textures) {
                    MtlTextures.Add(mtlTexture);
                }
            }
        }

        private void MtlFilesChanged(object? sender, NotifyCollectionChangedEventArgs e) {
            UpdateMtlFiles();
        }

        private void ModelsPlyChanged(object? sender, NotifyCollectionChangedEventArgs e) {
            switch (e.Action) {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems != null) {
                        foreach (var model in e.NewItems) {
                            var modelPly = (PlyModel3D)model!;
                            PlyItems.Add(new ModelsOverviewTreeItemViewModel(this) { 
                                HeaderText = modelPly.PlyFile?.Name ?? "",
                                IconSource = new BitmapImage().FromByteArray(Resources.PlyIcon)
                            });
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems != null) {
                        foreach (var model in e.OldItems) {
                            var modelPly = (PlyModel3D)model!;

                            var delete = PlyItems.FirstOrDefault(x => x.HeaderText == modelPly.PlyFile?.Name);

                            if (delete != null) {
                                PlyItems.Remove(delete);
                            }
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    break;
            }

        }

        public override void LoadData() {
            
        }
    }
}
