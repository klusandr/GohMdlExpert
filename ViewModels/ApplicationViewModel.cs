using GohMdlExpert.Models;
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
using static GohMdlExpert.Models.MdlSerialize;

namespace GohMdlExpert.ViewModels {
    public class ApplicationViewModel : ViewModelBase {
        public string Path { get; set; } = "F:\\SDK\\Content\\goh\\entity\\humanskin\\[germans]";

        public ICommand OpenFileCommand => CommandManager.GetCommand(OpenFile);

        public IAsyncCommand ThroughModelsCommand => CommandManager.GetAsyncCommand(ThroughModels);

        public void OpenFile() {
            var fileDialog = new OpenFileDialog();

            fileDialog.Filter = "Mdl files (*.mdl)|*.mdl";

            if (fileDialog.ShowDialog() ?? false) {
                var d = MdlSerialize.Deserialize(File.ReadAllText(fileDialog.FileName));

                List<string> files = new List<string>();

                foreach (var lodViewParameter in (IEnumerable<MdlParameter>)FindParameter(d, MdlTypes.Bone, "skin")?.Data!) {
                    files.Add((string)((IEnumerable<MdlParameter>)lodViewParameter.Data!).First().Data!);
                }

                var models3DViewModel = ViewModelManager.GetViewModel<Models3DViewModel>();

                foreach (var file in files) {
                    if (!file.Contains("head")) {
                        models3DViewModel.Models.Add(models3DViewModel.OpenPlyFile(file.Replace("..", Path)));
                    }
                }
            }
        }

        private async Task ThroughModels(object? p) {
            var ranksFiles = Directory.GetFiles("F:\\SDK\\Content\\goh\\entity\\humanskin\\[germans]\\[ger_source]\\ger_gear").Where(f => !f.Contains("lod")).ToArray();
            var ranksFiles1 = Directory.GetFiles("F:\\SDK\\Content\\goh\\entity\\humanskin\\[germans]\\[ger_source]\\ger_equipment").Where(f => !f.Contains("lod")).ToArray();

            var models3DViewModel = ViewModelManager.GetViewModel<Models3DViewModel>();

            models3DViewModel.Models.Add(models3DViewModel.OpenPlyFile(ranksFiles[0]));


            await Task.Run(() => {
                for (int i = 0; i < ranksFiles.Length || i < ranksFiles1.Length; i++) {
                    Thread.Sleep(100);
                    App.Current.Synchronize(() => {
                        if (i < ranksFiles.Length) {
                            models3DViewModel.Models.RemoveAt(models3DViewModel.Models.Count - 1);
                            models3DViewModel.Models.Add(models3DViewModel.OpenPlyFile(ranksFiles[i]));
                        }

                        if (i < ranksFiles1.Length) {
                            models3DViewModel.Models.RemoveAt(models3DViewModel.Models.Count - 2);
                            models3DViewModel.Models.Add(models3DViewModel.OpenPlyFile(ranksFiles1[i]));
                        }
                    });
                }

                foreach (var rankFile in ranksFiles) {

                    
                }
            });
        }

        private MdlParameter? FindParameter(IEnumerable<MdlParameter> parameters, MdlTypes type, string? name = null) {
            MdlParameter? result = null;

            foreach (var parameter in parameters) {
                if (parameter.Data != null) {
                    result = FindParameter(parameter, type, name);
                    if (result != null) {
                        break;
                    }
                }
            }

            return result;
        }

        private MdlParameter? FindParameter(MdlParameter parameter, MdlTypes type, string? name = null) {
            if (parameter.Type == type && name != null && parameter.Name == name) {
                return parameter;
            }

            if (parameter.Data is IEnumerable<MdlParameter> parametersCollection) {
                return FindParameter(parametersCollection, type, name);
            }

            return null;
        }
    }
}
