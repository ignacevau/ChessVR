using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Disable the "Field not assigned" warnings
#pragma warning disable 0649

public class ButtonUI3D : MonoBehaviour
{
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void OnHoverEnter()
    {
        anim.SetBool("hovering", true);
    }

    public void OnHoverExit()
    {
        anim.SetBool("hovering", false);
    }
}
