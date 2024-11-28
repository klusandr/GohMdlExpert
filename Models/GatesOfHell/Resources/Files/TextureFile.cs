namespace GohMdlExpert.Models.GatesOfHell.Resources.Files {
    public class TextureFile : GohResourceFile {
        public TextureFile(string name, string? path = null, string? relativePathPoint = null) : base(name, path, relativePathPoint) {
            Name += ".dds";
        }

        public override void LoadData() { }
    }
}
