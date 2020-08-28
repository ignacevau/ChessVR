using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextFade : MonoBehaviour
{
    TextMeshPro Text;

    private void OnEnable()
    {
        Text = GetComponent<TextMeshPro>();
    }

    public void FadeOut(float fadeOutTime)
    {
        fadeOutTime *= Time.timeScale;
        StartCoroutine(FadeOutRoutine(fadeOutTime));
    }

    private IEnumerator FadeOutRoutine(float fadeOutTime)
    {
        Color OriginalColor = Text.color;
        for (float t = 0.01f; t < fadeOutTime; t += Time.deltaTime)
        {
            Text.color = Color.Lerp(OriginalColor, new Color(OriginalColor.r,OriginalColor.g, OriginalColor.b, 0), Mathf.Min(1, t / fadeOutTime));
            yield return null;
        }

        Text.color = new Color(OriginalColor.r, OriginalColor.g, OriginalColor.b, 0);
    }

    public void FadeIn(float fadeInTime)
    {
        fadeInTime *= Time.timeScale;
        StartCoroutine(FadeInRoutine(fadeInTime));
    }

    private IEnumerator FadeInRoutine(float fadeInTime)
    {
        Color OriginalColor = Text.color;
        for (float t = 0.01f; t < fadeInTime; t += Time.deltaTime)
        {
            Text.color = Color.Lerp(OriginalColor, new Color(OriginalColor.r, OriginalColor.g, OriginalColor.b, 1), Mathf.Min(1, t / fadeInTime));
            yield return null;
        }

        Text.color = new Color(OriginalColor.r, OriginalColor.g, OriginalColor.b, 1);
    }
}