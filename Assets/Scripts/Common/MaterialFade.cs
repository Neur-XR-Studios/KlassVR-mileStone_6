using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MaterialFade : MonoBehaviour
{
    public Material targetMaterial;
    public float fadeDuration = 2f;

    private bool isFading = false;
    void OnEnable()
    {
        if (targetMaterial == null)
        {
            Debug.LogError("Target material not assigned!");
            return;
        }

        FadeOut();
    }

    void Update()
    {

    }

    public void FadeIn()
    {
        if (!isFading)
        {
            isFading = true;
            StartCoroutine(FadeMaterial(0f, 1f, fadeDuration));
        }
    }

    public void FadeOut()
    {
       
        if (!isFading)
        {
            isFading = true;
            StartCoroutine(FadeMaterial(1f, 0f, fadeDuration));
        }
    }
    public void manualyAssignFadeOut()
    {
        isFading = false;
        FadeOut();
       
    }
    IEnumerator FadeMaterial(float startAlpha, float targetAlpha, float duration)
    {
        float elapsedTime = 0f;
        Color startColor = targetMaterial.color;

        while (elapsedTime < duration)
        {
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / duration);
            targetMaterial.color = new Color(startColor.r, startColor.g, startColor.b, newAlpha);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the target alpha is reached
        targetMaterial.color = new Color(startColor.r, startColor.g, startColor.b, targetAlpha);

        isFading = false;
        if(elapsedTime >= duration)
        {
            this.gameObject.SetActive(false);
        }
    }
}
