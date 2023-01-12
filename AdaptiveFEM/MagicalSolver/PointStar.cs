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
        private Point myPoint;
        public Point MyPoint
        {
            get { return myPoint; }
            set { myPoint = value; }
        }
        public double Phi { get; set; }
        public List<int> neighborTriangles = new List<int>();
        public bool IsBoundary { get; set; }
        public bool IsFixedCharge { get; set; }
        public bool IsDielectricBoundary { get; set; }
        public bool IsOuter { get; set; }
        public int indexInList { get; set; }
        public int whichBoundary { get; set; }
        public PointStar(Point myPoint, List<int> neighborTriangles)
        {
            this.myPoint = myPoint;
            this.neighborTriangles = neighborTriangles;
        }

        public PointStar(Point myPoint, List<int> neighborTriangles, bool isBoundary)
        {
            this.myPoint = myPoint;
            this.neighborTriangles = neighborTriangles;
            IsBoundary = isBoundary;
        }
    }
}
