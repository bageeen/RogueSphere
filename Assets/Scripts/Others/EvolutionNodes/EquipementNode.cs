using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEquipementNode", menuName = "Weapon/Equipement Node")]
public class EquipementNode : EvolutionNode
{
    public Sprite equipementIcon;
    public GameObject equipementPrefab;

    public int tier = 1;
    public string type;

    public EquipementNode nextTier; // Reference to the next tier of this equipment

    // Attribute modifications
    public List<AttributeModification> attributeModifications;

    public override GameObject GetPrefab()
    {
        return equipementPrefab;
    }
}


