using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalData;
using Util;

// Disable the "Field not assigned" warnings
#pragma warning disable 0649

public class ManageTableHeight : MonoBehaviour, ICustomStart
{
    [SerializeField] private GameObject TableTop;
    [SerializeField] private ScaleTablePoles TablePoles;

    public void CustomStart()
    {
        TableTop.transform.position += Vector3.up * Data.TableHeight * Data.MoveTableFactor;
        TablePoles.ScalePoles(Data.TableHeight * Data.MoveTableFactor);
    }
}