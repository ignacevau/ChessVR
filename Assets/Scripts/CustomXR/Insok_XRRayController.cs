using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class Insok_XRRayController : MonoBehaviour
{
    public bool enabled = true;

    private XRController controller;
    public LineRenderer lineRdr;
    public XRRayInteractor rayInteractor;

    public bool primaryTriggerDown = false;
    public bool secondaryTriggerDown = false;
    public float triggerDownThreshold = 0.7f;

    public float rayLength;
    public LayerMask layerMask;

    private float primaryTriggerValue;
    private float secondaryTriggerValue;

    private void Awake()
    {
        controller = GetComponent<XRController>();
        rayInteractor = GetComponent<XRRayInteractor>();
    }

    private void Update()
    {
        if (this.enabled)
        {
            getInputValues();

            updateControllerData();

            Vector3 direction = transform.TransformDirection(Vector3.forward);
            RaycastHit hit;
            if (Physics.Raycast(transform.position, direction, out hit, rayLength, layerMask))
                lineRdr.enabled = true;
            else
                lineRdr.enabled = false;
        }
    }

    private void getInputValues()
    {
        // Get primary trigger input values
        controller.inputDevice.TryGetFeatureValue(CommonUsages.trigger, out primaryTriggerValue);

        // Get secondary trigger input values
        controller.inputDevice.TryGetFeatureValue(CommonUsages.grip, out secondaryTriggerValue);
    }

    private void updateControllerData()
    {
        // Update primary trigger data
        if (primaryTriggerDown)
            if (primaryTriggerValue < triggerDownThreshold)
                primaryTriggerDown = false;
            else
            if (primaryTriggerValue > triggerDownThreshold)
                primaryTriggerDown = true;


        // Update secondary trigger data
        if (secondaryTriggerDown)
            if (secondaryTriggerValue < triggerDownThreshold)
                secondaryTriggerDown = false;
            else
            if (secondaryTriggerValue > triggerDownThreshold)
                secondaryTriggerDown = true;
    }
}
