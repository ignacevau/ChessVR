using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR; //needs to be UnityEngine.VR in version before 2017.2
using UnityEngine.XR.Interaction.Toolkit;

// Disable the "Field not assigned" warnings
#pragma warning disable 0649

public class HandGrabbing : MonoBehaviour
{
    private XRController controller;
    public string InputName;
    public XRNode NodeType;
    public Vector3 ObjectGrabOffset;
    public float GrabDistance = 0.1f;
    public string GrabTag = "Grab";
    public float ThrowMultiplier = 1.5f;
    Collider[] Colliders = new Collider[10];
    public LayerMask GrabMask;
    private Vector3 currentPosOffset;
    private Quaternion currentRotOffset;

    private Transform _currentObject;
    private Vector3 _lastFramePosition;
    private Transform originalParent;
    private Transform pivotParent;

    private void Awake()
    {
        controller = GetComponent<XRController>();
    }

    // Use this for initialization
    void Start()
    {
        _currentObject = null;
        pivotParent = new GameObject("pivotParent").transform;
        _lastFramePosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //update hand position and rotation
        Vector3 localPos;
        Quaternion localRot;

        controller.inputDevice.TryGetFeatureValue(CommonUsages.devicePosition, out localPos);
        controller.inputDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out localRot);

        transform.localPosition = localPos;
        transform.localRotation = localRot;


        //if we don't have an active object in hand, look if there is one in proximity
        if (_currentObject == null)
        {
            //check for colliders in proximity
            int hits = Physics.OverlapSphereNonAlloc(transform.position, GrabDistance, Colliders, GrabMask, QueryTriggerInteraction.Ignore);
            if (hits > 0)
            {
                //if there are colliders, take the first one if we press the grab button and it has the tag for grabbing
                if (Input.GetAxis(InputName) >= 0.01f && Colliders[0].transform.CompareTag(GrabTag))
                {
                    //set current object to the object we have picked up
                    _currentObject = Colliders[0].transform;

                    //if there is no rigidbody to the grabbed object attached, add one
                    if (_currentObject.GetComponent<Rigidbody>() == null)
                    {
                        _currentObject.gameObject.AddComponent<Rigidbody>();
                    }

                    //set grab object to kinematic (disable physics)
                    _currentObject.GetComponent<Rigidbody>().isKinematic = true;

                    currentPosOffset = _currentObject.transform.position - transform.position;
                    currentRotOffset = _currentObject.transform.rotation;

                    originalParent = _currentObject.transform.parent;
                    pivotParent.position = transform.position;
                    pivotParent.rotation = transform.rotation;

                    _currentObject.transform.parent = pivotParent;
                    //_currentObject.transform.localPosition = currentPosOffset;
                }
            }
        }
        else
        //we have object in hand, update its position with the current hand position (+defined offset from it)
        {
            pivotParent.transform.position = transform.position;
            pivotParent.rotation = transform.rotation;

            //if we we release grab button, release current object
            if (Input.GetAxis(InputName) < 1f)
            {
                //set grab object to non-kinematic (enable physics)
                Rigidbody _objectRGB = _currentObject.GetComponent<Rigidbody>();
                _objectRGB.isKinematic = false;

                //calculate the hand's current velocity
                Vector3 CurrentVelocity = (transform.position - _lastFramePosition) / Time.deltaTime;

                //set the grabbed object's velocity to the current velocity of the hand
                _objectRGB.velocity = CurrentVelocity * ThrowMultiplier;

                _currentObject.transform.parent = originalParent;

                //release the reference
                _currentObject = null;
            }

        }

        //save the current position for calculation of velocity in next frame
        _lastFramePosition = transform.position;


    }
}