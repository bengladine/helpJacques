using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edge
{
    public int From;
    public int To;
    public float Cost;

    public Edge(int from, int to, float cost = 1.0f)
    {
        From = from;
        To = to;
        Cost = cost;
    }
}
