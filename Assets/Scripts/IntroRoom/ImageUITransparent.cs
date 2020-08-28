using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageUITransparent : MonoBehaviour
{
    public Image img;

    private void Awake()
    {
        img.alphaHitTestMinimumThreshold = 0.5f;
    }
}
