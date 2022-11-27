using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drawer : MonoBehaviour
{
    [SerializeField] private AnimationClip _clip;
    
    private Animation _animation;
    private bool _pulled;
    void Start()
    {
        _animation = GetComponent<Animation>();
    }

    void Update()
    {
        
    }

    void OnMouseDown()
    {
        Debug.Log(message: "On Mouse Down");
        if (!_pulled && _clip != null)
        {
            _animation.clip = _clip;
            _animation.Play();
            _pulled = true;
        }
    }
}
