using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public Vector2 Vertex;
    public bool IsVisited;
    public float GlobalGoal;
    public float LocalGoal;
    public Node Parent;
    public List<Node> Neighbours;

    // if start or end indices are another value than -1 (and positive), this means that the start and/or end node are a neighbour. 
    // These have to be after every calculation.

    public int startIndex = -1;
    public int endIndex = -1;

    public Node(Vector2 vertex)
    {
        Vertex = vertex;
        IsVisited = false;
        GlobalGoal = 0.0f;
        LocalGoal = 0.0f;
        Parent = null;
        Neighbours = new List<Node>();
    }

    public Node Clone()
    {
        Node newNode = new Node(Vertex);
        newNode.IsVisited = false;
        newNode.Neighbours = new List<Node>();
        foreach (var node in Neighbours)
        {
            newNode.Neighbours.Add(node);
        }

        return newNode;
    }
}
