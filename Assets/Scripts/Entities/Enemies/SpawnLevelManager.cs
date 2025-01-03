using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnLevelManager : MonoBehaviour
{

    [SerializeField] private AttributeModification[] modifs;
    private AttributesEnemy enemyAttributes;

    void Start()
    {
        enemyAttributes = GetComponent<AttributesEnemy>();
        
    }

    public void ApplyLevels(int level)
    {
        enemyAttributes.level = level;
        foreach (AttributeModification modif in modifs)
        {
            float finalMult = modif.Value * level;
            
            enemyAttributes.ApplyAttributeModification(modif.attributeName, finalMult);
        }
        enemyAttributes.UpdateGuns();
    }
    

}
