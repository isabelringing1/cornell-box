using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object : MonoBehaviour
{
	public delegate void ObjectClickedDelegate(Object obj);
	public ObjectClickedDelegate OnObjectClicked;
	public float RotateSpeed => _rotateSpeed;
	public Vector2 RotationBoundsX => _rotationBoundsX;
	public Vector2 RotationBoundsY => _rotationBoundsY;
	public string[] Description => _description;
	public int SpaceValue => _spaceValue;
	public Vector3 ShowPosition => _showPos;

	[SerializeField] private Vector3 _showPos = new Vector3(0, 1.79f, .2f);
	[SerializeField] private Vector2 _rotationBoundsX = new Vector2(0, 0);
	[SerializeField] private Vector2 _rotationBoundsY = new Vector2(0, 0);
	[SerializeField] private float _rotateSpeed = 100f;
	[SerializeField] private string[] _description;
	[SerializeField] private int _spaceValue = 10;
	private Rigidbody _rigidbody;
	private bool _held;

	// Start is called before the first frame update
    void Start()
    {
	    _rigidbody = GetComponent<Rigidbody>();
    }

    public void SetHeld()
    {
	    _held = true;
	    _rigidbody.useGravity = false;
	    _rigidbody.isKinematic = true;
	    transform.position = _showPos;
    }

    public void SetUnheld()
    {
	    _held = false;
	    _rigidbody.useGravity = true;
	    _rigidbody.isKinematic = false;
    }

    private void OnMouseDown()
    {
	    Debug.LogWarning(message: "Clicked " + name);
	    OnObjectClicked?.Invoke(this);
    }
    
}
    
