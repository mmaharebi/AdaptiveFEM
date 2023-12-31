﻿using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace AdaptiveFEM.Stores
{
    public class GeometryElements
    {
        #region Strokes
        private SolidColorBrush _domainStroke;

        private SolidColorBrush _regionStroke;

        private SolidColorBrush _axisStroke;

        private SolidColorBrush _meshLineStroke;
        #endregion

        #region StrokeThicknesses
        private double _domainStrokeThickness;

        private double _regionStrokeThickness;

        private double _axisStrokeThickness;

        private double _meshLineStrokeThickness;
        #endregion

        #region Fills
        private SolidColorBrush _domainFill;
        private SolidColorBrush _regionFill;
        #endregion

        public GeometryElements()
        {
            // Strokes
            _domainStroke = Brushes.Red.Clone();
            _regionStroke = Brushes.Black.Clone();
            _axisStroke = Brushes.Blue.Clone();
            _meshLineStroke = Brushes.Purple.Clone();

            // StrokeThicknesses
            _domainStrokeThickness = 1;
            _regionStrokeThickness = 1;
            _axisStrokeThickness = 0.5;
            _meshLineStrokeThickness = 1;

            // Fills
            _domainFill = Brushes.LightSeaGreen;
            _regionFill = Brushes.LightGray;
        }

        public Path XAxis(Point StartPoint, double length, double headSize)
        {
            Path xAxis = new()
            {
                Stroke = _axisStroke,
                StrokeThickness = _axisStrokeThickness,
                Data = new PathGeometry
                {
                    Figures = new PathFigureCollection
                    {
                        new PathFigure
                        {
                            StartPoint = StartPoint,
                            Segments = new PathSegmentCollection
                            {
                                new LineSegment { Point = StartPoint + new Vector(length, 0) },
                                new LineSegment { Point = StartPoint + new Vector(length, -headSize/4) },
                                new LineSegment { Point = StartPoint + new Vector(length + headSize, 0) },
                                new LineSegment { Point = StartPoint + new Vector(length, headSize/4) },
                                new LineSegment { Point = StartPoint + new Vector(length, 0) }
                            }
                        }
                    }
                }
            };

            return xAxis;
        }

        public Path YAxis(Point StartPoint, double length, double headSize)
        {
            Path yAxis = new Path
            {
                Stroke = _axisStroke,
                StrokeThickness = _axisStrokeThickness,
                Data = new PathGeometry
                {
                    Figures = new PathFigureCollection
                    {
                        new PathFigure
                        {
                            StartPoint= StartPoint,
                            Segments = new PathSegmentCollection
                            {
                                new LineSegment{ Point = StartPoint + new Vector(0, -length) },
                                new LineSegment{ Point = StartPoint + new Vector(-headSize/4, -length) },
                                new LineSegment{ Point = StartPoint + new Vector(0, -length-headSize) },
                                new LineSegment{ Point = StartPoint + new Vector(headSize/4, -length) },
                                new LineSegment{ Point = StartPoint + new Vector(0, -length) }
                            }
                        }
                    }
                }
            };

            return yAxis;
        }

        public Path DomainPath(Geometry geometry)
        {
            return new Path
            {
                Data = geometry,
                Stroke = _domainStroke,
                StrokeThickness = _domainStrokeThickness,
                Fill = _domainFill,
            };
        }

        public Path RegionPath(Geometry geometry)
        {
            return new Path
            {
                Data = geometry,
                Stroke = _regionStroke,
                StrokeThickness = _regionStrokeThickness,
                Fill = _regionFill,
            };
        }

        public Path MeshLinePath(LineGeometry geometry)
        {
            Path path = new Path();

            path = new Path
            {
                Data = geometry,
                Stroke = _meshLineStroke,
                StrokeThickness = _meshLineStrokeThickness,
                Fill = Brushes.Transparent
            };

            return path;
        }

    }
}
