using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Disable the "Field not assigned" warnings
#pragma warning disable 0649

public class MenuLoader : MonoBehaviour
{
    [SerializeField] private UnityEvent OpenMenuEvent;
    [SerializeField] private UnityEvent CloseMenuEvent;

    [SerializeField]
    private Animator circleAnim;
    private Animator anim;

    private bool leftHandInside = false;
    private bool rightHandInside = false;

    private bool isHandInside { get { return (leftHandInside || rightHandInside); } }

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        // Add the left and right trigger enter event listeners
        Insok_XRGrabControl.InstanceRight.TriggerEnterEvent.AddListener((other) => { 
            HandleTriggerEnter(other, ref rightHandInside); 
        });
        Insok_XRGrabControl.InstanceLeft.TriggerEnterEvent.AddListener((other) => { 
            HandleTriggerEnter(other, ref leftHandInside); 
        });

        // Add the left and right trigger exit event listeners
        Insok_XRGrabControl.InstanceRight.TriggerExitEvent.AddListener((other) => {
            HandleTriggerExit(other, ref rightHandInside);
        });
        Insok_XRGrabControl.InstanceLeft.TriggerExitEvent.AddListener((other) => {
            HandleTriggerExit(other, ref leftHandInside);
        });
    }

    private void StartHovering()
    {
        anim.SetBool("hovering", true);
    }
    private void StartLoading()
    {
        circleAnim.SetBool("loading", true);
    }

    private void StopLoading()
    {
        circleAnim.SetBool("loading", false);
        anim.SetBool("hovering", false);
    }
    private void StopHovering()
    {
        anim.SetBool("hovering", false);
    }

    private void HandleTriggerExit(Collider other, ref bool handSideBool)
    {
        // Hand exits the trigger zone
        if (other.gameObject.Equals(this.gameObject))
        {
            // Left/right hand inside
            handSideBool = false;

            // Sensor goes goes back to normal, no hovering
            StopHovering();

            // Check if there is still a hand in the trigger
            if (!isHandInside)
                StopLoading();
        }
    }

    private void HandleTriggerEnter(Collider other, ref bool handSideBool)
    {
        // Hand enters the trigger zone
        if (other.gameObject.Equals(this.gameObject))
        {
            // Left/right hand inside
            handSideBool = true;

            // Sensor goes yellow on hovering
            StartHovering();

            if (TableMenu.isOpen)
                CloseMenu();
            else
                StartLoading();
        }
    }

    // Called from the animation event
    public void OpenMenu()
    {
        // Circle doesn't need to load anymore - (it's fading out right now)
        circleAnim.SetBool("loading", false);

        // Open the menu in TableMenu script
        OpenMenuEvent?.Invoke();
    }

    private void CloseMenu()
    {
        // Close the menu in TableMenu script
        CloseMenuEvent?.Invoke();
    }
}
