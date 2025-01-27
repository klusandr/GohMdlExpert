using GohMdlExpert.Models.GatesOfHell.Resources.Humanskins;
using WpfMvvm.Collections.ObjectModel;
using WpfMvvm.ViewModels;

namespace GohMdlExpert.ViewModels {
    public class HumanskinResourcesViewModel : BaseViewModel {
        private readonly GohHumanskinResourceProvider _resourceProvider;

        public IObservableEnumerable<GohFactionHumanskinResource> HumanskinResources => _resourceProvider.HumanskinResources;

        public GohFactionHumanskinResource SelectedHumanskinResource { get => _resourceProvider.Current; }

        public int SelectedHumanskinResourceIndex {
            get => _resourceProvider.SelectedIndex;
            set {
                _resourceProvider.SelectedIndex = value;
                OnPropertyChanged();
            }
        }

        public HumanskinResourcesViewModel(GohHumanskinResourceProvider resourceProvider) {
            _resourceProvider = resourceProvider;
            _resourceProvider.ResourceUpdated += ResourceUpdatedHandler;
        }

        private void ResourceUpdatedHandler(object? sender, EventArgs e) {
            OnPropertyChanged(nameof(SelectedHumanskinResourceIndex));
        }
    }
}
