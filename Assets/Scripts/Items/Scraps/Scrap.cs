using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Scrap", menuName = "Inventory/Items/Scrap")]

public class Scrap : Item
{

    [SerializeField] public Sprite[] possibleSprites; // Array to hold possible sprites


    void Awake()
    {
        isStackable = true;
    }


    public void SetRandomSprite()
    {
        if (possibleSprites != null && possibleSprites.Length > 0)
        {
            this.icon = possibleSprites[Random.Range(0, possibleSprites.Length)];
            
        }
    }
}
