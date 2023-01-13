using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace AdaptiveFEM.Stores
{
    public class GeometryElements
    {
        public Path XAxis(Point StartPoint, double length, double headSize)
        {
            Path xAxis = new()
            {
                Stroke = Brushes.Blue,
                StrokeThickness = 1,
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
                                new LineSegment { Point = StartPoint + new Vector(length, -headSize/2) },
                                new LineSegment { Point = StartPoint + new Vector(length + headSize, 0) },
                                new LineSegment { Point = StartPoint + new Vector(length, headSize/2) },
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
                Stroke = Brushes.Blue,
                StrokeThickness = 1,
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
                                new LineSegment{ Point = StartPoint + new Vector(-headSize/2, -length) },
                                new LineSegment{ Point = StartPoint + new Vector(0, -length-headSize) },
                                new LineSegment{ Point = StartPoint + new Vector(headSize/2, -length) },
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
                Stroke = Brushes.Red,
                StrokeThickness = 1,
                Fill = Brushes.Transparent
            };
        }

        public Path RegionPath(Geometry geometry)
        {
            return new Path
            {
                Data = geometry,
                Stroke = Brushes.Black,
                StrokeThickness = 0.6,
                Fill = Brushes.Gray
            };
        }

    }
}
