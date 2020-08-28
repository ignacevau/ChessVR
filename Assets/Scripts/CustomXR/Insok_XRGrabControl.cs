using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Util;
using GlobalData;
using Extensions;

public class Insok_XRGrabControl : MonoBehaviour
{
    private enum GrabMethods
    {
        Pinch,
        Fist,
        PinchAndFist
    };

    [SerializeField]
    private GrabMethods grabMethod;

    [SerializeField]
    private bool throwOnRelease = true;

    [SerializeField]
    private Insok_XRControllerInfo controllerInfo;

    [SerializeField]
    private float triggerDownThreshold = 0.5f;

    [SerializeField]
    private LayerMask grabbingLayer;

    [HideInInspector] public bool isGrabbing = false;
    [HideInInspector] public bool isHovering = false;

    [HideInInspector] public GameObject hoverObject;

    // Original parent from grabbed object and grab-transform parent
    private Transform originalGrabObjTransformParent;
    [SerializeField] private Transform grabTransformParent;

    private void Update()
    {
        // Check for grab
        if (isGrabButtonDown())
        {
            if (!isGrabbing && isHovering)
                GrabObject(hoverObject);
        }

        // Check for release
        else if (isGrabButtonUp())
        {
            if (isGrabbing)
                ReleaseObject(Data.grabbedObject);
        }
    }

    private bool isGrabButtonDown()
    {
        if (grabMethod == GrabMethods.Pinch)
            return controllerInfo.isPrimaryTriggerDown(triggerDownThreshold);
        else if (grabMethod == GrabMethods.Fist)
            return controllerInfo.isSecondaryTriggerDown(triggerDownThreshold);
        else if (grabMethod == GrabMethods.PinchAndFist)
            return controllerInfo.isPrimaryTriggerDown(triggerDownThreshold) || controllerInfo.isSecondaryTriggerDown(triggerDownThreshold);
        else
            throw new System.Exception("Unknow grab method!");
    }

    private bool isGrabButtonUp()
    {
        if (grabMethod == GrabMethods.Pinch)
            return controllerInfo.isPrimaryTriggerUp(triggerDownThreshold);
        else if (grabMethod == GrabMethods.Fist)
            return controllerInfo.isSecondaryTriggerUp(triggerDownThreshold);
        else if (grabMethod == GrabMethods.PinchAndFist)
            return controllerInfo.isPrimaryTriggerUp(triggerDownThreshold) && controllerInfo.isSecondaryTriggerUp(triggerDownThreshold);
        else
            throw new System.Exception("Unknow grab method!");
    }

    private void GrabObject(GameObject obj)
    {
        // Invoke the grab events if there are any
        Insok_XRGrabEvent xrGrabEvent = obj.GetComponent<Insok_XRGrabEvent>();
        if (xrGrabEvent != null)
        {
            if (xrGrabEvent.grabbable)
            {
                xrGrabEvent.onGrabEnter.Invoke();
            }
            else
            {
                Debug.Log(xrGrabEvent.grabbable);
                return;
            }
        }

        Data.grabbedObject = obj;
        isGrabbing = true;

        // Set grabbed obj transform as child of hand
        originalGrabObjTransformParent = Data.grabbedObject.transform.parent;
        Data.grabbedObject.transform.SetParent(grabTransformParent);

        // Change rigidbody from grabbedObject to kinematic
        Data.grabbedObject.GetComponent<Rigidbody>().isKinematic = true;
    }

    private void ReleaseObject(GameObject obj)
    {
        // Invoke the grab events if there are any
        Insok_XRGrabEvent xrGrabEvent = obj.GetComponent<Insok_XRGrabEvent>();
        if (xrGrabEvent != null)
            xrGrabEvent.onGrabExit.Invoke();


        // Set grabbed obj transform back to its original transform parent
        Data.grabbedObject.transform.SetParent(originalGrabObjTransformParent);

        Rigidbody grabRb = Data.grabbedObject.GetComponent<Rigidbody>();

        // Change rigidbody from grabbedObject back to non kinematic
        grabRb.isKinematic = false;

        // Throw object
        if (throwOnRelease)
        {
            grabRb.velocity = controllerInfo.velocity;
            grabRb.angularVelocity = controllerInfo.angularVelocity;
        }

        Data.grabbedObject = null;
        isGrabbing = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (grabbingLayer.HasLayer(other.gameObject.layer))
        {
            hoverObject = other.gameObject.transform.parent.gameObject;
            isHovering = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (hoverObject != null)
        {
            if (grabbingLayer.HasLayer(other.gameObject.layer))
            {
                if (hoverObject.Equals(other.gameObject.transform.parent.gameObject))
                {
                    hoverObject = null;
                    isHovering = false;
                }
            }
        }
    }
}
