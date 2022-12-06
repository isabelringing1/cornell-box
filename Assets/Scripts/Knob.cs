using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knob : MonoBehaviour
{
    public delegate void KnobHeldDelegate();
    public KnobHeldDelegate OnKnobHeld;

    void Start()
    {
        
    }

    private void OnMouseDown()
    {
        OnKnobHeld?.Invoke();
    }
}
