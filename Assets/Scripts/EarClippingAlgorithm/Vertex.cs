using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vertex
{
    public Vector3 Position;

    //The outgoing halfedge (a halfedge that starts at this vertex)
    //Doesnt matter which edge we connect to it
    public HalfEdge HalfEdge;

    //Which triangle is this vertex a part of?
    public Triangle Triangle;

    //The previous and next vertex this vertex is attached to
    public Vertex PrevVertex;
    public Vertex NextVertex;

    //Properties this vertex may have
    //Reflex is concave
    public bool IsReflex;
    public bool IsConvex;
    public bool IsEar;

    public int Index;


    public Vertex(Vector3 position)
    {
        Position = position;
    }

    public Vertex(Vector3 pos, int index)
    {
        Position = pos;
        Index = index;
    }

    //Get 2d pos of this vertex
    public Vector2 GetPos2D_XY()
    {
        Vector2 pos_2d_xy = new Vector2(Position.x, Position.y);
        return pos_2d_xy;
    }
}
