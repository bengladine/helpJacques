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

        private Node _START;
        private Node _END;

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

        public AStarAlgorithm(Graph graph, Node start, Node end)
        {
            _graph = graph;
            _START = start;
            _END = end;

            SolveAStar();
        }

        private void SolveAStar()
        {
            var nodes = _graph.Nodes;
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].IsVisited = false;
                nodes[i].GlobalGoal = Mathf.Infinity;
                nodes[i].LocalGoal = Mathf.Infinity;
                nodes[i].Parent = null;
            }

            var currentNode = _START;
            _START.LocalGoal = 0.0f;
            _START.GlobalGoal = Vector2.Distance(_START.Vertex, _END.Vertex);

            List<Node> listNotTestedNodes = new List<Node>();
            listNotTestedNodes.Add(_START);

            while (listNotTestedNodes.Count > 0)
            {
                listNotTestedNodes.Sort((x, y) =>
                {
                    if (x.GlobalGoal < y.GlobalGoal) return 0;
                    return 1;
                });

                while (listNotTestedNodes.Count > 0 && listNotTestedNodes[0].IsVisited)
                    listNotTestedNodes.RemoveAt(0);

                if (listNotTestedNodes.Count == 0)
                    break;

                currentNode = listNotTestedNodes[0];
                currentNode.IsVisited = true;

                for (int i = 0; i < currentNode.Neighbours.Count; i++)
                {
                    var neighbour = currentNode.Neighbours[i];
                    if (!neighbour.IsVisited)
                        listNotTestedNodes.Add(neighbour);
                    float possiblyLowerGoal = currentNode.LocalGoal + Vector2.Distance(currentNode.Vertex, neighbour.Vertex);

                    if (possiblyLowerGoal < neighbour.LocalGoal)
                    {
                        neighbour.Parent = currentNode;
                        neighbour.LocalGoal = possiblyLowerGoal;
                        neighbour.GlobalGoal = neighbour.LocalGoal + Vector2.Distance(neighbour.Vertex, _END.Vertex);
                    }
                }
            }


        }

        private void Search()
        {
            var pq = new IndexPriorityQueue(_fCosts);
            pq.Insert(_start);
            while (!pq.IsEmpty())
            {
                int NextClosestNode = pq.Pop();
                _shortestPathTree[NextClosestNode] = _searchFrontier[NextClosestNode];
                //  if (NextClosestNode == _end) return;
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

        //public List<int> GetPath()
        //{
        //    List<int> path = new List<int>();
        //    if (_end < 0) return path;
        //    int end = _end;
        //    path.Add(end);
        //    while ((end != _start) && (_shortestPathTree[end] != null))
        //    {
        //        end = _shortestPathTree[end].From;
        //        path.Add(end);
        //    }

        //    path.Reverse();
        //    return path;
        //}

        public List<Vector2> GetPath()
        {
            List<Vector2> path = new List<Vector2>();
            if (_END != null)
            {
                path.Add(_END.Vertex);
                var p = _END;
                while (p.Parent != null)
                {
                    path.Add(p.Parent.Vertex);
                    p = p.Parent;
                }

                path.Reverse();
            }
            return path;
        }
    }
}

