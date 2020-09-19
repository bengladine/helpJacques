using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PointAndClick
{
    public class AStarAlgorithm
    {
        private Graph _graph;
        private int _start;
        private int _end;
        private List<Edge> _shortestPathTree = new List<Edge>();
        private List<Edge> _searchFrontier = new List<Edge>();
        private List<float> _gCosts = new List<float>();
        private List<float> _fCosts = new List<float>();

        public AStarAlgorithm(Graph graph, int start, int end)
        {
            _graph = graph;
            _start = start;
            _end = end;

            for (int i = 0; i < _graph.Nodes.Count; i++)
            {
                _shortestPathTree.Add(null);
                _searchFrontier.Add(null);
                _gCosts.Add(0.0f);
                _fCosts.Add(0.0f);
            }

            Search();
        }
        
        private void Search()
        {
            var pq = new IndexPriorityQueue(_fCosts);
            pq.Insert(_start);
            while (!pq.IsEmpty())
            {
                int NextClosestNode = pq.Pop();
                _shortestPathTree[NextClosestNode] = _searchFrontier[NextClosestNode];
                if (NextClosestNode == _end) return;
                var edges = _graph.Edges[NextClosestNode];
                foreach (var edge in edges)
                {
                    float hCost = (_graph.Nodes[edge.To].Vertex - _graph.Nodes[_end].Vertex).magnitude;
                    float gCost = _gCosts[NextClosestNode] + edge.Cost;
                    int to = edge.To;

                    if (_searchFrontier[edge.To] == null)
                    {
                        _fCosts[edge.To] = gCost + hCost;
                        _gCosts[edge.To] = gCost;
                        pq.Insert(edge.To);
                        _searchFrontier[edge.To] = edge;
                    }
                    else if ((gCost < _gCosts[edge.To]) && (_shortestPathTree[edge.To] == null))
                    {
                        _fCosts[edge.To] = gCost + hCost;
                        _gCosts[edge.To] = gCost;
                        pq.ReorderUp();
                        _searchFrontier[edge.To] = edge;
                    }
                }
            }
        }

        public List<int> GetPath()
        {
            List<int> path = new List<int>();
            if (_end < 0) return path;
            int end = _end;
            path.Add(end);
            while ((end != _start) && (_shortestPathTree[end] != null))
            {
                end = _shortestPathTree[end].From;
                path.Add(end);
            }

            path.Reverse();
            return path;
        }

    }
}

