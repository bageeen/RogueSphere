using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
[CreateAssetMenu(fileName = "NewEquippableItem", menuName = "Inventory/Items/EquippableItem")]
public class EquippableItem : Item
{

    public GameObject prefabEquipped;
    protected GameObject player;
    private Inventory inventory;

    public override void Use()
    {
        
        
    }

    public virtual void Equip()
    {
        Debug.Log($"WE EQUIP {prefabEquipped}");
        EquipmentManager.instance.Equip(this); // Pass the appropriate EquipmentSlot if needed

    }

    public virtual void Unequip()
    {
        Debug.Log($"WE UNEQUIP!");
        EquipmentManager.instance.Unequip(this);


    }
}
