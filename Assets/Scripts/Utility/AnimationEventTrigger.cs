using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Disable the "Field not assigned" warnings
#pragma warning disable 0649

public class AnimationEventTrigger : MonoBehaviour
{
    [SerializeField] UnityEvent action;

    public void TriggerEvent()
    {
        action.Invoke();
    }
}
