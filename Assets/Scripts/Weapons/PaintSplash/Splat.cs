using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Splat : MonoBehaviour
{
   public enum SplatLocation
    {
        Foreground,
        Background
    }

    public Color backgroundTint;
    public float minSizeMod = 0.8f;
    public float maxSizeMod = 1.5f;

    public Sprite[] sprites;

    private SplatLocation splatLocation;
    private SpriteRenderer spriteRenderer;
    private Color bulletColor;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Initialize(SplatLocation splatLocation, Color color, float minSize, float maxSize)
    {
        this.bulletColor = color;
        this.splatLocation = splatLocation;
        SetSprite();
        SetSize(minSize, maxSize);
        SetRotation();
        SetColor();

        SetLocationProperties();

        // Start the fade out coroutine
        StartCoroutine(FadeOut(15f));
    }
    private void SetColor()
    {
        float darkenColor = Random.Range(0.6f, 1.0f);
        spriteRenderer.color = new Color(bulletColor.r * darkenColor, bulletColor.g * darkenColor, bulletColor.b * darkenColor);
    }

    private void SetSprite()
    {
        int randomIndex = Random.Range(0,sprites.Length);
        spriteRenderer.sprite = sprites[randomIndex];
    }
    


    private void SetSize(float minSize, float maxSize)
    {
        minSizeMod = minSize;
        maxSizeMod = maxSize;

        
        if (splatLocation == Splat.SplatLocation.Background)
        {   
            float diff = maxSize - minSize;
            float sizeMod = Random.Range(maxSizeMod, maxSizeMod+diff) ;
            transform.localScale = new Vector3(sizeMod , sizeMod , sizeMod );
        }
        else
        {
            float sizeMod = Random.Range(minSizeMod, maxSizeMod);
            transform.localScale = new Vector3(sizeMod, sizeMod, sizeMod);
        }
    }

    private void SetRotation()
    {
        float randomRotation = Random.Range(-360f, 360f);
        transform.rotation = Quaternion.Euler(0f, 0f, randomRotation);
    }

    private void SetLocationProperties()
    {
        switch (splatLocation)
        {
            case SplatLocation.Background:
                float darkenFactor = 0.8f;
                spriteRenderer.color = new Color(spriteRenderer.color.r * darkenFactor, spriteRenderer.color.g * darkenFactor, spriteRenderer.color.b * darkenFactor);
                spriteRenderer.maskInteraction = SpriteMaskInteraction.None;
                spriteRenderer.sortingOrder = 0;
                break;

            case SplatLocation.Foreground:
                spriteRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                spriteRenderer.sortingOrder = 9;
                break;

        }
    }

    private IEnumerator FadeOut(float duration)
    {
        yield return new WaitForSeconds(5f);
        Color originalColor = spriteRenderer.color;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        // Ensure the object is completely invisible at the end
        spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);

        // Optionally, you can destroy the object after it fades out
        Destroy(gameObject);
    }
}
