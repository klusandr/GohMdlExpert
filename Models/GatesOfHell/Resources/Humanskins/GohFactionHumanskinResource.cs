using GohMdlExpert.Models.GatesOfHell.Exceptions;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using System.IO;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Humanskins {
    public class GohFactionHumanskinResource {
        public string Name { get; }
        public GohResourceDirectory Root { get; }
        public GohResourceDirectory Source { get; }

        public GohFactionHumanskinResource(string name, GohResourceDirectory factionRoot) {
            var source = factionRoot.FindResourceElements<GohResourceDirectory>(null, searchPattern: GohResourceLocations.HUMANSKIN_SOURCE_DIRECTORY_NAME_REG, first: true, deepSearch: false).FirstOrDefault();

            if (source == null 
                || !source.FindResourceElements<PlyFile>(first: true).Any() 
                || !factionRoot.FindResourceElements<MdlFile>(first: true).Any())
            {
                throw new GohResourcesException($"Directory {factionRoot.GetFullPath} is not faction Humanskin directory");
            }

            Name = name;
            Root = factionRoot;
            Source = source;
        }

        public IEnumerable<MdlFile> GetPlyMdlFiles(PlyFile plyFile) {
            var currentDirectory = Root;

            var mdlFiles = currentDirectory.FindResourceElements((r) => {
                if (r is MdlFile mdlFile) {
                    if (mdlFile.GetAllText().Contains(plyFile.Name)) {
                        return true;
                    }
                }

                return false;
            }).Select(rf => (MdlFile)rf);

            return mdlFiles;
        }

        public IEnumerable<PlyAggregateMtlFile> GetPlyAggregateMtlFiles(PlyFile plyFile) {
            var mtlFiles = new List<PlyAggregateMtlFile>();

            foreach (var mesh in plyFile.Data.Meshes) {
                mtlFiles.Add(new PlyAggregateMtlFile(mesh.TextureName, plyFile, this));
            }

            return mtlFiles;
        }

        public MtlTextureCollection GetPlyMeshMtlTextures(PlyFile plyFile, string meshTextureName) {
            ResourceChecking.ThrowCheckPlyFileMeshTextureName(plyFile, meshTextureName);

            var mdlFiles = GetPlyMdlFiles(plyFile);
            var mtlTextures = new MtlTextureCollection();

            foreach (var mdlFile in mdlFiles) {
                var directory = new GohResourceDirectory(mdlFile);

                foreach (var mtlFile in directory.GetFiles().OfType<MtlFile>()) {
                    if (mtlFile.Name == meshTextureName) {
                        mtlTextures.Add(mtlFile.Data);
                    }
                }
            }

            return mtlTextures;
        }
    }
}
