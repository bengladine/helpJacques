using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float _speed = 1.0f;
    [SerializeField] private float _radius = 1.0f;
    private Vector3 _target;
    private Vector2 _currentTarget;

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

            Debug.Log($"Mouse Position Screenspace: {mousePosition}");

          //  mousePosition.z = Camera.main.nearClipPlane;
            _target =  Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, mousePosition.z));
            Debug.Log($"Mouse Position World space before clipped: {mousePosition}");
            
            _target =  Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, -Camera.main.gameObject.transform.position.z));

            Debug.Log($"Mouse Postion world space after clopped : {_target}");
            //Debug.Log($"Postion: {transform.position}");
        }

        var distance = Vector3.Distance(_target, transform.position);
        var targetDirection = (_target - transform.position).normalized;

        if (distance > _radius)
            transform.position += targetDirection * _speed * Time.deltaTime;
    }
}
