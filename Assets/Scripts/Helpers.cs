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
        return Mathf.Sqrt(DistanceSquared(a, b));
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

    // source https://www.geeksforgeeks.org/check-if-two-given-line-segments-intersect/
    static private int Orientation(Vector2 p, Vector2 q, Vector2 r)
    {
        float val = (q.y - p.y) * (r.x - q.x) - (q.x - p.x) * (r.y - q.y);
        if (val < float.Epsilon && val > -float.Epsilon) return 0;
        return (val > 0) ? 1 : 2;
    }

    //static public bool OnSegment(Vector2 p1, Vector2 q, Vector2 p2)
    //{
    //    if (q.x <= Math.Max(p1.x, p2.x) && q.x >= Math.Min(p1.x, p2.x) &&
    //q.y <= Math.Max(p1.y, p2.y) && q.y >= Math.Min(p1.y, p2.y))
    //        return true;

    //    return false;
    //}

    static public bool OnSegment(Vector2 p1, Vector2 p2, Vector2 q)
    {
        var crossProduct = (q.y - p1.y) * (p2.x - p1.x) - (q.x - p1.x) * (p2.y - p1.y);

        if (Mathf.Abs(crossProduct) > float.Epsilon)
            return false;

        var dotProduct = (q.x - p1.x) * (p2.x - p1.x) + (q.y - p1.y) * (p2.y - p1.y);
        if (dotProduct < 0.0f)
            return false;

        var squaredLengthBA = (p2.x - p1.x) * (p2.x - p1.x) + (p2.y - p1.y) * (p2.y - p1.y);

        if (dotProduct > squaredLengthBA)
            return false;

        return true;
    }

    //static public bool OnSegment(Vector2 p1, Vector2 p2, Vector2 q)
    //{
    //   var distp1p2 = Vector2.Distance(p1, p2);
    //    var distp1q = Vector2.Distance(p1, q);
    //    var distp2q = Vector2.Distance(p2, q);

    //    if (distp1q + distp2q - distp1p2 > -float.Epsilon && distp1q + distp2q - distp1p2 < float.Epsilon)
    //        return true;
    //    return false;
    //}


    // source : https://stackoverflow.com/questions/217578/how-can-i-determine-whether-a-2d-point-is-within-a-polygon
    static public bool AreIntersecting(float v1x1, float v1y1, float v1x2, float v1y2, float v2x1, float v2y1, float v2x2, float v2y2)
    {
        float d1, d2;
        float a1, a2, b1, b2, c1, c2;

        a1 = v1y2 - v1y1;
        b1 = v1x1 - v1x2;
        c1 = (v1x2 * v1y1) - (v1x1 * v1y2);

        d1 = (a1 * v2x1) + (b1 * v2y1) + c1;
        d2 = (a1 * v2x2) + (b1 * v2y2) + c1;

        if (d1 > 0 && d2 > 0) return false;
        if (d1 < 0 && d2 < 0) return false;

        a2 = v2y2 - v2y1;
        b2 = v2x1 - v2x2;
        c2 = (v2x2 * v2y1) - (v2x1 * v2y2);

        d1 = (a2 * v1x1) + (b2 * v1y1) + c2;
        d2 = (a2 * v1x2) + (b2 * v1y2) + c2;

        if (d1 > 0 && d2 > 0) return false;
        if (d1 < 0 && d2 < 0) return false;

        if ((a1 * b2) - (a2 * b1) == 0.0f)
            return false;

        return true;
    }

    static public bool DoIntersect(Vector2 p1, Vector2 q1, Vector2 p2, Vector2 q2)
    {
        // Find the four orientations needed for general and 
        // special cases 
        int o1 = Orientation(p1, q1, p2);
        int o2 = Orientation(p1, q1, q2);
        int o3 = Orientation(p2, q2, p1);
        int o4 = Orientation(p2, q2, q1);

        // General case 
        if (o1 != o2 && o3 != o4)
            return true;

        // Special Cases 
        // p1, q1 and p2 are colinear and p2 lies on segment p1q1 
        if (o1 == 0 && OnSegment(p1, p2, q1)) return true;

        // p1, q1 and q2 are colinear and q2 lies on segment p1q1 
        if (o2 == 0 && OnSegment(p1, q2, q1)) return true;

        // p2, q2 and p1 are colinear and p1 lies on segment p2q2 
        if (o3 == 0 && OnSegment(p2, p1, q2)) return true;

        // p2, q2 and q1 are colinear and q1 lies on segment p2q2 
        if (o4 == 0 && OnSegment(p2, q1, q2)) return true;

        return false; // Doesn't fall in any of the above cases 
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
