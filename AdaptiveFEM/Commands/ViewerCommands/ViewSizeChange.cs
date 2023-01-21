using System.Windows;
using System;

namespace AdaptiveFEM.Commands.ViewerCommands
{
    public class ViewSizeChange : CommandBase
    {
        private Action<double, double> _onViewSizeChanged;

        public ViewSizeChange(Action<double, double> onViewSizeChanged)
        {
            _onViewSizeChanged = onViewSizeChanged;
        }

        public override void Execute(object? parameter)
        {
            if (parameter is FrameworkElement fe)
                _onViewSizeChanged(fe.ActualWidth, fe.ActualHeight);
        }
    }
}
