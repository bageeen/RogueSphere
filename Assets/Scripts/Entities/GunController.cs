using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;

public class GunController : MonoBehaviour
{
    [SerializeField] protected List<GunShoot> guns;
    [SerializeField] protected int gunsPerGroup = 1; // Number of guns to fire simultaneously

    [SerializeField] public float initialBulletMass;
    [SerializeField] public float initialBulletForce;
    [SerializeField] public float initialBulletLinearDrag;


    protected int lastFiredGroup = 0;
    protected float nextFireTime = 0f;
    protected List<float> groupFireRates;


    [HideInInspector] public Attributes entity;
    [SerializeField] protected float minSplatSize = 0.5f;
    [SerializeField] protected float maxSplatSize = 1.2f;


    [HideInInspector] public float effectiveBulletMass { get; set; }
    [HideInInspector] public float effectiveBulletForce { get; set; }
    [HideInInspector] public float effectiveLinearDrag { get; set; }


    public float GetMinSplatSize()
    {
        return minSplatSize;
    }
    public float GetMaxSplatSize()
    {
        return maxSplatSize;
    }

    protected virtual void Start()
    {
        entity = GetComponentInParent<Attributes>();
        
        UpdateAttributes();
    }

    protected virtual void FixedUpdate()
    {

        return;
    }
    public virtual float Shoot()
    {
        // Calculate the total number of groups
        int totalGroups = Mathf.CeilToInt((float)guns.Count / gunsPerGroup);

        // Get the current group index
        int currentGroupIndex = lastFiredGroup % totalGroups;

        // Fire all guns in the current group
        for (int i = 0; i < gunsPerGroup; i++)
        {
            int gunIndex = currentGroupIndex * gunsPerGroup + i;
            if (gunIndex < guns.Count)
            {
                guns[gunIndex].Shoot();
            }
        }

        // Move to the next group
        lastFiredGroup++;

        // Calculate the fire rate for the current group
        float currentGroupFireRate = groupFireRates[currentGroupIndex];

        // Set the next fire time based on the current group's fire rate divided by the total number of groups
        return nextFireTime = Time.time + (1 / (currentGroupFireRate * totalGroups));
    }

    public virtual void UpdateAttributes()
    {
        GunShoot[] gunShoots = GetComponentsInChildren<GunShoot>();
        foreach (var gunShoot in gunShoots)
        {
            gunShoot.UpdateAttributes();
        }
        CalculateGroupFireRates();
        effectiveBulletForce = initialBulletForce * entity.GetBulletForce();
        effectiveBulletMass = initialBulletMass * entity.GetBulletMass();
        effectiveLinearDrag = initialBulletLinearDrag;
    }


    protected void CalculateGroupFireRates()
    {
        groupFireRates = new List<float>();
        int totalGroups = Mathf.CeilToInt((float)guns.Count / gunsPerGroup);

        for (int group = 0; group < totalGroups; group++)
        {
            float groupFireRate = 0f;
            int groupStartIndex = group * gunsPerGroup;
            int groupEndIndex = Mathf.Min(groupStartIndex + gunsPerGroup, guns.Count);

            for (int i = groupStartIndex; i < groupEndIndex; i++)
            {
                groupFireRate += guns[i].GetFireRate();
            }

            groupFireRate /= (groupEndIndex - groupStartIndex);
            groupFireRates.Add(groupFireRate);
        }
    }

    public void UpdatePrefabAttributes(Dictionary<string, string> data)
    {
        foreach (var kvp in data)
        {
            string tmpName = kvp.Key;
            string parameterName = char.ToLower(tmpName[0]) + tmpName.Substring(1, tmpName.Length - 2);
            string parameterValue = kvp.Value;
            UpdateParameter(this, parameterName, parameterValue);
        }
    }

    private void UpdateParameter(Component component, string parameterName, string parameterValue)
    {
        Type type = component.GetType();
        FieldInfo field = type.GetField(parameterName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        PropertyInfo property = type.GetProperty(parameterName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        if (field != null)
        {
            // Update field value dynamically
            try
            {
                object convertedValue = Convert.ChangeType(parameterValue, field.FieldType);
                field.SetValue(component, convertedValue);
                Debug.Log($"Updated field {parameterName} with value {convertedValue}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to update field {parameterName}: {ex.Message}");
            }
        }
        else if (property != null)
        {
            // Update property value dynamically
            try
            {
                object convertedValue = Convert.ChangeType(parameterValue, property.PropertyType);
                property.SetValue(component, convertedValue);
                Debug.Log($"Updated property {parameterName} with value {convertedValue}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to update property {parameterName}: {ex.Message}");
            }
        }
        else
        {
            Debug.LogWarning($"Parameter not found: {parameterName}");
        }
    }
}


