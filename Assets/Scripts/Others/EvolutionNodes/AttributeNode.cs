using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAttributeNode", menuName = "Weapon/Attribute Node")]
public class AttributeNode : EvolutionNode
{
    public ColorEnum attributeColor;
   
    // List to store attribute modifications
    public List<AttributeModification> attributeModifications;
    public AttributeNode()
    {
        attributeModifications = new List<AttributeModification>();
    }
}

[System.Serializable]
public class AttributeModification
{
    public EntityAttributes attributeName;
    public float Value;
}
