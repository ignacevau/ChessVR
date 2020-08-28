using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Insok_XRGrabEvent : MonoBehaviour
{
    public UnityEvent onGrabEnter = null;
    public UnityEvent onGrabExit = null;
    public UnityEvent onHoverEnter = null;
    public UnityEvent onHoverExit = null;

    [HideInInspector] public bool grabbable = true;

    private void Awake()
    {
        // Don't ask me why but for some stupid reason Unity initializes it as false
        grabbable = true;
    }
}
