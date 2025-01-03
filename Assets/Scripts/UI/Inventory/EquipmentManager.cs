using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    public static EquipmentManager instance;
    private Dictionary<EquippableItem, GameObject> equippedItems;
    private GameObject player;


    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of EquipmentManager found!");
            return;
        }
        instance = this;
        equippedItems = new Dictionary<EquippableItem, GameObject>();
        player = GameObject.FindWithTag("Player");
    }


    public void Equip(EquippableItem item)
    {
        if (!equippedItems.ContainsKey(item))
        {
            Transform parent = player.transform.Find("Body");
            GameObject equippedObject = Instantiate(item.prefabEquipped, parent);
            equippedObject.transform.localScale = new Vector3(1f, 1f, 1f);
            equippedItems[item] = equippedObject;
        }
        else
        {
            Debug.Log($"{item.itemName} is already equipped.");
        }
    }

    public void Unequip(EquippableItem item)
    {
        if (equippedItems.ContainsKey(item))
        {
            Destroy(equippedItems[item]);
            equippedItems.Remove(item);
            Debug.Log($"Unequipped {item.itemName}");
        }
        else
        {
            Debug.Log($"{item.itemName} is not equipped.");
        }
    }
}
