using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MoneyDrop : MonoBehaviour
{
    [SerializeField] private GameObject moneyPrefab;
    [SerializeField] private int dropAmount;
    [SerializeField] private SpriteRenderer body;
    [SerializeField] private float dropDuration = 0.4f; // Total duration to drop all items
    [SerializeField] private float dropMinForce;
    [SerializeField] private float dropMaxForce;
    [SerializeField] private int dropAmountVariation;


    private Scrap moneyItem;
    private GameObject parent;

    void Start()
    {
        parent = GameObject.FindWithTag("LootParentObject");
        moneyItem = moneyPrefab.GetComponent<ItemPickup>().item as Scrap;
    }

    public void DropMoney()
    {
        var data = new MoneyDropData
        {
            MoneyPrefab = moneyPrefab,
            DropAmount = dropAmount,
            MoneyItem = moneyItem,
            ParentTransform = parent.transform,
            DropPosition = transform.position,
            DropDuration = dropDuration,
            DropMinForce = dropMinForce,
            DropMaxForce = dropMaxForce,
            DropAmountVariation = dropAmountVariation
        };

        CoroutineRunner.Instance.DropMoney(data);
    }
}

