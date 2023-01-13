using AdaptiveFEM.Models;
using System.Windows;
using System.Windows.Media;

namespace AdaptiveFEM.ViewModels.ComponentProfiles
{
    public class SquareProfileVM : ComponentProfileVMBase
    {
        #region Geometry properties
        private double _centerX;

        public double CenterX
        {
            get => _centerX;
            set
            {
                _centerX = value;
                OnPropertyChanged(nameof(CenterX));
            }
        }

        private double _centerY;

        public double CenterY
        {
            get => _centerY;
            set
            {
                _centerY = value;
                OnPropertyChanged(nameof(CenterY));
            }
        }

        private double _side;

        public double Side
        {
            get => _side;
            set
            {
                _side = value;
                OnPropertyChanged(nameof(IsGeometryValid));
                OnPropertyChanged(nameof(Side));
            }
        }
        #endregion

        public override ShapeType ShapeType => ShapeType.Square;

        public override bool IsGeometryValid => Side > 0;

        public override Geometry Geometry => new RectangleGeometry
        {
            Rect = new Rect
            {
                Size = new Size(Side, Side),
                Location = new Point(CenterX - Side / 2, CenterY - Side / 2),
            }
        };
    }
}
