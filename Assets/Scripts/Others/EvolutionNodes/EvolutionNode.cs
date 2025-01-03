using System.Collections.Generic;
using UnityEngine;


public class EvolutionNode : ScriptableObject
{

    public string description;
    public float weight = 1.0f; // Default weight
    public RarityColors rarityLevel;

    public virtual GameObject GetPrefab()
    {
        return null;
    }

}