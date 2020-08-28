using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Util;

public class Main : MonoBehaviour
{
    [SerializeField] UnityEvent customStart;
    private void Start()
    {
        customStart.Invoke();
    }
}
