using AdaptiveFEM.Models;
using System.Windows;
using System.Windows.Media;

namespace AdaptiveFEM.ViewModels.ComponentProfiles
{
    public class RectangleProfileVM : ComponentProfileVMBase
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

        private double _width;

        public double Width
        {
            get => _width;
            set
            {
                _width = value;
                OnPropertyChanged(nameof(IsGeometryValid));
                OnPropertyChanged(nameof(Width));
            }
        }

        private double _height;

        public double Height
        {
            get => _height;
            set
            {
                _height = value;
                OnPropertyChanged(nameof(IsGeometryValid));
                OnPropertyChanged(nameof(Height));
            }
        }
        #endregion

        public override ShapeType ShapeType => ShapeType.Rectangle;

        public override bool IsGeometryValid => Width > 0 && Height > 0;

        public override Geometry Geometry => new RectangleGeometry
        {
            Rect = new Rect
            {
                Size = new Size(Width, Height),
                Location = new Point(CenterX - Width / 2, CenterY - Height / 2)
            },
        };

        public RectangleProfileVM() { }
    }
}
