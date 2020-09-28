using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float _speed = 1.0f;
    private float _radius = 0.05f;
    private Vector3 _target;
    private Vector2 _currentTarget;
    private int _currentStep = 0;
    private List<Vector2> _steps;

    private bool _hasArrived = true;
    // Start is called before the first frame update
    void Start()
    {
        _target = transform.position;

        transform.position.Set(transform.position.x, transform.position.y, Camera.main.nearClipPlane);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Input.mousePosition;
            _currentStep = 0;
            _hasArrived = false;

            //   Debug.Log($"Mouse Position Screenspace: {mousePosition}");

            //  mousePosition.z = Camera.main.nearClipPlane;
            _target = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, mousePosition.z));
            //     Debug.Log($"Mouse Position World space before clipped: {mousePosition}");

            _target = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, -Camera.main.gameObject.transform.position.z));

            Debug.Log($"X: {_target.x}, Y: {_target.y}");

            _steps = CollisionManager.Instance.PolygonMap.CalculatePath(transform.position, _target);

            _currentTarget = _steps[_currentStep];
            //   Debug.Log($"Mouse Postion world space after clipped : {_target}");
        }

        if (!_hasArrived)
        {
            var distance = Vector3.Distance(_currentTarget, transform.position);
            var targetDirection = ((Vector3)_currentTarget - transform.position).normalized;

            if (distance > _radius)
                transform.position += targetDirection * _speed * Time.deltaTime;
            else
            {
                transform.position = _currentTarget;
                // transform.position = _currentTarget;
                _currentStep++;
                if (_currentStep < _steps.Count)
                {
                    _currentTarget = _steps[_currentStep]; // CollisionManager.Instance.PolygonMap.GetTarget(_steps[_currentStep]);
                }
                else _hasArrived = true;
            }
        }

        DrawInSightLines();
    }

    void DrawInSightLines()
    {
        var polygonMap = CollisionManager.Instance.PolygonMap;
        var mousePosition = Input.mousePosition;
        var endPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, mousePosition.z));
        endPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, -Camera.main.gameObject.transform.position.z));

        var polygons = polygonMap.Polygons;

        if (!polygons[0].IsPointInPolygon(endPos))
        {
            endPos = polygons[0].GetClosestPointOnEdge(endPos, false);
            if (!polygons[0].IsPointInPolygon(endPos))
                Debug.Log("Still not in the polygon dude");
        }

        for (int i = 1; i < polygons.Count; i++)
        {
            if (polygons[i].IsPointInPolygon(endPos))
            {
                endPos = polygons[i].GetClosestPointOnEdge(endPos, true);
            }
        }


        foreach (var node in polygonMap._mainWalkGraph.Nodes)
        {
            if (polygonMap.IsInLineOfSight(transform.position, node.Vertex))
            {
                Debug.DrawLine(transform.position, node.Vertex, Color.cyan);
            }

            if (Input.GetKey(KeyCode.A))
            {
                if (polygonMap.IsInLineOfSight(endPos, node.Vertex))
                {
                    Debug.DrawLine(endPos, node.Vertex, Color.blue);
                }

                if (_steps != null && _steps.Count > 0 && polygonMap.IsInLineOfSight(_steps[_steps.Count - 1], node.Vertex))
                {
                    Debug.DrawLine(_steps[_steps.Count - 1], node.Vertex, Color.red);
                }
            }
        }

        var steps = CollisionManager.Instance.PolygonMap.CalculatePath(transform.position, endPos);

        Color stepColor = new Color(0.23f, 0.4f, 0.8f);
        for (int i = 0; i < steps.Count - 1; i++)
        {
            var v1 = steps[i];
            var v2 = steps[(i + 1)];

            Debug.DrawLine(v1, v2, stepColor);
        }


        //for (int i = 0; i < polygonMap.Polygons[0].Points.Count; i++)
        //{
        //    var point = polygonMap.Polygons[0].Points[i];
        //    if (polygonMap.IsInLineOfSight(transform.position, point))
        //    {
        //        Debug.DrawLine(transform.position, point, Color.cyan);
        //    }
        //}


    }
}

