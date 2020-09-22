using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PointAndClick
{
    public class Graph
    {
        public Graph()
        {

        }
        public Graph(Graph graph)
        {
            Polygons = graph.Polygons;
            _concaveVertices = graph._concaveVertices;
            Nodes = graph.Nodes;
            Edges = graph.Edges;
        }


        public List<Polygon> Polygons = new List<Polygon>();

        private List<Vector2> _concaveVertices = new List<Vector2>();
        public List<Node> Nodes = new List<Node>();
        public List<List<Edge>> Edges = new List<List<Edge>>();

        public void AddEdge(Edge edge)
        {
            if (GetEdge(edge.From, edge.To) == null)
            {
                Edges[edge.From].Add(edge);
            }

            if (GetEdge(edge.To, edge.From) == null)
            {
                Edges[edge.To].Add(new Edge(edge.To, edge.From, edge.Cost));
            }
        }

        public Edge GetEdge(int from, int to)
        {
            var fromEdges = Edges[from];
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
            Nodes.Add(node);
            Edges.Add(new List<Edge>());
        }

        public Graph Clone()
        {
            var graph = new Graph();
            graph.Polygons = new List<Polygon>(Polygons);
            graph._concaveVertices =  new List<Vector2>(_concaveVertices);
            graph.Nodes = new List<Node>(Nodes);
            graph.Edges = new List<List<Edge>>();

            foreach (var edge in Edges)
            {
                graph.Edges.Add(new List<Edge>(edge));
            }

            return graph;
        }
    }
}

