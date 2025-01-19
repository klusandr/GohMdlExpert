using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Views;
using Microsoft.Win32;

namespace GohMdlExpert.Services {
    public class MaterialSelector {
        private MaterialLoadView? _materialLoad;
        private ChildWindow? _childWindow;

        public MaterialFile? SelectedMaterialFile { get; set; }

        public event EventHandler? SelectedMaterialFileChange;

        public MaterialSelector() { }

        public MaterialFile? GetMaterialDialog() {
            _materialLoad ??= LoadView();

            _childWindow = new ChildWindow() {
                Title = $"Load materials",
                Content = _materialLoad,
            };

            _childWindow.ShowDialog();

            return SelectedMaterialFile;
        }

        

        private MaterialLoadView LoadView() {
            var view = new MaterialLoadView();

            view.ViewModel.MaterialApprove += MaterialApproveHandler;
            view.ViewModel.MaterialApply += MaterialApplyHandler;

            return view;
        }

        private void MaterialApplyHandler(object? sender, EventArgs e) {
            SelectedMaterialFile = _materialLoad?.ViewModel.SelectedMaterialFile;
            SelectedMaterialFileChange?.Invoke(this, EventArgs.Empty);
        }

        private void MaterialApproveHandler(object? sender, EventArgs e) {
            SelectedMaterialFile = _materialLoad.ViewModel.SelectedMaterialFile;
            _childWindow?.Close();
        }
    }
}
