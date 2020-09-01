using GlobalData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class Insok_XRRayController : MonoBehaviour
{
    [SerializeField]
    private GameObject canvasDot;

    //private XRController controller;
    public LineRenderer lineRdr;
    public XRRayInteractor rayInteractor;

    public bool primaryTriggerDown = false;
    public bool secondaryTriggerDown = false;
    public float triggerDownThreshold = 0.7f;

    public float rayLength;
    public LayerMask layerMask;

    //private float primaryTriggerValue;
    //private float secondaryTriggerValue;

    private void Awake()
    {
        //controller = GetComponent<XRController>();
        rayInteractor = GetComponent<XRRayInteractor>();
    }

    private void FixedUpdate()
    {
        
    }

    private void Update()
    {
        //getInputValues();

        //updateControllerData();

        Vector3 direction = transform.TransformDirection(Vector3.forward);
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, rayLength, layerMask))
        {
            if(!lineRdr.enabled)
                lineRdr.enabled = true;
            if (!canvasDot.activeSelf)
                canvasDot.SetActive(true);

            canvasDot.transform.position = hit.point;

            // Adjust ray length
            Vector3 endPoint = Vector3.forward * hit.distance * 0.9f;
            Vector3 startPoint = Vector3.forward * hit.distance * Data.RayPointerSettings.CLOSEST_FURTHEST_RATIO;

            lineRdr.SetPosition(0, startPoint);   // Set startpoint
            lineRdr.SetPosition(1, endPoint);     // Set endpoint


            // Adjust ray thickness
            float widthIncrease = hit.distance * Data.RayPointerSettings.DISTANCE_THICKNESS_MULTIPLIER;
            lineRdr.endWidth = Data.RayPointerSettings.START_WIDTH + widthIncrease;

            lineRdr.startWidth = lineRdr.endWidth * 0.4f;
        }
        else
        {
            if (lineRdr.enabled)
                lineRdr.enabled = false;
            if (canvasDot.activeSelf)
                canvasDot.SetActive(false);
        }
    }

    //private void getInputValues()
    //{
    //    // Get primary trigger input values
    //    controller.inputDevice.TryGetFeatureValue(CommonUsages.trigger, out primaryTriggerValue);

    //    // Get secondary trigger input values
    //    controller.inputDevice.TryGetFeatureValue(CommonUsages.grip, out secondaryTriggerValue);
    //}

    //private void updateControllerData()
    //{
    //    // Update primary trigger data
    //    if (primaryTriggerDown)
    //        if (primaryTriggerValue < triggerDownThreshold)
    //            primaryTriggerDown = false;
    //        else
    //        if (primaryTriggerValue > triggerDownThreshold)
    //            primaryTriggerDown = true;


    //    // Update secondary trigger data
    //    if (secondaryTriggerDown)
    //        if (secondaryTriggerValue < triggerDownThreshold)
    //            secondaryTriggerDown = false;
    //        else
    //        if (secondaryTriggerValue > triggerDownThreshold)
    //            secondaryTriggerDown = true;
    //}
}
