﻿using GohMdlExpert.ViewModels;
using WpfMvvm.Views;
using WpfMvvm.Views.Attributes;

namespace GohMdlExpert.Views {
    /// <summary>
    /// Логика взаимодействия для TextureMaterialListView.xaml
    /// </summary>
    [BindingViewModel<AggregateTextureListViewModel>]
    public partial class TextureMaterialListView : BaseView {
        public TextureMaterialListView() {
            InitializeComponent();
        }
    }
}
