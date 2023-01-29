using System.Collections.Generic;

namespace AdaptiveFEM.MagicalSolver
{
    public class Triangle
    {
        public List<int> Nodes = new List<int>();
        public List<int> Neighbors = new List<int>();
        /// <summary>
        /// ///////////////////
        /// Number -100 is used for NULL neighbors when the triangle is on the outer boundary. 
        /// </summary>
        public float EpsilonR { get; set; }
        public float MuR { get; set; }
        public bool IsPEC { get; set; }
        public bool IsPMC { get; set; }
        public int IndexInList { get; set; }
        public Triangle(List<int> nodes, List<int> neighbors, float epsilonR, float muR, bool isPEC, bool isPMC, int indexInList)
        {
            Nodes = nodes;
            Neighbors = neighbors;
            EpsilonR = epsilonR;
            MuR = muR;
            IsPEC = isPEC;
            IsPMC = isPMC;
            IndexInList = indexInList;
        }
        public Triangle()
        {
            Nodes = new List<int>();
            Neighbors = new List<int>();
            EpsilonR = 1;
            MuR = 1;
            IsPEC = false;
            IsPMC = false;
            IndexInList = -1;
        }
        public Triangle(Triangle trng)
        {
            Nodes = new List<int>(trng.Nodes);
            Neighbors = new List<int>(trng.Neighbors);
            EpsilonR = trng.EpsilonR;
            MuR = trng.MuR;
            IsPEC = trng.IsPEC;
            IsPMC = trng.IsPMC;
            IndexInList = trng.IndexInList;
        }
    }
}
