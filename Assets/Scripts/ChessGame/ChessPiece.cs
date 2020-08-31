using GlobalData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Util.Util;

// Disable the "Field not assigned" warnings
#pragma warning disable 0649

public class ChessPiece : MonoBehaviour
{
    ConfigurableJoint joint;
    //OVRGrabbable grab;
    public string Type;
    private ChessManager chessMgr;
    TileFX TileFX;
    List<Coord> PossibleMoves = new List<Coord>();
    Rigidbody rb;
    Renderer rdr;

    AudioController audioMgr;

    // Only for pawns (en passant check)
    public bool DoubleAdvanced;

    [SerializeField] float TileSelectionShowHeight;

    [SerializeField] Vector3 FirstAnchorPos;
    [SerializeField] Vector3 Axis;
    [SerializeField] bool AutoConfigureConnectedAnchor;
    [SerializeField] Vector3 ConnectedAnchorPos;
    [SerializeField] Vector3 SecondaryAxis;
    [SerializeField] ConfigurableJointMotion xMotion;
    [SerializeField] ConfigurableJointMotion yMotion;
    [SerializeField] ConfigurableJointMotion zMotion;
    [SerializeField] ConfigurableJointMotion xAngularMotion;
    [SerializeField] ConfigurableJointMotion yAngularMotion;
    [SerializeField] ConfigurableJointMotion zAngularMotion;
    [SerializeField] float LowAngularXLimit;
    [SerializeField] float HighAngularXLimit;
    [SerializeField] float AngularYLimit;

    [HideInInspector] public Coord Coords;

    private void Awake()
    {
        chessMgr = GameObject.FindWithTag("GameController").GetComponent<ChessManager>();
        audioMgr = GetComponent<AudioController>();
        //grab = GetComponent<OVRGrabbable>();
        TileFX = chessMgr.TileFX.GetComponent<TileFX>();
        rb = GetComponent<Rigidbody>();
        rdr = GetComponentInChildren<Renderer>();
    }

    public void Lock()
    {
        joint = gameObject.AddComponent<ConfigurableJoint>();

        SoftJointLimit lowAngularXLimit = new SoftJointLimit { limit = LowAngularXLimit, bounciness = 0, contactDistance = 0 };
        SoftJointLimit highAngularXLimit = new SoftJointLimit { limit = HighAngularXLimit, bounciness = 0, contactDistance = 0 };
        SoftJointLimit angularYLimit = new SoftJointLimit { limit = AngularYLimit, bounciness = 0, contactDistance = 0 };

        joint.anchor = FirstAnchorPos;
        joint.axis = Axis;
        joint.autoConfigureConnectedAnchor = AutoConfigureConnectedAnchor;
        joint.connectedAnchor = ConnectedAnchorPos;
        joint.secondaryAxis = SecondaryAxis;
        joint.xMotion = xMotion;
        joint.yMotion = yMotion;
        joint.zMotion = zMotion;
        joint.angularXMotion = xAngularMotion;
        joint.angularYMotion = yAngularMotion;
        joint.angularZMotion = zAngularMotion;
        joint.lowAngularXLimit = lowAngularXLimit;
        joint.highAngularXLimit = highAngularXLimit;
        joint.angularYLimit = angularYLimit;

        joint.breakForce = 200;
    }
    // Built-in Unity function
    private void OnJointBreak(float breakForce)
    {
        this.RestorePiece();
    }

    public void Unlock()
    {
        if (joint != null)
        {
            Destroy(joint);
            joint = null;
        }
    }

    void SnapToBoard()
    {
        transform.rotation = Quaternion.Euler(-90, 0, 0);
        transform.position = new Vector3(transform.position.x, ChessManager.boardPos.y, transform.position.z);

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    // Locks auto
    void MovePiece()
    {
        Vector3 originalPos = transform.position;
        Quaternion originalRot = transform.rotation;

        SnapToBoard();
        Lock();

        //transform.rotation = Quaternion.Euler(Mathf.Clamp(transform.eulerAngles.x, -60, -120), Mathf.Clamp(transform.eulerAngles.y, -30, 30), transform.eulerAngles.z);

        Coord newCoords = ChessManager.GetCoordFromWorldPos(transform.position);

        // This is necessary because the old position is set to null
        if (newCoords.x != Coords.x || newCoords.y != Coords.y)
        {
            if (ChessManager.Board[newCoords.x][newCoords.y] != null)
            {
                ChessPiece capturedPiece = ChessManager.Board[newCoords.x][newCoords.y];
                capturedPiece.Die();
            }

            chessMgr.UpdatePiecePosition(Coords.Clone(), newCoords);
        }
    }

    public void Kill()
    {
        chessMgr.KillEnemy();
    }

    public void Die()
    {
        gameObject.SetActive(false);
        SpawnParticles();
        //Instantiate(char.IsUpper(Type[0]) ? chessMgr.ShattersWhite : chessMgr.ShattersBlack, transform.position, Quaternion.identity, chessMgr.ShatterParent);
    }

    private void SpawnParticles()
    {
        DeathParticles.Instance.Play(transform.position);
    }

    public void GrabEnter()
    {
        Unlock();
    }

    public void GrabExit()
    {
        if (joint == null)
        {
            Coord movedCoords = ChessManager.GetCoordFromWorldPos(Data.grabbedObject.transform.position);

            if (TileFX.isCoordAllowed)
                MovePiece();
            else if(Coords.x == movedCoords.x && Coords.y == movedCoords.y)
                ReturnToPlace();
            else
            {
                RestorePiece();
            }
        }
    }

    void ReturnToPlace()
    {
        Vector3 originalPos = transform.position;
        Quaternion originalRot = transform.rotation;

        SnapToBoard();
        Lock();

        //transform.rotation = Quaternion.Euler(Mathf.Clamp(transform.eulerAngles.x, -60, -120), Mathf.Clamp(transform.eulerAngles.y, -30, 30), transform.eulerAngles.z);
    }

    void RestorePiece()
    {
        Coroutine fadeRestorePiece = StartCoroutine(FadeRestorePiece());
    }

    IEnumerator FadeRestorePiece()
    {
        // Wait a bit first
        yield return new WaitForSecondsRealtime(0.5f);

        // Fade out
        float fadeOutTime = 0.7f;
        StartCoroutine(FadeAlphaTo(0, fadeOutTime));
        yield return new WaitForSecondsRealtime(fadeOutTime);

        // Lock back to board
        transform.position = ChessManager.GetWorldPosFromCoord(Coords);
        transform.rotation = Quaternion.Euler(Vector3.right * -90);

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        Lock();

        // Fade in
        float fadeInTime = 0.5f;
        StartCoroutine(FadeAlphaTo(1, fadeInTime));
    }

    IEnumerator FadeAlphaTo(float aEnd, float time)
    {
        Color startColor = new Color(
            rdr.material.color.r,
            rdr.material.color.g,
            rdr.material.color.b,
            rdr.material.color.a
        );

        // Transition to the desired value
        for (float t=0; t<1.0f; t+=Time.deltaTime/time)
        {
            Color color = new Color(startColor.r, startColor.g, startColor.b, Mathf.Lerp(startColor.a, aEnd, t));
            rdr.material.color = color;

            yield return null;
        }

        // At the end, set it to the exact value
        rdr.material.color = new Color(startColor.r, startColor.g, startColor.b, aEnd);
    }

    public void ClearPossibleMoves()
    {
        PossibleMoves.Clear();
    }

    public void AddPossibleMove(Coord move)
    {
        PossibleMoves.Add(move);
    }

    public bool IsMoveAllowed(Coord move)
    {
        if(PossibleMoves.Contains(move))
            return true;
        return false;
    }
}
