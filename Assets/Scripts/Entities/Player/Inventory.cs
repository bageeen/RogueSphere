using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Inventory : MonoBehaviour
{
    public List<ItemDraggable> items = new List<ItemDraggable>();
    [SerializeField] private int maxStackSize;

    //--------------- Controls ------------------------
    private InputMaster inputMaster;
    private bool isPaused;

    public XPBar XpBar;


    void Awake()
    {
        inputMaster = new InputMaster();
        inputMaster.UI.ToggleInventory.performed += ToggleInventory;
        XpBar.Initialize(0, 100);
    }

    void OnEnable()
    {
        inputMaster.UI.Enable();
    }

    void OnDisable()
    {
        inputMaster.UI.Disable();
    }

    private void ToggleInventory(InputAction.CallbackContext context)
    {
        bool isActive = !inventoryPanel.activeSelf;
        inventoryPanel.SetActive(!inventoryPanel.activeSelf);
        PauseGame(isActive);
    }

    public void PauseGame(bool pause)
    {
        if (pause)
        {
            Time.timeScale = 0f;
            isPaused = true;
        }
        else
        {
            Time.timeScale = 1f;
            isPaused = false;
        }
    }

    //--------------------------------------------------

    //--------------- UI Management ----------------------
    public GameObject inventoryPanel;
    public Transform inventoryGrid; // Reference to InventoryGrid
    public GameObject itemDraggablePrefab;

    private List<InventorySlot> inventorySlots;
    private List<EquipmentSlot> equipmentSlots;
    private EquipmentManager equipmentManager;

    public void SetInventorySlots(List<InventorySlot> l)
    {
        this.inventorySlots = l;
    }
    public void SetEquipmentSlots(List<EquipmentSlot> l)
    {
        this.equipmentSlots = l;
    }

    //--------------------------------------------------

    void Start()
    {
        inventoryPanel.SetActive(true);
        equipmentManager = EquipmentManager.instance; // Assuming you have a singleton EquipmentManager

        inventoryPanel.SetActive(false);
    }

    public void AddExperience(float amount)
    {
        XpBar.AddXP(amount);
    }

    public void AddItem(Item newItem)
    {
        if (newItem.isStackable)
        {
            foreach (InventorySlot slot in inventorySlots)
            {
                if (slot.currentItem != null && slot.currentItem.item.itemName == newItem.itemName && slot.currentItem.stackSize < maxStackSize)
                {
                    GameObject newItemObject = Instantiate(itemDraggablePrefab, slot.transform);
                    ItemDraggable newItemDraggable = newItemObject.GetComponent<ItemDraggable>();
                    newItemDraggable.Initialize(newItem);

                    slot.AddItem(newItemDraggable);
                    return;
                }
            }
        }
        
        // Find the first empty slot
        InventorySlot emptySlot = null;
        foreach (Transform child in inventoryGrid)
        {
            InventorySlot slot = child.GetComponent<InventorySlot>();
            if (slot != null && slot.currentItem == null)
            {
                emptySlot = slot;
                break;
            }
        }

        if (emptySlot != null)
        {
            newItem.Initialize(gameObject);
            // Instantiate the ItemDraggable and parent it to the empty slot
            GameObject newItemObject = Instantiate(itemDraggablePrefab, emptySlot.transform);
            ItemDraggable newItemDraggable = newItemObject.GetComponent<ItemDraggable>();
            newItemDraggable.Initialize(newItem);

            // Set RectTransform to match the parent slot
            RectTransform rectTransform = newItemObject.GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;

            items.Add(newItemDraggable);

            emptySlot.AddItem(newItemDraggable);

        }
        else
        {
        }
    }

    void UpdateUI()
    {
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            if (i < items.Count)
            {
                inventorySlots[i].AddItem(items[i]);
            }
            else
            {
                inventorySlots[i].ClearSlot();
            }
        }
    }

    public void UseItem(Item item)
    {
        item.Use();
    }

    public EquipmentSlot FindAvailableEquipmentSlot()
    {
        
        foreach (EquipmentSlot slot in equipmentSlots)
        {
            Debug.Log("We try a slot");
            if (slot.currentItem == null)
            {
                Debug.Log("We found a slot");
                return slot;
            }
        }
        return null;
    }

    public InventorySlot FindAvailableInventorySlot()
    {
        foreach (InventorySlot slot in inventorySlots)
        {
            if (slot.currentItem == null)
            {
                return slot;
            }
        }
        return null;
    }
}
