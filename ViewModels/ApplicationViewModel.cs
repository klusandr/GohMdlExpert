using GohMdlExpert.Models.GatesOfHell.Resources;
using GohMdlExpert.Models.GatesOfHell.Serialization;
using GohMdlExpert.Views.Models3D;
using Microsoft.Win32;
using MvvmWpf.Extensions;
using MvvmWpf.ViewModels;
using MvvmWpf.ViewModels.Commands;
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
    public class ApplicationViewModel : ViewModelBase {
        private MdlSerializer _mdlSerialize = new MdlSerializer();

        public string Path { get; set; } = "F:\\SDK\\Content\\goh\\entity\\humanskin\\[germans]";

        public ICommand OpenFileCommand => CommandManager.GetCommand(OpenFile);

        public IAsyncCommand ThroughModelsCommand => CommandManager.GetAsyncCommand(ThroughModels);

        public ApplicationViewModel() {
            GohResourceLocations.Instance.ResourcePath = "F:\\SDK\\Content\\goh";
        }

        public void OpenFile() {
            var fileDialog = new OpenFileDialog();

            fileDialog.Filter = "Mdl files (*.mdl)|*.mdl";

            if (fileDialog.ShowDialog() ?? false) {
                //var d = _mdlSerialize.Deserialize(File.ReadAllText(fileDialog.FileName));

                //List<string> files = new();

                //foreach (var lodViewParameter in (IEnumerable<ModelDataParametr>) ModelDataSerializer.FindParameter(d, MdlTypes.Bone.ToString(), "skin")?.Data!) {
                //    files.Add((string)((IEnumerable<ModelDataParametr>)lodViewParameter.Data!).First().Data!);
                //}

                var models3DViewModel = ViewModelManager.GetViewModel<Models3DViewModel>();

                //foreach (var file in files) {
                //    models3DViewModel.OpenPlyFile(file.Replace("..", Path));
                //}

                //var mdlFile = new MdlFile(fileDialog.FileName);

                //var text = mdlFile.Data.Textures[0].Data.Diffuse.Data;

                models3DViewModel.OpenMdlFile(fileDialog.FileName);
                //    }
                //}
            }
        }

        private async Task ThroughModels(object? p) {
            
        }
    }
}
