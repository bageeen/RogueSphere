using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public Image icon;
    public ItemDraggable currentItem;
    public Text stackSizeText;
    

    void Awake()
    {
        InitializeSlot();
    }

    private void InitializeSlot()
    {
        if (icon == null)
        {
            icon = GetComponentInChildren<Image>();
            if (icon == null)
            {
                Debug.LogError("No Image component found in children of InventorySlot!");
            }
        }

        if (stackSizeText == null)
        {
            stackSizeText = GetComponentInChildren<Text>();
            if (stackSizeText == null)
            {
                Debug.LogError("No Text component found in children of InventorySlot!");
            }
        }

        stackSizeText.enabled = false; // Hide the text initially
        icon.enabled = true;
        icon.raycastTarget = true;
    }

    public virtual void AddItem(ItemDraggable newItem)
    {
        if (newItem == null)
        {
            return;
        }

        if (currentItem != null)
        {
            Debug.Log($"Current Item: {currentItem.item.itemName}, Stackable: {currentItem.item.isStackable}");
            Debug.Log($"New Item: {newItem.item.itemName}, Stackable: {newItem.item.isStackable}");
        }

        if (currentItem != null && currentItem.item.isStackable && currentItem.item.itemName == newItem.item.itemName)
        {
            // If the item is stackable and the same type, increase the stack size
            currentItem.stackSize += 1;
            UpdateStackSizeUI();
            Destroy(newItem.gameObject); // Destroy the new item as it's now stacked
            return;
        }
        else
        {
            currentItem = newItem;

            currentItem.transform.SetParent(transform);
            currentItem.transform.localPosition = Vector3.zero;


            if (icon == null)
            {
                return;
            }

            //icon.sprite = currentItem.item.icon;
            icon.enabled = true;
            UpdateStackSizeUI();
        }
    }

    public virtual void ClearSlot()
    {
        if (currentItem != null)
        {
            currentItem.transform.SetParent(null); // Unparent the item but do not destroy it
            currentItem = null;
        }
        UpdateStackSizeUI();

        icon.sprite = null;
        icon.color = Color.white; // Reset the color to white

        Debug.Log($"Cleared slot: {gameObject.name}");
    }

    public void UpdateStackSizeUI()
    {
        if (currentItem != null)
        {
            stackSizeText.text = currentItem.stackSize.ToString();
            stackSizeText.enabled = currentItem.stackSize > 1;
        }
        else
        {
            stackSizeText.text = "0";
            stackSizeText.enabled = false;
        }

    }

    public virtual void OnDrop(PointerEventData eventData)
    {
        Debug.Log("OnDrop called on " + gameObject.name);
        if (eventData.pointerDrag != null)
        {
            ItemDraggable draggableItem = eventData.pointerDrag.GetComponent<ItemDraggable>();
            if (draggableItem != null)
            {
                Debug.Log("draggableItem found: " + draggableItem.item.itemName);
                InventorySlot originalSlot = draggableItem.originalSlot; // Use the stored reference
                if (originalSlot != null)
                {
                    Debug.Log("originalSlot found: " + originalSlot.gameObject.name);

                    // Check if the item is being dropped back into the same slot
                    if (originalSlot == this)
                    {
                        Debug.Log("Dropped item back into the same slot: " + gameObject.name);
                        return;
                    }

                    if (originalSlot is EquipmentSlot equipmentSlot)
                    {
                        if (currentItem == null || currentItem.item is EquippableItem)
                        {
                            SwapItems(originalSlot);

                            // Reparent the dragged item to the current slot
                            draggableItem.transform.SetParent(transform);
                            RectTransform rectTransform = draggableItem.GetComponent<RectTransform>();
                            rectTransform.anchorMin = Vector2.zero;
                            rectTransform.anchorMax = Vector2.one;
                            rectTransform.offsetMin = Vector2.zero;
                            rectTransform.offsetMax = Vector2.zero;

                            Debug.Log("Dropped item: " + draggableItem.item.itemName + " on slot: " + gameObject.name);
                        }
                    }
                    else
                    {
                        SwapItems(originalSlot);

                        // Reparent the dragged item to the current slot
                        draggableItem.transform.SetParent(transform);
                        RectTransform rectTransform = draggableItem.GetComponent<RectTransform>();
                        rectTransform.anchorMin = Vector2.zero;
                        rectTransform.anchorMax = Vector2.one;
                        rectTransform.offsetMin = Vector2.zero;
                        rectTransform.offsetMax = Vector2.zero;

                        Debug.Log("Dropped item: " + draggableItem.item.itemName + " on slot: " + gameObject.name);
                    }
                }
                else
                {
                    Debug.LogError("originalSlot is null");
                }
            }
            else
            {
                Debug.LogError("draggableItem is null");
            }
        }
        else
        {
            Debug.LogError("pointerDrag is null");
        }
    }

    public virtual void SwapItems(InventorySlot originalSlot)
    {
        Debug.Log($"Swapping items between: {gameObject.name} and {originalSlot.gameObject.name}");

        ItemDraggable tempItem = currentItem;
        // Clear the current slot without destroying the item
        ClearSlot();

        if (originalSlot.currentItem != null)
        {

           
                // Add the item from the original slot to this slot
                AddItem(originalSlot.currentItem);
                originalSlot.ClearSlot();
            

        }

        // Add the item to the original slot if present
        if (tempItem != null)
        {
            originalSlot.AddItem(tempItem);
        }

        UpdateStackSizeUI();
        originalSlot.UpdateStackSizeUI();

        Debug.Log($"Swapped items. {gameObject.name} now has: {(currentItem != null ? currentItem.item.itemName : "none")}, {originalSlot.gameObject.name} now has: {(originalSlot.currentItem != null ? originalSlot.currentItem.item.itemName : "none")}");
    }
}
