using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Copyright(c) 2020 Erik Nordeus

public class EarClipping
{
    public List<Triangle> TriangulateConcavePolygon(List<Vector3> points)
    {
        List<Vertex> vertices = new List<Vertex>();
        List<Triangle> triangles = new List<Triangle>();

        //If we just have three points, then we dont have to do all calculations
        if (points.Count == 3)
        {
            Vertex p1 = new Vertex(points[0], 0);
            Vertex p2 = new Vertex(points[1], 1);
            Vertex p3 = new Vertex(points[2], 2);

            triangles.Add(new Triangle(p1, p2, p3));
            return triangles;
        }

        //Step 1. Store the vertices in a list and we also need to know the next and prev vertex
        for (int i = 0; i < points.Count; i++)
        {
            vertices.Add(new Vertex(points[i], i));
        }

        //Find the next and previous vertex
        for (int i = 0; i < vertices.Count; i++)
        {
            int nextPos = ClampListIndex(i + 1, vertices.Count);
            int prevPos = ClampListIndex(i - 1, vertices.Count);
            vertices[i].PrevVertex = vertices[prevPos];
            vertices[i].NextVertex = vertices[nextPos];
        }

        //Step 2. Find the reflex (concave) and convex vertices, and ear vertices
        for (int i = 0; i < vertices.Count; i++)
        {
            CheckIfReflexOrConvex(vertices[i]);
        }

        //Have to find the ears after we have found if the vertex is reflex or convex
        List<Vertex> earVertices = new List<Vertex>();

        for (int i = 0; i < vertices.Count; i++)
        {
            IsVertexEar(vertices[i], vertices, earVertices);
        }

        while (true)
        {
            //This means we have just one triangle left
            if (vertices.Count == 3)
            {
                //The final triangle
                triangles.Add(new Triangle(vertices[0], vertices[0].PrevVertex, vertices[0].NextVertex));

                break;
            }

            if (earVertices.Count == 2)
                Debug.Log("");

            if (earVertices.Count == 0)
                Debug.Log("");
            //Make a triangle of the first ear
            Vertex earVertex = earVertices[0];

            Vertex earVertexPrev = earVertex.PrevVertex;
            Vertex earVertexNext = earVertex.NextVertex;

            Triangle newTriangle = new Triangle(earVertex, earVertexPrev, earVertexNext);

            triangles.Add(newTriangle);

            //Remove the vertex from the lists
            earVertices.Remove(earVertex);

            vertices.Remove(earVertex);
        
            //Update the previous vertex and next vertex
            earVertexPrev.NextVertex = earVertexNext;
            earVertexNext.PrevVertex = earVertexPrev;

            //...see if we have found a new ear by investigating the two vertices that was part of the ear
            CheckIfReflexOrConvex(earVertexPrev);
            CheckIfReflexOrConvex(earVertexNext);

            earVertices.Remove(earVertexPrev);
            earVertices.Remove(earVertexNext);

            IsVertexEar(earVertexPrev, vertices, earVertices);
            IsVertexEar(earVertexNext, vertices, earVertices);
        }

        //Debug.Log(triangles.Count);

        return triangles;
    }

    public int ClampListIndex(int index, int listSize)
    {
        index = ((index % listSize) + listSize) % listSize;

        return index;
    }

    public void CheckIfReflexOrConvex(Vertex v)
    {
        v.IsReflex = false;
        v.IsConvex = false;

        //This is a reflex vertex if its triangle is oriented clockwise
        Vector2 a = v.PrevVertex.GetPos2D_XY();
        Vector2 b = v.GetPos2D_XY();
        Vector2 c = v.NextVertex.GetPos2D_XY();

        if (IsTriangleOrientedClockWise(a, b, c))
        {
            v.IsReflex = true;
        }
        else
        {
            v.IsConvex = true;
        }
    }

    public bool IsTriangleOrientedClockWise(Vector2 p1, Vector2 p2, Vector2 p3)
    {
        bool isClockWise = true;
        float determinant = p1.x * p2.y + p3.x * p1.y + p2.x * p3.y - p1.x * p3.y - p3.x * p2.y - p2.x * p1.y;

        if (determinant > 0.0f)
        {
            isClockWise = false;
        }

        return isClockWise;
    }

    public void IsVertexEar(Vertex v, List<Vertex> vertices, List<Vertex> earVertices)
    {
        //A reflex vertex cant be an ear!
        if (v.IsReflex)
        {
            return;
        }

        //This triangle to check point in triangle
        Vector2 a = v.PrevVertex.GetPos2D_XY();
        Vector2 b = v.GetPos2D_XY();
        Vector2 c = v.NextVertex.GetPos2D_XY();

        bool hasPointInside = false;

        for (int i = 0; i < vertices.Count; i++)
        {
            //We only need to check if a reflex vertex is inside of the triangle
            if (vertices[i].IsReflex)
            {
                Vector2 p = vertices[i].GetPos2D_XY();

                //This means inside and not on the hull
                if (GeometryFunctions.IsPointInTriangle(a, b, c, p))
                {
                    hasPointInside = true;

                    break;
                }
            }
        }

        if (!hasPointInside)
        {
            earVertices.Add(v);
        }
    }
}
