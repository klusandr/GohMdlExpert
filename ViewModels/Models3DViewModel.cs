using GohMdlExpert.Extensions;
using GohMdlExpert.Models.GatesOfHell.Media3D;
using GohMdlExpert.Models.GatesOfHell.Resources;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Models.GatesOfHell.Serialization;
using Microsoft.Win32;
using WpfMvvm.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using static GohMdlExpert.Models.GatesOfHell.Resources.PlyModel;
using GohMdlExpert.ViewModels.LoadModels;

namespace GohMdlExpert.ViewModels
{
    public sealed class Models3DViewModel : BaseViewModel {
        private List<Model3DPly> _modelsPly;
        private Model3DCollection _models;

        public IReadOnlyCollection<Model3DPly> ModelsPly => _modelsPly;
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
