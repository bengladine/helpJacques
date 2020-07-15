using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HalfEdge
{
    //The vertex the edge points to
    public Vertex V;

    //The face this edge is a part of
    public Triangle T;

    //The next edge
    public HalfEdge NextEdge;
    //The previous
    public HalfEdge PrevEdge;
    //The edge going in the opposite direction
    public HalfEdge OppositeEdge;

    //This structure assumes we have a vertex class with a reference to a half edge going from that vertex
    //and a face (triangle) class with a reference to a half edge which is a part of this face 
    public HalfEdge(Vertex v)
    {
        this.V = v;
    }
}
