using GlobalData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Util.Util;

public class TileFX : MonoBehaviour
{
    public static bool isCoordAllowed = false;

    bool hovering = false;
    [SerializeField] float TileSelectionShowHeight;
    [SerializeField] private ParticleSystem ps;

    Coord curCoords = new Coord(0, 0);

    void Update()
    {
        // Not hovering yet
        if (!hovering)
        {
            if (Data.grabbedObject != null)
            {
                if (Data.grabbedObject.CompareTag("ChessPiece"))
                {
                    if (Data.grabbedObject.transform.position.y - ChessManager.boardPos.y < TileSelectionShowHeight)
                    {
                        Coord grabbedCoords = ChessManager.GetCoordFromWorldPos(Data.grabbedObject.transform.position);

                        isCoordAllowed = Data.grabbedObject.GetComponent<ChessPiece>().IsMoveAllowed(grabbedCoords);

                        // Position is allowed
                        if (isCoordAllowed)
                        {
                            Vector3 fxPosition = ChessManager.GetWorldPosFromCoord(grabbedCoords);
                            TryHideTileSelection();
                            TryShowTileSelection(fxPosition);
                        }
                        // Position is not allowed
                        else
                        {
                            // No need to hide, not hovering
                        }

                        curCoords.x = grabbedCoords.x;
                        curCoords.y = grabbedCoords.y;
                    }
                }
            }
        }
        // Already hovering
        if (hovering)
        {
            if (Data.grabbedObject != null)
            {
                if (Data.grabbedObject.CompareTag("ChessPiece"))
                {
                    if (Data.grabbedObject.transform.position.y - ChessManager.boardPos.y < TileSelectionShowHeight)
                    {
                        Coord grabbedCoords = ChessManager.GetCoordFromWorldPos(Data.grabbedObject.transform.position);

                        // Position changed, but still hovering
                        if (grabbedCoords.x != curCoords.x || grabbedCoords.y != curCoords.y)
                        {
                            curCoords.x = grabbedCoords.x;
                            curCoords.y = grabbedCoords.y;
                            isCoordAllowed = Data.grabbedObject.GetComponent<ChessPiece>().IsMoveAllowed(grabbedCoords);

                            // New position is allowed
                            if (isCoordAllowed)
                            {
                                Vector3 fxPosition = ChessManager.GetWorldPosFromCoord(grabbedCoords);
                                TryHideTileSelection();
                                TryShowTileSelection(fxPosition);
                            }
                            // New position is not allowed
                            else
                            {
                                TryHideTileSelection();
                            }
                        }
                    }
                    // Position too high above board
                    else
                    {
                        TryHideTileSelection();
                    }
                }
            }
            // Grabbed object is null
            else
            {
                TryHideTileSelection();
            }
        }
    }

    bool CheckCoordAllowed(Coord coord, List<Coord> _allowedCoords)
    {
        Coord[] allowedCoords = _allowedCoords.ToArray();
        for (int i = 0; i < allowedCoords.Length; i++)
        {
            if (coord.x == allowedCoords[i].x && coord.y == allowedCoords[i].y)
            {
                return true;
            }
        }
        return false;
    }

    private void TryHideTileSelection()
    {
        if (hovering)
        {
            transform.GetChild(0).gameObject.SetActive(false);

            if (ps.isPlaying)
            {
                ps.Clear();
                ps.Stop();
                ps.Clear();
            }
            hovering = false;
        }
    }

    private void TryShowTileSelection(Vector3 pos)
    {
        if (!hovering)
        {
            transform.GetChild(0).gameObject.SetActive(true);

            ps.Clear();
            ps.Play();
            ps.Clear();

            transform.position = new Vector3(pos.x, transform.position.y, pos.z);

            hovering = true;
        }
    }
}
