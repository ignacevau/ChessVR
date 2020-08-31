using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

// Disable the "Field not assigned" warnings
#pragma warning disable 0649

public class Insok_XRControllerInfo : MonoBehaviour
{
    [SerializeField]
    private XRController xrController;

    private float _primaryTriggerValue;
    [HideInInspector] public float primaryTriggerValue => _primaryTriggerValue;

    private float _secondaryTriggerValue;
    [HideInInspector] public float secondaryTriggerValue => _secondaryTriggerValue;

    private Vector3 _velocity;
    [HideInInspector] public Vector3 velocity => _velocity;

    private Vector3 _angularVelocity;
    [HideInInspector] public Vector3 angularVelocity => _angularVelocity;

    private void Update()
    {
        getInputValues();
    }

    private void getInputValues()
    {
        // Get primary trigger input values
        xrController.inputDevice.TryGetFeatureValue(CommonUsages.trigger, out _primaryTriggerValue);
        // Get secondary trigger input values
        xrController.inputDevice.TryGetFeatureValue(CommonUsages.grip, out _secondaryTriggerValue);

        // Get controller's velocity
        xrController.inputDevice.TryGetFeatureValue(CommonUsages.deviceVelocity, out _velocity);
        // Get controller's angular velocity
        xrController.inputDevice.TryGetFeatureValue(CommonUsages.deviceAngularVelocity, out _angularVelocity);
    }

    public bool isPrimaryTriggerDown(float threshold)
    {
        return _primaryTriggerValue > threshold;
    }
    public bool isPrimaryTriggerUp(float threshold)
    {
        return _primaryTriggerValue < threshold;
    }

    public bool isSecondaryTriggerDown(float threshold)
    {
        return _secondaryTriggerValue > threshold;
    }
    public bool isSecondaryTriggerUp(float threshold)
    {
        return _secondaryTriggerValue < threshold;
    }
}
