using System.Windows.Input;
using System.Xml.Serialization;
using GohMdlExpert.Models.GatesOfHell.Exceptions;
using GohMdlExpert.Models.GatesOfHell.Media3D;
using GohMdlExpert.Models.GatesOfHell.Resources;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Models.GatesOfHell.Resources.Humanskins;
using WpfMvvm.ViewModels;
using WpfMvvm.ViewModels.Commands;

namespace GohMdlExpert.ViewModels {
    public class ModelAdderViewModel : BaseViewModel {
        private readonly Models3DViewModel _models3DView;
        private PlyAggregateMtlFiles? _aggregateMtlFiles;

        public PlyModel3D? AddedModel {
            get { return _models3DView.AddedModel; }
            private set {
                _models3DView.AddedModel = value;
                OnPropertyChanged();
            }
        }

        public PlyAggregateMtlFiles? AggregateMtlFiles { get => _aggregateMtlFiles; set => _aggregateMtlFiles = value; }

        public bool IsAddedInProgress => AddedModel != null;

        public ICommand AddModelCommand => CommandManager.GetCommand(AddModel, canExecute: (_) => IsAddedInProgress);
        public ICommand ClearModelCommand => CommandManager.GetCommand(ClearModel, canExecute: (_) => IsAddedInProgress);

        public event EventHandler? ModelAdded;
        public event EventHandler? CancelModelAdded;

        public ModelAdderViewModel(Models3DViewModel models3DView) {
            _models3DView = models3DView;

            PropertyChangeHandler.AddHandler(nameof(AddedModel), (_, _) => { 
                ((Command)AddModelCommand).OnCanExecuteChanged();
                ((Command)ClearModelCommand).OnCanExecuteChanged();
            });
        }

        public void SetModel(PlyFile plyFile, PlyAggregateMtlFiles? aggregateMtlFiles) {
            ClearModel();

            if (aggregateMtlFiles != null && aggregateMtlFiles.PlyFile != plyFile) {
                throw TextureException.NotBelongPlyModel(aggregateMtlFiles);
            }

            AggregateMtlFiles = aggregateMtlFiles;
            AddedModel = new PlyModel3D(plyFile, aggregateMtlFiles);
        }

        public void SetMtlFiles(PlyAggregateMtlFiles aggregateMtlFiles) {
            if (!IsAddedInProgress) {
                throw new InvalidOperationException("Error setting texture model. Model not added yet.");
            }

            if (aggregateMtlFiles.PlyFile != AddedModel!.PlyFile) {
                throw TextureException.NotBelongPlyModel(aggregateMtlFiles);
            }

            foreach (var aggregateMtlFile in aggregateMtlFiles) {
                AddedModel.SetMeshTexture(aggregateMtlFile.Name, aggregateMtlFile.Data.FirstOrDefault());
            }

            AggregateMtlFiles = aggregateMtlFiles;
        }

        public void ClearModel() {
            if (IsAddedInProgress) {
                AggregateMtlFiles = null;
                AddedModel = null;
                CancelModelAdded?.Invoke(this, EventArgs.Empty);
            }
        }

        public void AddModel() {
            if (AddedModel != null) {
                try {
                    _models3DView.AddModel(AddedModel, AggregateMtlFiles);
                    AddedModel = null;
                    ModelAdded?.Invoke(this, EventArgs.Empty);
                } catch (OperationCanceledException) { }   
            }
        }

        public void AddModel(PlyFile plyFile, PlyAggregateMtlFiles? aggregateMtlFiles) {
            _models3DView.AddModel(new PlyModel3D(plyFile, aggregateMtlFiles), aggregateMtlFiles);
        }

        public void SelectModelMeshTexture(string mashTextureName, MtlTexture mtlTexture) {
            if (!IsAddedInProgress) {
                throw new InvalidOperationException("Error setting texture model. Model not added yet.");
            }

            ResourceChecking.ThrowCheckPlyFileMeshTextureName(AddedModel!.PlyFile, mashTextureName);

            if (AggregateMtlFiles != null) {
                if (!AggregateMtlFiles[mashTextureName].Data.Contains(mtlTexture)) {
                    throw PlyModelException.AttemptInstallInvalidMtlTexture(AddedModel.PlyFile, mtlTexture);
                }
            }

            AddedModel!.SetMeshTexture(mashTextureName, mtlTexture);
        }
    }
}
