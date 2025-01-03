using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class EquipmentUI : MonoBehaviour
{
    public List<EquipmentSlot> equipmentSlots;
    public GameObject equipmentSlotPrefab;
    public Transform equipmentGrid;
    public int numberOfSlots = 4;

    void Start()
    {
        for (int i = 0; i < numberOfSlots; i++)
        {
            GameObject slot = Instantiate(equipmentSlotPrefab, equipmentGrid);
            equipmentSlots.Add(slot.GetComponent<EquipmentSlot>());
        }

        Inventory inventory = GameObject.FindWithTag("Player").GetComponent<Inventory>(); ;
        inventory.SetEquipmentSlots(equipmentSlots);
    }
}

