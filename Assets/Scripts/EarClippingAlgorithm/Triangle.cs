using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Triangle
{
    //Corners
    public Vertex V1;
    public Vertex V2;
    public Vertex V3;

    //If we are using the half edge mesh structure, we just need one half edge
    public HalfEdge halfEdge;

    public Triangle(Vertex v1, Vertex v2, Vertex v3)
    {
        this.V1 = v1;
        this.V2 = v2;
        this.V3 = v3;
    }

    public Triangle(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        this.V1 = new Vertex(v1);
        this.V2 = new Vertex(v2);
        this.V3 = new Vertex(v3);
    }

    public Triangle(HalfEdge halfEdge)
    {
        this.halfEdge = halfEdge;
    }

    //Change orientation of triangle from cw -> ccw or ccw -> cw
    public void ChangeOrientation()
    {
        Vertex temp = this.V1;

        this.V1 = this.V2;

        this.V2 = temp;
    }

    public Vertex[] GetVertices()
    {
        Vertex[] vertices = { V1, V2, V3 };
        return vertices;
    }
}