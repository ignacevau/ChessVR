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
        UpdateTableHeight();
    }

    private void UpdateTableHeight()
    {
        TableTop.transform.localPosition = Vector3.up * Data.TableHeight * Data.MoveTableFactor;
        TablePoles.ScalePoles(Data.TableHeight * Data.MoveTableFactor);

        ChessManager.Instance.UpdateBoardPos();
        ChessManager.Instance.RestoreAllPieces();
    }

    public void TableUp()
    {
        Data.TableHeight = Mathf.Clamp(Data.TableHeight + 1, Data.minTableHeight, Data.maxTableHeight);
        UpdateTableHeight();

        ChessManager.Instance.UpdateBoardPos();
        ChessManager.Instance.RestoreAllPieces();
    }

    public void TableDown()
    {
        Data.TableHeight = Mathf.Clamp(Data.TableHeight - 1, Data.minTableHeight, Data.maxTableHeight);
        UpdateTableHeight();
    }
}