using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shatter : MonoBehaviour
{
    private MeshRenderer[] childrenMesh;
    private Material[] childrenMat;

    private void OnEnable()
    {
        childrenMesh = GetComponentsInChildren<MeshRenderer>();
        childrenMat = new Material[childrenMesh.Length];

        for(int i=0; i<childrenMesh.Length; i++)
        {
            childrenMat[i] = childrenMesh[i].material;
        }

        StartCoroutine(FadeShatters(2f, 1f));
    }

    IEnumerator FadeShatters(float delay, float fadeTime)
    {
        yield return new WaitForSeconds(delay);

        float counter = 0;
        while(counter < fadeTime)
        {
            counter += Time.deltaTime;

            foreach(Material mat in childrenMat)
            {
                mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, 1 - counter / fadeTime);
            }
            
            yield return null;
        }

        DestroyImmediate(gameObject);
    }
}
