using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PointAndClick
{
    public class Path : MonoBehaviour
    {
        [SerializeField] private Polygon _walkablePoly;
        [SerializeField] private List<Polygon> _obstaclePolygons;

        // Start is called before the first frame update
        void Start()
        {
          //  if (_walkablePoly == null)
            //    Debug.LogError("No walkable polygon added!");
        }


    }
}
