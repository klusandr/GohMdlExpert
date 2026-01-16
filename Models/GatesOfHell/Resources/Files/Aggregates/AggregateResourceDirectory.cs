namespace GohMdlExpert.Models.GatesOfHell.Resources.Files.Aggregates {
    public class AggregateResourceDirectory : GohResourceDirectory {
        private readonly IEnumerable<GohResourceDirectory> _directories;

        public AggregateResourceDirectory(string name, IEnumerable<GohResourceDirectory> directories) : base(name) {
            _directories = directories;
        }

        public override void LoadData() {
            foreach (var directory in _directories) {
                Items.AddRange(directory.Items);
            }
        }
    }
}
