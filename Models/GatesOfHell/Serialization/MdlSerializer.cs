using System.Text;

namespace GohMdlExpert.Models.GatesOfHell.Serialization {
    public class MdlSerializer : ModelDataSerializer {
        public enum MdlTypes {
            Skeleton,
            Bone,
            BoneRevolute,
            Limits,
            Speed,
            Orientation,
            Matrix34,
            LODView,
            Position,
            VolumeView,
        }

        public MdlSerializer() {
            AddType("Skeleton", "skeleton");   
            AddType("Bone", "bone");   
            AddType("BoneRevolute", "bone revolute");   
            AddType("Limits", "limits");   
            AddType("Speed", "speed");   
            AddType("Orientation", "orientation");   
            AddType("Matrix34", "matrix34");   
            AddType("Position", "Position");   
            AddType("LODView", "LODView");   
            AddType("VolumeView", "VolumeView");
        }
    }
}
