using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOut : MonoBehaviour
{
    public float fadeDuration = 2f;  // Duration of the fade effect

    private SpriteRenderer[] spriteRenderers;
    private Vector3 originalScale;

    void Start()
    {
        // Get all SpriteRenderer components on the enemy and its children (like weapons)
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        originalScale = transform.localScale;
    }

    public void StartFading()
    {
        StartCoroutine(Fade());
    }

    private IEnumerator Fade()
    {
        float startAlpha = 1f;
        float rate = 1f / fadeDuration;
        float progress = 0f;
        Vector3 targetScale = originalScale * 2;

        while (progress < 1f)
        {
            foreach (SpriteRenderer sr in spriteRenderers)
            {
                if (sr != null)
                {
                    Color tempColor = sr.color;
                    tempColor.a = Mathf.Lerp(startAlpha, 0, progress);
                    sr.color = tempColor;
                }
            }

            transform.localScale = Vector3.Lerp(originalScale, targetScale, progress);

            progress += rate * Time.deltaTime;
            yield return null;
        }

        // Ensure all sprites are fully transparent at the end
        foreach (SpriteRenderer sr in spriteRenderers)
        {
            if (sr != null)
            {
                Color tempColor = sr.color;
                tempColor.a = 0;
                sr.color = tempColor;
            }
        }
        transform.localScale = targetScale;

        // Optionally, destroy the enemy GameObject after fading out
        Destroy(gameObject);
    }
}