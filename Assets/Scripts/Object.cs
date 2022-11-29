using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object : MonoBehaviour
{
	public delegate void ObjectHeldDelegate();

	public ObjectHeldDelegate _onObjectHeld;

	[SerializeField] private Vector3 _showPos = new Vector3(0, 1.79f, 0);
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
	    _onObjectHeld?.Invoke();
    }
}
    
