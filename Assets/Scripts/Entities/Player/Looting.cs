using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Looting : MonoBehaviour
{
    [SerializeField] private CircleCollider2D pickupCollider;
    [SerializeField] private float pickupRadius;
    [SerializeField] private Transform vacuumPoint;
    [SerializeField] private Inventory inventory;

    void Start()
    {
        if (pickupRadius <= 0)
        {
            pickupRadius = 2.2f;
        }
    }

    void Update()
    {
        pickupCollider.radius = pickupRadius;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        ItemPickup itemPickup = other.GetComponent<ItemPickup>();
        if (itemPickup != null && !itemPickup.isPickedUp)
        {
            itemPickup.isPickedUp = true;
            Item item = itemPickup.item;
            if (item != null)
            {
                {
                    if (item is XPOrb orb)
                    {
                        inventory.AddExperience(orb.amount);
                    }
                    else 
                    {
                        inventory.AddItem(item);
                    }
                    
                    StartCoroutine(MoveItemToPlayer(itemPickup.gameObject)); // Destroy the game object after adding the value to inventory
                    
                }
            }
        }
    }


    IEnumerator MoveItemToPlayer(GameObject itemObject)
    {
        Vector3 targetPosition = vacuumPoint.position;

        float elapsedTime = 0f;
        Vector3 initialPosition = itemObject.transform.position;

        float distance = Vector3.Distance(targetPosition, itemObject.transform.position);
        while (distance > 2.2f)
        {
            elapsedTime += Time.deltaTime;
            itemObject.transform.position = Vector3.Lerp(initialPosition, targetPosition, elapsedTime / 0.2f);
            distance = Vector3.Distance(targetPosition, itemObject.transform.position);

            yield return null;
        }

        // Ensure the item reaches exactly the target position

        Destroy(itemObject.gameObject);
        
    }
}
