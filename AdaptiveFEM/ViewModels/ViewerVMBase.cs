using AdaptiveFEM.Commands;
using AdaptiveFEM.Commands.ViewerCommands;
using AdaptiveFEM.Models;
using AdaptiveFEM.Stores;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace AdaptiveFEM.ViewModels
{
    public abstract class ViewerVMBase : ViewModelBase
    {
        public abstract ObservableCollection<Component> Items { get; set; }

        #region View properties
        public double ViewWidth { get; protected set; }

        public double ViewHeight { get; protected set; }

        public double ZoomFactor { get; protected set; }
        #endregion

        #region Commands
        public ICommand ViewLoad { get; }

        public ICommand ViewSizeChange { get; }

        public ICommand Zoom { get; }

        public ICommand ButtonTranslate { get; }
        #endregion

        #region Coordinate element names
        protected string centerCircleName = "Coordinate-CenterCircle";

        protected string xAxisName = "Coordinate-XAxis";

        protected string yAxisName = "Coordinate-YAxis";

        protected Point coordinateCenterPosition = new Point(0, 0);
        #endregion

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
            Zoom = new Zoom(OnZoom, ResetZoom);
            ButtonTranslate = new ButtonTranslate(OnTranslate);
        }

        private void OnViewLoaded(double viewWidth, double viewHeight)
        {
            ViewWidth = viewWidth;
            ViewHeight = viewHeight;

            if (viewWidth > 0 &&
                viewHeight > 0)
            {
                coordinateCenterPosition = new Point(viewWidth / 2, viewHeight / 2);
                double centerCircleRadius = 1;
                double axisLength = 15;
                double axisHeadSize = 5;

                // Center circle
                Component centerCircle = new GeometryComponent("CenterCircle", new EllipseGeometry
                {
                    Center = new Point(0, 0),
                    RadiusX = centerCircleRadius,
                    RadiusY = centerCircleRadius
                });

                // XAxis
                Component xAxis = new GeometryComponent("XAxis", new PathGeometry
                {
                    Figures = new PathFigureCollection
                    {
                        new PathFigure
                        {
                            StartPoint = new Point(0, 0),
                            Segments = new PathSegmentCollection
                            {
                                new LineSegment { Point = new Point(0, 0) + new Vector(axisLength, 0) },
                                new LineSegment { Point = new Point(0, 0) + new Vector(axisLength, -axisHeadSize/4) },
                                new LineSegment { Point = new Point(0, 0) + new Vector(axisLength + axisHeadSize, 0) },
                                new LineSegment { Point = new Point(0, 0) + new Vector(axisLength, axisHeadSize/4) },
                                new LineSegment { Point = new Point(0, 0) + new Vector(axisLength, 0) }
                            }
                        }
                    }
                });

                // YAxis
                Component yAxis = new GeometryComponent("YAxis", new PathGeometry
                {
                    Figures = new PathFigureCollection
                    {
                        new PathFigure
                        {
                            StartPoint= new Point(0, 0),
                            Segments = new PathSegmentCollection
                            {
                                new LineSegment{ Point = new Point(0, 0) + new Vector(0, axisLength) },
                                new LineSegment{ Point = new Point(0, 0) + new Vector(-axisHeadSize / 4, axisLength) },
                                new LineSegment{ Point = new Point(0, 0) + new Vector(0, axisLength + axisHeadSize) },
                                new LineSegment{ Point = new Point(0, 0) + new Vector(axisHeadSize / 4, axisLength) },
                                new LineSegment{ Point = new Point(0, 0) + new Vector(0, axisLength) }
                            }
                        }
                    }
                });

                // Rearrange coordinate
                TransformGroup transform = new TransformGroup();
                transform.Children.Add(new ScaleTransform(1, -1));
                transform.Children
                    .Add(new TranslateTransform(coordinateCenterPosition.X,
                    coordinateCenterPosition.Y));

                centerCircle.Geometry.Transform = transform;
                xAxis.Geometry.Transform = transform;
                yAxis.Geometry.Transform = transform;

                Items.Add(centerCircle);
                Items.Add(xAxis);
                Items.Add(yAxis);
            }

        }

        private void OnViewSizeChanged(double viewWidth, double viewHeight)
        {
            if (ViewWidth == 0 ||
                ViewHeight == 0 ||
                Items.Count <= 0)
            {
                coordinateCenterPosition = new Point(viewWidth / 2, viewHeight / 2);
                double centerCircleRadius = 1;
                double axisLength = 15;
                double axisHeadSize = 5;

                // Center circle
                Component centerCircle = new GeometryComponent("CenterCircle", new EllipseGeometry
                {
                    Center = new Point(0, 0),
                    RadiusX = centerCircleRadius,
                    RadiusY = centerCircleRadius
                });

                // XAxis
                Component xAxis = new GeometryComponent("XAxis", new PathGeometry
                {
                    Figures = new PathFigureCollection
                    {
                        new PathFigure
                        {
                            StartPoint = new Point(0, 0),
                            Segments = new PathSegmentCollection
                            {
                                new LineSegment { Point = new Point(0, 0) + new Vector(axisLength, 0) },
                                new LineSegment { Point = new Point(0, 0) + new Vector(axisLength, -axisHeadSize/4) },
                                new LineSegment { Point = new Point(0, 0) + new Vector(axisLength + axisHeadSize, 0) },
                                new LineSegment { Point = new Point(0, 0) + new Vector(axisLength, axisHeadSize/4) },
                                new LineSegment { Point = new Point(0, 0) + new Vector(axisLength, 0) }
                            }
                        }
                    }
                });

                // YAxis
                Component yAxis = new GeometryComponent("YAxis", new PathGeometry
                {
                    Figures = new PathFigureCollection
                    {
                        new PathFigure
                        {
                            StartPoint= new Point(0, 0),
                            Segments = new PathSegmentCollection
                            {
                                new LineSegment{ Point = new Point(0, 0) + new Vector(0, axisLength) },
                                new LineSegment{ Point = new Point(0, 0) + new Vector(-axisHeadSize / 4, axisLength) },
                                new LineSegment{ Point = new Point(0, 0) + new Vector(0, axisLength + axisHeadSize) },
                                new LineSegment{ Point = new Point(0, 0) + new Vector(axisHeadSize / 4, axisLength) },
                                new LineSegment{ Point = new Point(0, 0) + new Vector(0, axisLength) }
                            }
                        }
                    }
                });

                // Rearrange coordinate
                TransformGroup transform = new TransformGroup();
                transform.Children.Add(new ScaleTransform(1, -1));
                transform.Children
                    .Add(new TranslateTransform(coordinateCenterPosition.X,
                    coordinateCenterPosition.Y));

                centerCircle.Geometry.Transform = transform;
                xAxis.Geometry.Transform = transform;
                yAxis.Geometry.Transform = transform;

                Items.Add(centerCircle);
                Items.Add(xAxis);
                Items.Add(yAxis);
            }
            ViewWidth = viewWidth;
            ViewHeight = viewHeight;
        }

        private void OnTranslate(double deltaX, double deltaY)
        {
            if (Items.Count > 0)
            {
                coordinateCenterPosition.X -= deltaX;
                coordinateCenterPosition.Y -= deltaY;

                Transform oldTransform = Items[0].Geometry.Transform;

                TransformGroup newTransform = new TransformGroup();
                newTransform.Children.Add(oldTransform);
                newTransform.Children.Add(new TranslateTransform(-deltaX, -deltaY));

                for (int i = 0; i < Items.Count; i++)
                    Items[i].Geometry.Transform = newTransform;
            }
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
