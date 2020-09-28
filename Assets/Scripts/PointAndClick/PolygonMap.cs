using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PointAndClick
{
    public class PolygonMap
    {
        public Graph _mainWalkGraph;
        public List<Polygon> Polygons = new List<Polygon>();
        private List<Vector2> _concaveVertices = new List<Vector2>();

        private int _startIndex;
        private int _endIndex;
        public PolygonMap(List<Polygon> polygons)
        {
            Polygons = polygons;
            CreateGraph();
        }

        public bool IsInLineOfSight(Vector2 start, Vector2 end)
        {
            // if start or end is outside of polygon
            //if (!Polygons[0].IsPointInPolygon(start) || !Polygons[0].IsPointInPolygon(end))
            //    return false;

            // if start and end are the same or too close to eachother
            if (Vector2.Distance(start, end) < 0.001f)
                return true;

            // not in LOS if any edge is internsected by the start-end line segment
            foreach (var polygon in Polygons)
            {
                for (int i = 0; i < polygon.Points.Count; i++)
                {
                    Vector2 v1 = polygon.Points[i];
                    Vector2 v2 = polygon.Points[(i + 1) % polygon.Points.Count];

                    var distStartToSegment = Helpers.DistanceToSegment(start, v1, v2);
                    var distEndToSegment = Helpers.DistanceToSegment(end, v1, v2);
                                                         
                    if (Helpers.AreIntersecting(start.x, start.y, end.x, end.y, v1.x, v1.y, v2.x, v2.y))
                    {
                        if (distStartToSegment > 0.001f && distEndToSegment > 0.001f)
                        {
                            return false;
                        }
                        else if (distStartToSegment < 0.001f && distEndToSegment < 0.001f)
                            return true;
                    }

                    if ((Helpers.Distance(v1, end) < float.Epsilon || Helpers.Distance(v2, end) < float.Epsilon) &&
                distStartToSegment < 0.0001f)
                    {
                        return true;
                    }
                }
            }

            //Middle point in segment determines if in LOS or not
            Vector2 v = (start + end) / 2.0f;
            bool isInside = Polygons[0].IsPointInPolygon(v);
            for (int i = 1; i < Polygons.Count; i++)
            {
                if (Polygons[i].IsPointInPolygon(v))
                {
                    isInside = false;
                    break;
                }
            }

            return isInside;
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
                    if (IsInLineOfSight(a, b) && i != j)
                    {
                        _mainWalkGraph.Nodes[i].Neighbours.Add(_mainWalkGraph.Nodes[j]);
                    }
                }
            }
        }

        private bool AreConsecutiveVertices(Vector2 a, Vector2 b)
        {
            foreach (var poly in Polygons)
            {
                for (int i = 0; i < poly.Points.Count; i++)
                {
                    var p1 = poly.Points[i];
                    var p2 = poly.Points[(i + 1) % poly.Points.Count];

                    if (Vector2.Distance(p1, a) < float.Epsilon && Vector2.Distance(p2, b) < float.Epsilon)
                        return true;
                }
            }

            return false;
        }

        public List<Vector2> CalculatePath(Vector2 start, Vector2 end)
        {
            if (!Polygons[0].IsPointInPolygon(start))
            {
                start = Polygons[0].GetClosestPointOnEdge(start, false);
            }
            if (!Polygons[0].IsPointInPolygon(end))
            {
                end = Polygons[0].GetClosestPointOnEdge(end, false);
            }

            // are there more polygons? Then check if endpoint is inside one of them and if so find closest point on edge
            if (Polygons.Count > 1)
            {
                bool startChanged = false;
                bool endChanged = false;
                for (int i = 1; i < Polygons.Count; i++)
                {
                    if (Polygons[i].IsPointInPolygon(start))
                    {
                        start = Polygons[i].GetClosestPointOnEdge(start, true);
                        startChanged = true;
                    }

                    if (Polygons[i].IsPointInPolygon(end))
                    {
                        end = Polygons[i].GetClosestPointOnEdge(end, true);
                        endChanged = true;
                    }

                    if (startChanged && endChanged) break;
                }
            }

            // create new node on start position
            _startIndex = _mainWalkGraph.Nodes.Count;
            var startNode = new Node(start);
            _mainWalkGraph.Nodes.Add(startNode);

            //create new node on endposition
            _endIndex = _mainWalkGraph.Nodes.Count;
            var endNode = new Node(end);
            _mainWalkGraph.Nodes.Add(endNode);

            if (IsInLineOfSight(start, end))
            {
                startNode.Neighbours.Add(endNode);
                endNode.Neighbours.Add(startNode);
            }
            else
            {
                for (int i = 0; i < _mainWalkGraph.Nodes.Count; i++)
                {
                    var c = _mainWalkGraph.Nodes[i];
                    if (IsInLineOfSight(start, c.Vertex))
                    {
                        startNode.Neighbours.Add(c);
                        c.Neighbours.Add(startNode);
                        c.startIndex = c.Neighbours.Count - 1;
                    }

                    if (IsInLineOfSight(end, c.Vertex))
                    {
                        endNode.Neighbours.Add(c);
                        c.Neighbours.Add(endNode);
                        c.endIndex = c.Neighbours.Count - 1;
                    }
                }
            }

            var aStar = new AStarAlgorithm(_mainWalkGraph.Nodes, startNode, endNode);

            ResetWalkGraph();
            return aStar.GetPath();
        }

        public void DrawLineOfSight()
        {
            if (_mainWalkGraph != null)
            {
                var nodes = _mainWalkGraph.Nodes;
                for (int i = 0; i < nodes.Count; ++i)
                {
                    var v1 = nodes[i].Vertex;

                    for (int j = 0; j < nodes.Count; j++)
                    {
                        var v2 = nodes[j].Vertex;
                        if ((i != j && IsInLineOfSight(v1, v2)) || AreConsecutiveVertices(v1, v2))
                        {
                            Color orange = new Color(1.0f, 0.388f, 0.2784f);
                            Debug.DrawLine(v1, v2, orange);
                        }
                    }
                }
            }
        }

        public void DrawNodes()
        {
            if (_mainWalkGraph.Nodes.Count > 0)
            {
                foreach (var node in _mainWalkGraph.Nodes)
                {
                    Gizmos.color = Color.black;
                    Gizmos.DrawSphere(node.Vertex, 0.05f);
                }
            }
        }

        public void DrawNeighbours()
        {
            if (_mainWalkGraph != null)
            {
                foreach (var node in _mainWalkGraph.Nodes)
                {
                    foreach (var neighbour in node.Neighbours)
                    {
                        Debug.DrawLine(node.Vertex, neighbour.Vertex, Color.black);
                    }
                }
            }
        }

        private void ResetWalkGraph()
        {
            if (_startIndex >= 0 && _endIndex >= 0)
            {
                _mainWalkGraph.Nodes.RemoveAt(_endIndex);
                _mainWalkGraph.Nodes.RemoveAt(_startIndex);
            }

            foreach (var node in _mainWalkGraph.Nodes)
            {
                if (node.endIndex >= 0)
                {
                    node.Neighbours.RemoveAt(node.endIndex);
                    node.endIndex = -1;
                }

                if (node.startIndex >= 0)
                {
                    node.Neighbours.RemoveAt(node.startIndex);
                    node.startIndex = -1;
                }
            }

        }
    }
}
