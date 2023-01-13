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
    public class ComponentViewerVM : ViewModelBase
    {
        private ObservableCollection<Path> _geometries;

        public ObservableCollection<Path> Geometries
        {
            get => _geometries;
            set
            {
                _geometries = value;
                OnPropertyChanged(nameof(Geometries));
            }
        }

        private Point _coordinateCenter;

        public Point CoordinateCenter
        {
            get => _coordinateCenter;
            set
            {
                _coordinateCenter = value;
                UpdateGeometries();
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
            _geometryElements.XAxis(CoordinateCenter, length: 15, headSize: 5);

        public Path YAxis =>
            _geometryElements.YAxis(CoordinateCenter, length: 15, headSize: 5);

        public double ViewWidth { get; private set; }

        public double ViewHeight { get; private set; }

        public ICommand ViewLoad { get; }

        public ICommand KeyboardTranslate { get; }


        private readonly Design _design;

        private GeometryElements _geometryElements;

        public ComponentViewerVM(Design design)
        {
            //
            _design = design;

            //
            _geometries = new ObservableCollection<Path>();
            _geometryElements = new GeometryElements();

            //
            ViewLoad = new ViewLoad(OnViewLoaded);
            KeyboardTranslate = new KeyboardTranslate(OnTranslate);

            //
            _design.DesignChanged += OnDesignChanged;
        }

        private void OnViewLoaded(double viewWidth, double viewHeight)
        {
            ViewWidth = viewWidth;
            ViewHeight = viewHeight;

            CoordinateCenter = new Point(ViewWidth / 2, ViewHeight / 2);
        }

        private void OnTranslate(double deltaX, double deltaY)
        {
            CoordinateCenter =
                new Point(CoordinateCenter.X + deltaX,
                CoordinateCenter.Y + deltaY);
        }

        private void OnDesignChanged(object? sender, System.EventArgs e)
        {
            UpdateGeometries();
        }

        private void UpdateGeometries()
        {
            Geometries.Clear();

            TransformGroup transformGroup = new TransformGroup();
            transformGroup.Children.Add(new ScaleTransform(1, -1));
            transformGroup.Children.Add(new TranslateTransform(CoordinateCenter.X,
                CoordinateCenter.Y));

            if (_design.Model.Domain != null)
            {
                Geometry dg = _design.Model.Domain.Geometry;
                Path dp = _geometryElements.DomainPath(dg);
                dp.RenderTransform = transformGroup;
                Geometries.Add(dp);
            }

            _design.Model.Regions.ForEach(r =>
            {
                Geometry rg = r.Geometry;
                Path rp = _geometryElements.RegionPath(rg);
                rp.RenderTransform = transformGroup;
                Geometries.Add(rp);
            });

            Geometries.Add(CoordinateCircle);
            Geometries.Add(XAxis);
            Geometries.Add(YAxis);

            OnPropertyChanged(nameof(Geometries));
        }
    }
}
