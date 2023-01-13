using System;

namespace AdaptiveFEM.Commands.ComponentViewerCommands
{
    public class KeyboardTranslate : CommandBase
    {
        private const double OFFSET = 10;

        private Action<double, double> _onTranslate;

        public KeyboardTranslate(Action<double, double> onTranslate)
        {
            _onTranslate = onTranslate;
        }

        public override void Execute(object? parameter)
        {
            if (parameter is string direction)
            {
                switch (direction)
                {
                    case "Left":
                        _onTranslate(-OFFSET, 0);
                        break;
                    case "Up":
                        _onTranslate(0, -OFFSET);
                        break;
                    case "Right":
                        _onTranslate(OFFSET, 0);
                        break;
                    case "Down":
                        _onTranslate(0, OFFSET);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
