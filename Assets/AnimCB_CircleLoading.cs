using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Disable the "Field not assigned" warnings
#pragma warning disable 0649

public class AnimCB_CircleLoading : MonoBehaviour
{
    [SerializeField]
    private UnityEvent circleLoadedAction;

    public void StartCircleLoadedAction()
    {
        circleLoadedAction?.Invoke();
    }
}
