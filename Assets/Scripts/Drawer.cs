using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drawer : MonoBehaviour
{
    [SerializeField] private GameObject _drawerFloor;
    
    private bool _pulled;
    
    private Rigidbody _floorRigidbody;
    
    void Start()
    {
        _floorRigidbody = _drawerFloor.GetComponent<Rigidbody>();
    }
}
