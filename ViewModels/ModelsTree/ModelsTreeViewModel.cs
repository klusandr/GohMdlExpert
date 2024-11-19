using GohMdlExpert.Models.GatesOfHell.Resources;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Views.ModelsTree;
using MvvmWpf.ViewModels;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;

namespace GohMdlExpert.ViewModels.ModelsTree {
    public class ModelsTreeViewModel : ViewModelBase {
        public Models3DViewModel Models3DView { get; }
        public GohResourceLoader ResourceLoader { get; }
        public ObservableCollection<TreeViewItem> Items { get; }

        public ICommand LoadModelsCommand => CommandManager.GetCommand(LoadResources);

        public ModelsTreeViewModel() {
            Items = [];
            ResourceLoader = GohResourceLoader.Instance;
            Models3DView = ViewModelManager.GetViewModel<Models3DViewModel>()!;
        }

        public void LoadResources() {
            Items.Add(new ModelsTreeItemView(new ModelsTreeDirectoryViewModel(ResourceLoader.GetResourceDirectory("ger_humanskin_source"), this)));
        }
    }
}
