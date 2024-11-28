using GohMdlExpert.ViewModels.ModelsTree;
using GohMdlExpert.Views.ModelsTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfMvvm.Exceptions;
using WpfMvvm.ViewModels;

namespace GohMdlExpert.ViewModels
{
    public static class ViewModelsStartup {
        public static void Startup(object? sender, EventArgs e) {
            if (sender is ViewModelsProvider provider) {
                provider.Add<ApplicationViewModel>();
                provider.Add<Models3DViewModel>();
                provider.Add<ModelAdderViewModel>();
                provider.Add<LoadModelsTreeViewModel>();
            } else {
                throw new ViewModelsException($"Error startup viewmodels. Expected type was not {nameof(ViewModelsProvider)}");
            }
        }
    }
}
