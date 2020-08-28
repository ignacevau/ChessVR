using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandPerformance : MonoBehaviour
{
    [SerializeField] private Transform LocalAvatar;
    [SerializeField] private Material HandMaterial;
    public static bool HandsInitialized = false;
    GameObject RightHand;
    GameObject LeftHand;

    private void Start()
    {
        StartCoroutine(SpotChildren());
    }

    IEnumerator SpotChildren()
    {
        while(LocalAvatar.childCount < 7)
        {
            yield return null;
        }

        RightHand = LocalAvatar.GetChild(7).GetChild(0).gameObject;
        LeftHand = LocalAvatar.GetChild(6).GetChild(0).gameObject;

        HandsInitialized = true;

        // Shitty code ikr, do this with awaiter extension and than async void...    :/
        UpdateHandRenderer(LeftHand);
        UpdateHandRenderer(RightHand);
    }

    private void UpdateHandRenderer(GameObject hand)
    {
        SkinnedMeshRenderer rdr = hand.GetComponent<SkinnedMeshRenderer>();
        rdr.material = HandMaterial;

        rdr.receiveShadows = false;
        rdr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
    }
}
