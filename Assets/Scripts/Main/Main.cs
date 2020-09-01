using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Util;

// Disable the "Field not assigned" warnings
#pragma warning disable 0649

public class Main : MonoBehaviour
{
    [SerializeField] UnityEvent customStart;
    private void Start()
    {
        customStart.Invoke();
    }

    public void HelloWorld()
    {
        Debug.Log("Hello World");
    }
}
