using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edge
{
    public Vertex V1;
    public Vertex V2;

    //Is this edge intersecting with another edge?
    public bool IsIntersecting = false;

    public Edge(Vertex v1, Vertex v2)
    {
        this.V1 = v1;
        this.V2 = v2;
    }

    public Edge(Vector3 v1, Vector3 v2)
    {
        this.V1 = new Vertex(v1);
        this.V2 = new Vertex(v2);
    }

    //Get vertex in 2d space (assuming x, y)
    public Vector2 GetVertex2D(Vertex v)
    {
        return new Vector2(v.Position.x, v.Position.y);
    }

    //Flip edge
    public void FlipEdge()
    {
        Vertex temp = V1;
        V1 = V2;
        V2 = temp;
    }
}
