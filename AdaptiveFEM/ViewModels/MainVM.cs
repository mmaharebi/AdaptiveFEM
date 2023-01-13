using AdaptiveFEM.Models;
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

        private ViewModelBase _componentViewerVM;

        public ViewModelBase ComponentViewerVM
        {
            get => _componentViewerVM;
            set
            {
                _componentViewerVM = value;
                OnPropertyChanged(nameof(ComponentViewerVM));
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
        }
    }
}
