using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Disable the "Field not assigned" warnings
#pragma warning disable 0649

public class TableMenu : MonoBehaviour
{
    Animator anim;

    public static bool isOpen;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void MenuPopup()
    {
        anim.SetBool("open", true);
        isOpen = true;
    }

    public void MenuClose()
    {
        anim.SetBool("open", false);
        isOpen = false;
    }
}
