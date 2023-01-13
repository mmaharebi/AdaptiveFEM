using System;

namespace AdaptiveFEM.Commands
{
    public class Zoom : CommandBase
    {
        public const double SCALE_FACTOR = 0.05;

        private Action<double> _onZoom;

        private Action _resetZoom;

        public Zoom(Action<double> onZoom, Action resetZoom)
        {
            _onZoom = onZoom;
            _resetZoom = resetZoom;
        }

        public override void Execute(object? parameter)
        {
            if (parameter is string zoomOption)
            {
                switch (zoomOption)
                {
                    case "ZoomIn":
                        _onZoom(1 + SCALE_FACTOR);
                        break;
                    case "ZoomOut":
                        _onZoom(1 - SCALE_FACTOR);
                        break;
                    case "Reset":
                        _resetZoom();
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
