using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class HandAnimator : MonoBehaviour
{
    public float speed = 5.0f;
    public float pinchValue = 0.35f;
    public XRController controller = null;
    [SerializeField] private Animator FingersColliderAnim;

    private Animator animator = null;

    private float GripValue;
    private float PointerValue;

    private readonly List<Finger> gripFingers = new List<Finger>
    {
        new Finger(FingerType.Middle),
        new Finger(FingerType.Ring),
        new Finger(FingerType.Pinky)
    };

    private readonly List<Finger> pointFingers = new List<Finger>
    {
        new Finger(FingerType.Index),
        new Finger(FingerType.Thumb)
    };

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Store input
        GetGripValue();
        GetPointerValue();

        // Get target position for fingers
        SetFingerTargets(gripFingers, GripValue);
        SetFingerTargets(pointFingers, PointerValue);

        // Smooth input values
        SmoothFinger(pointFingers);
        SmoothFinger(gripFingers);

        // Apply smoothed values
        AnimateFinger(pointFingers);
        AnimateFinger(gripFingers);

        // Animate the collider for the fingers
        AnimateFingerCollider(GripValue);
    }

    private void GetGripValue()
    {
        if (controller.inputDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue))
            GripValue = gripValue;
    }

    private void GetPointerValue()
    {
        if (controller.inputDevice.TryGetFeatureValue(CommonUsages.trigger, out float pointerValue))
            PointerValue = ClampPointerValue(pointerValue);
    }

    private float ClampPointerValue(float value)
    {
        float result;
        result = Mathf.Clamp(value, 0, Mathf.Max(pinchValue, GripValue));
        return result;
    }

    private void SetFingerTargets(List<Finger> fingers, float value)
    {
        foreach (Finger finger in fingers)
            finger.target = value;
    }

    private void SmoothFinger(List<Finger> fingers)
    {
        foreach(Finger finger in fingers)
        {
            float time = speed * Time.unscaledDeltaTime;
            finger.current = Mathf.MoveTowards(finger.current, finger.target, time);
        }
    }

    private void AnimateFinger(List<Finger> fingers)
    {
        foreach (Finger finger in fingers)
            AnimateFinger(finger.type.ToString(), finger.current);
    }

    private void AnimateFinger(string finger, float blend)
    {
        animator.SetFloat(finger, blend);
    }

    private void AnimateFingerCollider(float value)
    {
        FingersColliderAnim.SetFloat("Blend", value);
    }
}