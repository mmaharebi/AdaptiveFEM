using AdaptiveFEM.MagicalSolver;
using AdaptiveFEM.Models.Materials;
using NumSharp;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public Dictionary<Point, double> Potential { get; private set; }

        public bool IsPotentialValid { get; private set; }

        public int PotentialXSize => 97;

        public int PotentialYSize => 59;

        public event EventHandler<List<List<Point>>>? MeshPointsUpdated;

        public event EventHandler? SolutionChanged;

        const int unifomSamplesSize = 23;

        private readonly Design _design;

        public Solution(Design design)
        {
            MeshPoints = new List<List<Point>>();
            Potential = new Dictionary<Point, double>();
            IsPotentialValid = false;
            _design = design;

            //
            _design.DesignChanged += OnDesignChanged;
            _design.DesignReset += OnDesignReset;
        }


        /// <summary>
        /// <warning>
        /// In this function an "elementary problem" is solved; which means the
        /// outer box is actually a rectangle and there is some elements inside this box.
        /// </warning>
        /// </summary>
        /// <exception cref="System.MissingMemberException"></exception>
        public void Solve()
        {
            if (_design.Model.Domain == null)
                throw new System
                    .MissingMemberException(nameof(_design.Model.Domain));

            OnSolutionChanged();

            MeshPoints = new List<List<Point>>();
            Potential = new Dictionary<Point, double>();
            IsPotentialValid = false;

            // Four essential points of outer box
            Point p0, p1, p2, p3;
            PointStar P0, P1, P2, P3;

            // Check if the Domain bound is rectangular
            if (_design.Model.Domain.Geometry is not RectangleGeometry)
                throw new System.NotImplementedException();

            RectangleGeometry rectangle =
                (RectangleGeometry)_design.Model.Domain.Geometry;

            // Assuming p1-p3 diagonal is drawn up. First triangle
            // with index = 0 consists of (p0, p1, p3) vetices and second
            // triangle consists of (p0, p2, p3) vertices; vetices are named
            // by clock-wise naming contract in computer's coordinates.
            #region Solution domain PointStars
            p0 = rectangle.Rect.TopLeft;
            P0 = new PointStar
            {
                Point = p0,
                NeighborTriangles = new List<int> { 0, 1, },
                IndexInList = 0,
                Phi = _design.Model.Domain.Phi,
                IsOuter = true,
                IsBoundary = true,
                WhichBoundary = 0,
                IsFixedCharge = false,
            };

            p1 = rectangle.Rect.TopRight;
            P1 = new PointStar
            {
                Point = p1,
                NeighborTriangles = new List<int>() { 0, -100 },
                IndexInList = 1,
                Phi = _design.Model.Domain.Phi,
                IsOuter = true,
                IsBoundary = true,
                WhichBoundary = 0,
                IsFixedCharge = false,
            };

            p2 = rectangle.Rect.BottomRight;
            P2 = new PointStar
            {
                Point = p2,
                NeighborTriangles = new List<int>() { 0, 1, },
                IndexInList = 2,
                Phi = _design.Model.Domain.Phi,
                IsOuter = true,
                IsBoundary = true,
                WhichBoundary = 0,
                IsFixedCharge = false,
            };

            p3 = rectangle.Rect.BottomLeft;
            P3 = new PointStar
            {
                Point = p3,
                NeighborTriangles = new List<int>() { 1, -100 },
                IndexInList = 3,
                Phi = _design.Model.Domain.Phi,
                IsOuter = true,
                IsBoundary = true,
                WhichBoundary = 0,
                IsFixedCharge = false,
            };
            #endregion

            #region Triangulation
            Triangle trng0 = new Triangle
            {
                IndexInList = 0,
                Nodes = new List<int> { 0, 1, 2 },
                Neighbors = new List<int> { -100, 1, -100 },
            };

            Triangle trng1 = new Triangle
            {
                IndexInList = 1,
                Nodes = new List<int> { 0, 2, 3 },
                Neighbors = new List<int> { -100, -100, 0 },
            };
            #endregion

            MagicalSolver.Mesh mesh =
                new MagicalSolver.Mesh(new List<PointStar> { P0, P1, P2, P3 },
                new List<Triangle> { trng0, trng1 });

            #region Add regions
            foreach (Region region in _design.Model.Regions)
            {
                bool regionIsComponent = region.Name.Split('-')[0] == "Component";
                bool regionIsDomain = region.LayerIndex == 0;

                if (regionIsComponent && !regionIsDomain)
                {
                    List<Point> samplePoints = GenerateUniformSample(region.Geometry);

                    MeshPoints.Add(samplePoints);

                    foreach (Point point in samplePoints)
                    {
                        mesh.MakeTriangle(new PointStar
                        {
                            Point = point,
                            NeighborTriangles = new List<int>(),
                            IsBoundary = true,
                            IsDielectricBoundary = region.BoundaryType == BoundaryType.Dielectric,
                            WhichBoundary = region.LayerIndex,
                            IsOuter = false,
                            Phi = region.Phi,
                        }, mesh.Triangles.Count - 1);
                    }
                }
            }
            #endregion

            #region Material assignment
            List<int> PECIndices = _design.Model.Regions
                .Where(r => r.BoundaryType == BoundaryType.PerfectElectricConductor ||
                r.Material.MaterialType == MaterialType.PerfectElectricConductor)
                .Select(r => r.LayerIndex).ToList();


            int inequalBoundaries = 0;
            foreach (Triangle triangle in mesh.Triangles)
            {
                bool isDomainTriangle = (triangle.Nodes[0] == 0) || (triangle.Nodes[0] == 1) ||
                    (triangle.Nodes[0] == 2) || (triangle.Nodes[0] == 3) ||
                    (triangle.Nodes[1] == 0) || (triangle.Nodes[1] == 1) ||
                    (triangle.Nodes[1] == 2) || (triangle.Nodes[1] == 3) ||
                    (triangle.Nodes[2] == 0) || (triangle.Nodes[2] == 1) ||
                    (triangle.Nodes[2] == 2) || (triangle.Nodes[2] == 3);


                foreach (int index in PECIndices)
                {
                    if ((mesh.PointStars[triangle.Nodes[0]].WhichBoundary == index) &&
                        (mesh.PointStars[triangle.Nodes[1]].WhichBoundary == index) &&
                        (mesh.PointStars[triangle.Nodes[2]].WhichBoundary == index))
                        triangle.IsPEC = true;
                }

                int boundayIndex0 = mesh.PointStars[triangle.Nodes[0]].WhichBoundary;
                int boundayIndex1 = mesh.PointStars[triangle.Nodes[1]].WhichBoundary;
                int boundayIndex2 = mesh.PointStars[triangle.Nodes[2]].WhichBoundary;


                bool inequalIndices = !((boundayIndex0 == boundayIndex1) && (boundayIndex1 == boundayIndex2));
                if (inequalIndices)
                {
                    // First problem: this scope is reached.
                    inequalBoundaries++;
                }
                else if (boundayIndex0 < _design.Model.Regions.Count)
                {
                    // Second problem: this scope is never reached.
                    triangle.EpsilonR = (float)_design.Model.Regions[boundayIndex0].Material.RelativePermittivity;
                    triangle.MuR = (float)_design.Model.Regions[boundayIndex0].Material.RelativePermeability;
                }
            }
            MessageBox.Show(inequalBoundaries.ToString(), mesh.Triangles.Count.ToString());
            #endregion

            // Actual solve region
            #region Solution to mesh
            int NumKC = 10;
            double[] KC = new double[NumKC];
            double[] numTrng = new double[NumKC];
            mesh.meshRefining(1, 5000);
            mesh.LAPLACEFilter();
            mesh.LAPLACEFilter();
            mesh.LAPLACEFilter();
            mesh.LAPLACEFilter();
            mesh.LAPLACEFilter();
            mesh.LAPLACEFilter();
            mesh.LAPLACEFilter();
            mesh.LAPLACEFilter();
            mesh.LAPLACEFilter();
            mesh.LAPLACEFilter();
            mesh.LAPLACEFilter();
            mesh.LAPLACEFilter();
            mesh.meshRefining(1, 5000);
            mesh.LAPLACEFilter();
            mesh.LAPLACEFilter();
            mesh.LAPLACEFilter();
            mesh.LAPLACEFilter();
            mesh.LAPLACEFilter();
            mesh.LAPLACEFilter();
            mesh.LAPLACEFilter();
            mesh.LAPLACEFilter();
            mesh.LAPLACEFilter();
            mesh.LAPLACEFilter();
            mesh.LAPLACEFilter();
            mesh.LAPLACEFilter();
            mesh.LAPLACEFilter();
            mesh.LAPLACEFilter();
            mesh.LAPLACEFilter();
            #endregion

            #region Update MeshLines
            MeshPoints = new List<List<Point>>();
            mesh.Triangles.ForEach(triangle =>
            {
                MeshPoints.Add(new List<Point>
                {
                    mesh.PointStars[triangle.Nodes[0]].Point,
                    mesh.PointStars[triangle.Nodes[1]].Point,
                    mesh.PointStars[triangle.Nodes[2]].Point,

                });
            });
            OnMeshPointsUpdated();
            #endregion

            #region Get potential data
            //double[,] potentialPhi = GetPotential(rectangle, mesh);
            //Potential = GetPotential(rectangle, mesh, PotentialXSize, PotentialYSize);
            //IsPotentialValid = true;

            OnSolutionChanged();
            #endregion
        }

        private Dictionary<Point, double> GetPotential(RectangleGeometry rectangle, MagicalSolver.Mesh mesh, int potentialXSize, int potentialYSize)
        {
            Dictionary<Point, double> potential = new Dictionary<Point, double>();

            Point topLeft = rectangle.Rect.TopLeft;

            double width = rectangle.Rect.Width;
            double height = rectangle.Rect.Height;

            // Steps in each direction
            double dx = 2 * (width + height) / potentialXSize;
            double dy = 2 * (width + height) / potentialYSize;

            for (int i = 0; i < potentialXSize; i++)
            {
                for (int j = 0; j < potentialYSize; j++)
                {
                    Point point = topLeft + new Vector(i * dx, j * dy);
                    PointStar newNode = new PointStar
                    {
                        Point = point,
                        NeighborTriangles = new List<int>(),
                        IsBoundary = true,
                        WhichBoundary = 1,
                        IsOuter = false,
                        Phi = 0,
                    };

                    Triangle triangle = new Triangle(mesh.WalkSearch(1, newNode));
                    double Area = mesh.ElementArea(triangle.IndexInList);

                    double X1 = mesh.PointStars[triangle.Nodes[0]].Point.X;
                    double X2 = mesh.PointStars[triangle.Nodes[1]].Point.X;
                    double X3 = mesh.PointStars[triangle.Nodes[2]].Point.X;
                    double Y1 = mesh.PointStars[triangle.Nodes[0]].Point.Y;
                    double Y2 = mesh.PointStars[triangle.Nodes[1]].Point.Y;
                    double Y3 = mesh.PointStars[triangle.Nodes[2]].Point.Y;

                    double alpha1 = (X2 * Y3 - X3 * Y2 + (Y2 - Y3) * point.X + (X3 - X2) * point.Y) / (2 * Area);
                    double alpha2 = (X3 * Y1 - X1 * Y3 + (Y3 - Y1) * point.X + (X1 - X3) * point.Y) / (2 * Area);
                    double alpha3 = (X1 * Y2 - X2 * Y1 + (Y1 - Y2) * point.X + (X2 - X1) * point.Y) / (2 * Area);

                    double phi1 = mesh.PointStars[triangle.Nodes[0]].Phi;
                    double phi2 = mesh.PointStars[triangle.Nodes[1]].Phi;
                    double phi3 = mesh.PointStars[triangle.Nodes[2]].Phi;

                    double partialPotential = alpha1 * phi1 + alpha2 * phi2 + alpha3 * phi3;

                    List<int> outerBoxNodes = new List<int> { 0, 1, 2, 3 };

                    bool isPartOfOuterBox = outerBoxNodes.Intersect(triangle.Nodes).Count() > 0;

                    if (isPartOfOuterBox)
                        partialPotential = 0;

                    potential.Add(point, partialPotential);
                }
            }

            return potential;
        }

        /// <summary>
        /// Calculates the scalar potential within the domain
        /// </summary>
        /// <param name="rectangle"></param>
        /// <param name="mesh"></param>
        /// <returns>potential</returns>
        private double[,] GetPotential(RectangleGeometry rectangle, MagicalSolver.Mesh mesh)
        {
            Point topLeft = rectangle.Rect.TopLeft;

            double width = rectangle.Rect.Width;
            double height = rectangle.Rect.Height;

            // Steps in each direction
            double dx = 2 * (width + height) / unifomSamplesSize;
            double dy = 2 * (width + height) / unifomSamplesSize;

            // Size of array
            int xSize = Convert.ToInt32(Math.Ceiling(width / dx));
            int ySize = Convert.ToInt32(Math.Ceiling(height / dy));

            double[,] potential = new double[xSize, ySize];

            for (int i = 0; i < xSize; i++)
            {
                for (int j = 0; j < ySize; j++)
                {
                    Point point = topLeft + new Vector(i * dx, j * dy);
                    PointStar newNode = new PointStar
                    {
                        Point = point,
                        NeighborTriangles = new List<int>(),
                        IsBoundary = true,
                        WhichBoundary = 1,
                        IsOuter = false,
                        Phi = 0,
                    };

                    Triangle triangle = new Triangle(mesh.WalkSearch(1, newNode));
                    double Area = mesh.ElementArea(triangle.IndexInList);

                    double X1 = mesh.PointStars[triangle.Nodes[0]].Point.X;
                    double X2 = mesh.PointStars[triangle.Nodes[1]].Point.X;
                    double X3 = mesh.PointStars[triangle.Nodes[2]].Point.X;
                    double Y1 = mesh.PointStars[triangle.Nodes[0]].Point.Y;
                    double Y2 = mesh.PointStars[triangle.Nodes[1]].Point.Y;
                    double Y3 = mesh.PointStars[triangle.Nodes[2]].Point.Y;

                    double alpha1 = (X2 * Y3 - X3 * Y2 + (Y2 - Y3) * point.X + (X3 - X2) * point.Y) / (2 * Area);
                    double alpha2 = (X3 * Y1 - X1 * Y3 + (Y3 - Y1) * point.X + (X1 - X3) * point.Y) / (2 * Area);
                    double alpha3 = (X1 * Y2 - X2 * Y1 + (Y1 - Y2) * point.X + (X2 - X1) * point.Y) / (2 * Area);

                    double phi1 = mesh.PointStars[triangle.Nodes[0]].Phi;
                    double phi2 = mesh.PointStars[triangle.Nodes[1]].Phi;
                    double phi3 = mesh.PointStars[triangle.Nodes[2]].Phi;

                    potential[i, j] = alpha1 * phi1 + alpha2 * phi2 + alpha3 * phi3;

                    List<int> outerBoxNodes = new List<int> { 0, 1, 2, 3 };

                    bool isPartOfOuterBox = outerBoxNodes.Intersect(triangle.Nodes).Count() > 0;

                    if (isPartOfOuterBox)
                        potential[i, j] = 0;
                }
            }

            return potential;
        }

        private void PhiOnMeshGrid(int NumX, int NumY, MagicalSolver.Mesh mesh, out double[,] Phi)
        {
            double dx = 495.0 / (NumX - 1);
            double dy = 495.0 / (NumY - 1);
            double x0 = 5.0;
            double y0 = 5.0;
            Phi = new double[NumY, NumX];
            for (int i = 0; i < NumY; i++)
            {
                for (int j = 0; j < NumX; j++)
                {
                    double x = x0 + j * dx;
                    double y = y0 + i * dy;
                    Point newnode = new Point();
                    newnode.X = x;
                    newnode.Y = y;
                    PointStar newNode = new PointStar(newnode, new List<int>());
                    newNode.IsBoundary = true;
                    newNode.WhichBoundary = 1;
                    newNode.IsOuter = false;
                    newNode.Phi = 0;
                    Triangle trng = new Triangle(mesh.WalkSearch(1, newNode));
                    double A = mesh.ElementArea(trng.IndexInList);
                    double X1 = mesh.PointStars[trng.Nodes[0]].Point.X;
                    double X2 = mesh.PointStars[trng.Nodes[1]].Point.X;
                    double X3 = mesh.PointStars[trng.Nodes[2]].Point.X;
                    double Y1 = mesh.PointStars[trng.Nodes[0]].Point.Y;
                    double Y2 = mesh.PointStars[trng.Nodes[1]].Point.Y;
                    double Y3 = mesh.PointStars[trng.Nodes[2]].Point.Y;
                    double alpha1 = (X2 * Y3 - X3 * Y2 + (Y2 - Y3) * x + (X3 - X2) * y) / (2 * A);
                    double alpha2 = (X3 * Y1 - X1 * Y3 + (Y3 - Y1) * x + (X1 - X3) * y) / (2 * A);
                    double alpha3 = (X1 * Y2 - X2 * Y1 + (Y1 - Y2) * x + (X2 - X1) * y) / (2 * A);
                    double Phi1 = mesh.PointStars[trng.Nodes[0]].Phi;
                    double Phi2 = mesh.PointStars[trng.Nodes[1]].Phi;
                    double Phi3 = mesh.PointStars[trng.Nodes[2]].Phi;
                    Phi[i, j] = alpha1 * Phi1 + alpha2 * Phi2 + alpha3 * Phi3;
                    bool check = trng.Nodes[0] == 0 || trng.Nodes[0] == 1 || trng.Nodes[0] == 2 || trng.Nodes[0] == 3 ||
                        trng.Nodes[1] == 0 || trng.Nodes[1] == 1 || trng.Nodes[1] == 2 || trng.Nodes[1] == 3 ||
                        trng.Nodes[2] == 0 || trng.Nodes[2] == 1 || trng.Nodes[2] == 2 || trng.Nodes[2] == 3;
                    if (check)
                    {
                        Phi[i, j] = 0;
                    }
                }
            }
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

        private void OnSolutionChanged()
        {
            SolutionChanged?.Invoke(this, new EventArgs());
        }

        private void OnDesignChanged(object? sender, EventArgs e)
        {
            MeshPoints.Clear();
            Potential.Clear();
            IsPotentialValid = false;
        }

        private void OnDesignReset(object? sender, System.EventArgs e)
        {
            ResetMesh();
        }

        private void ResetMesh()
        {
            MeshPoints.Clear();
            Potential.Clear();
            IsPotentialValid = false;
        }
    }
}
