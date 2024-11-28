using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GohMdlExpert.Models.GatesOfHell.Media3D;

namespace GohMdlExpert.ViewModels.ModelsTree.OverviewModels {
    public class ModelsOverviewTreeViewModel : ModelsTreeViewModel {
        public Models3DViewModel Models3DViewModel { get; }

        public ModelsOverviewTreeViewModel(Models3DViewModel models3DViewModel) {
            Models3DViewModel = models3DViewModel;
            models3DViewModel.ModelsPly.CollectionChanged += OnModelsPlyChanged;
        }

        private void OnModelsPlyChanged(object? sender, NotifyCollectionChangedEventArgs e) {
            switch (e.Action) {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems != null) {
                        foreach (var model in e.NewItems) {
                            var modelPly = (Model3DPly)model!;
                            Items.Add(new ModelsTreeItemViewModel(this) { HeaderText = modelPly.PlyFile?.Name ?? ""});
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems != null) {
                        foreach (var model in e.OldItems) {
                            var modelPly = (Model3DPly)model!;

                            var delete = Items.FirstOrDefault(x => x.HeaderText == modelPly.PlyFile?.Name);

                            if (delete != null) {
                                Items.Remove(delete);
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

        public override void LoadResources() {
            
        }


    }
}
