using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InsokButtonVR : MonoBehaviour
{
    [System.Serializable]
    public class ButtonEvent : UnityEvent { }
    private bool pressed = false;

    public ButtonEvent downEvent;

    [SerializeField] private float PressClampMin;
    private float PressClampMax;

    private bool isHandTouching = false;

    private void Start()
    {
        PressClampMax = transform.localPosition.y;
    }

    private void Update()
    {
        if(pressed && isHandTouching)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, PressClampMax + PressClampMin, transform.localPosition.z);
            return;
        }

        transform.localPosition = new Vector3(transform.localPosition.x, Mathf.Clamp(transform.localPosition.y, PressClampMax, PressClampMax + PressClampMin), transform.localPosition.z);

        if (!pressed)
        {
            if (Mathf.Abs(transform.localPosition.y - (PressClampMax + PressClampMin)) < 0.01f)
            {
                downEvent.Invoke();
                pressed = true;
            }
        }

        if(pressed)
        {
            if (Mathf.Abs(transform.localPosition.y - PressClampMax) < 0.01f)
            {
                pressed = false;
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.gameObject.CompareTag("Hand"))
        {
            isHandTouching = true;
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (isHandTouching)
        {
            if (other.collider.gameObject.CompareTag("Hand"))
            {
                isHandTouching = false;
            }
        }
    }
}