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

    static public bool IsVertexConcave(List<Vertex> vertices, int index)
    {
        var currentVertex = vertices[index];
        var nextVertex = vertices[ClampListIndex(index + 1, vertices.Count)];
        var prevVertex = vertices[ClampListIndex(index - 1, vertices.Count)];

        var leftVector = currentVertex.Position - prevVertex.Position;
        var rightVector = nextVertex.Position - currentVertex.Position;

        return Vector3.Cross(leftVector, rightVector).magnitude < 0;
    }
}
