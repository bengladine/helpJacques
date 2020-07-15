using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Vertex = PointAndClick.Vertex;

namespace PointAndClick
{
    public class Node
    {
        public Vector3 Position;
        public Node(Vector3 position)
        {
            Position = position;
        }
    }
    public class Graph : MonoBehaviour
    {
        public List<Polygon> Polygons = new List<Polygon>();

        private List<Vertex> _concaveVertices = new List<Vertex>();

        // Start is called before the first frame update
        void Start()
        {
            bool firstPoly = true;
            for (int i = 0; i < Polygons.Count; i++)
            {
                if (Polygons[i].Points.Count > 2)
                {
                    for (int j = 0; j < Polygons[i].Points.Count; j++)
                    {
                        if (IsVertexConcave(Polygons[i].Points, j) == firstPoly)
                        {
                            _concaveVertices.Add(Polygons[i].Points[j]);
                        }
                    }
                }

                firstPoly = false;
            }
            foreach (var polygon in Polygons)
            {

            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

