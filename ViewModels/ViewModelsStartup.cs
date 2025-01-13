using GohMdlExpert.ViewModels.ModelsTree.LoadModels;
using GohMdlExpert.ViewModels.ModelsTree.OverviewModels;
using WpfMvvm.Exceptions;
using WpfMvvm.ViewModels;

namespace GohMdlExpert.ViewModels
{
    public static class ViewModelsStartup {
        public static void Startup(object? sender, EventArgs e) {
            if (sender is ViewModelsProvider provider) {
                provider.Add<ApplicationViewModel>();
                provider.Add<HumanskinMdlOverviewViewModel>();
                provider.Add<HumanskinMdlGeneratorViewModel>();
            } else {
                //throw new ViewModelsException($"Error startup viewmodels. Expected type was not {nameof(ViewModelsProvider)}");
            }
        }
    }
}
