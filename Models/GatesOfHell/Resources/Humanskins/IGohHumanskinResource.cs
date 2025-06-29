using GohMdlExpert.Models.GatesOfHell.Resources.Data;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Models.GatesOfHell.Resources.Files.Aggregates;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Humanskins {
    public interface IGohHumanskinResource {
        GohResourceDirectory Root { get; }
        GohResourceDirectory Source { get; }

        PlyFile GetNullPlyFile(PlyFile plyFile);
        IEnumerable<AggregateMtlFile> GetPlyAggregateMtlFiles(PlyFile plyFile);
        IEnumerable<MdlFile> GetPlyMdlFiles(PlyFile plyFile);
        MtlTextureCollection GetPlyMeshMtlTextures(PlyFile plyFile, string meshTextureName);
    }
}