using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Vertex
{
    [SerializeField]
    public Vector2 Position;

    public Vertex(Vector2 pos)
    {
        Position = pos;
    }
}
