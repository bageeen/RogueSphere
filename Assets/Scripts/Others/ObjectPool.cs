using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance;

    private void Awake()
    {
        Instance = this;
    }

    private Dictionary<string, Queue<GameObject>> poolDictionary = new Dictionary<string, Queue<GameObject>>();

    public void CreatePool(string tag, GameObject prefab, int size)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            poolDictionary[tag] = new Queue<GameObject>();

            for (int i = 0; i < size; i++)
            {
                GameObject obj = Instantiate(prefab);
                obj.SetActive(false);
                poolDictionary[tag].Enqueue(obj);
            }
        }
    }

    public GameObject GetObject(string tag)
    {
        if (!poolDictionary.ContainsKey(tag) || poolDictionary[tag].Count == 0)
        {
            Debug.LogWarning("Pool with tag " + tag + " is empty or does not exist.");
            return null;
        }

        GameObject obj = poolDictionary[tag].Dequeue();
        obj.SetActive(true);
        return obj;
    }

    public void ReturnObject(string tag, GameObject obj)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + " does not exist.");
            return;
        }

        obj.SetActive(false);
        poolDictionary[tag].Enqueue(obj);
    }
}
