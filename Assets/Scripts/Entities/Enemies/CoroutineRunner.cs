using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class CoroutineRunner : MonoBehaviour
{
    private static CoroutineRunner _instance;

    public static CoroutineRunner Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("CoroutineRunner");
                _instance = go.AddComponent<CoroutineRunner>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    public void RunCoroutine(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
    }

    public void DropMoney(MoneyDropData data)
    {
        StartCoroutine(DropMoneyCoroutine(data));
    }

    private IEnumerator DropMoneyCoroutine(MoneyDropData data)
    {

        int finalDropAmount = data.DropAmount + (int)Random.Range(-data.DropAmountVariation, data.DropAmountVariation);


        for (int i = 0; i < finalDropAmount / 3; i++)
        {
            SpawnSingleMoney(data);
            SpawnSingleMoney(data);
            SpawnSingleMoney(data);

            yield return new WaitForSeconds(0.01f); 
        }
        for (int j = 0; j < finalDropAmount % 3; j++)
        {
            SpawnSingleMoney(data);
        }
    }

    private void SpawnSingleMoney(MoneyDropData data)
    {
        GameObject money = Instantiate(data.MoneyPrefab, data.DropPosition, Quaternion.identity, data.ParentTransform);
        ItemPickup itemPickup = money.GetComponent<ItemPickup>();

        /*
        if (itemPickup != null)
        {
            // Create a new instance of the item to avoid shared state
            Scrap newItem = ScriptableObject.CreateInstance<Scrap>();
            newItem.itemName = data.MoneyItem.itemName; // Copy the name
            newItem.possibleSprites = data.MoneyItem.possibleSprites;

            itemPickup.item = newItem;
            newItem.SetRandomSprite();
        }
        */

        itemPickup.UpdateIcon();

        Rigidbody2D rbMoney = money.GetComponent<Rigidbody2D>();
        Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f).normalized;
        rbMoney.AddForce(randomDirection * Random.Range(data.DropMinForce, data.DropMaxForce), ForceMode2D.Force);
    }
}
