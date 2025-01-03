using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

using System.Globalization;
using System.Data;

public class WeaponParameterUpdater : MonoBehaviour
{
    public string csvFileName = "prefabData.csv";
    public GameObject prefab;

    private Dictionary<string, Dictionary<string, string>> data;

    void Start()
    {
        string filePath = Path.Combine(Application.dataPath, "Prefabs/Weapons", csvFileName);
        data = ParseCSV(filePath);
        //LogNestedDictionary(data);
        UpdatePrefabs(data);
    }

    public void LogNestedDictionary(Dictionary<string, Dictionary<string, string>> nestedDict)
    {
        foreach (var outerPair in nestedDict)
        {
            string outerKey = outerPair.Key;
            Dictionary<string, string> innerDict = outerPair.Value;

            Debug.Log($"Outer Key: {outerKey}");
            foreach (var innerPair in innerDict)
            {
                string innerKey = innerPair.Key;
                string innerValue = innerPair.Value;
                Debug.Log($"    Inner Key: {innerKey}, Inner Value: {innerValue}");
            }
        }
    }
    static void PrintDictionary(Dictionary<string, string> dictionary)
    {
        foreach (var kvp in dictionary)
        {
            Debug.Log($"{kvp.Key}: {kvp.Value}");
        }
    }

    static Dictionary<string, Dictionary<string, string>> ParseCSV(string filePath)
    {
        var data = new Dictionary<string, Dictionary<string, string>>();

        var lines = File.ReadAllLines(filePath);

        string[] names = lines[1].Split('/');
        for(int i = 0; i < names.Length; i++)
        {
            data[names[i]] = new Dictionary<string,string>();
        }
        
        for(int i = 2; i < lines.Length;i++)
        {
            var line = lines[i].Split('/');

            for (int j = 1; j < names.Length; j++)
            {
                if (line[j] != "")
                {
                    data[names[j]][line[0]] = line[j];
                }
            }
        }

        return data;
    }

    private void UpdatePrefabs(Dictionary<string, Dictionary<string, string>> data)
    {
        bool isFirstKey = true;
        foreach (var kvp in data)
        {
            if (isFirstKey)
            {
                isFirstKey = false;
                continue;
            }

            string prefabPath = "Assets/Prefabs/Weapons/" + kvp.Key + ".prefab"; // Adjust the path as needed
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

            if (prefab == null)
            {
                Debug.LogWarning("Prefab not found at path: " + prefabPath);
                continue;
            }

            var prefabData = kvp.Value;

            // Modify the prefab instance
            GameObject prefabInstance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);

            // Update AmmoManager parameters
            AmmoManager ammoManager = prefabInstance.GetComponent<AmmoManager>();
            if (ammoManager != null && prefabData.ContainsKey("MaxAmmo") && prefabData.ContainsKey("AmmoPerSecond"))
            {
                ammoManager.maxAmmo = int.Parse(prefabData["MaxAmmo"]);
                ammoManager.ammoPerSecond = int.Parse(prefabData["AmmoPerSecond"]);
                ammoManager.turnSpeedMult = int.Parse(prefabData["TurnSpeedMult"]);
            }

            // Update Main parameters
            Transform mainTransform = prefabInstance.transform.Find("Main");
            if (mainTransform != null)
            {
                UpdateComponentParameters(mainTransform, prefabData, "A");
                UpdateGunParameters(mainTransform, prefabData, "A");
            }

            // Update Alt parameters
            Transform altTransform = prefabInstance.transform.Find("Alt");
            if (altTransform != null)
            {
                UpdateComponentParameters(altTransform, prefabData, "B");
                UpdateGunParameters(altTransform, prefabData, "B");
            }

            // Apply changes to the prefab
            PrefabUtility.SaveAsPrefabAsset(prefabInstance, prefabPath);
            DestroyImmediate(prefabInstance);
        }
    }

    private void UpdateComponentParameters(Transform parent, Dictionary<string, string> data, string suffix)
    {
        var mainComponent = parent.GetComponent<GunController>();
        if (mainComponent != null)
        {
            Dictionary<string, string> filteredData = FilterDictionaryBySuffix(data, suffix);
            mainComponent.UpdatePrefabAttributes(filteredData);
        }
    }

    private void UpdateGunParameters(Transform parent, Dictionary<string, string> data, string suffix)
    {
        GunShoot[] gunShoots = parent.GetComponentsInChildren<GunShoot>();
        Debug.Log($"For the suffix {suffix}, {gunShoots.Length} guns found");

        foreach (GunShoot gunTransform in gunShoots)
        {
            var gunComponent = gunTransform.GetComponent<GunShoot>();
            if (gunComponent != null)
            {
                Dictionary<string, string> filteredData = FilterDictionaryBySuffix(data, suffix);
                gunComponent.UpdatePrefabAttributes(filteredData);
            }
        }
    }
    private Dictionary<string, string> FilterDictionaryBySuffix(Dictionary<string, string> data, string suffix)
    {
        Dictionary<string, string> filteredData = new Dictionary<string, string>();
        foreach (var kvp in data)
        {
            if (kvp.Key.EndsWith(suffix))
            {
                filteredData[kvp.Key] = kvp.Value;
            }
        }
        return filteredData;
    }

}
