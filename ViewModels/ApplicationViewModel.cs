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
        private MdlSerializer _mdlSerialize = new MdlSerializer();

        public string Path { get; set; } = "F:\\SDK\\Content\\goh\\entity\\Humanskin\\[germans]";

        public ICommand OpenFileCommand => CommandManager.GetCommand(OpenResourceDirectory);

        public GohResourceProvider GohResourceProvider { get; }

        public ApplicationViewModel(GohResourceProvider gohResourceProvider) {
            GohResourceProvider = gohResourceProvider;
        }

        public void OpenFile() {
            var fileDialog = new OpenFileDialog();

            fileDialog.Filter = "Mdl files (*.mdl)|*.mdl";

            if (fileDialog.ShowDialog() ?? false) {
                
            }
        }

        public void OpenResourceDirectory() {
            var folderDialog = new OpenFolderDialog();

            if (folderDialog.ShowDialog() ?? false) {
                GohResourceProvider.OpenResources(folderDialog.FolderName);
            }
        }
    }
}
