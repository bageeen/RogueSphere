using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class ScrapMoney : MonoBehaviour
{

    [SerializeField] private SpriteAtlas spriteAtlas;
    [SerializeField] private string[] scrapSpritesNames;
    [SerializeField] private float maxScale;
    [SerializeField] private float minScale;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        AssignRandomSprite();
        SetRandomScale();
    }

    void AssignRandomSprite()
    {

        if (scrapSpritesNames.Length > 0)
        {
            int randomIndex = Random.Range(0, scrapSpritesNames.Length);
            Sprite randomSprite = spriteAtlas.GetSprite(scrapSpritesNames[randomIndex]);
            spriteRenderer.sprite = randomSprite;
        }
    }

    public void SetSpriteColor(Color c)
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = c;
       
    }

    void SetRandomScale()
    {
        float randomScale = Random.Range(minScale, maxScale);
        transform.localScale = new Vector3(randomScale, randomScale, 1f);
    }

}
