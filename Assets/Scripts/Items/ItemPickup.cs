using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Item item;
    public bool isPickedUp = false;

    public Collider2D pickupCollider; // Attach the trigger collider reference via the inspector
    public float unpickableDurationMin = 0.25f; // Time in seconds the scrapMoney is unpickable
    public float unpickableDurationMax = 1f; // Time in seconds the scrapMoney is unpickable



    void Start()
    {
        pickupCollider = GetComponent<CircleCollider2D>();
        StartCoroutine(MakePickupableAfterDelay());
        if (item is ScrapMoneyItem scrapMoneyItem)
        {
            scrapMoneyItem.SetRandomSprite();
            GetComponent<SpriteRenderer>().sprite = item.icon;
        }
    }
    public void UpdateIcon()
    {
        SpriteRenderer sp = GetComponent<SpriteRenderer>();
        sp.sprite = this.item.icon;
    }

    IEnumerator MakePickupableAfterDelay()
    {
        // Initially disable the collider
        pickupCollider.enabled = false;

        // Wait for the defined duration
        yield return new WaitForSeconds(Random.Range(unpickableDurationMin, unpickableDurationMax));

        // Enable the collider to allow pickup
        pickupCollider.enabled = true;
    }
}
