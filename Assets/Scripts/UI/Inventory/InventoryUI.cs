using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public List<InventorySlot> inventorySlots;
    public GameObject inventorySlotPrefab;
    public Transform inventoryGrid;
    public int numberOfSlots = 20;

    void Awake()
    {

        for (int i = 0; i < numberOfSlots; i++)
        {
            GameObject slot = Instantiate(inventorySlotPrefab, inventoryGrid);
            inventorySlots.Add(slot.GetComponent<InventorySlot>());
        }

        Inventory inventory = GameObject.FindWithTag("Player").GetComponent<Inventory>(); ;
        inventory.SetInventorySlots(inventorySlots);

    }
}
