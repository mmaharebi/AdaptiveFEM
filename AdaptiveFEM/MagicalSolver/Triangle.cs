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
        public float epsilonR { get; set; }
        public float muR { get; set; }
        public bool IsPEC { get; set; }
        public bool IsPMC { get; set; }
        public int indexInList { get; set; }
        public Triangle(List<int> nodes, List<int> neighbors, float E_R, float M_R, bool isPEC, bool isPMC, int index)
        {
            Nodes = nodes;
            Neighbors = neighbors;
            this.epsilonR = E_R;
            this.muR = M_R;
            this.IsPEC = isPEC;
            this.IsPMC = isPMC;
            this.indexInList = index;
        }
        public Triangle()
        {
            Nodes = new List<int>();
            Neighbors = new List<int>();
            epsilonR = 1;
            muR = 1;
            IsPEC = false;
            IsPMC = false;
            indexInList = -1;
        }
        public Triangle(Triangle trng)
        {
            Nodes = new List<int>(trng.Nodes);
            Neighbors = new List<int>(trng.Neighbors);
            epsilonR = trng.epsilonR;
            muR = trng.muR;
            IsPEC = trng.IsPEC;
            IsPMC = trng.IsPMC;
            indexInList = trng.indexInList;
        }
    }
}
