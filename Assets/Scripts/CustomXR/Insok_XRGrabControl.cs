﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Util;
using GlobalData;
using Extensions;
using UnityEngine.Events;
using System;

// Disable the "Field not assigned" warnings
#pragma warning disable 0649

public class Insok_XRGrabControl : MonoBehaviour
{
    private enum GrabMethods
    {
        Pinch,
        Fist,
        PinchAndFist
    };

    public enum HandSide
    {
        Right, Left
    };

    [SerializeField]
    public HandSide handSide;

    public UnityEvent_Collider TriggerEnterEvent = new UnityEvent_Collider();
    public UnityEvent_Collider TriggerExitEvent = new UnityEvent_Collider();

    public UnityEvent_Collider GrabEnterEvent = new UnityEvent_Collider();
    public UnityEvent_Collider GrabExitEvent = new UnityEvent_Collider();

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

    [SerializeField]
    private LayerMask triggerLayer;

    [HideInInspector] public bool isGrabbing = false;
    [HideInInspector] public bool isHovering = false;

    [HideInInspector] public GameObject hoverObject;

    // Original parent from grabbed object and grab-transform parent
    private Transform originalGrabObjTransformParent;
    [SerializeField] private Transform grabTransformParent;

    public static Insok_XRGrabControl InstanceRight;
    public static Insok_XRGrabControl InstanceLeft;

    private void Awake()
    {
        if (handSide == HandSide.Right)
        {
            InstanceRight = this;
        }
        if (handSide == HandSide.Left)
            InstanceLeft = this;
    }

    private void Update()
    {
        // Check for grab
        if (isGrabButtonDown() && Data.grabbedObject == null)
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
        // Check whether hand enters trigger zone
        if (triggerLayer.HasLayer(other.gameObject.layer))
        {
            TriggerEnterEvent.Invoke(other);
        }


        // Check whether hand is grabbing an object
        if (grabbingLayer.HasLayer(other.gameObject.layer))
        {
            GrabEnterEvent?.Invoke(other);

            hoverObject = other.gameObject.transform.parent.gameObject;
            isHovering = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check whether hand enters trigger zone
        if (triggerLayer.HasLayer(other.gameObject.layer))
        {
            TriggerExitEvent?.Invoke(other);
        }

        // Check whether hand is dropping grabbed object
        if (hoverObject != null)
        {
            if (grabbingLayer.HasLayer(other.gameObject.layer))
            {
                if (hoverObject.Equals(other.gameObject.transform.parent.gameObject))
                {
                    GrabExitEvent?.Invoke(other);

                    hoverObject = null;
                    isHovering = false;
                }
            }
        }
    }
}
