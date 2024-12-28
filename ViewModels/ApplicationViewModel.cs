using GohMdlExpert.Models.GatesOfHell.Resources;
using GohMdlExpert.Models.GatesOfHell.Serialization;
using GohMdlExpert.Views.Models3D;
using Microsoft.Win32;
using WpfMvvm.Extensions;
using WpfMvvm.ViewModels;
using WpfMvvm.ViewModels.Commands;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Reflection.Metadata;
using System.Windows.Input;
using ModelDataParametr = GohMdlExpert.Models.GatesOfHell.Serialization.ModelDataSerializer.ModelDataParameter;
using MdlTypes = GohMdlExpert.Models.GatesOfHell.Serialization.MdlSerializer.MdlTypes;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;

namespace GohMdlExpert.ViewModels
{
    public class ApplicationViewModel : BaseViewModel {
        private readonly GohResourceProvider _gohResourceProvider;
        private readonly Models3DViewModel _models3DView;
        
        public ICommand OpenResourceCommand => CommandManager.GetCommand(OpenResourceDirectory);
        public ICommand OpenFileCommand => CommandManager.GetCommand(OpenFile);

        public ApplicationViewModel(GohResourceProvider gohResourceProvider, Models3DViewModel models3DView) {
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
