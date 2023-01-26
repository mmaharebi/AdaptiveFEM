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

        private ViewModelBase _tabMeshVM;

        public ViewModelBase TabMeshVM
        {
            get => _tabMeshVM;
            set
            {
                _tabMeshVM = value;
                OnPropertyChanged(nameof(TabMeshVM));
            }
        }

        private ViewModelBase _tabSolutionVM;

        public ViewModelBase TabSolutionVM
        {
            get => _tabSolutionVM;
            set
            {
                _tabSolutionVM = value;
                OnPropertyChanged(nameof(TabSolutionVM));
            }
        }

        public ToolbarVM(Design design,
            MaterialStore materialStore,
            MessageService messageService)
        {
            _tabNewVM = new TabNewVM(design, messageService, materialStore);
            _tabMeshVM = new TabMeshVM();
            _tabSolutionVM = new TabSolutionVM(design);
        }
    }
}
