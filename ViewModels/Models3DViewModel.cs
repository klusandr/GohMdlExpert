using GohMdlExpert.Extensions;
using GohMdlExpert.Models.GatesOfHell.Resources;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Models.GatesOfHell.Serialization;
using Microsoft.Win32;
using MvvmWpf.ViewModels;
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

namespace GohMdlExpert.ViewModels
{
    public sealed class Models3DViewModel : ViewModelBase {
        private readonly Dictionary<string, MtlFile> _texturesCache;

        public Model3DCollection Models { get; set; }

        public ModelAdderViewModel Adder { get; set; }

        public Point3D? ModelsCenter => ((GeometryModel3D?)Models.FirstOrDefault())?.GetCentrPoint();

        public Models3DViewModel() {
            Adder = new ModelAdderViewModel(this);
            Models = [];
            _texturesCache = [];
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
