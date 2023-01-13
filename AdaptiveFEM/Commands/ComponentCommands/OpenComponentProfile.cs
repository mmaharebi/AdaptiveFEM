using AdaptiveFEM.Models;
using AdaptiveFEM.Services;
using AdaptiveFEM.Stores;
using AdaptiveFEM.ViewModels.ComponentProfiles;
using AdaptiveFEM.Views.ComponentProfiles;

namespace AdaptiveFEM.Commands.ComponentCommands
{
    public class OpenComponentProfile : CommandBase
    {
        private readonly Design _design;

        private readonly MessageService _messageService;

        private readonly MaterialStore _materialStore;

        public OpenComponentProfile(Design design,
            MessageService messageService,
            MaterialStore materialStore)
        {
            _design = design;
            _messageService = messageService;
            _materialStore = materialStore;
        }

        public override void Execute(object? parameter)
        {
            if (parameter is object[] parameters &&
                parameters.Length == 2)
            {
                ShapeType shapeType = (ShapeType)parameters[0];
                ComponentType componentType = (ComponentType)parameters[1];

                ComponentProfileVMBase componentProfileVM = null;

                switch (shapeType)
                {
                    case ShapeType.Circle:
                        componentProfileVM = new CircleProfileVM();
                        break;
                    case ShapeType.Ellipse:
                        componentProfileVM = new EllipseProfileVM();
                        break;
                    case ShapeType.Rectangle:
                        componentProfileVM = new RectangleProfileVM();
                        break;
                    case ShapeType.Square:
                        componentProfileVM = new SquareProfileVM();
                        break;
                    default:
                        break;
                }

                if (componentProfileVM != null)
                {
                    var win = new ProfileContainer()
                    {
                        DataContext = new ProfileContainerVM(_design,
                            _materialStore,
                            _messageService,
                            componentProfileVM,
                            componentType)
                    };
                    win.ShowDialog();
                }
            }
        }
    }
}
