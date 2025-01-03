using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttributesPlayer : Attributes
{

    private MouseFollow mouseFollow;

    [SerializeField] private float camSize;
    public void ApplyAttributeNode(AttributeNode attributeNode)
    {
        foreach (var modification in attributeNode.attributeModifications)
        {
            ApplyAttributeModification(modification.attributeName, modification.Value);
        }
        // Apply the color change
        AddColor(attributeNode.attributeColor);
        UpdateGuns();
    }


    public void ApplyAttributeModification(EntityAttributes attributeName, float modificationValue)
    {
        switch (attributeName)
        {
            case EntityAttributes.MaxHealth:
                maxHealth += modificationValue;
                currentHealth += modificationValue; // Adjust current health proportionally
                break;
            case EntityAttributes.RamDamage:
                ramDamage += modificationValue;
                break;
            case EntityAttributes.MoveSpeed:
                moveSpeed += modificationValue;
                break;
            case EntityAttributes.RecoilValue:
                recoilValue += modificationValue;
                break;
            case EntityAttributes.KnockbackPower:
                knockbackPower += modificationValue;
                break;
            case EntityAttributes.FireRate:
                fireRate += modificationValue;
                break;
            case EntityAttributes.BulletForce:
                bulletForce += modificationValue;
                break;
            case EntityAttributes.BulletDurability:
                bulletDurability += modificationValue;
                break;
            case EntityAttributes.BulletDamage:
                bulletDamage += modificationValue;
                break;
            case EntityAttributes.BulletSize:
                bulletSize += modificationValue;
                break;
            case EntityAttributes.BulletMass:
                bulletMass += modificationValue;
                break;
            case EntityAttributes.Penetration:
                penetration += (int)modificationValue;
                break;
            case EntityAttributes.TurnSpeed:
                turnSpeed += (int)modificationValue;
                break;
            case EntityAttributes.CamSize:
                camSize += modificationValue;
                UpdateCamSize();
                break;
            default:
                Debug.LogWarning($"Unknown attribute: {attributeName}");
                break;
        }
    }
    public void RemoveAttributeModification(EntityAttributes attributeName, float modificationValue)
    {
        ApplyAttributeModification(attributeName, -modificationValue);
    }

    protected override void Start()
    {
        base.Start();
        this.mouseFollow = GetComponent<MouseFollow>();
        UpdateCamSize();
    }

    private void UpdateCamSize()
    {
        Camera.main.orthographicSize = camSize;
    }

    public override void UpdateGuns()
    {
        base.UpdateGuns();
        mouseFollow.SetTurnSpeed(this.turnSpeed);
    }


}
