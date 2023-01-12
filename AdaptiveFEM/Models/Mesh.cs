using AdaptiveFEM.Models.MathElements;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace AdaptiveFEM.Models
{
    public class Mesh
    {
        private readonly Model _model;

        public Mesh(Model model)
        {
            _model = model;
        }

        private List<Point> UniformSampler(Geometry geometry, uint samples)
        {
            if (samples <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(samples));
            }

            MathSet<Point> points = new MathSet<Point>();

            double deltaPhi = 2 * Math.PI / samples;

            // parametrization: a cos(phi) i + b sin(phi) j
            if (geometry is EllipseGeometry ellipse)
            {
                double a = ellipse.RadiusX;
                double b = ellipse.RadiusY;

                for (int i = 0; i < samples; i++)
                    points.Add(new Point(a * Math.Cos(deltaPhi * i), b * Math.Sin(deltaPhi * i)));
            }

            if (geometry is RectangleGeometry rectangle)
            {
                double w = rectangle.Rect.Width;
                double h = rectangle.Rect.Height;
                Point center = new Point(rectangle.Rect.X + w / 2, rectangle.Rect.Y + h / 2);

                // Add corners
                if (samples > 4)
                {
                    points.Add(rectangle.Rect.TopLeft);
                    points.Add(rectangle.Rect.TopRight);
                    points.Add(rectangle.Rect.BottomLeft);
                    points.Add(rectangle.Rect.BottomRight);
                }

                // Uniform
                samples = (points.Count > 0) ? samples - (uint)points.Count : samples;
                deltaPhi = 2 * Math.PI / samples;
                for (int i = 0; i < samples; i++)
                {
                    double phi = deltaPhi * i;
                    double alpha = Math.Atan(h / w);

                    if ((phi >= 2 * Math.PI - alpha && phi < 2 * Math.PI) ||
                        (Math.Tan(phi) >= 0 && phi < alpha))
                        points.Add(new Point(center.X + w / 2, center.Y + w / 2 * Math.Tan(phi)));

                    if (phi >= alpha && phi < Math.PI - alpha)
                        points.Add(new Point(center.X + w / 2 * Math.Cos(phi), center.Y + h / 2));

                    if (phi >= Math.PI - alpha && phi < Math.PI + alpha)
                        points.Add(new Point(center.X - w / 2, center.Y + w / 2 * Math.Tan(phi)));

                    if (phi >= Math.PI + alpha &&
                        phi < 2 * Math.PI - alpha)
                        points.Add(new Point(center.X + w / 2 * Math.Cos(phi), center.Y - h / 2));
                }
            }

            return new List<Point>(points);
        }
    }
}
