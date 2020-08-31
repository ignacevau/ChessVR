using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Disable the "Field not assigned" warnings
#pragma warning disable 0649

public class ScaleTablePoles : MonoBehaviour
{
    [SerializeField] private float ScaleFactor;

    public void ScalePoles(float amount)
    {
        transform.localScale += Vector3.forward * amount * ScaleFactor;
    }
}