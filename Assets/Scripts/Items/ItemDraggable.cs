using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ItemDraggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Item item; // Reference to the ScriptableObject
    private CanvasGroup canvasGroup;
    private Transform originalParent;
    public InventorySlot originalSlot;
    public int stackSize;
    public Text stackSizeText;

    private Inventory inventory;

    private bool isMouseOver = false;
    private InputAction useItemAction;

    public void Initialize(Item newItem)
    {
        stackSize = 1;
        item = newItem;
        GetComponent<Image>().sprite = item.icon;
        inventory = GameObject.FindWithTag("Player").GetComponent<Inventory>();
    }

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        var inputActions = new InputMaster(); // Replace with the generated class name if different
        useItemAction = inputActions.UI.UseItem;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Transform parentObject = transform.parent;
        stackSizeText = parentObject.GetComponentInChildren<Text>();
        stackSizeText.transform.SetParent(transform.root);

        originalParent = transform.parent;
        originalSlot = originalParent.GetComponent<InventorySlot>(); // Store the original slot reference

        canvasGroup.blocksRaycasts = false;
        transform.SetParent(transform.root); // Move to the root canvas to avoid clipping
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition; // Follow the mouse position

        if (stackSizeText != null)
        {
            stackSizeText.transform.position = new Vector3(Input.mousePosition.x + 9, Input.mousePosition.y - 18, Input.mousePosition.z);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // If dropped on a valid slot, the OnDrop method in InventorySlot will handle reparenting
        // If not dropped on a valid slot, return to the original parent
        if (transform.parent == originalParent.root)
        {
            transform.SetParent(originalParent);
            transform.localPosition = Vector3.zero;
        }

        stackSizeText.transform.SetParent(originalParent);
        stackSizeText.transform.localPosition = new Vector3(9, -18, 0);

        canvasGroup.blocksRaycasts = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseOver = true;
        useItemAction.performed += OnUseItem;
        useItemAction.Enable();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseOver = false;
        useItemAction.performed -= OnUseItem;
        useItemAction.Disable();
    }

    private void OnUseItem(InputAction.CallbackContext context)
    {
        UseItem();
    }

    private void UseItem()
    {
        if (this.item is EquippableItem eqItem)
        {
            if(transform.parent.GetComponent<EquipmentSlot>() != null)
            {
                InventorySlot availableSlot = inventory.FindAvailableInventorySlot();
                if (availableSlot != null)
                {
                    availableSlot.SwapItems(transform.parent.GetComponent<EquipmentSlot>());
                    transform.parent = availableSlot.transform;
                }
            }
            else
            {
                EquipmentSlot availableSlot = inventory.FindAvailableEquipmentSlot();
                if (availableSlot != null)
                {
                    availableSlot.SwapItems(transform.parent.GetComponent<InventorySlot>());
                    transform.parent = availableSlot.transform;
                }
            }
        }
        else {

            item.Use();
            if (originalSlot != null)
            {
                originalSlot.UpdateStackSizeUI();
            }
        }
        

        
    }
}
