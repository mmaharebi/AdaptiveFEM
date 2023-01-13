using AdaptiveFEM.Models;
using System.Windows;
using System.Windows.Media;

namespace AdaptiveFEM.ViewModels.ComponentProfiles
{
    public class CircleProfileVM : ComponentProfileVMBase
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

        private double _radius;

        public double Radius
        {
            get => _radius;
            set
            {
                _radius = value;
                OnPropertyChanged(nameof(Radius));
                OnPropertyChanged(nameof(IsGeometryValid));
            }
        }
        #endregion

        public override ShapeType ShapeType => ShapeType.Circle;

        public override bool IsGeometryValid => Radius > 0;

        public override Geometry Geometry => new EllipseGeometry
        {
            RadiusX = Radius,
            RadiusY = Radius,
            Center = new Point(CenterX, CenterY),
        };

        public CircleProfileVM() { }
    }
}
