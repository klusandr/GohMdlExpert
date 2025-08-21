using System.IO;
using System.Linq;
using GohMdlExpert.Models.GatesOfHell.Caches;
using GohMdlExpert.Models.GatesOfHell.Exceptions;
using GohMdlExpert.Models.GatesOfHell.Resources.Data;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Models.GatesOfHell.Resources.Files.Aggregates;
using GohMdlExpert.Models.GatesOfHell.Resources.Files.Loaders;
using GohMdlExpert.Models.GatesOfHell.Сaches;
using Microsoft.Extensions.DependencyInjection;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Humanskins
{
    public class GohFactionHumanskinResource : IGohHumanskinResource {
        private const string HUMANSKIN_SOURCE_DIRECTORY_NAME_REG = @"\[.*_source\]";

        private readonly GohResourceProvider _resourceProvider;
        //private readonly GohCacheProvider? _cacheProvider;

        public string Name { get; }
        public GohResourceDirectory Root { get; }
        public GohResourceDirectory Source { get; }

        public GohFactionHumanskinResource(string name, GohResourceDirectory factionRoot, GohResourceProvider resourceProvider) {
            var source = factionRoot.FindResourceElements<GohResourceDirectory>(null, searchPattern: HUMANSKIN_SOURCE_DIRECTORY_NAME_REG, first: true, deepSearch: false).FirstOrDefault();

            if (source == null)
            //|| !source.FindResourceElements<PlyFile>(first: true).Any()
            //|| !factionRoot.FindResourceElements<MdlFile>(first: true).Any())
            {
                throw new GohResourceLoadException($"Directory {factionRoot.GetFullPath} is not faction Humanskin directory");
            }

            _resourceProvider = resourceProvider;
            
            Name = name;
            Root = factionRoot;
            Source = source;
        }

        public IEnumerable<MdlFile> GetPlyMdlFiles(PlyFile plyFile) {
            var currentDirectory = Root;

            var mdlFiles = currentDirectory.FindResourceElements((r) => {
                if (r is MdlFile mdlFile) {
                    //if (mdlFile.GetAllText().Contains(plyFile.Name)) {
                    //    return true;
                    //}
                    return true;
                }

                return false;
            }).Cast<MdlFile>();

            mdlFiles = mdlFiles.Where(m => m.GetAllText().Contains(plyFile.Name));

            return mdlFiles;
        }

        public IEnumerable<AggregateMtlFile> GetPlyAggregateMtlFiles(PlyFile plyFile) {
            var mtlFiles = new List<AggregateMtlFile>();

            foreach (var mesh in plyFile.Data.Meshes) {
                mtlFiles.Add(new AggregateMtlFile(mesh.TextureName, () => GetPlyMeshMtlTextures(plyFile, mesh.TextureName)));
            }

            return mtlFiles;
        }

        public MtlTextureCollection GetPlyMeshMtlTextures(PlyFile plyFile, string meshTextureName) {
            throw new NotImplementedException();
        }

        public PlyFile GetNullPlyFile(PlyFile plyFile) {
            return new PlyFile(@"F:\SDK\Content\goh\entity\humanskin\[germans]\[ger_source]\ger_null.ply");
        }
    }
}
