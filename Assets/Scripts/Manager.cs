using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using PointAndClick;

public class Manager : MonoBehaviour
{
    public Path Path;
    private List<Polygon> _listOfColliderPolygons = null;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Awake()
    {

    }



    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (Path.WalkablePoly != null)
        {
            _listOfColliderPolygons = new List<Polygon>() { Path.WalkablePoly };
            for (int i = 0; i < Path.ObstaclePolygons.Count; i++)
            {
                _listOfColliderPolygons.Add(Path.ObstaclePolygons[i]);
            }
            CollisionManager.Instance.PolygonMap = new PolygonMap(_listOfColliderPolygons);
        }
        else Debug.LogError("No colliders attached, did you forget to add them to the manager?");
        //if (ListOfColliderPolygons.Count > 0)
        //{
        //    Instance.PolygonMap = new PolygonMap(ListOfColliderPolygons);
        //}
        //else Debug.LogError("No colliders attached, did you forget to add them to the manager?");
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
