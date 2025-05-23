﻿using GohMdlExpert.ViewModels.Trees.LoadModels;
using WpfMvvm.Views;
using WpfMvvm.Views.Attributes;

namespace GohMdlExpert.Views.Trees {
    /// <summary>
    /// Логика взаимодействия для ModelsTreeView.xaml
    /// </summary>
    [BindingViewModel<ModelsLoadTreeViewModel>]
    public partial class ModelsLoadTreeView : BaseView {
        public ModelsLoadTreeView() {
            InitializeComponent();
        }
    }
}
