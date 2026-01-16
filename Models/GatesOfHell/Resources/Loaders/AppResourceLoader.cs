namespace GohMdlExpert.Models.GatesOfHell.Resources.Loaders {
    public class AppResourceLoader : FileSystemResourceLoader {
        public AppResourceLoader(string path) : base(path) { }

        public override bool CheckResourceDirectory(string path) {
            return true;
        }
    }
}
