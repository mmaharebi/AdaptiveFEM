using System;

namespace AdaptiveFEM.Commands
{
    public class CloseWindow : CommandBase
    {
        private Action _onCloseThis;

        public CloseWindow(Action onCloseThis)
        {
            _onCloseThis = onCloseThis;
        }

        public override void Execute(object? parameter)
        {
            _onCloseThis();
        }
    }
}
