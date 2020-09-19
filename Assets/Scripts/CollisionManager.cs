using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using PointAndClick;

public class CollisionManager
{
    private static CollisionManager _instance = null;
    public PolygonMap PolygonMap;

    public static CollisionManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = new CollisionManager();
            return _instance;
        }
    }
}
