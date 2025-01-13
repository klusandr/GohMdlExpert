using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GohMdlExpert.Models.GatesOfHell.Resources;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Models.GatesOfHell.Resources.Humanskins;
using Microsoft.Win32;
using WpfMvvm.ViewModels;

namespace GohMdlExpert.ViewModels {
    public class HumanskinMdlGeneratorViewModel : BaseViewModel {

        public readonly GohHumanskinResourceProvider _humanskinResourceProvider;

        public HumanskinMdlGeneratorViewModel(GohHumanskinResourceProvider humanskinResourceProvider) {
            _humanskinResourceProvider = humanskinResourceProvider;
        }

        public void CreateMtlFile(MdlFile? mdlFile, IEnumerable<PlyFile> plyFiles, Dictionary<string, MtlTexture> mtlTextures, Dictionary<PlyFile, PlyFile[]> lodFiles) {
            var fileDialog = new SaveFileDialog() {
                Filter = ResourceLoading.MdlFileOpenFilter,
                AddExtension = true,
                DefaultExt = "mdl"
            };

            if (fileDialog.ShowDialog() == true) {
                var parameters = mdlFile?.Exists() == true ? mdlFile.Data.Parameters : ResourceLoading.GetHumanskinMdlParametersTemplate();
                var newMdlFile = new MdlFile(fileDialog.FileName);
                var newDefFile = new DefFile(Path.GetFileNameWithoutExtension(newMdlFile.Name) + ".def", newMdlFile.Path);

                var mtlFiles = new List<MtlFile>();
                var refPlyFiles = new List<PlyFile>();
                var refLodFiles = new Dictionary<PlyFile, PlyFile[]>();

                foreach (var mtlTexture in mtlTextures) {
                    mtlFiles.Add(new MtlFile(mtlTexture.Key, newMdlFile.GetDirectoryPath()) { Data = mtlTexture.Value });
                }

                foreach (var plyFile in plyFiles) {
                    var refPlyFile = new PlyFile(Path.Join("..", Path.GetRelativePath(_humanskinResourceProvider.Current!.Root.GetFullPath(), plyFile.GetFullPath())));

                    refPlyFiles.Add(refPlyFile);

                    if (lodFiles.TryGetValue(plyFile, out var plyLogFiles)){
                        var refPlyLodFiles = new List<PlyFile>();
                        foreach (var lodFile in plyLogFiles) {
                            refPlyLodFiles.Add(new PlyFile(Path.Join("..", Path.GetRelativePath(_humanskinResourceProvider.Current!.Root.GetFullPath(), lodFile.GetFullPath()))));
                        }

                        refLodFiles.Add(refPlyFile, [.. refPlyLodFiles]);
                    }
                }

                newMdlFile.Data = new MdlModel(parameters, refPlyFiles, mtlFiles, refLodFiles);

                newMdlFile.SaveData();
                newDefFile.SaveData();
            }
        }
    }
}
