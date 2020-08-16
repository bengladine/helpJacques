using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helpers
{
    // clamps an index postively as negatively to ensure the index will never go out of bounds.
    static public int ClampListIndex(int index, int listSize)
    {
        index = ((index % listSize) + listSize) % listSize;

        return index;
    }

    static public bool IsVertexConcave(List<Vector2> vertices, int index)
    {
        var currentVertex = vertices[index];
        var nextVertex = vertices[ClampListIndex(index + 1, vertices.Count)];
        var prevVertex = vertices[ClampListIndex(index - 1, vertices.Count)];

        var leftVector = currentVertex - prevVertex;
        var rightVector = nextVertex - currentVertex;

        float cross = (leftVector.x * rightVector.y) - (leftVector.y * rightVector.x);

        return cross > 0;
        //return Vector3.Cross(leftVector, rightVector).magnitude < 0;
    }

    static public float DistanceSquared(Vector2 a, Vector2 b)
    {
        return (b.x - a.x) * (b.x - a.x) + (b.y - a.y) * (b.y - a.y);
    }

    static public float Distance(Vector2 a, Vector2 b)
    {
        return Mathf.Sqrt(DistanceSquared(a,b));
    }

    static public bool LineSegmentsCross(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
    {
        float denominator = ((b.x - a.x) * (d.y - c.y)) - ((b.y - a.y) * (d.x - c.x));

        if (denominator < Mathf.Epsilon && denominator > -Mathf.Epsilon)
            return false;

        float numerator1 = ((a.y - c.y) * (d.x - c.x)) - ((a.x - c.x) * (d.y - c.y));
        float numerator2 = ((a.y - c.y) * (b.x - a.x)) - ((a.x - c.x) * (b.y - a.y));


        if ((numerator1 < Mathf.Epsilon && numerator1 > -Mathf.Epsilon) || (numerator2 < Mathf.Epsilon && numerator2 > -Mathf.Epsilon))
            return false;

        float r = numerator1 / denominator;
        float s = numerator2 / denominator;

        return (r > 0 && r < 1) && (s > 0 && s < 1);
    }


    static public float DistanceToSegmentSquared(Vector2 pos, Vector2 v1, Vector2 v2)
    {
        float lengthSegment = DistanceSquared(v1, v2);
        if (lengthSegment < Mathf.Epsilon && lengthSegment > -Mathf.Epsilon)
            return DistanceSquared(pos, v1);
        float t = ((pos.x - v1.x) * (v2.x - v1.x) + (pos.y - v1.y) * (v2.y - v1.y)) / lengthSegment;
        if (t < 0) return DistanceSquared(pos, v1);
        if (t > 1) return DistanceSquared(pos, v2);

        return DistanceSquared(pos, new Vector2(v1.x + t * (v2.x - v1.x), v1.y + t * (v2.y - v1.y)));
    }

    static public float DistanceToSegment(Vector2 pos, Vector2 v1, Vector2 v2)
    {
        return Mathf.Sqrt(DistanceToSegmentSquared(pos, v1, v2));
    }
}
