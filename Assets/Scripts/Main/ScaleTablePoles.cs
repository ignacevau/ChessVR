using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleTablePoles : MonoBehaviour
{
    [SerializeField] private float ScaleFactor;

    public void ScalePoles(float amount)
    {
        transform.localScale += Vector3.forward * amount * ScaleFactor;
    }
}