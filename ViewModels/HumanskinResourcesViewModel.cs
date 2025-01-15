using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GohMdlExpert.Models.GatesOfHell.Resources.Humanskins;
using WpfMvvm.ViewModels;

namespace GohMdlExpert.ViewModels {
    public class HumanskinResourcesViewModel : BaseViewModel {
        private readonly GohHumanskinResourceProvider _resourceProvider;

        public GohHumanskinResourceProvider ResourceProvider => _resourceProvider;
        
        public HumanskinResourcesViewModel(GohHumanskinResourceProvider resourceProvider) {
            _resourceProvider = resourceProvider;
            _resourceProvider.ResourceUpdated += ResourceUpdatedHandler;
        }

        private void ResourceUpdatedHandler(object? sender, EventArgs e) {
            OnPropertyChanged(nameof(ResourceProvider));
        }
    }
}
