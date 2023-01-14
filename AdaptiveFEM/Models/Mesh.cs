using NumSharp;
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

        public List<LineGeometry> UniformMeshLines(Geometry geometry, uint samples)
        {
            List<LineGeometry> lines = new List<LineGeometry>();

            var phi = np.arange(0, 2 * np.pi, 2 * np.pi / samples);

            // Parametrization: a cos(phi) i + b sin(phi) j
            if (geometry is EllipseGeometry ellipse)
            {
                double a = ellipse.RadiusX;
                double b = ellipse.RadiusY;


                for (int i = 0; i < np.size(phi) - 1; i++)
                    lines.Add(new LineGeometry
                    {
                        StartPoint = new Point(a * np.cos(phi[i]), b * np.sin(phi[i])),
                        EndPoint = new Point(a * np.cos(phi[i + 1]), b * np.sin(phi[i + 1]))
                    });

                // Last to first
                lines.Add(new LineGeometry
                {
                    StartPoint = new Point(a * np.cos(phi[-1]), b * np.sin(phi[-1])),
                    EndPoint = new Point(a * np.cos(phi[0]), b * np.sin(phi[0]))
                });
            }

            // Parametrization: sample-length traversal:
            // Sarting from top-left corner and making lines for each
            // sample-length forward and in cw direction
            if (geometry is RectangleGeometry rectangle)
            {
                double w = rectangle.Rect.Width;
                double h = rectangle.Rect.Height;

                Point topLeft = rectangle.Rect.TopLeft;
                Point topRight = rectangle.Rect.TopRight;
                Point bottomRight = rectangle.Rect.BottomRight;
                Point bottomLeft = rectangle.Rect.BottomLeft;

                // Differential length of sample
                double ds = 2 * (w + h) / samples;

                for (int i = 0; i < samples; i++)
                {
                    double s = ds * i;
                    // Top line
                    if (s + ds < w)
                        lines.Add(new LineGeometry
                        {
                            StartPoint = topLeft + new Vector(s, 0),
                            EndPoint = topLeft + new Vector(s + ds, 0),
                        });
                    // Top-Right corner
                    if (s < w && s + ds >= w)
                        lines.Add(new LineGeometry
                        {
                            StartPoint = topLeft + new Vector(s, 0),
                            EndPoint = topRight + new Vector(0, s + ds - w),
                        });
                    // Right-line
                    if (s > w && s + ds < w + h)
                        lines.Add(new LineGeometry
                        {
                            StartPoint = topRight + new Vector(0, s - w),
                            EndPoint = topRight + new Vector(0, s + ds - w),
                        });
                    // Bottom-Right corner
                    if (s < w + h && s + ds >= w + h)
                        lines.Add(new LineGeometry
                        {
                            StartPoint = topRight + new Vector(0, s - w),
                            EndPoint = bottomRight - new Vector(s + ds - w - h, 0),
                        });
                    // Bottom line
                    if (s > w + h && s + ds < 2 * w + h)
                        lines.Add(new LineGeometry
                        {
                            StartPoint = bottomRight - new Vector(s - w - h, 0),
                            EndPoint = bottomRight - new Vector(s + ds - w - h, 0),
                        });
                    // Bottom-Left corner
                    if (s < 2*w + h && s + ds >= 2 * w + h)
                        lines.Add(new LineGeometry
                        {
                            StartPoint = bottomRight - new Vector(s - w - h, 0),
                            EndPoint = bottomLeft - new Vector(0, s + ds - 2 * w - h),
                        });
                    // Left line
                    if (s > 2 * w + h)
                        lines.Add(new LineGeometry
                        {
                            StartPoint = bottomLeft - new Vector(0, s - 2 * w - h),
                            EndPoint = bottomLeft - new Vector(0, s + ds - 2 * w - h),
                        });
                }
            }

            return lines;
        }
    }
}
