using System;
using System.Collections.Generic;
using System.Windows;

/////////////////////////////////////////// ATTENTION //////////////////////////////////////////////
/// Before Running the code, you should add the ALGLIB library. Its related files 
/// can be reached in "Other useful materials and references" folder.
/// THE Y AXIS HAS OPOSITE DIRECTION HERE.
///////////////////////////////////////////////////////////////////////////////////////////////////

namespace AdaptiveFEM.MagicalSolver
{
    public class Mesh
    {
        /// <summary>
        /// ///////////////////////////
        /// These lists are the lists of the point and trangles in order of adding. 
        /// //////////////////////////
        /// </summary>
        public List<PointStar> PointStars { get; set; }
        public List<Triangle> Triangles { get; set; }
        /////////////////////////////////////
        /// This is the constructor of the mesh
        /// /////////////////////////////////
        public Mesh(List<PointStar> pointStars, List<Triangle> triangles)
        {
            PointStars = pointStars;
            Triangles = triangles;
        }

        /////////////////////////////////////
        /// Length of the edges in a triangle with index trngIndex in the Triangle List, will be returned.
        ////////////////////////////////////
        public List<double> Length(int trngIndex)
        {
            Triangle trng = this.Triangles[trngIndex];
            var lenList = new List<double>();
            for (int i = 0; i < 3; i++)
            {
                double X1 = PointStars[trng.Nodes[i]].MyPoint.X;
                double X2 = PointStars[trng.Nodes[(i + 1) % 3]].MyPoint.X;
                double Y1 = PointStars[trng.Nodes[i]].MyPoint.Y;
                double Y2 = PointStars[trng.Nodes[(i + 1) % 3]].MyPoint.Y;
                lenList.Add(Math.Sqrt(Math.Pow(X1 - X2, 2) + Math.Pow(Y1 - Y2, 2)));
            }

            return lenList;
        }
        //////////////////////
        //// beta = minLength/maxLength ratio in  a triangle
        /////////////////////
        public double beta(int trngIndex) // min length to max ratio
        {
            double max = this.Length(trngIndex)[0];
            double min = this.Length(trngIndex)[0];
            foreach (var L in this.Length(trngIndex))
            {
                max = Math.Max(max, L);
                min = Math.Min(min, L);
            }
            return (min / max);
        }
        //////////////////////
        //// Calculation of the area of a triangle with index trngIndex in the list.
        /////////////////////
        public double elementArea(int trngIndex)
        {
            Triangle trng = this.Triangles[trngIndex];
            double X1 = PointStars[trng.Nodes[0]].MyPoint.X;
            double X2 = PointStars[trng.Nodes[1]].MyPoint.X;
            double X3 = PointStars[trng.Nodes[2]].MyPoint.X;
            double Y1 = PointStars[trng.Nodes[0]].MyPoint.Y;
            double Y2 = PointStars[trng.Nodes[1]].MyPoint.Y;
            double Y3 = PointStars[trng.Nodes[2]].MyPoint.Y;
            double A = ((X1 * Y2 - X2 * Y1) + (X3 * Y1 - X1 * Y3) + (X2 * Y3 - X3 * Y2)) / 2;
            return A;
        }
        ////////////////////////////////
        /// this function find the triangle which contain a particular node using the presented method in the THESIS
        ////////////////////////////////
        public Triangle walkSearch(int lastTriangleNum, PointStar newNode)
        {
            Triangle trng = new Triangle();
            //////////////////////////////////
            // checking that the new node is in the meshing region
            if ((newNode.MyPoint.X > this.PointStars[1].MyPoint.X) || (newNode.MyPoint.X < this.PointStars[0].MyPoint.X) ||
                (newNode.MyPoint.Y > this.PointStars[2].MyPoint.Y) || (newNode.MyPoint.Y < this.PointStars[0].MyPoint.Y))
            {
                trng.epsilonR = 1;
                trng.indexInList = -100;
                trng.IsPEC = false;
                trng.IsPMC = false;
                trng.muR = 1;
                trng.Neighbors = new List<int>() { -1, -1, -1 };
                trng.Nodes = new List<int>() { -1, -1, -1 };
                return trng;
            }
            //////////////////////////////////
            Triangle lastTriangle = this.Triangles[lastTriangleNum];
            bool searchOK = false;
            double[,] middlePoints = new double[3, 2];
            while (!searchOK)
            {
                double[,] node = new double[3, 2];
                for (int i = 0; i < 3; ++i)
                {
                    node[i, 0] = this.PointStars[lastTriangle.Nodes[i]].MyPoint.X;
                    node[i, 1] = this.PointStars[lastTriangle.Nodes[i]].MyPoint.Y;
                }
                for (int j = 0; j < 3; ++j)
                {
                    middlePoints[j, 0] = (node[j, 0] + node[(j + 1) % 3, 0]) / 2;
                    middlePoints[j, 1] = (node[j, 1] + node[(j + 1) % 3, 1]) / 2;
                }
                double[] Vector01 = new double[2] { node[1, 0] - node[0, 0], node[0, 1] - node[1, 1] };
                // Our coordinate is (x,-y). So I use a "-" for y component to get true vector.
                double Norm01 = Math.Sqrt(Vector01[0] * Vector01[0] + Vector01[1] * Vector01[1]);
                Vector01[0] /= Norm01;
                Vector01[1] /= Norm01;
                double[] Vector12 = new double[2] { node[2, 0] - node[1, 0], node[1, 1] - node[2, 1] };
                double Norm12 = Math.Sqrt(Vector12[0] * Vector12[0] + Vector12[1] * Vector12[1]);
                Vector12[0] /= Norm12;
                Vector12[1] /= Norm12;
                double[] Vector20 = new double[2] { node[0, 0] - node[2, 0], node[2, 1] - node[0, 1] };
                double Norm20 = Math.Sqrt(Vector20[0] * Vector20[0] + Vector20[1] * Vector20[1]);
                Vector20[0] /= Norm20;
                Vector20[1] /= Norm20;
                // 90 deg rotation matrix is [0 -1;1 0]
                double[] Orthogonal0 = new double[2] { -Vector01[1], Vector01[0] };
                double[] Orthogonal1 = new double[2] { -Vector12[1], Vector12[0] };
                double[] Orthogonal2 = new double[2] { -Vector20[1], Vector20[0] };
                double[] middle2Node0 = new double[2] { newNode.MyPoint.X - middlePoints[0, 0], -newNode.MyPoint.Y + middlePoints[0, 1] };
                double NormMid0 = Math.Sqrt(middle2Node0[0] * middle2Node0[0] + middle2Node0[1] * middle2Node0[1]);
                double[] middle2Node1 = new double[2] { newNode.MyPoint.X - middlePoints[1, 0], -newNode.MyPoint.Y + middlePoints[1, 1] };
                double NormMid1 = Math.Sqrt(middle2Node1[0] * middle2Node1[0] + middle2Node1[1] * middle2Node1[1]);
                double[] middle2Node2 = new double[2] { newNode.MyPoint.X - middlePoints[2, 0], -newNode.MyPoint.Y + middlePoints[2, 1] };
                double NormMid2 = Math.Sqrt(middle2Node2[0] * middle2Node2[0] + middle2Node2[1] * middle2Node2[1]);
                double[] Products = new double[3]
                {
                    (middle2Node0[0] * Orthogonal0[0] + middle2Node0[1] * Orthogonal0[1]) / NormMid0,
                    (middle2Node1[0] * Orthogonal1[0] + middle2Node1[1] * Orthogonal1[1]) / NormMid1,
                    (middle2Node2[0] * Orthogonal2[0] + middle2Node2[1] * Orthogonal2[1]) / NormMid2 };
                double maxProduct = Products[0];
                for (int i = 0; i < 3; i++)
                {
                    if (maxProduct <= Products[i])
                    {
                        maxProduct = Products[i];
                    }
                }
                //MessageBox.Show("0: " + Products[0].ToString() + " 1: " + Products[1].ToString() + " 2: " + Products[2].ToString()); 
                //////////////////////////////////////////////////////
                /// these conditions avoids each problem like point on an common edge of two triangles and other geometrical
                /// issues. note that I assume nodes has different (x,y)s.
                /// //////////////////////////////////////////////////
                if (maxProduct == Products[0] && maxProduct > 0)
                {
                    lastTriangle = Triangles[lastTriangle.Neighbors[2]];
                }
                else if (maxProduct == Products[1] && maxProduct > 0)
                {
                    lastTriangle = Triangles[lastTriangle.Neighbors[0]];
                }
                else if (maxProduct == Products[2] && maxProduct > 0)
                {
                    lastTriangle = Triangles[lastTriangle.Neighbors[1]];
                }
                else
                {
                    searchOK = true;
                }
            }
            return lastTriangle;
        }

        ////////////////////////////////////// 
        /// following function determines that the neighbor of a triangle in placed completely on its Peripheral circle or not.
        /// /////////////////////////////////
        public bool inCircle(int triangleNum, int neighborNum)
        {
            if (Triangles[triangleNum].Neighbors[neighborNum] == -100)
            {
                return false;
            }
            else
            {
                Triangle trng = this.Triangles[triangleNum];
                int p1, p2, p3, p4;
                p1 = trng.Nodes[neighborNum];
                p2 = trng.Nodes[(neighborNum + 1) % 3];
                p3 = trng.Nodes[(neighborNum + 2) % 3];
                p4 = -p2 - p3;
                for (int j = 0; j < 3; ++j)
                {
                    p4 = p4 + Triangles[trng.Neighbors[neighborNum]].Nodes[j];
                }

                if (p1 == p2 || p1 == p3 || p1 == p4 || p2 == p3 || p2 == p4 || p3 == p4)
                {
                    return false;
                }
                else
                {
                    double[] Vector21 = new double[2] {
                    PointStars[p2].MyPoint.X - PointStars[p1].MyPoint.X,
                    PointStars[p2].MyPoint.Y - PointStars[p1].MyPoint.Y };
                    double[] Vector31 = new double[2] {
                    PointStars[p3].MyPoint.X - PointStars[p1].MyPoint.X,
                    PointStars[p3].MyPoint.Y - PointStars[p1].MyPoint.Y };
                    double[] Vector24 = new double[2] {
                    PointStars[p2].MyPoint.X - PointStars[p4].MyPoint.X,
                    PointStars[p2].MyPoint.Y - PointStars[p4].MyPoint.Y };
                    double[] Vector34 = new double[2] {
                    PointStars[p3].MyPoint.X - PointStars[p4].MyPoint.X,
                    PointStars[p3].MyPoint.Y - PointStars[p4].MyPoint.Y };
                    double Norm21 = Math.Sqrt(Vector21[0] * Vector21[0] + Vector21[1] * Vector21[1]);
                    double Norm31 = Math.Sqrt(Vector31[0] * Vector31[0] + Vector31[1] * Vector31[1]);
                    double Norm24 = Math.Sqrt(Vector24[0] * Vector24[0] + Vector24[1] * Vector24[1]);
                    double Norm34 = Math.Sqrt(Vector34[0] * Vector34[0] + Vector34[1] * Vector34[1]);
                    double Angle1 = Math.Acos((Vector21[0] * Vector31[0] + Vector21[1] * Vector31[1]) / (Norm21 * Norm31));
                    double Angle2 = Math.Acos((Vector24[0] * Vector34[0] + Vector24[1] * Vector34[1]) / (Norm24 * Norm34));
                    //MessageBox.Show((Angle1 + Angle2).ToString());
                    return (Angle1 + Angle2 > Math.PI);
                }
            }
        }

        ////////////////////////////////////// 
        /// following function uses the last function to check the Delanauy property 
        /////////////////////////////////////
        bool DelaunayCheck(int triangleNum, int neighborNum)
        {
            if (Triangles[triangleNum].Neighbors[neighborNum] == -100)
            {
                return false;
            }
            else
            {
                Triangle trng = this.Triangles[triangleNum];
                int[] neighborNodes = new int[3] {
                    Triangles[trng.Neighbors[neighborNum]].Nodes[0],
                    Triangles[trng.Neighbors[neighborNum]].Nodes[1],
                    Triangles[trng.Neighbors[neighborNum]].Nodes[2]};
                bool check = false;
                foreach (int k in neighborNodes)
                {
                    if (k != Triangles[triangleNum].Nodes[(neighborNum + 1) % 3] &&
                        k != Triangles[triangleNum].Nodes[(neighborNum + 2) % 3] &&
                        Triangles[triangleNum].Neighbors[neighborNum] != -100)
                    {
                        check = inCircle(triangleNum, neighborNum);
                    }
                }
                return check;
            }
        }
        ////////////////////////////////////// 
        /// following function uses the last function to swap lines and make the mesh a delanauy one. 
        /////////////////////////////////////
        void swap(int triangleNum, int neighborNum)
        {
            if (triangleNum == -100)
            {
                return;
            }
            int triangleNum2 = this.Triangles[triangleNum].Neighbors[neighborNum];
            if (triangleNum2 == -100)
            {
                return;
            }
            // Finding p1,2,3,4. see report. 
            int p1, p2, p3, p4;
            p1 = Triangles[triangleNum].Nodes[neighborNum];
            p2 = Triangles[triangleNum].Nodes[(neighborNum + 1) % 3];
            p3 = Triangles[triangleNum].Nodes[(neighborNum + 2) % 3];
            p4 = -p2 - p3;
            for (int j = 0; j < 3; ++j)
            {
                p4 = p4 + Triangles[triangleNum2].Nodes[j];
            }
            // we neew p1,2,3,4 places in triangles 
            int[] pPlace1 = new int[3] { neighborNum, (neighborNum + 1) % 3, (neighborNum + 2) % 3 };
            // p1,p2,p3 place in triangleNum
            int[] pPlace2 = new int[3];
            // p2,p4,p3 place in triangleNum2
            for (int i = 0; i < 3; ++i)
            {
                if (p2 == this.Triangles[triangleNum2].Nodes[i])
                {
                    pPlace2[0] = i;
                    pPlace2[1] = (i + 1) % 3;
                    pPlace2[2] = (i + 2) % 3;
                }
            }
            // Finding shared neighbors 
            int alpha, beta, gamma, delta;
            alpha = Triangles[triangleNum].Neighbors[pPlace1[2]];
            beta = Triangles[triangleNum2].Neighbors[pPlace2[2]];
            gamma = Triangles[triangleNum2].Neighbors[pPlace2[0]];
            delta = Triangles[triangleNum].Neighbors[pPlace1[1]];
            int[] neighbors = new int[2] { beta, delta };
            int[] triangles = new int[2] { triangleNum2, triangleNum };
            Triangle newTriangle1 = new Triangle(new List<int>() { p1, p2, p4 }, new List<int>() { beta, triangleNum2, alpha },
                Triangles[triangleNum].epsilonR, Triangles[triangleNum].muR,
                Triangles[triangleNum].IsPEC, Triangles[triangleNum].IsPMC,
                triangleNum);
            Triangle newTriangle2 = new Triangle(new List<int>() { p4, p3, p1 }, new List<int>() { delta, triangleNum, gamma },
                Triangles[triangleNum2].epsilonR, Triangles[triangleNum2].muR,
                Triangles[triangleNum2].IsPEC, Triangles[triangleNum2].IsPMC,
                triangleNum2);
            // Updating beta and delta neighbors. 
            for (int j = 0; j < 2; j++)
            {
                int newNeighbor = 0;
                if (j == 0)
                {
                    if (neighbors[j] == -100)
                    {
                        continue;
                    }
                    newNeighbor = triangleNum;
                }
                else if (j == 1)
                {
                    if (neighbors[j] == -100)
                    {
                        continue;
                    }
                    newNeighbor = triangleNum2;
                }
                if (newNeighbor != -100)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        int neighbor = Triangles[neighbors[j]].Neighbors[i];
                        if (neighbor == triangles[j])
                        {
                            if (i == 0)
                            {
                                Triangles[neighbors[j]].Neighbors = new List<int>(){ newNeighbor,
                                    Triangles[neighbors[j]].Neighbors[1],
                                    Triangles[neighbors[j]].Neighbors[2]};
                            }
                            else if (i == 1)
                            {
                                Triangles[neighbors[j]].Neighbors = new List<int>(){Triangles[neighbors[j]].Neighbors[0],
                                    newNeighbor,
                                    Triangles[neighbors[j]].Neighbors[2] };
                            }
                            else if (i == 2)
                            {
                                Triangles[neighbors[j]].Neighbors = new List<int>() {Triangles[neighbors[j]].Neighbors[0],
                                    Triangles[neighbors[j]].Neighbors[1],
                                    newNeighbor };
                            }
                        }
                    }
                }
            }
            // Updating p1,2,3,4 neighbors
            PointStars[p1].neighborTriangles.Add(triangleNum2);
            int index1 = PointStars[p2].neighborTriangles.FindIndex(0, PointStars[p2].neighborTriangles.Count,
                a => a.Equals(triangleNum2));
            PointStars[p2].neighborTriangles.RemoveAt(index1);
            int index2 = PointStars[p3].neighborTriangles.FindIndex(0, PointStars[p3].neighborTriangles.Count,
                a => a.Equals(triangleNum));
            PointStars[p3].neighborTriangles.RemoveAt(index2);
            PointStars[p4].neighborTriangles.Add(triangleNum);
            // Updating triangles' list
            Triangles[triangleNum] = newTriangle1;
            Triangles[triangleNum2] = newTriangle2;
        }
        ////////////////////////////////////// 
        /// Adding a node without swapping bad lines. in this function, neighbors of triangles and nodes will be updated.  
        /////////////////////////////////////
        public int addNode(PointStar newNode, int last)
        {
            // This function add a new node to the mesh, without any swapping or epsilon_r considerations. 
            PointStar lastNode = this.PointStars[PointStars.Count - 1];
            // First step: Finding the triangle that the node is in it. 
            Triangle inTriangle = new Triangle(walkSearch(last, newNode));
            int lastTriangle = inTriangle.indexInList;
            //MessageBox.Show(lastTriangle.ToString());
            // Second step: Creating two new triangles 
            Triangle newTriangle1 = new Triangle();
            newTriangle1.Nodes = new List<int> { inTriangle.Nodes[0], inTriangle.Nodes[1], PointStars.Count };
            newTriangle1.Neighbors = new List<int> { Triangles.Count + 1, inTriangle.indexInList, inTriangle.Neighbors[2] };
            newTriangle1.indexInList = Triangles.Count;
            newTriangle1.epsilonR = inTriangle.epsilonR;
            newTriangle1.muR = inTriangle.muR;
            newTriangle1.IsPEC = inTriangle.IsPEC;
            newTriangle1.IsPMC = inTriangle.IsPMC;

            Triangle newTriangle2 = new Triangle();
            newTriangle2.Nodes = new List<int> { PointStars.Count, inTriangle.Nodes[1], inTriangle.Nodes[2] };
            newTriangle2.Neighbors = new List<int> { inTriangle.Neighbors[0], inTriangle.indexInList, Triangles.Count };
            newTriangle2.indexInList = Triangles.Count + 1;
            newTriangle2.epsilonR = inTriangle.epsilonR;
            newTriangle2.muR = inTriangle.muR;
            newTriangle2.IsPEC = inTriangle.IsPEC;
            newTriangle2.IsPMC = inTriangle.IsPMC;

            int[] neighbors = new int[3] { inTriangle.Neighbors[0], inTriangle.Neighbors[1], inTriangle.Neighbors[2] };

            // Updating inTriangle neighbors.
            Triangles[inTriangle.indexInList].Neighbors = new List<int>() { Triangles.Count + 1, inTriangle.Neighbors[1], Triangles.Count };
            Triangles[inTriangle.indexInList].Nodes = new List<int> { inTriangle.Nodes[0], PointStars.Count, inTriangle.Nodes[2] };

            // Updating inTriangle neighbors' neighbors
            for (int j = 0; j < 3; ++j)
            {
                int newNeighbor = 0;
                if (j == 0)
                {
                    newNeighbor = Triangles.Count + 1;
                }
                else if (j == 1)
                {
                    newNeighbor = inTriangle.indexInList;
                }
                else if (j == 2)
                {
                    newNeighbor = Triangles.Count;
                }
                if (neighbors[j] != -100)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        int neighbor = Triangles[neighbors[j]].Neighbors[i];
                        if (neighbor == inTriangle.indexInList)
                        {
                            if (i == 0)
                            {
                                Triangles[neighbors[j]].Neighbors = new List<int>(){newNeighbor,
                                    Triangles[neighbors[j]].Neighbors[1],
                                    Triangles[neighbors[j]].Neighbors[2] };
                            }
                            else if (i == 1)
                            {
                                Triangles[neighbors[j]].Neighbors = new List<int>() {Triangles[neighbors[j]].Neighbors[0],
                                    newNeighbor,
                                    Triangles[neighbors[j]].Neighbors[2] };
                            }
                            else if (i == 2)
                            {
                                Triangles[neighbors[j]].Neighbors = new List<int>(){Triangles[neighbors[j]].Neighbors[0],
                                    Triangles[neighbors[j]].Neighbors[1],
                                    newNeighbor };
                            }
                        }
                    }
                }
            }
            // Now, we should update newNode Neighbors
            newNode.neighborTriangles = new List<int>() { lastTriangle, Triangles.Count, Triangles.Count + 1 };
            newNode.indexInList = PointStars.Count;
            //MessageBox.Show(inTriangle.Nodes[1].ToString());
            // Now, we should update node 0,1,2 Neighbors
            PointStars[inTriangle.Nodes[0]].neighborTriangles.Add(Triangles.Count);
            PointStars[inTriangle.Nodes[2]].neighborTriangles.Add(Triangles.Count + 1);
            int index = PointStars[inTriangle.Nodes[1]].neighborTriangles.FindIndex(0,
                PointStars[inTriangle.Nodes[1]].neighborTriangles.Count,
                a => a.Equals(lastTriangle));
            PointStars[inTriangle.Nodes[1]].neighborTriangles[index] = Triangles.Count;
            PointStars[inTriangle.Nodes[1]].neighborTriangles.Add(Triangles.Count + 1);
            // Adding new elements to their lists
            PointStars.Add(newNode);
            Triangles.Add(newTriangle1);
            Triangles.Add(newTriangle2);
            return lastTriangle;
        }
        ////////////////////////////////////// 
        /// Adding a node with swapping bad lines in doubtful triangles. a doubtful triangle is the new triangle in the list 
        /// and its neighbors and its neighbors' neighbors.
        /// in this function, neighbors of triangles and nodes will be updated.  
        /////////////////////////////////////
        public void makeTriangle(PointStar newNode, int last)
        {
            int lastTriangle = addNode(newNode, last);
            List<int> doubtfulTriangles = new List<int>();
            List<int> doubtfulTrianglesNeighborNum = new List<int>();
            doubtfulTriangles.Add(lastTriangle);
            doubtfulTrianglesNeighborNum.Add(1);
            doubtfulTriangles.Add(Triangles.Count - 2);
            doubtfulTrianglesNeighborNum.Add(2);
            doubtfulTriangles.Add(Triangles.Count - 1);
            doubtfulTrianglesNeighborNum.Add(0);
            int k = 0;
            while (k < doubtfulTriangles.Count)
            {
                //int p2 = Triangles[doubtfulTriangles[k]].Nodes[(doubtfulTrianglesNeighborNum[k] + 1) % 3];
                //int p3 = Triangles[doubtfulTriangles[k]].Nodes[(doubtfulTrianglesNeighborNum[k] + 2) % 3];
                // p2 and p3 are defined similar to swap function's definition of them
                //bool boundaryCheck = Nodes[p2].isBoundary1() && Nodes[p3].isBoundary1()
                //      && Nodes[p2].getWhichBoundary() == Nodes[p3].getWhichBoundary()
                //    && (abs(p2 - p3) == 1 || (abs(p2 - p3) == 15 && Nodes[p2].getWhichBoundary() == 2) || (abs(p2 - p3) == 24 && Nodes[p2].getWhichBoundary() == 1));
                if (Triangles[doubtfulTriangles[k]].Neighbors[doubtfulTrianglesNeighborNum[k]] != -100)
                {
                    bool check = (Triangles[doubtfulTriangles[k]].IsPEC == Triangles[Triangles[doubtfulTriangles[k]].Neighbors[doubtfulTrianglesNeighborNum[k]]].IsPEC);
                    //check = true;
                    bool epsCheck = (Triangles[doubtfulTriangles[k]].epsilonR == Triangles[Triangles[doubtfulTriangles[k]].Neighbors[doubtfulTrianglesNeighborNum[k]]].epsilonR);
                    if (DelaunayCheck(doubtfulTriangles[k], doubtfulTrianglesNeighborNum[k]) && check && epsCheck)
                    {
                        int neighb = Triangles[doubtfulTriangles[k]].Neighbors[doubtfulTrianglesNeighborNum[k]];
                        swap(doubtfulTriangles[k], doubtfulTrianglesNeighborNum[k]);
                        doubtfulTriangles.Add(doubtfulTriangles[k]);
                        doubtfulTrianglesNeighborNum.Add(0);
                        doubtfulTriangles.Add(neighb);
                        doubtfulTrianglesNeighborNum.Add(2);
                    }
                    else if (DelaunayCheck(doubtfulTriangles[k], doubtfulTrianglesNeighborNum[k]))
                    {

                    }
                }
                k++;
            }
        }
        ////////////////////////////////////// 
        /// the following function add new nodes to make the mesh better according to minBeta and maxLength values. 
        /// in this function, neighbors of triangles and nodes will be updated. 
        /////////////////////////////////////
        public void meshRefining(double minBeta, double maxLength)
        {
            int Num = this.Triangles.Count;
            for (int trngIndex = 0; trngIndex < Num; trngIndex++)
            {
                bool IsOuter = Triangles[trngIndex].Nodes.Contains(0) || Triangles[trngIndex].Nodes.Contains(1) ||
                    Triangles[trngIndex].Nodes.Contains(2) || Triangles[trngIndex].Nodes.Contains(3);
                bool refineCheck = (((this.Length(trngIndex)[0] > maxLength || this.Length(trngIndex)[1] > maxLength || this.Length(trngIndex)[2] > maxLength) || this.beta(trngIndex) < minBeta) && !IsOuter);

                if (refineCheck && !Triangles[trngIndex].IsPEC)
                {
                    //MessageBox.Show("trngIndex" + trngIndex.ToString());
                    Point newnode = new Point();
                    newnode.X = (PointStars[Triangles[trngIndex].Nodes[0]].MyPoint.X +
                        PointStars[Triangles[trngIndex].Nodes[1]].MyPoint.X +
                        PointStars[Triangles[trngIndex].Nodes[2]].MyPoint.X) / 3;
                    newnode.Y = (PointStars[Triangles[trngIndex].Nodes[0]].MyPoint.Y +
                        PointStars[Triangles[trngIndex].Nodes[1]].MyPoint.Y +
                        PointStars[Triangles[trngIndex].Nodes[2]].MyPoint.Y) / 3;
                    PointStar newNode = new PointStar(newnode, new List<int>());
                    newNode.IsBoundary = false;
                    newNode.whichBoundary = -1;
                    newNode.IsOuter = false;
                    newNode.Phi = (PointStars[Triangles[trngIndex].Nodes[0]].Phi +
                        PointStars[Triangles[trngIndex].Nodes[1]].Phi +
                        PointStars[Triangles[trngIndex].Nodes[2]].Phi) / 3;
                    makeTriangle(newNode, trngIndex);
                }

            }
        }
        ////////////////////////////////////// 
        /// the following function add new nodes to make the mesh better according to min/maxPhi and maxGrad values. 
        /// in this function, neighbors of triangles and nodes will be updated. 
        /////////////////////////////////////
        public void meshRefining_grad(double maxGrad, double maxPhi, double minPhi)
        {
            int Num = this.Triangles.Count;
            for (int trngIndex = 0; trngIndex < Num; trngIndex++)
            {
                double dPhi = maxPhi - minPhi;
                double A = elementArea(trngIndex);
                bool IsOuter = Triangles[trngIndex].Nodes.Contains(0) || Triangles[trngIndex].Nodes.Contains(1) ||
                    Triangles[trngIndex].Nodes.Contains(2) || Triangles[trngIndex].Nodes.Contains(3);
                double[] grad = new double[3]
                { Math.Sqrt(A)*Math.Abs(PointStars[Triangles[trngIndex].Nodes[0]].Phi - PointStars[Triangles[trngIndex].Nodes[1]].Phi) / (Length(trngIndex)[0] * dPhi),
                  Math.Sqrt(A)*Math.Abs(PointStars[Triangles[trngIndex].Nodes[1]].Phi - PointStars[Triangles[trngIndex].Nodes[2]].Phi) / (Length(trngIndex)[1] * dPhi),
                  Math.Sqrt(A)*Math.Abs(PointStars[Triangles[trngIndex].Nodes[2]].Phi - PointStars[Triangles[trngIndex].Nodes[0]].Phi) / (Length(trngIndex)[2] * dPhi)};
                //MessageBox.Show("grad0:" + grad[0]);
                bool refineCheck = (grad[0] > maxGrad || grad[1] > maxGrad || grad[2] > maxGrad) && !IsOuter;

                if (refineCheck && !Triangles[trngIndex].IsPEC)
                {
                    //MessageBox.Show("trngIndex" + trngIndex.ToString());
                    Point newnode = new Point();
                    newnode.X = (PointStars[Triangles[trngIndex].Nodes[0]].MyPoint.X +
                        PointStars[Triangles[trngIndex].Nodes[1]].MyPoint.X +
                        PointStars[Triangles[trngIndex].Nodes[2]].MyPoint.X) / 3;
                    newnode.Y = (PointStars[Triangles[trngIndex].Nodes[0]].MyPoint.Y +
                        PointStars[Triangles[trngIndex].Nodes[1]].MyPoint.Y +
                        PointStars[Triangles[trngIndex].Nodes[2]].MyPoint.Y) / 3;
                    PointStar newNode = new PointStar(newnode, new List<int>());
                    newNode.IsBoundary = false;
                    newNode.whichBoundary = -1;
                    newNode.IsOuter = false;
                    newNode.Phi = (PointStars[Triangles[trngIndex].Nodes[0]].Phi +
                        PointStars[Triangles[trngIndex].Nodes[1]].Phi +
                        PointStars[Triangles[trngIndex].Nodes[2]].Phi) / 3;
                    makeTriangle(newNode, trngIndex);
                }

            }
        }


        ////////////////////////////////////// 
        /// the following functions calculate appropriate C^{(e)}_{ij}s for different problems introduced in the thesis.
        /////////////////////////////////////
        public List<double> elementCoeff_LAPLACE(int trngIndex)
        {
            Triangle trng = this.Triangles[trngIndex];
            double X1 = PointStars[trng.Nodes[0]].MyPoint.X;
            double X2 = PointStars[trng.Nodes[1]].MyPoint.X;
            double X3 = PointStars[trng.Nodes[2]].MyPoint.X;
            double Y1 = PointStars[trng.Nodes[0]].MyPoint.Y;
            double Y2 = PointStars[trng.Nodes[1]].MyPoint.Y;
            double Y3 = PointStars[trng.Nodes[2]].MyPoint.Y;
            double A = ((X1 * Y2 - X2 * Y1) + (X3 * Y1 - X1 * Y3) + (X2 * Y3 - X3 * Y2)) / 2;
            var elemC = new List<double>
            {
                trng.epsilonR * ((Y2 - Y3) * (Y2 - Y3) + (X3 - X2) * (X3 - X2)) / (4 * A), // C11
                trng.epsilonR * ((Y2 - Y3) * (Y3 - Y1) + (X3 - X2) * (X1 - X3)) / (4 * A), // C12
                trng.epsilonR * ((Y2 - Y3) * (Y1 - Y2) + (X3 - X2) * (X2 - X1)) / (4 * A), // C13
                trng.epsilonR * ((Y3 - Y1) * (Y3 - Y1) + (X1 - X3) * (X1 - X3)) / (4 * A), // C22
                trng.epsilonR * ((Y3 - Y1) * (Y1 - Y2) + (X1 - X3) * (X2 - X1)) / (4 * A), // C23
                trng.epsilonR * ((Y1 - Y2) * (Y1 - Y2) + (X2 - X1) * (X2 - X1)) / (4 * A)  // C33
            };
            // $C^{(e)}_{ij}$ in this order: 11 12 13 22 23 33. $C^{(e)}_{ij} = C^{(e)}_{ji}$
            return elemC;
        }
        public List<double> elementCoeff_TM(int trngIndex)
        {
            Triangle trng = this.Triangles[trngIndex];
            double X1 = PointStars[trng.Nodes[0]].MyPoint.X;
            double X2 = PointStars[trng.Nodes[1]].MyPoint.X;
            double X3 = PointStars[trng.Nodes[2]].MyPoint.X;
            double Y1 = PointStars[trng.Nodes[0]].MyPoint.Y;
            double Y2 = PointStars[trng.Nodes[1]].MyPoint.Y;
            double Y3 = PointStars[trng.Nodes[2]].MyPoint.Y;
            double A = ((X1 * Y2 - X2 * Y1) + (X3 * Y1 - X1 * Y3) + (X2 * Y3 - X3 * Y2)) / 2;
            var elemC = new List<double>
            {
                1 * ((Y2 - Y3) * (Y2 - Y3) + (X3 - X2) * (X3 - X2)) / (4 * A), // C11
                1 * ((Y2 - Y3) * (Y3 - Y1) + (X3 - X2) * (X1 - X3)) / (4 * A), // C12
                1 * ((Y2 - Y3) * (Y1 - Y2) + (X3 - X2) * (X2 - X1)) / (4 * A), // C13
                1 * ((Y3 - Y1) * (Y3 - Y1) + (X1 - X3) * (X1 - X3)) / (4 * A), // C22
                1 * ((Y3 - Y1) * (Y1 - Y2) + (X1 - X3) * (X2 - X1)) / (4 * A), // C23
                1 * ((Y1 - Y2) * (Y1 - Y2) + (X2 - X1) * (X2 - X1)) / (4 * A)  // C33
            }; // for magnetic materials, we should change 1s with 1/trng.MuR
            // $C^{(e)}_{ij}$ in this order: 11 12 13 22 23 33. $C^{(e)}_{ij} = C^{(e)}_{ji}$
            return elemC;
        }
        public List<double> elementCoeff_TE(int trngIndex)
        {
            Triangle trng = this.Triangles[trngIndex];
            double X1 = PointStars[trng.Nodes[0]].MyPoint.X;
            double X2 = PointStars[trng.Nodes[1]].MyPoint.X;
            double X3 = PointStars[trng.Nodes[2]].MyPoint.X;
            double Y1 = PointStars[trng.Nodes[0]].MyPoint.Y;
            double Y2 = PointStars[trng.Nodes[1]].MyPoint.Y;
            double Y3 = PointStars[trng.Nodes[2]].MyPoint.Y;
            double A = ((X1 * Y2 - X2 * Y1) + (X3 * Y1 - X1 * Y3) + (X2 * Y3 - X3 * Y2)) / 2;
            var elemC = new List<double>
            {
                (1 / trng.epsilonR) * ((Y2 - Y3) * (Y2 - Y3) + (X3 - X2) * (X3 - X2)) / (4 * A), // C11
                (1 / trng.epsilonR) * ((Y2 - Y3) * (Y3 - Y1) + (X3 - X2) * (X1 - X3)) / (4 * A), // C12
                (1 / trng.epsilonR) * ((Y2 - Y3) * (Y1 - Y2) + (X3 - X2) * (X2 - X1)) / (4 * A), // C13
                (1 / trng.epsilonR) * ((Y3 - Y1) * (Y3 - Y1) + (X1 - X3) * (X1 - X3)) / (4 * A), // C22
                (1 / trng.epsilonR) * ((Y3 - Y1) * (Y1 - Y2) + (X1 - X3) * (X2 - X1)) / (4 * A), // C23
                (1 / trng.epsilonR) * ((Y1 - Y2) * (Y1 - Y2) + (X2 - X1) * (X2 - X1)) / (4 * A)  // C33
            };
            // $C^{(e)}_{ij}$ in this order: 11 12 13 22 23 33. $C^{(e)}_{ij} = C^{(e)}_{ji}$
            return elemC;
        }
        public List<double> elementCoeff_TE2(int trngIndex)
        {
            Triangle trng = this.Triangles[trngIndex];
            double X1 = PointStars[trng.Nodes[0]].MyPoint.X;
            double X2 = PointStars[trng.Nodes[1]].MyPoint.X;
            double X3 = PointStars[trng.Nodes[2]].MyPoint.X;
            double Y1 = PointStars[trng.Nodes[0]].MyPoint.Y;
            double Y2 = PointStars[trng.Nodes[1]].MyPoint.Y;
            double Y3 = PointStars[trng.Nodes[2]].MyPoint.Y;
            double A = ((X1 * Y2 - X2 * Y1) + (X3 * Y1 - X1 * Y3) + (X2 * Y3 - X3 * Y2)) / 2;
            var elemC = new List<double>
            {
                (1 / trng.epsilonR / trng.epsilonR) * ((Y2 - Y3) * (Y2 - Y3) + (X3 - X2) * (X3 - X2)) / (4 * A), // C11
                (1 / trng.epsilonR / trng.epsilonR) * ((Y2 - Y3) * (Y3 - Y1) + (X3 - X2) * (X1 - X3)) / (4 * A), // C12
                (1 / trng.epsilonR / trng.epsilonR) * ((Y2 - Y3) * (Y1 - Y2) + (X3 - X2) * (X2 - X1)) / (4 * A), // C13
                (1 / trng.epsilonR / trng.epsilonR) * ((Y3 - Y1) * (Y3 - Y1) + (X1 - X3) * (X1 - X3)) / (4 * A), // C22
                (1 / trng.epsilonR / trng.epsilonR) * ((Y3 - Y1) * (Y1 - Y2) + (X1 - X3) * (X2 - X1)) / (4 * A), // C23
                (1 / trng.epsilonR / trng.epsilonR) * ((Y1 - Y2) * (Y1 - Y2) + (X2 - X1) * (X2 - X1)) / (4 * A)  // C33
            };
            // $C^{(e)}_{ij}$ in this order: 11 12 13 22 23 33. $C^{(e)}_{ij} = C^{(e)}_{ji}$
            return elemC;
        }


        ////////////////////////////////////// 
        /// the following functions calculate appropriate C_{ij}s for different problems introduced in the thesis.
        /////////////////////////////////////
        public double twoNodeCoeff_LAPLACE(int I, int J)
        {
            // This function returns $C_{IJ}$
            PointStar node = this.PointStars[I];
            double C_IJ = 0;
            if (I == J)
            {
                // 1st, we should check $I == J$ case. 
                foreach (int trngNum in node.neighborTriangles)
                {
                    // For this case, for each triangle, we should find I-th node place in trng.Nodes
                    if (trngNum != -100)
                    {
                        Triangle trng = this.Triangles[trngNum];
                        int I_place = 0;
                        while (trng.Nodes[I_place] != I && I_place < 3)
                        {
                            I_place++;
                        }
                        // Now, we should apply trnNum-th triangle contribution in $C_{IJ}$
                        if (!trng.IsPEC)
                        {
                            switch (I_place)
                            {
                                case 0:
                                    C_IJ += elementCoeff_LAPLACE(trngNum)[0];
                                    break;
                                case 1:
                                    C_IJ += elementCoeff_LAPLACE(trngNum)[3];
                                    break;
                                case 2:
                                    C_IJ += elementCoeff_LAPLACE(trngNum)[5];
                                    break;
                            }
                        }
                    }
                }
            }
            else
            {
                // when $I != J$, we should now "Is there any triangle containing I and J as its nodes or not?"; so,
                bool IsNeighbor = false;
                int trngNum1 = -100;
                int trngNum2 = -100;
                // if IsNeighbor == true, then there are two triangles containing both I ang J. 
                foreach (int trngNum in node.neighborTriangles)
                {
                    // For this case, for each triangle, we should find I-th node place in trng.Nodes
                    if (trngNum != -100)
                    {
                        Triangle trng = this.Triangles[trngNum];
                        int I_place = 0;
                        while (trng.Nodes[I_place] != I && I_place < 3)
                        {
                            I_place++;
                        }
                        if (J == trng.Nodes[(I_place + 1) % 3] || J == trng.Nodes[(I_place + 2) % 3])
                        {
                            IsNeighbor = true;
                            if (trngNum1 == -100 && trngNum2 == -100)
                            {
                                trngNum1 = trngNum;
                            }
                            else if (trngNum2 == -100)
                            {
                                trngNum2 = trngNum;
                                break;
                            }
                        }
                    }
                }
                if (IsNeighbor)
                {
                    if (trngNum1 != -100)
                    {
                        Triangle trng1 = this.Triangles[trngNum1];
                        int I_place1 = 0;
                        while (trng1.Nodes[I_place1] != I && I_place1 < 3)
                        {
                            I_place1++;
                        }
                        int J_place1 = 0;
                        while (trng1.Nodes[J_place1] != J && J_place1 < 3)
                        {
                            J_place1++;
                        }
                        if (!trng1.IsPEC)
                        {
                            switch (10 * I_place1 + J_place1 + 11)
                            {
                                case 12:
                                    C_IJ += elementCoeff_LAPLACE(trngNum1)[1];
                                    break;
                                case 21:
                                    C_IJ += elementCoeff_LAPLACE(trngNum1)[1];
                                    break;
                                case 13:
                                    C_IJ += elementCoeff_LAPLACE(trngNum1)[2];
                                    break;
                                case 31:
                                    C_IJ += elementCoeff_LAPLACE(trngNum1)[2];
                                    break;
                                case 23:
                                    C_IJ += elementCoeff_LAPLACE(trngNum1)[4];
                                    break;
                                case 32:
                                    C_IJ += elementCoeff_LAPLACE(trngNum1)[4];
                                    break;
                            }
                        }
                    }

                    if (trngNum2 != -100)
                    {
                        Triangle trng2 = this.Triangles[trngNum2];
                        int I_place2 = 0;
                        while (trng2.Nodes[I_place2] != I && I_place2 < 3)
                        {
                            I_place2++;
                        }
                        int J_place2 = 0;
                        while (trng2.Nodes[J_place2] != J && J_place2 < 3)
                        {
                            J_place2++;
                        }
                        if (!trng2.IsPEC)
                        {
                            switch (10 * I_place2 + J_place2 + 11)
                            {
                                case 12:
                                    C_IJ += elementCoeff_LAPLACE(trngNum2)[1];
                                    break;
                                case 21:
                                    C_IJ += elementCoeff_LAPLACE(trngNum2)[1];
                                    break;
                                case 13:
                                    C_IJ += elementCoeff_LAPLACE(trngNum2)[2];
                                    break;
                                case 31:
                                    C_IJ += elementCoeff_LAPLACE(trngNum2)[2];
                                    break;
                                case 23:
                                    C_IJ += elementCoeff_LAPLACE(trngNum2)[4];
                                    break;
                                case 32:
                                    C_IJ += elementCoeff_LAPLACE(trngNum2)[4];
                                    break;
                            }
                        }
                    }
                }
                else
                {
                    C_IJ = 0;
                }
            }

            return C_IJ;
        }
        public double twoNodeCoeff_TM(int I, int J)
        {
            // This function returns $C_{IJ}$
            PointStar node = this.PointStars[I];
            double C_IJ = 0;
            if (I == J)
            {
                // 1st, we should check $I == J$ case. 
                foreach (int trngNum in node.neighborTriangles)
                {
                    // For this case, for each triangle, we should find I-th node place in trng.Nodes
                    if (trngNum != -100)
                    {
                        Triangle trng = this.Triangles[trngNum];
                        int I_place = 0;
                        while (trng.Nodes[I_place] != I && I_place < 3)
                        {
                            I_place++;
                        }
                        // Now, we should apply trnNum-th triangle contribution in $C_{IJ}$
                        if (!trng.IsPEC)
                        {
                            switch (I_place)
                            {
                                case 0:
                                    C_IJ += elementCoeff_TM(trngNum)[0];
                                    break;
                                case 1:
                                    C_IJ += elementCoeff_TM(trngNum)[3];
                                    break;
                                case 2:
                                    C_IJ += elementCoeff_TM(trngNum)[5];
                                    break;
                            }
                        }
                    }
                }
            }
            else
            {
                // when $I != J$, we should now "Is there any triangle containing I and J as its nodes or not?"; so,
                bool IsNeighbor = false;
                int trngNum1 = -100;
                int trngNum2 = -100;
                // if IsNeighbor == true, then there are two triangles containing both I ang J. 
                foreach (int trngNum in node.neighborTriangles)
                {
                    // For this case, for each triangle, we should find I-th node place in trng.Nodes
                    if (trngNum != -100)
                    {
                        Triangle trng = this.Triangles[trngNum];
                        int I_place = 0;
                        while (trng.Nodes[I_place] != I && I_place < 3)
                        {
                            I_place++;
                        }
                        if (J == trng.Nodes[(I_place + 1) % 3] || J == trng.Nodes[(I_place + 2) % 3])
                        {
                            IsNeighbor = true;
                            if (trngNum1 == -100 && trngNum2 == -100)
                            {
                                trngNum1 = trngNum;
                            }
                            else if (trngNum2 == -100)
                            {
                                trngNum2 = trngNum;
                                break;
                            }
                        }
                    }
                }
                if (IsNeighbor)
                {
                    if (trngNum1 != -100)
                    {
                        Triangle trng1 = this.Triangles[trngNum1];
                        int I_place1 = 0;
                        while (trng1.Nodes[I_place1] != I && I_place1 < 3)
                        {
                            I_place1++;
                        }
                        int J_place1 = 0;
                        while (trng1.Nodes[J_place1] != J && J_place1 < 3)
                        {
                            J_place1++;
                        }
                        if (!trng1.IsPEC)
                        {
                            switch (10 * I_place1 + J_place1 + 11)
                            {
                                case 12:
                                    C_IJ += elementCoeff_TM(trngNum1)[1];
                                    break;
                                case 21:
                                    C_IJ += elementCoeff_TM(trngNum1)[1];
                                    break;
                                case 13:
                                    C_IJ += elementCoeff_TM(trngNum1)[2];
                                    break;
                                case 31:
                                    C_IJ += elementCoeff_TM(trngNum1)[2];
                                    break;
                                case 23:
                                    C_IJ += elementCoeff_TM(trngNum1)[4];
                                    break;
                                case 32:
                                    C_IJ += elementCoeff_TM(trngNum1)[4];
                                    break;
                            }
                        }
                    }

                    if (trngNum2 != -100)
                    {
                        Triangle trng2 = this.Triangles[trngNum2];
                        int I_place2 = 0;
                        while (trng2.Nodes[I_place2] != I && I_place2 < 3)
                        {
                            I_place2++;
                        }
                        int J_place2 = 0;
                        while (trng2.Nodes[J_place2] != J && J_place2 < 3)
                        {
                            J_place2++;
                        }
                        if (!trng2.IsPEC)
                        {
                            switch (10 * I_place2 + J_place2 + 11)
                            {
                                case 12:
                                    C_IJ += elementCoeff_TM(trngNum2)[1];
                                    break;
                                case 21:
                                    C_IJ += elementCoeff_TM(trngNum2)[1];
                                    break;
                                case 13:
                                    C_IJ += elementCoeff_TM(trngNum2)[2];
                                    break;
                                case 31:
                                    C_IJ += elementCoeff_TM(trngNum2)[2];
                                    break;
                                case 23:
                                    C_IJ += elementCoeff_TM(trngNum2)[4];
                                    break;
                                case 32:
                                    C_IJ += elementCoeff_TM(trngNum2)[4];
                                    break;
                            }
                        }
                    }
                }
                else
                {
                    C_IJ = 0;
                }
            }

            return C_IJ;
        }
        public double twoNodeCoeff_TE(int I, int J)
        {
            // This function returns $C_{IJ}$
            PointStar node = this.PointStars[I];
            double C_IJ = 0;
            if (I == J)
            {
                // 1st, we should check $I == J$ case. 
                foreach (int trngNum in node.neighborTriangles)
                {
                    // For this case, for each triangle, we should find I-th node place in trng.Nodes
                    if (trngNum != -100)
                    {
                        Triangle trng = this.Triangles[trngNum];
                        int I_place = 0;
                        while (trng.Nodes[I_place] != I && I_place < 3)
                        {
                            I_place++;
                        }
                        // Now, we should apply trnNum-th triangle contribution in $C_{IJ}$
                        if (!trng.IsPEC)
                        {
                            switch (I_place)
                            {
                                case 0:
                                    C_IJ += elementCoeff_TE(trngNum)[0];
                                    break;
                                case 1:
                                    C_IJ += elementCoeff_TE(trngNum)[3];
                                    break;
                                case 2:
                                    C_IJ += elementCoeff_TE(trngNum)[5];
                                    break;
                            }
                        }
                    }
                }
            }
            else
            {
                // when $I != J$, we should now "Is there any triangle containing I and J as its nodes or not?"; so,
                bool IsNeighbor = false;
                int trngNum1 = -100;
                int trngNum2 = -100;
                // if IsNeighbor == true, then there are two triangles containing both I ang J. 
                foreach (int trngNum in node.neighborTriangles)
                {
                    // For this case, for each triangle, we should find I-th node place in trng.Nodes
                    if (trngNum != -100)
                    {
                        Triangle trng = this.Triangles[trngNum];
                        int I_place = 0;
                        while (trng.Nodes[I_place] != I && I_place < 3)
                        {
                            I_place++;
                        }
                        if (J == trng.Nodes[(I_place + 1) % 3] || J == trng.Nodes[(I_place + 2) % 3])
                        {
                            IsNeighbor = true;
                            if (trngNum1 == -100 && trngNum2 == -100)
                            {
                                trngNum1 = trngNum;
                            }
                            else if (trngNum2 == -100)
                            {
                                trngNum2 = trngNum;
                                break;
                            }
                        }
                    }
                }
                if (IsNeighbor)
                {
                    if (trngNum1 != -100)
                    {
                        Triangle trng1 = this.Triangles[trngNum1];
                        int I_place1 = 0;
                        while (trng1.Nodes[I_place1] != I && I_place1 < 3)
                        {
                            I_place1++;
                        }
                        int J_place1 = 0;
                        while (trng1.Nodes[J_place1] != J && J_place1 < 3)
                        {
                            J_place1++;
                        }
                        if (!trng1.IsPEC)
                        {
                            switch (10 * I_place1 + J_place1 + 11)
                            {
                                case 12:
                                    C_IJ += elementCoeff_TE(trngNum1)[1];
                                    break;
                                case 21:
                                    C_IJ += elementCoeff_TE(trngNum1)[1];
                                    break;
                                case 13:
                                    C_IJ += elementCoeff_TE(trngNum1)[2];
                                    break;
                                case 31:
                                    C_IJ += elementCoeff_TE(trngNum1)[2];
                                    break;
                                case 23:
                                    C_IJ += elementCoeff_TE(trngNum1)[4];
                                    break;
                                case 32:
                                    C_IJ += elementCoeff_TE(trngNum1)[4];
                                    break;
                            }
                        }
                    }

                    if (trngNum2 != -100)
                    {
                        Triangle trng2 = this.Triangles[trngNum2];
                        int I_place2 = 0;
                        while (trng2.Nodes[I_place2] != I && I_place2 < 3)
                        {
                            I_place2++;
                        }
                        int J_place2 = 0;
                        while (trng2.Nodes[J_place2] != J && J_place2 < 3)
                        {
                            J_place2++;
                        }
                        if (!trng2.IsPEC)
                        {
                            switch (10 * I_place2 + J_place2 + 11)
                            {
                                case 12:
                                    C_IJ += elementCoeff_TE(trngNum2)[1];
                                    break;
                                case 21:
                                    C_IJ += elementCoeff_TE(trngNum2)[1];
                                    break;
                                case 13:
                                    C_IJ += elementCoeff_TE(trngNum2)[2];
                                    break;
                                case 31:
                                    C_IJ += elementCoeff_TE(trngNum2)[2];
                                    break;
                                case 23:
                                    C_IJ += elementCoeff_TE(trngNum2)[4];
                                    break;
                                case 32:
                                    C_IJ += elementCoeff_TE(trngNum2)[4];
                                    break;
                            }
                        }
                    }
                }
                else
                {
                    C_IJ = 0;
                }
            }

            return C_IJ;
        }
        public double twoNodeCoeff_TE2(int I, int J)
        {
            // This function returns $C_{IJ}$
            PointStar node = this.PointStars[I];
            double C_IJ = 0;
            if (I == J)
            {
                // 1st, we should check $I == J$ case. 
                foreach (int trngNum in node.neighborTriangles)
                {
                    // For this case, for each triangle, we should find I-th node place in trng.Nodes
                    if (trngNum != -100)
                    {
                        Triangle trng = this.Triangles[trngNum];
                        int I_place = 0;
                        while (trng.Nodes[I_place] != I && I_place < 3)
                        {
                            I_place++;
                        }
                        // Now, we should apply trnNum-th triangle contribution in $C_{IJ}$
                        if (!trng.IsPEC)
                        {
                            switch (I_place)
                            {
                                case 0:
                                    C_IJ += elementCoeff_TE2(trngNum)[0];
                                    break;
                                case 1:
                                    C_IJ += elementCoeff_TE2(trngNum)[3];
                                    break;
                                case 2:
                                    C_IJ += elementCoeff_TE2(trngNum)[5];
                                    break;
                            }
                        }
                    }
                }
            }
            else
            {
                // when $I != J$, we should now "Is there any triangle containing I and J as its nodes or not?"; so,
                bool IsNeighbor = false;
                int trngNum1 = -100;
                int trngNum2 = -100;
                // if IsNeighbor == true, then there are two triangles containing both I ang J. 
                foreach (int trngNum in node.neighborTriangles)
                {
                    // For this case, for each triangle, we should find I-th node place in trng.Nodes
                    if (trngNum != -100)
                    {
                        Triangle trng = this.Triangles[trngNum];
                        int I_place = 0;
                        while (trng.Nodes[I_place] != I && I_place < 3)
                        {
                            I_place++;
                        }
                        if (J == trng.Nodes[(I_place + 1) % 3] || J == trng.Nodes[(I_place + 2) % 3])
                        {
                            IsNeighbor = true;
                            if (trngNum1 == -100 && trngNum2 == -100)
                            {
                                trngNum1 = trngNum;
                            }
                            else if (trngNum2 == -100)
                            {
                                trngNum2 = trngNum;
                                break;
                            }
                        }
                    }
                }
                if (IsNeighbor)
                {
                    if (trngNum1 != -100)
                    {
                        Triangle trng1 = this.Triangles[trngNum1];
                        int I_place1 = 0;
                        while (trng1.Nodes[I_place1] != I && I_place1 < 3)
                        {
                            I_place1++;
                        }
                        int J_place1 = 0;
                        while (trng1.Nodes[J_place1] != J && J_place1 < 3)
                        {
                            J_place1++;
                        }
                        if (!trng1.IsPEC)
                        {
                            switch (10 * I_place1 + J_place1 + 11)
                            {
                                case 12:
                                    C_IJ += elementCoeff_TE2(trngNum1)[1];
                                    break;
                                case 21:
                                    C_IJ += elementCoeff_TE2(trngNum1)[1];
                                    break;
                                case 13:
                                    C_IJ += elementCoeff_TE2(trngNum1)[2];
                                    break;
                                case 31:
                                    C_IJ += elementCoeff_TE2(trngNum1)[2];
                                    break;
                                case 23:
                                    C_IJ += elementCoeff_TE2(trngNum1)[4];
                                    break;
                                case 32:
                                    C_IJ += elementCoeff_TE2(trngNum1)[4];
                                    break;
                            }
                        }
                    }

                    if (trngNum2 != -100)
                    {
                        Triangle trng2 = this.Triangles[trngNum2];
                        int I_place2 = 0;
                        while (trng2.Nodes[I_place2] != I && I_place2 < 3)
                        {
                            I_place2++;
                        }
                        int J_place2 = 0;
                        while (trng2.Nodes[J_place2] != J && J_place2 < 3)
                        {
                            J_place2++;
                        }
                        if (!trng2.IsPEC)
                        {
                            switch (10 * I_place2 + J_place2 + 11)
                            {
                                case 12:
                                    C_IJ += elementCoeff_TE2(trngNum2)[1];
                                    break;
                                case 21:
                                    C_IJ += elementCoeff_TE2(trngNum2)[1];
                                    break;
                                case 13:
                                    C_IJ += elementCoeff_TE2(trngNum2)[2];
                                    break;
                                case 31:
                                    C_IJ += elementCoeff_TE2(trngNum2)[2];
                                    break;
                                case 23:
                                    C_IJ += elementCoeff_TE2(trngNum2)[4];
                                    break;
                                case 32:
                                    C_IJ += elementCoeff_TE2(trngNum2)[4];
                                    break;
                            }
                        }
                    }
                }
                else
                {
                    C_IJ = 0;
                }
            }

            return C_IJ;
        }


        ////////////////////////////////////// 
        /// the following function calculates C^{(e)}_{ij}s for a particular "i".
        /////////////////////////////////////
        public List<double> nodeCoeff(int nodeIndex)
        {
            PointStar node = this.PointStars[nodeIndex];
            for (int i = 0; i < node.neighborTriangles.Count; i++)
            {

            }
            return new List<double>();
        }


        ////////////////////////////////////// 
        /// the following functions calculate appropriate T^{(e)}_{ij}s for different problems introduced in the thesis.
        /////////////////////////////////////
        public List<double> elementT_TE(int trngIndex)
        {
            //Triangle trng = this.Triangles[trngIndex];
            /*double X1 = PointStars[trng.Nodes[0]].MyPoint.X;
            double X2 = PointStars[trng.Nodes[1]].MyPoint.X;
            double X3 = PointStars[trng.Nodes[2]].MyPoint.X;
            double Y1 = PointStars[0].MyPoint.Y;
            double Y2 = PointStars[1].MyPoint.Y;
            double Y3 = PointStars[2].MyPoint.Y;*/
            double A = elementArea(trngIndex);
            var elemT = new List<double>
            {
                A/6,  // T11
                A/12, // T12
                A/12, // T13
                A/6,  // T22
                A/12, // T23
                A/6   // T33
            };
            // $T^{(e)}_{ij}$ in this order: 11 12 13 22 23 33. $T^{(e)}_{ij} = T^{(e)}_{ji}$
            return elemT;
        }
        public List<double> elementT_TM(int trngIndex)
        {
            Triangle trng = this.Triangles[trngIndex];
            /*double X1 = PointStars[trng.Nodes[0]].MyPoint.X;
            double X2 = PointStars[trng.Nodes[1]].MyPoint.X;
            double X3 = PointStars[trng.Nodes[2]].MyPoint.X;
            double Y1 = PointStars[0].MyPoint.Y;
            double Y2 = PointStars[1].MyPoint.Y;
            double Y3 = PointStars[2].MyPoint.Y;*/
            double A = elementArea(trngIndex);
            var elemT = new List<double>
            {
                trng.epsilonR * A / 6,  // T11
                trng.epsilonR * A / 12, // T12
                trng.epsilonR * A / 12, // T13
                trng.epsilonR * A / 6,  // T22
                trng.epsilonR * A / 12, // T23
                trng.epsilonR * A /6   // T33
            };
            // $T^{(e)}_{ij}$ in this order: 11 12 13 22 23 33. $T^{(e)}_{ij} = T^{(e)}_{ji}$
            return elemT;
        }

        ////////////////////////////////////// 
        /// the following functions calculate appropriate T_{ij}s for different problems introduced in the thesis.
        /////////////////////////////////////
        public double twoNodeT_TE(int I, int J)
        {
            // This function returns $C_{IJ}$
            PointStar node = this.PointStars[I];
            double T_IJ = 0;
            if (I == J)
            {
                // 1st, we should check $I == J$ case. 
                foreach (int trngNum in node.neighborTriangles)
                {
                    // For this case, for each triangle, we should find I-th node place in trng.Nodes
                    if (trngNum != -100)
                    {
                        Triangle trng = this.Triangles[trngNum];
                        int I_place = 0;
                        while (trng.Nodes[I_place] != I && I_place < 3)
                        {
                            I_place++;
                        }
                        // Now, we should apply trnNum-th triangle contribution in $C_{IJ}$
                        if (!trng.IsPEC)
                        {
                            switch (I_place)
                            {
                                case 0:
                                    T_IJ += elementT_TE(trngNum)[0];
                                    break;
                                case 1:
                                    T_IJ += elementT_TE(trngNum)[3];
                                    break;
                                case 2:
                                    T_IJ += elementT_TE(trngNum)[5];
                                    break;
                            }
                        }
                    }
                }
            }
            else
            {
                // when $I != J$, we should now "Is there any triangle containing I and J as its nodes or not?"; so,
                bool IsNeighbor = false;
                int trngNum1 = -100;
                int trngNum2 = -100;
                // if IsNeighbor == true, then there are two triangles containing both I ang J. 
                foreach (int trngNum in node.neighborTriangles)
                {
                    // For this case, for each triangle, we should find I-th node place in trng.Nodes
                    if (trngNum != -100)
                    {
                        Triangle trng = this.Triangles[trngNum];
                        int I_place = 0;
                        while (trng.Nodes[I_place] != I && I_place < 3)
                        {
                            I_place++;
                        }
                        if (J == trng.Nodes[(I_place + 1) % 3] || J == trng.Nodes[(I_place + 2) % 3])
                        {
                            IsNeighbor = true;
                            if (trngNum1 == -100 && trngNum2 == -100)
                            {
                                trngNum1 = trngNum;
                            }
                            else if (trngNum2 == -100)
                            {
                                trngNum2 = trngNum;
                                break;
                            }
                        }
                    }
                }
                if (IsNeighbor)
                {
                    if (trngNum1 != -100)
                    {
                        Triangle trng1 = this.Triangles[trngNum1];
                        int I_place1 = 0;
                        while (trng1.Nodes[I_place1] != I && I_place1 < 3)
                        {
                            I_place1++;
                        }
                        int J_place1 = 0;
                        while (trng1.Nodes[J_place1] != J && J_place1 < 3)
                        {
                            J_place1++;
                        }
                        if (!trng1.IsPEC)
                        {
                            switch (10 * I_place1 + J_place1 + 11)
                            {
                                case 12:
                                    T_IJ += elementT_TE(trngNum1)[1];
                                    break;
                                case 21:
                                    T_IJ += elementT_TE(trngNum1)[1];
                                    break;
                                case 13:
                                    T_IJ += elementT_TE(trngNum1)[2];
                                    break;
                                case 31:
                                    T_IJ += elementT_TE(trngNum1)[2];
                                    break;
                                case 23:
                                    T_IJ += elementT_TE(trngNum1)[4];
                                    break;
                                case 32:
                                    T_IJ += elementT_TE(trngNum1)[4];
                                    break;
                            }
                        }

                    }

                    if (trngNum2 != -100)
                    {
                        Triangle trng2 = this.Triangles[trngNum2];
                        int I_place2 = 0;
                        while (trng2.Nodes[I_place2] != I && I_place2 < 3)
                        {
                            I_place2++;
                        }
                        int J_place2 = 0;
                        while (trng2.Nodes[J_place2] != J && J_place2 < 3)
                        {
                            J_place2++;
                        }
                        if (!trng2.IsPEC)
                        {
                            switch (10 * I_place2 + J_place2 + 11)
                            {
                                case 12:
                                    T_IJ += elementT_TE(trngNum2)[1];
                                    break;
                                case 21:
                                    T_IJ += elementT_TE(trngNum2)[1];
                                    break;
                                case 13:
                                    T_IJ += elementT_TE(trngNum2)[2];
                                    break;
                                case 31:
                                    T_IJ += elementT_TE(trngNum2)[2];
                                    break;
                                case 23:
                                    T_IJ += elementT_TE(trngNum2)[4];
                                    break;
                                case 32:
                                    T_IJ += elementT_TE(trngNum2)[4];
                                    break;
                            }
                        }
                    }
                }
                else
                {
                    T_IJ = 0;
                }
            }

            return T_IJ;
        }
        public double twoNodeT_TM(int I, int J)
        {
            // This function returns $C_{IJ}$
            PointStar node = this.PointStars[I];
            double T_IJ = 0;
            if (I == J)
            {
                // 1st, we should check $I == J$ case. 
                foreach (int trngNum in node.neighborTriangles)
                {
                    // For this case, for each triangle, we should find I-th node place in trng.Nodes
                    if (trngNum != -100)
                    {
                        Triangle trng = this.Triangles[trngNum];
                        int I_place = 0;
                        while (trng.Nodes[I_place] != I && I_place < 3)
                        {
                            I_place++;
                        }
                        // Now, we should apply trnNum-th triangle contribution in $C_{IJ}$
                        if (!trng.IsPEC)
                        {
                            switch (I_place)
                            {
                                case 0:
                                    T_IJ += elementT_TM(trngNum)[0];
                                    break;
                                case 1:
                                    T_IJ += elementT_TM(trngNum)[3];
                                    break;
                                case 2:
                                    T_IJ += elementT_TM(trngNum)[5];
                                    break;
                            }
                        }
                    }
                }
            }
            else
            {
                // when $I != J$, we should now "Is there any triangle containing I and J as its nodes or not?"; so,
                bool IsNeighbor = false;
                int trngNum1 = -100;
                int trngNum2 = -100;
                // if IsNeighbor == true, then there are two triangles containing both I ang J. 
                foreach (int trngNum in node.neighborTriangles)
                {
                    // For this case, for each triangle, we should find I-th node place in trng.Nodes
                    if (trngNum != -100)
                    {
                        Triangle trng = this.Triangles[trngNum];
                        int I_place = 0;
                        while (trng.Nodes[I_place] != I && I_place < 3)
                        {
                            I_place++;
                        }
                        if (J == trng.Nodes[(I_place + 1) % 3] || J == trng.Nodes[(I_place + 2) % 3])
                        {
                            IsNeighbor = true;
                            if (trngNum1 == -100 && trngNum2 == -100)
                            {
                                trngNum1 = trngNum;
                            }
                            else if (trngNum2 == -100)
                            {
                                trngNum2 = trngNum;
                                break;
                            }
                        }
                    }
                }
                if (IsNeighbor)
                {
                    if (trngNum1 != -100)
                    {
                        Triangle trng1 = this.Triangles[trngNum1];
                        int I_place1 = 0;
                        while (trng1.Nodes[I_place1] != I && I_place1 < 3)
                        {
                            I_place1++;
                        }
                        int J_place1 = 0;
                        while (trng1.Nodes[J_place1] != J && J_place1 < 3)
                        {
                            J_place1++;
                        }
                        if (!trng1.IsPEC)
                        {
                            switch (10 * I_place1 + J_place1 + 11)
                            {
                                case 12:
                                    T_IJ += elementT_TM(trngNum1)[1];
                                    break;
                                case 21:
                                    T_IJ += elementT_TM(trngNum1)[1];
                                    break;
                                case 13:
                                    T_IJ += elementT_TM(trngNum1)[2];
                                    break;
                                case 31:
                                    T_IJ += elementT_TM(trngNum1)[2];
                                    break;
                                case 23:
                                    T_IJ += elementT_TM(trngNum1)[4];
                                    break;
                                case 32:
                                    T_IJ += elementT_TM(trngNum1)[4];
                                    break;
                            }
                        }

                    }

                    if (trngNum2 != -100)
                    {
                        Triangle trng2 = this.Triangles[trngNum2];
                        int I_place2 = 0;
                        while (trng2.Nodes[I_place2] != I && I_place2 < 3)
                        {
                            I_place2++;
                        }
                        int J_place2 = 0;
                        while (trng2.Nodes[J_place2] != J && J_place2 < 3)
                        {
                            J_place2++;
                        }
                        if (!trng2.IsPEC)
                        {
                            switch (10 * I_place2 + J_place2 + 11)
                            {
                                case 12:
                                    T_IJ += elementT_TM(trngNum2)[1];
                                    break;
                                case 21:
                                    T_IJ += elementT_TM(trngNum2)[1];
                                    break;
                                case 13:
                                    T_IJ += elementT_TM(trngNum2)[2];
                                    break;
                                case 31:
                                    T_IJ += elementT_TM(trngNum2)[2];
                                    break;
                                case 23:
                                    T_IJ += elementT_TM(trngNum2)[4];
                                    break;
                                case 32:
                                    T_IJ += elementT_TM(trngNum2)[4];
                                    break;
                            }
                        }
                    }
                }
                else
                {
                    T_IJ = 0;
                }
            }

            return T_IJ;
        }


        ////////////////////////////////////// 
        /// the following function calculates appropriate T_{ij}s for a particular "i".
        /////////////////////////////////////
        public List<double> nodeT(int nodeIndex)
        {
            return new List<double>();
        }



        ////////////////////////////////////// 
        /// This is the electrostatic energy calculator.
        /////////////////////////////////////
        public double CostFunction()
        {
            // not efficient. use node neighbors to calculate this effieciently.
            double W = 0;
            for (int i = 4; i < PointStars.Count; i++)
            {
                W += 0.5 * twoNodeCoeff_LAPLACE(i, i) * PointStars[i].Phi * PointStars[i].Phi;

                /*foreach (int trngNum in PointStars[i].neighborTriangles)
                {
                    if (trngNum != -100)
                    {
                        Triangle trng = this.Triangles[trngNum];
                        int I_place = 0;
                        while (trng.Nodes[I_place] != i && I_place < 3)
                        {
                            I_place++;
                        }
                        int j = trng.Nodes[(I_place + 1) % 3];
                        W += 0.25 * twoNodeCoeff(i, j) * PointStars[i].Phi * PointStars[j].Phi;
                        j = trng.Nodes[(I_place + 2) % 3];
                        W += 0.25 * twoNodeCoeff(i, j) * PointStars[i].Phi * PointStars[j].Phi;
                    }
                }*/
                List<int> neighbs = new List<int>(PointStars[i].neighborTriangles);
                List<int> nodeNeighbs = new List<int>();
                foreach (int trngNum in neighbs)
                {
                    if (trngNum != -100)
                    {
                        Triangle trng = Triangles[trngNum];
                        int I_place = 0;
                        while (trng.Nodes[I_place] != i && I_place < 3)
                        {
                            I_place++;
                        }
                        int j = trng.Nodes[(I_place + 1) % 3];
                        if (!nodeNeighbs.Contains(j))
                        {
                            nodeNeighbs.Add(j);
                            W += 0.5 * twoNodeCoeff_LAPLACE(i, j) * PointStars[i].Phi * PointStars[j].Phi;
                        }
                        j = trng.Nodes[(I_place + 2) % 3];
                        if (!nodeNeighbs.Contains(j))
                        {
                            nodeNeighbs.Add(j);
                            W += 0.5 * twoNodeCoeff_LAPLACE(i, j) * PointStars[i].Phi * PointStars[j].Phi;
                        }
                    }
                }
            }
            return W;
        }


        ////////////////////////////////////// 
        /// Solving Laplace Eq. with Gauss-Seidel method and a relaxation factor.
        /////////////////////////////////////
        public void LAPLACESolver(double error, double cost)
        {
            int count = 0;
            double w = 1.5;
            double Cost1 = 0;
            //double Cost2 = CostFunction();
            double relativeCost = 1.0;
            double relativeError = 1.0;
            List<double> Phi_nodes1 = new List<double>();
            List<double> Phi_nodes2 = new List<double>();
            for (int I = 0; I < PointStars.Count; I++)
            {
                if (!PointStars[I].IsBoundary || PointStars[I].IsDielectricBoundary)
                {
                    Phi_nodes1.Add(PointStars[I].Phi);
                }
            }
            while (relativeCost >= cost || relativeError >= error)
            {
                Cost1 = CostFunction();
                for (int I = 0; I < PointStars.Count; I++)
                {
                    if (PointStars[I].IsBoundary)
                    {
                        continue;
                    }
                    double Phi = (1 - w) * PointStars[I].Phi;
                    PointStar node = PointStars[I];
                    List<int> nodeNeighbors = new List<int>();
                    foreach (int trngNum in node.neighborTriangles)
                    {
                        // For this case, for each triangle, we should find I-th node place in trng.Nodes
                        if (trngNum != -100)
                        {
                            Triangle trng = this.Triangles[trngNum];
                            int I_place = 0;
                            while (trng.Nodes[I_place] != I && I_place < 3)
                            {
                                I_place++;
                            }
                            int node1 = trng.Nodes[(I_place + 1) % 3];
                            int node2 = trng.Nodes[(I_place + 2) % 3];
                            if (!nodeNeighbors.Contains(node1) && !nodeNeighbors.Contains(node2))
                            {
                                Phi += -w * (twoNodeCoeff_LAPLACE(I, node1) * PointStars[node1].Phi + twoNodeCoeff_LAPLACE(I, node2) * PointStars[node2].Phi) / twoNodeCoeff_LAPLACE(I, I);
                                nodeNeighbors.Add(node1);
                                nodeNeighbors.Add(node2);
                            }
                            else if (!nodeNeighbors.Contains(node1))
                            {
                                Phi += -w * (twoNodeCoeff_LAPLACE(I, node1) * PointStars[node1].Phi) / twoNodeCoeff_LAPLACE(I, I);
                                nodeNeighbors.Add(node1);
                            }
                            else if (!nodeNeighbors.Contains(node2))
                            {
                                Phi += -w * (twoNodeCoeff_LAPLACE(I, node2) * PointStars[node2].Phi) / twoNodeCoeff_LAPLACE(I, I);
                                nodeNeighbors.Add(node2);
                            }
                        }
                    }
                    PointStars[I].Phi = Phi;
                }
                for (int I = 0; I < PointStars.Count; I++)
                {
                    if (!PointStars[I].IsBoundary || PointStars[I].IsDielectricBoundary)
                    {
                        Phi_nodes2.Add(PointStars[I].Phi);
                    }
                }
                relativeError = 0;
                double den = 0;
                for (int I = 0; I < Phi_nodes2.Count; I++)
                {
                    relativeError += Math.Abs(Phi_nodes2[I] - Phi_nodes1[I]);
                    den += Math.Abs(Phi_nodes2[I]);
                }
                relativeError /= den;
                double Cost2 = CostFunction();
                relativeCost = Math.Abs(Cost2 - Cost1) / Cost2;
                Cost1 = Cost2;
                Phi_nodes1 = Phi_nodes2;
                Phi_nodes2 = new List<double>();
                //MessageBox.Show("relative error: " + (relativeError >= error).ToString());
                count++;
                //MessageBox.Show("count: " + count.ToString());
            }
        }


        ////////////////////////////////////// 
        /// The Laplace filter which is used to make the mesh more uniform.
        /////////////////////////////////////
        public void LAPLACEFilter()
        {
            for (int i = 0; i < PointStars.Count; i++)
            {
                if (!PointStars[i].IsBoundary && !PointStars[i].IsDielectricBoundary)
                {
                    double avX = 0;
                    double avY = 0;
                    double avPhi = 0;
                    foreach (int trngNum in PointStars[i].neighborTriangles)
                    {
                        if (trngNum != -100)
                        {
                            Triangle trng = this.Triangles[trngNum];
                            int I_place = 0;
                            while (trng.Nodes[I_place] != i && I_place < 3)
                            {
                                I_place++;
                            }
                            avX += PointStars[trng.Nodes[(I_place + 1) % 3]].MyPoint.X;
                            avX += PointStars[trng.Nodes[(I_place + 2) % 3]].MyPoint.X;
                            avY += PointStars[trng.Nodes[(I_place + 1) % 3]].MyPoint.Y;
                            avY += PointStars[trng.Nodes[(I_place + 2) % 3]].MyPoint.Y;
                            avPhi += PointStars[trng.Nodes[(I_place + 1) % 3]].Phi;
                            avPhi += PointStars[trng.Nodes[(I_place + 2) % 3]].Phi;
                        }
                    }
                    avX /= (2 * PointStars[i].neighborTriangles.Count);
                    avY /= (2 * PointStars[i].neighborTriangles.Count);
                    avPhi /= (2 * PointStars[i].neighborTriangles.Count);
                    Point p = new Point();
                    p.X = avX;
                    p.Y = avY;
                    PointStars[i].MyPoint = p;
                    PointStars[i].Phi = avPhi;
                }
            }
        }


        ////////////////////////////////////// 
        /// Solving Laplace Eq. with Least Square and Conjugate Gradient methods.
        /////////////////////////////////////
        public void LAPLACESolver_alglib()
        {
            List<int> freeNodes = new List<int>();
            List<int> fixNodes = new List<int>();
            for (int i = 0; i < PointStars.Count; i++)
            {
                if (!PointStars[i].IsBoundary || PointStars[i].whichBoundary == -5)
                {
                    freeNodes.Add(i);
                }
                else
                {
                    fixNodes.Add(i);
                }
            }
            alglib.sparsematrix a;
            alglib.sparsecreate(freeNodes.Count, freeNodes.Count, out a);
            double[] b = new double[freeNodes.Count];

            for (int i = 0; i < freeNodes.Count; i++)
            {
                for (int j = 0; j < freeNodes.Count; j++)
                {
                    double Cij = twoNodeCoeff_LAPLACE(freeNodes[i], freeNodes[j]);
                    if (Math.Abs(Cij) > 0.000000001)
                    {
                        alglib.sparseset(a, i, j, Cij);
                    }
                }
            }
            alglib.sparseconverttocrs(a);
            for (int i = 0; i < freeNodes.Count; i++)
            {
                b[i] = 0;
                List<int> neighbs = new List<int>(PointStars[freeNodes[i]].neighborTriangles);
                List<int> nodeNeighbs = new List<int>();
                foreach (int trngNum in neighbs)
                {
                    if (trngNum != -100)
                    {
                        Triangle trng = Triangles[trngNum];
                        int I_place = 0;
                        while (trng.Nodes[I_place] != freeNodes[i] && I_place < 3)
                        {
                            I_place++;
                        }
                        if (!nodeNeighbs.Contains(trng.Nodes[(I_place + 1) % 3]) && PointStars[trng.Nodes[(I_place + 1) % 3]].IsBoundary)
                        {
                            nodeNeighbs.Add(trng.Nodes[(I_place + 1) % 3]);
                        }
                        if (!nodeNeighbs.Contains(trng.Nodes[(I_place + 2) % 3]) && PointStars[trng.Nodes[(I_place + 2) % 3]].IsBoundary)
                        {
                            nodeNeighbs.Add(trng.Nodes[(I_place + 2) % 3]);
                        }
                    }
                }
                foreach (int node in nodeNeighbs)
                {
                    b[i] -= twoNodeCoeff_LAPLACE(freeNodes[i], node) * PointStars[node].Phi;
                }
            }
            alglib.linlsqrstate s;
            alglib.linlsqrreport rep;
            double[] x;
            alglib.linlsqrcreate(freeNodes.Count, freeNodes.Count, out s);
            alglib.linlsqrsolvesparse(s, a, b);
            alglib.linlsqrresults(s, out x, out rep);
            /*alglib.lincgstate s;
            alglib.lincgreport rep;
            double[] x;
            alglib.lincgcreate(freeNodes.Count, out s);
            alglib.lincgsolvesparse(s, a, true, b);
            alglib.lincgresults(s, out x, out rep);*/
            for (int i = 0; i < freeNodes.Count; i++)
            {
                PointStars[freeNodes[i]].Phi = x[i];
            }
            //MessageBox.Show("rep:" + rep.terminationtype);
        }


        ////////////////////////////////////// 
        /// Solving Laplace Eq. with Zero Charge conductors on it. Unfortunately, this Function is NOT COMPLETELY TESTED.
        /////////////////////////////////////
        public void LAPLACESolver_Constrained()
        {
            List<int> freeNodes = new List<int>();
            List<int> fixNodes = new List<int>();
            List<int> constrainedNodes = new List<int>();
            for (int i = 0; i < PointStars.Count; i++)
            {
                if (!PointStars[i].IsBoundary || PointStars[i].whichBoundary == -5 || PointStars[i].IsFixedCharge)
                {
                    freeNodes.Add(i);
                    if (PointStars[i].IsFixedCharge)
                    {
                        constrainedNodes.Add(i);
                    }
                }
                else
                {
                    fixNodes.Add(i);
                }
            }
            alglib.sparsematrix a;
            alglib.sparsecreate(freeNodes.Count + constrainedNodes.Count, freeNodes.Count + constrainedNodes.Count, out a);
            double[] b = new double[freeNodes.Count + constrainedNodes.Count];

            for (int i = 0; i < freeNodes.Count + constrainedNodes.Count; i++)
            {
                for (int j = 0; j < freeNodes.Count + constrainedNodes.Count; j++)
                {
                    if (i < freeNodes.Count && j < freeNodes.Count)
                    {
                        double Cij = twoNodeCoeff_LAPLACE(freeNodes[i], freeNodes[j]);
                        if (Math.Abs(Cij) > 0.000000001)
                        {
                            alglib.sparseset(a, i, j, Cij);
                        }
                    }
                    else if (j < freeNodes.Count)
                    {
                        int I = i - freeNodes.Count;
                        if (I < constrainedNodes.Count - 1)
                        {
                            alglib.sparseset(a, i, constrainedNodes[I], 1);
                            alglib.sparseset(a, i, constrainedNodes[(I + 1) % 3], -1);
                        }
                        else
                        {
                            List<int> boundaryTrngs = new List<int>();
                            List<int> e_stars = new List<int>();
                            for (int k = 0; k < constrainedNodes.Count; k++)
                            {
                                List<int> neighbs = new List<int>(PointStars[constrainedNodes[k]].neighborTriangles);
                                foreach (int trngNum in neighbs)
                                {
                                    if (trngNum != -100)
                                    {
                                        Triangle trng = Triangles[trngNum];
                                        int I_place = 0;
                                        while (trng.Nodes[I_place] != constrainedNodes[k] && I_place < 3)
                                        {
                                            I_place++;
                                        }
                                        if (!boundaryTrngs.Contains(trngNum) && PointStars[trng.Nodes[(I_place + 1) % 3]].IsFixedCharge && !PointStars[trng.Nodes[(I_place + 2) % 3]].IsFixedCharge)
                                        {
                                            boundaryTrngs.Add(trngNum);
                                            e_stars.Add((I_place + 2) % 3);
                                        }
                                        if (!boundaryTrngs.Contains(trngNum) && PointStars[trng.Nodes[(I_place + 2) % 3]].IsFixedCharge && !PointStars[trng.Nodes[(I_place + 1) % 3]].IsFixedCharge)
                                        {
                                            boundaryTrngs.Add(trngNum);
                                            e_stars.Add((I_place + 1) % 3);
                                        }
                                    }
                                }
                            }
                            for (int l = 0; l < boundaryTrngs.Count; l++)
                            {
                                int e = boundaryTrngs[l];
                                int e_star = e_stars[l];
                                int index;
                                if (e_star == 0)
                                {
                                    index = 0;
                                }
                                else if (e_star == 1)
                                {
                                    index = 3;
                                }
                                else
                                {
                                    index = 5;
                                }
                                double C = elementCoeff_LAPLACE(e)[index];
                                //MessageBox.Show(Triangles[e].Nodes[e_star].ToString());
                                int index1 = freeNodes.FindIndex(0, freeNodes.Count, a => a.Equals(Triangles[e].Nodes[e_star]));
                                int index2 = freeNodes.FindIndex(0, freeNodes.Count, a => a.Equals(Triangles[e].Nodes[(e_star + 1) % 3]));
                                alglib.sparseset(a, i, index1, C);
                                alglib.sparseset(a, i, index2, -C);
                                alglib.sparseset(a, index1, i, C);
                                alglib.sparseset(a, index2, i, -C);

                            }
                            MessageBox.Show(boundaryTrngs.Count.ToString());
                        }
                    }
                }
            }
            alglib.sparseconverttocrs(a);
            for (int i = 0; i < freeNodes.Count; i++)
            {
                b[i] = 0;
                List<int> neighbs = new List<int>(PointStars[freeNodes[i]].neighborTriangles);
                List<int> nodeNeighbs = new List<int>();
                foreach (int trngNum in neighbs)
                {
                    if (trngNum != -100)
                    {
                        Triangle trng = Triangles[trngNum];
                        int I_place = 0;
                        while (trng.Nodes[I_place] != freeNodes[i] && I_place < 3)
                        {
                            I_place++;
                        }
                        if (!nodeNeighbs.Contains(trng.Nodes[(I_place + 1) % 3]) && PointStars[trng.Nodes[(I_place + 1) % 3]].IsBoundary)
                        {
                            nodeNeighbs.Add(trng.Nodes[(I_place + 1) % 3]);
                        }
                        if (!nodeNeighbs.Contains(trng.Nodes[(I_place + 2) % 3]) && PointStars[trng.Nodes[(I_place + 2) % 3]].IsBoundary)
                        {
                            nodeNeighbs.Add(trng.Nodes[(I_place + 2) % 3]);
                        }
                    }
                }
                foreach (int node in nodeNeighbs)
                {
                    b[i] -= twoNodeCoeff_LAPLACE(freeNodes[i], node) * PointStars[node].Phi;
                }
            }
            for (int i = freeNodes.Count; i < freeNodes.Count + constrainedNodes.Count; i++)
            {
                b[i] = 0;
            }
            alglib.linlsqrstate s;
            alglib.linlsqrreport rep;
            double[] x;
            alglib.linlsqrcreate(freeNodes.Count + constrainedNodes.Count, freeNodes.Count + constrainedNodes.Count, out s);
            alglib.linlsqrsolvesparse(s, a, b);
            alglib.linlsqrresults(s, out x, out rep);
            /*alglib.lincgstate s;
            alglib.lincgreport rep;
            double[] x;
            alglib.lincgcreate(freeNodes.Count, out s);
            alglib.lincgsolvesparse(s, a, true, b);
            alglib.lincgresults(s, out x, out rep);*/
            for (int i = 0; i < freeNodes.Count; i++)
            {
                PointStars[freeNodes[i]].Phi = x[i];
            }
            //MessageBox.Show("rep:" + rep.terminationtype);

        }

        public bool HELMHOLTZSolver_TM(out double[] D, out double[,] z)
        {
            List<int> freeNodes = new List<int>();
            List<int> fixNodes = new List<int>();
            for (int i = 0; i < PointStars.Count; i++)
            {
                if (!PointStars[i].IsBoundary)
                {
                    freeNodes.Add(i);
                }
                else
                {
                    fixNodes.Add(i);
                }
            }
            double[,] A = new double[freeNodes.Count, freeNodes.Count];
            double[,] B = new double[freeNodes.Count, freeNodes.Count];
            for (int i = 0; i < freeNodes.Count; i++)
            {
                for (int j = 0; j < freeNodes.Count; j++)
                {
                    double Cij = twoNodeCoeff_TM(freeNodes[i], freeNodes[j]);
                    double Tij = twoNodeT_TM(freeNodes[i], freeNodes[j]);
                    A[i, j] = Cij;
                    B[i, j] = Tij;
                }
            }
            //double[,] z;
            bool result = alglib.smatrixgevd(A, freeNodes.Count, true, B, true, 1, 1, out D, out z);
            return result;
        }
        public bool HELMHOLTZSolver_TE(out double[] D, out double[,] z)
        {
            List<int> freeNodes = new List<int>();
            for (int i = 4; i < PointStars.Count; i++)
            {
                freeNodes.Add(i);
            }
            double[,] A = new double[freeNodes.Count, freeNodes.Count];
            double[,] B = new double[freeNodes.Count, freeNodes.Count];
            for (int i = 0; i < freeNodes.Count; i++)
            {
                for (int j = 0; j < freeNodes.Count; j++)
                {
                    double Cij = twoNodeCoeff_TE(freeNodes[i], freeNodes[j]);
                    double Tij = twoNodeT_TE(freeNodes[i], freeNodes[j]);
                    A[i, j] = Cij;
                    B[i, j] = Tij;
                }
            }
            //double[,] z;
            bool result = alglib.smatrixgevd(A, freeNodes.Count, true, B, true, 0, 1, out D, out z);
            return result;
        }
        public bool HELMHOLTZSolver_TETM(out double[] D, out double[,] z)
        {
            List<int> freeNodes = new List<int>();
            List<int> fixNodes = new List<int>();
            for (int i = 0; i < PointStars.Count; i++)
            {
                if (!PointStars[i].IsBoundary)
                {
                    freeNodes.Add(i);
                }
                else
                {
                    fixNodes.Add(i);
                }
            }
            double[,] A = new double[freeNodes.Count + PointStars.Count - 4, freeNodes.Count + PointStars.Count - 4];
            double[,] B = new double[freeNodes.Count + PointStars.Count - 4, freeNodes.Count + PointStars.Count - 4];
            for (int i = 0; i < freeNodes.Count; i++)
            {
                for (int j = 0; j < freeNodes.Count; j++)
                {
                    double Cij = twoNodeCoeff_TM(freeNodes[i], freeNodes[j]);
                    double Tij = twoNodeT_TM(freeNodes[i], freeNodes[j]);
                    A[i, j] = Cij;
                    B[i, j] = Tij;
                }
            }
            for (int i = 0; i < PointStars.Count - 4; i++)
            {
                for (int j = 0; j < PointStars.Count - 4; j++)
                {
                    double Cij = twoNodeCoeff_TE(i + 4, j + 4);
                    double Tij = twoNodeT_TE(i + 4, j + 4);
                    A[freeNodes.Count + i, freeNodes.Count + j] = Cij;
                    B[freeNodes.Count + i, freeNodes.Count + j] = Tij;
                }
            }
            bool result = alglib.smatrixgevd(A, freeNodes.Count + PointStars.Count - 4, true, B, true, 0, 1, out D, out z);
            return result;
        }

        public bool HELMHOLTZSolver_TM_beta(double freq, out double[] D, out double[,] z)
        {
            double c0 = 300000000;
            double k0 = 2 * Math.PI * freq / c0;
            List<int> freeNodes = new List<int>();
            List<int> fixNodes = new List<int>();
            for (int i = 0; i < PointStars.Count; i++)
            {
                if (!PointStars[i].IsBoundary)
                {
                    freeNodes.Add(i);
                }
                else
                {
                    fixNodes.Add(i);
                }
            }
            double[,] A = new double[freeNodes.Count, freeNodes.Count];
            double[,] B = new double[freeNodes.Count, freeNodes.Count];
            for (int i = 0; i < freeNodes.Count; i++)
            {
                for (int j = 0; j < freeNodes.Count; j++)
                {
                    double Cij = twoNodeCoeff_TM(freeNodes[i], freeNodes[j]);
                    double Tij = twoNodeT_TM(freeNodes[i], freeNodes[j]);
                    A[i, j] = Cij - k0 * k0 * Tij;
                    B[i, j] = twoNodeCoeff_TE(freeNodes[i], freeNodes[j]);
                }
            }
            //double[,] z;
            bool result = alglib.smatrixgevd(A, freeNodes.Count, true, B, true, 0, 1, out D, out z);
            return result;
        }
        public bool HELMHOLTZSolver_TM_beta2(double freq, double beta2, out double[] D, out double[,] z)
        {
            double c0 = 300000000;
            double k0 = 2 * Math.PI * freq / c0;
            List<int> freeNodes = new List<int>();
            List<int> fixNodes = new List<int>();
            for (int i = 0; i < PointStars.Count; i++)
            {
                if (!PointStars[i].IsBoundary)
                {
                    freeNodes.Add(i);
                }
                else
                {
                    fixNodes.Add(i);
                }
            }
            double[,] A = new double[freeNodes.Count, freeNodes.Count];
            double[,] B = new double[freeNodes.Count, freeNodes.Count];
            for (int i = 0; i < freeNodes.Count; i++)
            {
                for (int j = 0; j < freeNodes.Count; j++)
                {
                    double Cij = twoNodeCoeff_TM(freeNodes[i], freeNodes[j]);
                    double Tij = twoNodeT_TM(freeNodes[i], freeNodes[j]);
                    A[i, j] = Cij - k0 * k0 * Tij + beta2 * beta2 * twoNodeCoeff_TE2(freeNodes[i], freeNodes[j]);
                    B[i, j] = twoNodeCoeff_TE(freeNodes[i], freeNodes[j]);
                }
            }
            //double[,] z;
            bool result = alglib.smatrixgevd(A, freeNodes.Count, true, B, true, 0, 1, out D, out z);
            return result;
        }
        public bool HELMHOLTZSolver_TM_beta_second_order(double freq, out double[] D, out double[,] z)
        {
            double c0 = 300000000;
            double k0 = 2 * Math.PI * freq / c0;
            List<int> freeNodes = new List<int>();
            List<int> fixNodes = new List<int>();
            for (int i = 0; i < PointStars.Count; i++)
            {
                if (!PointStars[i].IsBoundary)
                {
                    freeNodes.Add(i);
                }
                else
                {
                    fixNodes.Add(i);
                }
            }
            double[,] A = new double[2 * freeNodes.Count, 2 * freeNodes.Count];
            double[,] B = new double[2 * freeNodes.Count, 2 * freeNodes.Count];
            for (int i = 0; i < freeNodes.Count; i++)
            {
                for (int j = 0; j < freeNodes.Count; j++)
                {
                    A[i, j] = -twoNodeCoeff_TE(freeNodes[i], freeNodes[j]);
                    A[i, j + freeNodes.Count] = k0 * k0 * twoNodeT_TM(freeNodes[i], freeNodes[j]) - twoNodeCoeff_TM(freeNodes[i], freeNodes[j]);
                    A[i + freeNodes.Count, j] = k0 * k0 * twoNodeT_TM(freeNodes[i], freeNodes[j]) - twoNodeCoeff_TM(freeNodes[i], freeNodes[j]);
                    A[i + freeNodes.Count, j + freeNodes.Count] = 0;
                    B[i, j] = twoNodeCoeff_TE2(freeNodes[i], freeNodes[j]);
                    B[i + freeNodes.Count, j] = 0;
                    B[i, j + freeNodes.Count] = 0;
                    B[i + freeNodes.Count, j + freeNodes.Count] = k0 * k0 * twoNodeT_TM(freeNodes[i], freeNodes[j]) - twoNodeCoeff_TM(freeNodes[i], freeNodes[j]);
                }
            }
            //double[,] z;
            bool result = alglib.smatrixgevd(A, 2 * freeNodes.Count, true, B, true, 0, 1, out D, out z);
            return result;
        }
    }
}
