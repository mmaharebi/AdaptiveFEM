﻿using AdaptiveFEM.Models;
using AdaptiveFEM.Services;
using AdaptiveFEM.Stores;

namespace AdaptiveFEM.ViewModels
{
    public class MainVM : ViewModelBase
    {
        private ViewModelBase _toolbarVM;

        public ViewModelBase ToolbarVM
        {
            get => _toolbarVM;
            set
            {
                _toolbarVM = value;
                OnPropertyChanged(nameof(ToolbarVM));
            }
        }

        private ComponentViewerVM _componentViewerVM;

        public ComponentViewerVM ComponentViewerVM
        {
            get => _componentViewerVM;
            set
            {
                _componentViewerVM = value;
                OnPropertyChanged(nameof(ComponentViewerVM));
            }
        }

        private MeshViewerVM _meshViewerVM;

        public MeshViewerVM MeshViewerVM
        {
            get => _meshViewerVM;
            set
            {
                _meshViewerVM = value;
                OnPropertyChanged(nameof(MeshViewerVM));
            }
        }




        private readonly Design _design;

        private readonly MessageService _messageService;

        public MainVM(Design design,
            MaterialStore materialStore,
            MessageService messageService)
        {
            _design = design;
            _messageService = messageService;

            _toolbarVM = new ToolbarVM(design, materialStore, messageService);
            _componentViewerVM = new ComponentViewerVM(design);
            _meshViewerVM = new MeshViewerVM(design);
        }
    }
}
