using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Bson;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    [SerializeField] private GameObject _floor;

    private Rigidbody _floorRigidbody;
    private bool _mouseDown;
    private bool _objectHeld;
    private float _targetZ = -2.85f;

    private Object[] _objects;

    void Start()
    {
        if (_floor != null)
        {
            _floorRigidbody = _floor.GetComponent<Rigidbody>();
        }
        _objects = GameObject.FindObjectsOfType<Object>();
        foreach (Object obj in _objects)
        {
            obj._onObjectHeld += OnObjectHeld;
        }
    }
    
    public void Update()
    {
        if (_objectHeld)
        {
            return;
        }
        
        // detect mouse press
        if (!_mouseDown && Input.GetMouseButtonDown(0))
        {
            _mouseDown = true;
        }
        else if (_mouseDown && Input.GetMouseButton(0))
        {
            float currentY = Input.mousePosition.y;
            float zConversion = (1 - (currentY / Screen.height)) * _targetZ;
            Debug.Log("current y is " + _floor.transform.position.z + " and z conversion is " + zConversion);

            if (_floor.transform.position.z < 0 || zConversion > 0)
            {
                return;
            }
            
            Vector3 tempVect = new Vector3(0, 0, zConversion);
            tempVect = tempVect * (1.2f * Time.deltaTime);
            _floorRigidbody.MovePosition(_floor.transform.position + tempVect);
        }
        else if (_mouseDown && Input.GetMouseButtonUp(0))
        {
            _mouseDown = false;
        }
    }

    private void OnObjectHeld()
    {
        _objectHeld = true;
    }
}

