using NumSharp;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace AdaptiveFEM.Models
{
    /// <summary>
    /// This object does all the dirty, smelly, spaghetti, and ... works.
    /// All of my intention was to make this process a little less confusing.
    /// So I hope you don't blame me!
    /// </summary>
    public class Solution
    {
        public List<List<Point>> MeshPoints { get; private set; }

        public event EventHandler<List<List<Point>>>? MeshPointsUpdated;

        const int unifomSamplesSize = 100;

        private readonly Design _design;

        public Solution(Design design)
        {
            MeshPoints = new List<List<Point>>();
            _design = design;

            //
            _design.DesignReset += OnDesignReset;
        }

        public void Solve()
        {
            if (_design.Model.Domain == null)
                throw new System
                    .MissingMemberException(nameof(_design.Model.Domain));

            MeshPoints = new List<List<Point>>();

            // Uniform sample
            MeshPoints = GenerateUniformSamples();

            // To alert MeshScene and update MeshLines
            OnMeshPointsUpdated();
        }

        private List<List<Point>> GenerateUniformSamples()
        {
            List<List<Point>> uniformMesh = new List<List<Point>>();

            if (_design.Model.Domain is not null)
            {
                // The first item in the list is Domain's samples
                uniformMesh.Add(GenerateUniformSample(_design.Model.Domain.Geometry));

                // Regions
                _design.Model.Regions.ForEach(region =>
                    uniformMesh.Add(GenerateUniformSample(region.Geometry)));
            }

            return uniformMesh;
        }

        private List<Point> GenerateUniformSample(Geometry geometry)
        {
            List<Point> uniformSamples = new List<Point>();

            if (geometry is RectangleGeometry rectangle)
                uniformSamples = GenerateUniformSampleForRectangle(rectangle);
            else if (geometry is EllipseGeometry ellipse)
                uniformSamples = GenerateUniformSampleForEllipse(ellipse);


            return uniformSamples;
        }

        private List<Point> GenerateUniformSampleForEllipse(EllipseGeometry ellipse)
        {
            List<Point> uniformSamples = new List<Point>();

            double rX = ellipse.RadiusX;
            double rY = ellipse.RadiusY;

            Point c = ellipse.Center;

            var phi = np.linspace(0, 2 * np.pi, unifomSamplesSize, endpoint: false);

            for (int i = 0; i < phi.shape[0]; i++)
                uniformSamples.Add(c + new Vector(rX * np.cos(phi[i]),
                    rY * np.sin(phi[i])));

            return uniformSamples;
        }

        private List<Point> GenerateUniformSampleForRectangle(RectangleGeometry rectangle)
        {
            List<Point> uniformSamples = new List<Point>();

            double w = rectangle.Rect.Width;
            double h = rectangle.Rect.Height;

            Point tl = rectangle.Rect.TopLeft;
            Point tr = rectangle.Rect.TopRight;
            Point br = rectangle.Rect.BottomRight;
            Point bl = rectangle.Rect.BottomLeft;

            double dl = 2 * (w + h) / unifomSamplesSize;

            var x = np.arange(0, w, dl);
            var y = np.arange(0, h, dl);

            // Top line
            for (int i = 0; i < x.shape[0]; i++)
                uniformSamples.Add(tl + new Vector(x[i], 0));

            // Right line
            for (int i = 0; i < y.shape[0]; i++)
                uniformSamples.Add(tr + new Vector(0, y[i]));

            // Down line
            for (int i = 0; i < x.shape[0]; i++)
                uniformSamples.Add(br - new Vector(x[i], 0));

            // Left line
            for (int i = 0; i < y.shape[0]; i++)
                uniformSamples.Add(bl - new Vector(0, y[i]));

            return uniformSamples;
        }



        private void OnMeshPointsUpdated()
        {
            MeshPointsUpdated?.Invoke(this, MeshPoints);
        }

        private void OnDesignReset(object? sender, System.EventArgs e)
        {
            ResetMesh();
        }

        private void ResetMesh()
        {
        }
    }
}
