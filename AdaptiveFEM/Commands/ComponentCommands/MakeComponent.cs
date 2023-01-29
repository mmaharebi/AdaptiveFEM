using AdaptiveFEM.Models;
using AdaptiveFEM.Services;
using AdaptiveFEM.ViewModels.ComponentProfiles;
using System;
using System.ComponentModel;

namespace AdaptiveFEM.Commands.ComponentCommands
{
    public class MakeComponent : CommandBase
    {
        private readonly Design _design;
        private readonly MessageService _messageService;
        private readonly ProfileContainerVM _windowVM;
        private Action _closeWindow;

        public MakeComponent(Design design,
            MessageService messageService,
            ProfileContainerVM profileContainerVM,
            Action closeWindow)
        {
            _design = design;
            _messageService = messageService;
            _windowVM = profileContainerVM;
            _closeWindow = closeWindow;

            _windowVM.ComponentProfileVM.PropertyChanged += OnComponentProfileChanged;
        }

        public override bool CanExecute(object? parameter)
        {
            return base.CanExecute(parameter) &&
                _windowVM.IsGeometryValid;
        }

        public override void Execute(object? parameter)
        {
            Models.Component component;

            switch (_windowVM.ComponentType)
            {
                case ComponentType.Domain:
                    component =
                        new Domain(_windowVM.ComponentProfileVM.Geometry,
                        _windowVM.SelectedBoundaryType,
                        _windowVM.Phi,
                        _windowVM.SelectedMaterial,
                        _messageService);
                    if (_design.AddDomain((Domain)component))
                        _closeWindow();
                    break;
                case ComponentType.Region:
                    component =
                        new Region(_windowVM.ComponentProfileVM.Geometry,
                        _windowVM.SelectedBoundaryType,
                        _windowVM.Phi,
                        _windowVM.SelectedMaterial,
                        _messageService);
                    if (_design.AddRegion((Region)component))
                        _closeWindow();
                    break;
                default:
                    break;
            }
        }

        private void OnComponentProfileChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_windowVM.IsGeometryValid))
                OnCanExecuteChanged();
        }
    }
}
