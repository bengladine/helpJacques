using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PointAndClick
{
    public class PolygonMap
    {
        private Graph _mainWalkGraph;
        private List<Polygon> Polygons = new List<Polygon>();
        private List<Vector2> _concaveVertices = new List<Vector2>();

        public PolygonMap(List<Polygon> polygons)
        {
            Polygons = polygons;
            CreateGraph();
        }

        public Vector2 GetTarget(int nodeIndex)
        {
            return _mainWalkGraph.Nodes[nodeIndex].Vertex;
        }

        private bool IsInLineOfSight(Vector2 start, Vector2 end)
        {
            float epsilon = 0.5f;

            // if start or end is outside of polygon
            if (!Polygons[0].IsPointInPolygon(start) || !Polygons[0].IsPointInPolygon(end))
                return false;

            // if start and end are the same or too close to eachother
            if (Vector2.Distance(start, end) < epsilon)
                return false;

            // not in LOS if any edge is internsected by the start-end line segment
            bool isInSight = true;
            foreach (var polygon in Polygons)
            {
                for (int i = 0; i < polygon.Points.Count; i++)
                {
                    Vector2 v1 = polygon.Points[i];
                    Vector2 v2 = polygon.Points[Helpers.ClampListIndex(i + 1, polygon.Points.Count)];
                    if (Helpers.LineSegmentsCross(start, end, v1, v2))
                    {
                        if (Helpers.DistanceToSegment(start, v1, v2) > 0.5f && Helpers.DistanceToSegment(end, v1, v2) > 0.5f)
                        {
                            return false;
                        }
                    }
                }
            }

            //Middle point in segment determines if in LOS or not
            Vector2 v = (start + end) / 2.0f;
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

        private void CreateGraph()
        {
            _mainWalkGraph = new Graph();

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
                            _mainWalkGraph.AddNode(new Node(Polygons[i].Points[j]));
                        }
                    }
                }

                firstPoly = false;
            }

            for (int i = 0; i < _concaveVertices.Count; i++)
            {
                Vector2 a = _concaveVertices[i];
                for (int j = 0; j < _concaveVertices.Count; j++)
                {
                    Vector2 b = _concaveVertices[j];
                    if (IsInLineOfSight(a, b))
                    {
                        _mainWalkGraph.AddEdge(new Edge(i, j, Vector2.Distance(a, b)));
                    }
                }
            }
        }

        public List<int> CalculatePath(Vector2 start, Vector2 end)
        {
             var walkGraph = new Graph(_mainWalkGraph);

            float minDistanceFrom = Mathf.Infinity;
            float minDistanceTo = Mathf.Infinity;

            // create new node on start position
            int startNodeIndex = walkGraph.Nodes.Count;

            if (!Polygons[0].IsPointInPolygon(start))
            {
                start = Polygons[0].GetClosestPointOnEdge(start);
            }
            if (!Polygons[0].IsPointInPolygon(end))
            {
                end = Polygons[0].GetClosestPointOnEdge(end);
            }

            // are there more polygons? Then check if endpoint is inside one of them and ifind closest point on edge
            if (Polygons.Count > 1)
            {
                for (int i = 1; i < Polygons.Count; i++)
                {
                    if (Polygons[i].IsPointInPolygon(end))
                    {
                        end = Polygons[i].GetClosestPointOnEdge(end);
                        break;
                    }
                }
            }

            var startNode = new Node(new Vector2(start.x, start.y));
            walkGraph.AddNode(startNode);

            for (int i = 0; i < _concaveVertices.Count; i++)
            {
                var c = _concaveVertices[i];
                if (IsInLineOfSight(start, c))
                    walkGraph.AddEdge(new Edge(startNodeIndex, i, Helpers.Distance(start, c)));
            }

            //create new node on endposition
            int endNodeIndex = walkGraph.Nodes.Count;

            var endNode = new Node(end);
            walkGraph.AddNode(endNode);

            for (int i = 0; i < _concaveVertices.Count; i++)
            {
                var c = _concaveVertices[i];
                if (IsInLineOfSight(end, c))
                    walkGraph.AddEdge(new Edge(i, endNodeIndex, Helpers.Distance(end, c)));
            }

            if (IsInLineOfSight(start, end))
                walkGraph.AddEdge(new Edge(startNodeIndex, endNodeIndex, Helpers.Distance(start, end)));

            var aStar = new AStarAlgorithm(walkGraph, startNodeIndex, endNodeIndex);
            return aStar.GetPath();
        }
    }
}
