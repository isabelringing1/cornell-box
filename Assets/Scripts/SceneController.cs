using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Bson;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    [SerializeField] private GameObject _floor;
    [SerializeField] private GameObject _heldContainer;

    private Rigidbody _floorRigidbody;
    private bool _mouseDown;
    private Object _heldObject;
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
            obj.OnObjectHeld += OnObjectHeld;
        }
    }
    
    public void Update()
    {
        if (_objectHeld)
        {
            if (Input.GetMouseButton(0))
            {
                Transform t = _heldContainer.transform;
                float xMov = Input.GetAxis("Mouse X");
                float yMov = Input.GetAxis("Mouse Y");
                if (t.rotation.eulerAngles.z < _heldObject.RotationBoundsX[0] || t.rotation.eulerAngles.z > _heldObject.RotationBoundsX[1])
                {
                    xMov = 0;
                }
                if (t.rotation.eulerAngles.x < _heldObject.RotationBoundsY[0] || t.rotation.eulerAngles.x > _heldObject.RotationBoundsY[1])
                {
                    yMov = 0;
                }
                Vector3 rotationVector = new Vector3(yMov, 0, -xMov);
                t.Rotate(eulers: rotationVector * Time.deltaTime * _heldObject.RotateSpeed);
                return;
            }
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

    private void OnObjectHeld(Object obj)
    {
        _heldObject = obj;
        _objectHeld = true;
        _heldObject.transform.SetParent(p: _heldContainer.transform);
    }
}

