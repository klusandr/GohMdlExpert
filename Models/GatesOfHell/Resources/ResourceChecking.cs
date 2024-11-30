using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GohMdlExpert.Models.GatesOfHell.Exceptions;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;

namespace GohMdlExpert.Models.GatesOfHell.Resources {
    public static class ResourceChecking {
        public static void ThrowCheckPlyFileMeshTextureName(PlyFile plyFile, string meshTextureName) {
            if (!CheckPlyModelMeshTextureName(plyFile.Data, meshTextureName)) {
                throw PlyModelException.NoContainMeshTextureName(null, meshTextureName);
            }
        }

        public static void ThrowCheckPlyModelMeshTextureName(PlyModel plyModel, string meshTextureName) {
            if (!CheckPlyModelMeshTextureName(plyModel, meshTextureName)) {
                throw PlyModelException.NoContainMeshTextureName(null, meshTextureName);
            }
        }

        public static bool CheckPlyModelMeshTextureName(PlyModel plyModel, string meshTextureName) {
            return plyModel.Meshes.Any(m => m.TextureName == meshTextureName);
        }
    }
}
