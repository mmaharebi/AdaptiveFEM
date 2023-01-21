using System;
using System.Windows;

namespace AdaptiveFEM.Commands.ViewerCommands
{
    public class ViewLoad : CommandBase
    {
        private Action<double, double> _onViewLoaded;

        public ViewLoad(Action<double, double> onViewLoaded)
        {
            _onViewLoaded = onViewLoaded;
        }

        public override void Execute(object? parameter)
        {
            if (parameter is FrameworkElement fe)
                _onViewLoaded(fe.ActualWidth, fe.ActualHeight);
        }
    }
}
