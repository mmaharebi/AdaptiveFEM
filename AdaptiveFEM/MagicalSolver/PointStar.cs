using System.Collections.Generic;
using System.Windows;

/// <summary>
/// //////////////////////////////////////////////////
/// This class is for points which have some extra features (star!) in our problem.
/// </summary>

namespace AdaptiveFEM.MagicalSolver
{
    public class PointStar
    {
        private Point _point;
        public Point Point
        {
            get { return _point; }
            set { _point = value; }
        }
        public double Phi { get; set; }
        public List<int> NeighborTriangles = new List<int>();
        public bool IsBoundary { get; set; }
        public bool IsFixedCharge { get; set; }
        public bool IsDielectricBoundary { get; set; }
        public bool IsOuter { get; set; }
        public int IndexInList { get; set; }
        public int WhichBoundary { get; set; }

        public PointStar() { }

        public PointStar(Point point, List<int> neighborTriangles)
        {
            _point = point;
            NeighborTriangles = neighborTriangles;
        }

        public PointStar(Point myPoint, List<int> neighborTriangles, bool isBoundary)
        {
            _point = myPoint;
            NeighborTriangles = neighborTriangles;
            IsBoundary = isBoundary;
        }
    }
}
