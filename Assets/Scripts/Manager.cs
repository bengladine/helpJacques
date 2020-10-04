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

    private void Update()
    {

        //debug
        if (Input.GetKey(KeyCode.W))
        {
            CollisionManager.Instance.PolygonMap.DrawLineOfSight();
        }

        if (Input.GetKey(KeyCode.D))
        {
            CollisionManager.Instance.PolygonMap.DrawNeighbours();
        }
    }
    private void OnDrawGizmos()
    {
        if (Input.GetKey(KeyCode.W))
        {
            CollisionManager.Instance.PolygonMap.DrawNodes();
        }
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

        //if (DistanceTexture != null && DepthSprite != null)
        //{
        //    BackgroundManager.Instance.DistanceTexture = DistanceTexture;
        //    BackgroundManager.Instance.DepthSpriteObject = DepthSprite.GetComponent<SpriteRenderer>().sprite;
        //}
        //else Debug.LogError("No background depth texture added");
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
