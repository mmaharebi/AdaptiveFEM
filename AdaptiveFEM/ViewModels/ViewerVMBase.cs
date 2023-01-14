using AdaptiveFEM.Commands;
using AdaptiveFEM.Commands.ComponentViewerCommands;
using AdaptiveFEM.Models;
using AdaptiveFEM.Stores;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace AdaptiveFEM.ViewModels
{
    public abstract class ViewerVMBase : ViewModelBase
    {
        public abstract ObservableCollection<Path> Items { get; set; }

        private Point _coordinateCenter;

        public Point CoordinateCenter
        {
            get => _coordinateCenter;
            set
            {
                _coordinateCenter = value;
                UpdateItems();
            }
        }


        public Path CoordinateCircle => new Path
        {
            Fill = Brushes.Blue,
            Data = new EllipseGeometry
            {
                Center = CoordinateCenter,
                RadiusX = 2,
                RadiusY = 2
            }
        };

        public Path XAxis =>
            geometryElements.XAxis(CoordinateCenter, length: 15, headSize: 5);

        public Path YAxis =>
            geometryElements.YAxis(CoordinateCenter, length: 15, headSize: 5);

        public double ViewWidth { get; protected set; }

        public double ViewHeight { get; protected set; }

        public double ZoomFactor { get; protected set; }

        public ICommand ViewLoad { get; }

        public ICommand ViewSizeChange { get; }

        public ICommand KeyboardTranslate { get; }

        public ICommand Zoom { get; }

        protected readonly Design design;
        protected GeometryElements geometryElements;

        public ViewerVMBase(Design design)
        {
            //
            this.design = design;
            geometryElements = new GeometryElements();
            ZoomFactor = 1.0;

            //
            ViewLoad = new ViewLoad(OnViewLoaded);
            ViewSizeChange = new ViewSizeChange(OnViewSizeChanged);
            KeyboardTranslate = new KeyboardTranslate(OnTranslate);
            Zoom = new Zoom(OnZoom, ResetZoom);

        }

        protected abstract void UpdateItems();

        private void OnViewLoaded(double viewWidth, double viewHeight)
        {
            ViewWidth = viewWidth;
            ViewHeight = viewHeight;

            CoordinateCenter = new Point(ViewWidth / 2, ViewHeight / 2);
        }

        private void OnViewSizeChanged(double viewWidth, double viewHeight)
        {
            ViewWidth = viewWidth;
            ViewHeight = viewHeight;
        }

        private void OnTranslate(double deltaX, double deltaY)
        {
            CoordinateCenter =
                new Point(CoordinateCenter.X + deltaX,
                CoordinateCenter.Y + deltaY);
        }

        private void OnZoom(double scaleFactor)
        {
            ZoomFactor *= scaleFactor;
            OnPropertyChanged(nameof(ZoomFactor));
        }

        private void ResetZoom()
        {
            ZoomFactor = 1;
            OnPropertyChanged(nameof(ZoomFactor));
        }
    }
}
