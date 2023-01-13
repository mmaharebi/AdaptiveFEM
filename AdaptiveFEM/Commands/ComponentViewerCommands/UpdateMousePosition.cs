using System;
using System.Windows;
using System.Windows.Input;

namespace AdaptiveFEM.Commands.ComponentViewerCommands
{
    public class UpdateMousePosition : CommandBase
    {
        private Action<Point> _onMouseMove;

        public UpdateMousePosition(Action<Point> onMouseMove)
        {
            _onMouseMove = onMouseMove;
        }

        public override void Execute(object? parameter)
        {
            if (parameter is FrameworkElement fe)
                _onMouseMove(Mouse.GetPosition(fe));
        }
    }
}
