using GohMdlExpert.Models.GatesOfHell.Resources.Humanskins;
using WpfMvvm.Extensions;
using WpfMvvm.ViewModels.Controls;

namespace GohMdlExpert.ViewModels.Trees.Humanskins {
    public class HumanskinTreeViewModel : TreeViewModel {
        private readonly GohHumanskinResourceProvider _humanskinProvider;
        private IGohHumanskinResource HumanskinResource => _humanskinProvider.Resource;

        public HumanskinMdlOverviewViewModel HumanskinOverview { get; }

        public HumanskinTreeViewModel(HumanskinMdlOverviewViewModel humanskinOverview, GohHumanskinResourceProvider humanskinProvider) {
            HumanskinOverview = humanskinOverview;
            _humanskinProvider = humanskinProvider;

            humanskinProvider.ResourceUpdated += HumanskinProviderUpdatedHandler;
        }

        public override void LoadData() {
            if (Items.Any()) {
                ClearData();
            }

            App.Current.Synchronize(() => AddItem(new HumanskinTreeDirectoryViewModel(HumanskinResource.Humanskins, this)));
        }

        private void HumanskinProviderUpdatedHandler(object? sender, EventArgs e) {
            UpdateData();
        }
    }
}
