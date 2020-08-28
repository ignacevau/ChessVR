using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageFade : MonoBehaviour
{
    Image Image;

    private void OnEnable()
    {
        Image = GetComponent<Image>();
    }

    public void FadeOut(float fadeOutTime)
    {
        fadeOutTime *= Time.timeScale;
        StartCoroutine(FadeOutRoutine(fadeOutTime));
    }

    private IEnumerator FadeOutRoutine(float fadeOutTime)
    {
        Color OriginalColor = Image.color;
        for (float t = 0.01f; t < fadeOutTime; t += Time.deltaTime)
        {
            Image.color = Color.Lerp(OriginalColor, new Color(OriginalColor.r,OriginalColor.g, OriginalColor.b, 0), Mathf.Min(1, t / fadeOutTime));
            yield return null;
        }

        Image.color = new Color(OriginalColor.r, OriginalColor.g, OriginalColor.b, 0);
    }

    public void FadeIn(float fadeInTime)
    {
        fadeInTime *= Time.timeScale;
        StartCoroutine(FadeInRoutine(fadeInTime));
    }

    private IEnumerator FadeInRoutine(float fadeInTime)
    {
        Color OriginalColor = Image.color;
        for (float t = 0.01f; t < fadeInTime; t += Time.deltaTime)
        {
            Image.color = Color.Lerp(OriginalColor, new Color(OriginalColor.r, OriginalColor.g, OriginalColor.b, 1), Mathf.Min(1, t / fadeInTime));
            yield return null;
        }

        Image.color = new Color(OriginalColor.r, OriginalColor.g, OriginalColor.b, 1);
    }
}