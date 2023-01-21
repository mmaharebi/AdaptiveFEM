using NumSharp;
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

        public List<LineGeometry> UniformMeshLines(Geometry geometry, uint samples)
        {
            List<LineGeometry> lineGeometries = new List<LineGeometry>();

            #region Ellipse sampling
            // Parametrization: a cos(phi) i + b sin(phi) j
            if (geometry is EllipseGeometry ellipse)
            {
                double a = ellipse.RadiusX;
                double b = ellipse.RadiusY;
                var phi = np.arange(0, 2 * np.pi, 2 * np.pi / samples);

                for (int i = 0; i < samples - 1; i++)
                    lineGeometries.Add(new LineGeometry
                    {
                        StartPoint = new Point(ellipse.Center.X + a * np.cos(phi[i]), ellipse.Center.Y + b * np.sin(phi[i])),
                        EndPoint = new Point(ellipse.Center.X + a * np.cos(phi[i + 1]), ellipse.Center.Y + b * np.sin(phi[i + 1]))
                    });

                // Last to first
                lineGeometries.Add(new LineGeometry
                {
                    StartPoint = new Point(ellipse.Center.X + a * np.cos(phi[-1]), ellipse.Center.Y + b * np.sin(phi[-1])),
                    EndPoint = new Point(ellipse.Center.X + a * np.cos(phi[0]), ellipse.Center.Y + b * np.sin(phi[0]))
                });
            }
            #endregion

            #region Rectangle sampling
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

                int wSamples;
                int hSamples;
                double dw;
                double dh;

                wSamples = Convert.ToInt32(w / (w + h) * samples / 2);
                hSamples = Convert.ToInt32(h / (w + h) * samples / 2);

                dw = w / wSamples;
                dh = h / hSamples;

                // Top and bottom sides
                for (int i = 0; i < wSamples; i++)
                {
                    lineGeometries.Add(new LineGeometry
                    {
                        StartPoint = topLeft + new Vector(dw * i, 0),
                        EndPoint = topLeft + new Vector(dw * (i + 1), 0)
                    });
                    lineGeometries.Add(new LineGeometry
                    {
                        StartPoint = bottomRight - new Vector(dw * i, 0),
                        EndPoint = bottomRight - new Vector(dw * (i + 1), 0)
                    });
                }

                // Right and left sides
                for (int i = 0; i < hSamples; i++)
                {
                    lineGeometries.Add(new LineGeometry
                    {
                        StartPoint = topRight + new Vector(0, dh * i),
                        EndPoint = topRight + new Vector(0, dh * (i + 1))
                    });
                    lineGeometries.Add(new LineGeometry
                    {
                        StartPoint = bottomLeft - new Vector(0, dh * i),
                        EndPoint = bottomLeft - new Vector(0, dh * (i + 1))
                    });
                }

            }
            #endregion

            return lineGeometries;
        }
    }
}
