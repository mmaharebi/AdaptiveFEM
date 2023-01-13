using AdaptiveFEM.Commands;
using AdaptiveFEM.Commands.ComponentCommands;
using AdaptiveFEM.Models;
using AdaptiveFEM.Services;
using AdaptiveFEM.Stores;
using System.Windows.Input;

namespace AdaptiveFEM.ViewModels
{
    public class TabNewVM : ViewModelBase
    {
        private readonly Design _design;

        public bool DomainDoesNotExist => _design.Model.Domain is null;

        public bool DomainExists => _design.Model.Domain is not null;

        public ICommand ResetProject { get; }

        public ICommand OpenComponentProfile { get; }

        public TabNewVM(Design design,
            MessageService messageService,
            MaterialStore materialStore)
        {
            // Assign fields
            _design = design;

            // Instantiate commands
            ResetProject = new ResetProject(design, messageService);
            OpenComponentProfile =
                new OpenComponentProfile(design,
                messageService,
                materialStore);

            _design.DesignChanged += OnDesignChanged;
        }

        private void OnDesignChanged(object? sender, System.EventArgs e)
        {
            OnPropertyChanged(nameof(DomainDoesNotExist));
            OnPropertyChanged(nameof(DomainExists));
        }
    }
}
