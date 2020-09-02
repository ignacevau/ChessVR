using GlobalData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Disable the "Field not assigned" warnings
#pragma warning disable 0649

public class ScaleTablePoles : MonoBehaviour
{
    public void ScalePoles(float amount)
    {
        transform.localScale = Vector3.forward * Data.ScalePolesBias + new Vector3(1, 1, 0) + Vector3.forward * amount * Data.ScalePolesFactor;
    }
}