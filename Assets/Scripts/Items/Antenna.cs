using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAntennaItem", menuName = "Inventory/Items/Antenna")]
public class AntennaItem : EquippableItem
{
    public float cameraSizeIncrease = 5f;



    public override void Use()
    {
        base.Use();
    }

    public override void Equip()
    {
        base.Equip();
        Camera.main.orthographicSize += cameraSizeIncrease;
        Debug.Log("Equipping " + itemName + ": Camera size increased by " + cameraSizeIncrease);
    }

    public override void Unequip()
    {
        base.Unequip();
        Camera.main.orthographicSize -= cameraSizeIncrease;
        Debug.Log("Unequipping " + itemName + ": Camera size decreased by " + cameraSizeIncrease);
    }
}
