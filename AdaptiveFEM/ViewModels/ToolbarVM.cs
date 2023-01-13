using AdaptiveFEM.Models;
using AdaptiveFEM.Services;
using AdaptiveFEM.Stores;

namespace AdaptiveFEM.ViewModels
{
    public class ToolbarVM : ViewModelBase
    {
        private ViewModelBase _tabNewVM;

        public ViewModelBase TabNewVM
        {
            get => _tabNewVM;
            set
            {
                _tabNewVM = value;
                OnPropertyChanged(nameof(TabNewVM));
            }
        }

        private readonly Design _design;

        private readonly MessageService _messageService;

        public ToolbarVM(Design design,
            MaterialStore materialStore,
            MessageService messageService)
        {
            _design = design;
            _messageService = messageService;
            _tabNewVM = new TabNewVM(design, messageService, materialStore);
        }
    }
}
