using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EquipmentSlot : InventorySlot
{


    public override void AddItem(ItemDraggable newItem)
    {
        if (newItem.item is EquippableItem equippableItem)
        {
            // Equip the item if it's an equippable item
            base.AddItem(newItem);
            equippableItem.Equip();
        }
        else
        {
            Debug.Log("Item is not equippable: " + newItem.item.itemName);
        }
    }

    public override void ClearSlot()
    {
        if (currentItem != null && currentItem.item is EquippableItem equippableItem)
        {
            // Unequip the item if it's an equippable item
            equippableItem.Unequip();
        }
        base.ClearSlot();
    }


    public override void OnDrop(PointerEventData eventData)
    {
        //Debug.Log("OnDrop called on equipment slot: " + gameObject.name);
        if (eventData.pointerDrag != null)
        {
            ItemDraggable draggableItem = eventData.pointerDrag.GetComponent<ItemDraggable>();
            if (draggableItem != null)
            {
                //Debug.Log("draggableItem found: " + draggableItem.item.itemName);
                InventorySlot originalSlot = draggableItem.originalSlot; // Use the stored reference
                if (originalSlot != null)
                {
                    //Debug.Log("originalSlot found: " + originalSlot.gameObject.name);

                    // Check if the item is being dropped back into the same slot
                    if (originalSlot == this)
                    {
                        //Debug.Log("Dropped item back into the same slot: " + gameObject.name);
                        return;
                    }

                    // Ensure the item is an EquippableItem
                    if (draggableItem.item is EquippableItem equippableItem)
                    {
                        //Debug.Log("IsEquippable!!!!!!!!!!!!!!!!!!!!!");
                        SwapItems(originalSlot);

                        // Reparent the dragged item to the current slot
                        draggableItem.transform.SetParent(transform);
                        RectTransform rectTransform = draggableItem.GetComponent<RectTransform>();
                        rectTransform.anchorMin = Vector2.zero;
                        rectTransform.anchorMax = Vector2.one;
                        rectTransform.offsetMin = Vector2.zero;
                        rectTransform.offsetMax = Vector2.zero;

                        //Debug.Log("Dropped equippable item: " + equippableItem.itemName + " on slot: " + gameObject.name);
                    }
                    else
                    {
                        //Debug.LogError("Item is not equippable: " + draggableItem.item.itemName);
                    }
                }
                else
                {
                    //Debug.LogError("originalSlot is null");
                }
            }
            else
            {
                //Debug.LogError("draggableItem is null");
            }
        }
        else
        {
            //Debug.LogError("pointerDrag is null");
        }
    }

    public override void SwapItems(InventorySlot originalSlot)
    {
        //Debug.Log($"Swapping items between: {gameObject.name} and {originalSlot.gameObject.name}");


        ItemDraggable tempItem = currentItem;

        // Clear the current slot without destroying the item
        ClearSlot();

        // Add the item from the original slot to this slot
        if (originalSlot.currentItem != null && originalSlot.currentItem.item is EquippableItem)
        {
            ItemDraggable originalTempItem = originalSlot.currentItem;
            originalSlot.ClearSlot(); 
            AddItem(originalTempItem);

        }

        // Add the item to the original slot if present
        if (tempItem != null)
        {
            originalSlot.AddItem(tempItem);
        }
    }
}
