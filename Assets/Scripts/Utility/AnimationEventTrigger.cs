using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationEventTrigger : MonoBehaviour
{
    [SerializeField] UnityEvent action;

    public void TriggerEvent()
    {
        action.Invoke();
    }
}
