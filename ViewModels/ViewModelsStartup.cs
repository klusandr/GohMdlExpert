using GohMdlExpert.ViewModels.LoadModels;
using GohMdlExpert.Views.ModelsTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfMvvm.ViewModels;

namespace GohMdlExpert.ViewModels
{
    public static class ViewModelsStartup {
        public static void Startup(ViewModelProvider provider) {
            provider.Add<ApplicationViewModel>();
            provider.Add<Models3DViewModel>();
            provider.Add<ModelAdderViewModel>();
            provider.Add<LoadModelsTreeViewModel>();
        }
    }
}
