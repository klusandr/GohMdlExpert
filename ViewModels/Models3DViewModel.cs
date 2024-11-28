using System.Collections.ObjectModel;
using System.Windows.Media.Media3D;
using GohMdlExpert.Extensions;
using GohMdlExpert.Models.GatesOfHell.Media3D;
using WpfMvvm.ViewModels;

namespace GohMdlExpert.ViewModels {
    public sealed class Models3DViewModel : BaseViewModel {
        private ObservableCollection<Model3DPly> _modelsPly;
        private Model3DCollection _models;

        public ObservableCollection<Model3DPly> ModelsPly => _modelsPly;
        public Model3DCollection Models => _models;


        public Point3D? ModelsCenter => ((GeometryModel3D?)Models.FirstOrDefault())?.GetCentrPoint();

        public Models3DViewModel() {
            _models = [];
            _modelsPly = [];
        }

        public void AddModel(Model3DPly modelPly) {
            _modelsPly.Add(modelPly);
            _models.Add(modelPly);

            OnPropertyChanged(nameof(Models));
        }

        public void RemoveModel(Model3DPly modelPly) {
            _models.Remove(modelPly);
            _modelsPly.Remove(modelPly);
        }

        public void Clear() {
            _models.Clear();
            _modelsPly.Clear();
        }

        private Point3D GetPointsCenter(params Point3D[] points3D) {
            return new Point3D() {
                X = points3D.Average(p => p.X),
                Y = points3D.Average(p => p.Y),
                Z = points3D.Average(p => p.Z),
            };
        }
    }
}
