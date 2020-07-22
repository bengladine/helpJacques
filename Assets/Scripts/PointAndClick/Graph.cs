using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PointAndClick
{
    public class Node
    {
        public Vertex Vertex;
        public Node(Vertex vertex)
        {
            Vertex = vertex;
        }
    }
    public class Graph : MonoBehaviour
    {
        public List<Polygon> Polygons = new List<Polygon>();

        private List<Vertex> _concaveVertices = new List<Vertex>();
        private List<Node> _nodes = new List<Node>();
        private List<List<Edge>> _edges = new List<List<Edge>>();

        void Start()
        {
            bool firstPoly = true;
            for (int i = 0; i < Polygons.Count; i++)
            {
                if (Polygons[i].Points.Count > 2)
                {
                    for (int j = 0; j < Polygons[i].Points.Count; j++)
                    {
                        if (Helpers.IsVertexConcave(Polygons[i].Points, j) == firstPoly)
                        {
                            _concaveVertices.Add(Polygons[i].Points[j]);
                            AddNode(new Node(Polygons[i].Points[j]));
                        }
                    }
                }

                firstPoly = false;
            }

            for (int i = 0; i < _concaveVertices.Count; i++)
            {
                Vertex a = _concaveVertices[i];
                for (int j = 0; j < _concaveVertices.Count; j++)
                {
                    Vertex b = _concaveVertices[j];
                    if (IsInLineOfSight(a, b))
                    {

                    }
                }
            }
        }


        private bool IsInLineOfSight(Vertex start, Vertex end)
        {
            float epsilon = 0.5f;

            // if start or end is outside of polygon
            if (!Polygons[0].IsPointInPolygon(start.Position) || !Polygons[0].IsPointInPolygon(end.Position))
                return false;

            // if start and end are the same or too close to eachother
            if (Vector2.Distance(start.Position, end.Position) < epsilon)
                return false;

            // not in LOS if any edge is internsected by the start-end line segment
            bool isInSight = true;
            foreach (var polygon in Polygons)
            {
                for (int i = 0; i < polygon.Points.Count; i++)
                {
                    Vector2 v1 = polygon.Points[i].Position;
                    Vector2 v2 = polygon.Points[Helpers.ClampListIndex(i + 1, polygon.Points.Count)].Position;
                    if (Helpers.LineSegmentsCross(start.Position, end.Position, v1, v2))
                    {
                        if (Helpers.DistanceToSegment(start.Position, v1, v2) > 0.5f && Helpers.DistanceToSegment(end.Position, v1, v2) > 0.5f)
                        {
                            return false;
                        }
                    }
                }
            }

            //Middle point in segment determines if in LOS or not
            Vector2 v = (start.Position + end.Position) / 2.0f;
            bool isInside = Polygons[0].IsPointInPolygon(v);
            for (int i = 1; i < Polygons.Count; i++)
            {
                if (Polygons[i].IsPointInPolygon(v, false))
                {
                    return false;
                }
            }
            return true;
        }

        private void AddEdge(Edge edge)
        {
            if (GetEdge(edge.From, edge.To) == null)
            {
                _edges[edge.From].Add(edge);
            }

            if (GetEdge(edge.To, edge.From) == null)
            {
                _edges[edge.To].Add(new Edge(edge.To, edge.From, edge.Cost));
            }
        }

        public Edge GetEdge(int from, int to)
        {
            var fromEdges = _edges[from];
            for (int i = 0; i < fromEdges.Count; i++)
            {
                if (fromEdges[i].To == to)
                {
                    return fromEdges[i];
                }
            }

            return null;
        }

        public void AddNode(Node node)
        {
            _nodes.Add(node);
            _edges.Add(new List<Edge>());
        }
    }
}

