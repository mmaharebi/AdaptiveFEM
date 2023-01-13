using AdaptiveFEM.Models;
using System.Windows;
using System.Windows.Media;

namespace AdaptiveFEM.ViewModels.ComponentProfiles
{
    public class EllipseProfileVM : ComponentProfileVMBase
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

        private double _majorRadius;

        public double MajorRadius
        {
            get => _majorRadius;
            set
            {
                _majorRadius = value;
                OnPropertyChanged(nameof(MajorRadius));
                OnPropertyChanged(nameof(IsGeometryValid));
            }
        }

        private double _minorRadius;

        public double MinorRadius
        {
            get => _minorRadius;
            set
            {
                _minorRadius = value;
                OnPropertyChanged(nameof(MinorRadius));
                OnPropertyChanged(nameof(IsGeometryValid));
            }
        }
        #endregion

        public override ShapeType ShapeType => ShapeType.Ellipse;

        public override bool IsGeometryValid => MajorRadius > 0 && MinorRadius > 0;

        public override Geometry Geometry => new EllipseGeometry
        {
            Center = new Point(CenterX, CenterY),
            RadiusX = MajorRadius,
            RadiusY = MinorRadius
        };

        public EllipseProfileVM() { }
    }
}
