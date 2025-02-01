using GohMdlExpert.Models.GatesOfHell.Resources.Data;
using GohMdlExpert.Models.GatesOfHell.Resources.Files.Aggregates;
using GohMdlExpert.Services;
using WpfMvvm.ViewModels;

namespace GohMdlExpert.ViewModels
{
    public class TextureMaterialListViewModel : BaseViewModel {
        private AggregateMtlFile? _mtlFile;
        private int _selectedMaterialIndex;
        private readonly TextureSelectorService _materialSelector;

        public AggregateMtlFile? MtlFile {
            get => _mtlFile;
            set {
                _mtlFile = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Items));
            }
        }

        public MtlTextureCollection? Items => MtlFile?.Data;

        public int SelectedMaterialIndex {
            get => _selectedMaterialIndex;
            set {
                _selectedMaterialIndex = value;
                OnPropertyChanged();
            }
        }

        public TextureMaterialListViewModel(TextureSelectorService materialSelector) {
            _materialSelector = materialSelector;
        }

        public void AddMaterial() {
            var newMaterial = _materialSelector.GetMaterialDialog();

            //if (newMaterial != null) {
            //    MtlFile.Data.Add(newMaterial);
            //}
        }
    }
}