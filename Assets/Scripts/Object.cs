using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object : MonoBehaviour
{
	public delegate void ObjectHeldDelegate(Object obj);
	public ObjectHeldDelegate OnObjectHeld;
	public float RotateSpeed => _rotateSpeed;
	public Vector2 RotationBoundsX => _rotationBoundsX;
	public Vector2 RotationBoundsY => _rotationBoundsY;

	[SerializeField] private Vector3 _showPos = new Vector3(0, 1.79f, 0);
	[SerializeField] private Vector2 _rotationBoundsX = new Vector2(0, 0);
	[SerializeField] private Vector2 _rotationBoundsY = new Vector2(0, 0);
	[SerializeField] private float _rotateSpeed = 100f;
	private Rigidbody _rigidbody;
	private bool _held;

	// Start is called before the first frame update
    void Start()
    {
	    _rigidbody = GetComponent<Rigidbody>();
    }

    private void OnMouseDown()
    {
	    Debug.LogWarning(message: "Clicked " + name);
	    HoldObject();
    }

    private void HoldObject()
    {
	    _held = true;
	    _rigidbody.useGravity = false;
	    transform.position = _showPos;
	    OnObjectHeld?.Invoke(this);
    }
}
    
