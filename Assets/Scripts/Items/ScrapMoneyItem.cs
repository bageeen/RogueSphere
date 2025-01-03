using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewScrapMoneyItem", menuName = "Inventory/Items/ScrapMoney")]
public class ScrapMoneyItem : Item
{
    public int value;

    [SerializeField] public Sprite[] possibleSprites; // Array to hold possible sprites
    [SerializeField] public int minValue;
    [SerializeField] public int maxValue;



    public void SetRandomSprite()
    {
        if (possibleSprites != null && possibleSprites.Length > 0)
        {
            value = Random.Range(minValue,maxValue);
            this.icon = possibleSprites[Random.Range(0, possibleSprites.Length)];
        }
    }
}
