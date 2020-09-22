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

    public Node(Vector2 vertex)
    {
        Vertex = vertex;
        IsVisited = false;
        GlobalGoal = 0.0f;
        LocalGoal = 0.0f;
        Parent = null;
        Neighbours = new List<Node>();
    }

}
