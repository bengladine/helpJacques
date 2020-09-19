using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using PointAndClick;

public class Manager : MonoBehaviour
{
    public List<Polygon> ListOfColliderPolygons;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Awake()
    {
        if (ListOfColliderPolygons.Count > 0)
        {
            CollisionManager.Instance.PolygonMap = new PolygonMap(ListOfColliderPolygons);
        }
        else Debug.LogError("No colliders attached, did you forget to add them to the manager?");
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
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
