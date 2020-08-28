using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalData;

public class ChessKingIntro : MonoBehaviour
{
    ConfigurableJoint joint;
    Rigidbody rb;

    private Vector3 StartPos;
    private Insok_XRGrabEvent xrGrabEvent;
    [SerializeField] private Transform EndPos;
    [SerializeField] float EndPosRadius;
    [SerializeField] IntroUI UI_Intro;
    [SerializeField] private Animator TableGlowAnim;
    private Animator anim;

    [SerializeField] ParticleSystem HoveringPS;
    private bool HoveringOverEnd = false;


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
    [SerializeField] float AngularZLimit;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        xrGrabEvent = GetComponent<Insok_XRGrabEvent>();
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        StartPos = transform.localPosition;
        Lock();
    }

    public void Lock()
    {
        joint = gameObject.AddComponent<ConfigurableJoint>();

        SoftJointLimit lowAngularXLimit = new SoftJointLimit { limit = LowAngularXLimit, bounciness = 0, contactDistance = 0 };
        SoftJointLimit highAngularXLimit = new SoftJointLimit { limit = HighAngularXLimit, bounciness = 0, contactDistance = 0 };
        SoftJointLimit angularYLimit = new SoftJointLimit { limit = AngularYLimit, bounciness = 0, contactDistance = 0 };
        SoftJointLimit angularZLimit = new SoftJointLimit { limit = AngularZLimit, bounciness = 0, contactDistance = 0 };

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
        joint.angularZLimit = angularZLimit;
    }

    public void Unlock()
    {
        if (joint != null)
        {
            Destroy(joint);
            joint = null;
        }
    }

    public void EndGrab()
    {
        Debug.LogWarning("EndGrab");
        if(HoveringOverEnd)
        {
            UI_Intro.StartGame();
        }
        else
        {
            RestorePiece();
        }
    }

    public void StartGrab()
    {
        Unlock();
        TableGlowAnim.SetTrigger("glow2");
    }

    private void Update()
    {
        if(Data.grabbedObject == this.gameObject)
        {
            if (!HoveringOverEnd)
            {
                if (Vector3.Distance(transform.position, EndPos.position) < EndPosRadius)
                {
                    HoveringOverEnd = true;
                    StartHoveringAnimation();
                }
            }
            if(HoveringOverEnd)
            {
                if (Vector3.Distance(transform.position, EndPos.position) > EndPosRadius)
                {
                    HoveringOverEnd = false;
                    EndHoveringAnimation();
                }
            }
        }
    }

    private void StartHoveringAnimation()
    {
        HoveringPS.Play();
    }

    private void EndHoveringAnimation()
    {
        HoveringPS.Stop();
    }

    void RestorePiece()
    {
        Coroutine fadeRestorePiece = StartCoroutine(FadeRestorePiece());
    }

    IEnumerator FadeRestorePiece()
    {
        xrGrabEvent.grabbable = false;
        anim.SetTrigger("fade");
        yield return new WaitForSecondsRealtime(1f);
        TableGlowAnim.SetTrigger("glow1");

        transform.localPosition = StartPos;
        transform.localRotation = Quaternion.Euler(new Vector3(90, 0, -90));
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        xrGrabEvent.grabbable = true;
        Lock();
    }
}
