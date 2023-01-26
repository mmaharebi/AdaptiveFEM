using AdaptiveFEM.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;

namespace AdaptiveFEM.ViewModels
{
    public class MeshVM : SceneVMBase
    {
        public ObservableCollection<MeshPolyLine> Items { get; set; }

        private bool _isLoaded;

        public MeshVM(Design design) : base(design)
        {
            _isLoaded = false;

            Items = new ObservableCollection<MeshPolyLine>();
            design.Solution.MeshPointsUpdated += OnMeshPointsUpdated;
        }

        private void OnMeshPointsUpdated(object? sender, List<List<Point>> e)
        {
            if (Items.Count > 0)
            {
                TransformGroup transform = Items[0].TransformGroup.CloneCurrentValue();

                foreach (List<Point> points in e)
                    Items.Insert(0, new MeshPolyLine
                    {
                        Points = new PointCollection(points),
                        Stroke = Brushes.Black,
                        TransformGroup = transform
                    });
            }

        }

        protected override void OnViewLoaded(double viewWidth, double viewHeight)
        {

            if (!_isLoaded &&
                viewWidth > 0 &&
                viewHeight > 0)
            {
                base.OnViewLoaded(viewWidth, viewHeight);
                TransformGroup transform = new TransformGroup();
                transform.Children.Add(new ScaleTransform(1, -1));
                transform.Children.Add(new TranslateTransform(viewWidth / 2, viewHeight / 2));

                #region XAxis
                List<Point> points = new List<Point>
                {
                    new Point(0, 0),
                    new Point(15, 0),
                    new Point(15, -2),
                    new Point(20, 0),
                    new Point(15, 2),
                    new Point(15, 0),
                };

                Items.Add(new MeshPolyLine
                {
                    Points = new PointCollection(points),
                    Stroke = Brushes.Blue,
                    TransformGroup = transform
                });
                #endregion

                #region YAxis
                points = new List<Point>
                {
                    new Point(0, 0),
                    new Point(0, 15),
                    new Point(-2, 15),
                    new Point(0, 20),
                    new Point(2, 15),
                    new Point(0, 15),
                };

                Items.Add(new MeshPolyLine
                {
                    Points = new PointCollection(points),
                    Stroke = Brushes.Blue,
                    TransformGroup = transform
                });
                #endregion

                _isLoaded = true;
            }
        }

        protected override void OnTranslate(double deltaX, double deltaY)
        {
            base.OnTranslate(deltaX, deltaY);

            if (Items.Count > 0)
            {
                for (int i = 0; i < Items.Count; i++)
                    Items[i].TransformGroup.Children.Add(new TranslateTransform(-deltaX, -deltaY));

                OnPropertyChanged(nameof(Items));

            }
        }

    }
}
