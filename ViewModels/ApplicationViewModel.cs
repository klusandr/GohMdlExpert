using System.Windows.Input;
using GohMdlExpert.Models.GatesOfHell.Resources;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using Microsoft.Win32;
using WpfMvvm.ViewModels;
using WpfMvvm.ViewModels.Commands;

namespace GohMdlExpert.ViewModels {
    public class ApplicationViewModel : BaseViewModel {
        private readonly GohResourceProvider _gohResourceProvider;
        private readonly HumanskinMdlOverviewViewModel _models3DView;

        public ICommand OpenResourceCommand => CommandManager.GetCommand(OpenResourceDirectory);
        public ICommand OpenFileCommand => CommandManager.GetCommand(OpenFile);

        public ApplicationViewModel(GohResourceProvider gohResourceProvider, HumanskinMdlOverviewViewModel models3DView) {
            _gohResourceProvider = gohResourceProvider;
            _models3DView = models3DView;
        }

        public void OpenFile() {
            var fileDialog = new OpenFileDialog {
                Filter = "Mdl files (*.mdl)|*.mdl"
            };

            if (fileDialog.ShowDialog() ?? false) {
                _models3DView.SetMtlFile(new MdlFile(fileDialog.FileName));
            }
        }

        public void OpenResourceDirectory() {
            var folderDialog = new OpenFolderDialog();

            if (folderDialog.ShowDialog() ?? false) {
                _gohResourceProvider.OpenResources(folderDialog.FolderName);
            }
        }
    }
}
