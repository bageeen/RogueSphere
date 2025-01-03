using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttributesEnemy : Attributes
{
    private MoneyDrop moneyDropScript;
    [HideInInspector] public int level;

    protected override void Start()
    {
        base.Start();
        if (TryGetComponent<MoneyDrop>(out MoneyDrop moneyScript))
        {
            moneyDropScript = moneyScript;
        }
    }

    protected override void isDead()
    {
        if (currentHealth <= 0 && !isDeadBool)
        {
            HealthBarManager.Instance.RemoveHealthBar(gameObject);
            isDeadBool = true;
            GunControllerEnemy gunC = GetComponent<GunControllerEnemy>();

            if (gunC != null)
            {
                gunC.enabled = false;
            }

            moneyDropScript.DropMoney();


            rb.isKinematic = false;

            ApplyDeathImpulse();
            StartCoroutine(DisableScriptsAfterDelay(0.5f)); // Optional delay for the impulse to be noticeable
        }
    }

    public void ApplyAttributeModification(EntityAttributes attributeName, float modificationValue)
    {
        switch (attributeName)
        {
            case EntityAttributes.MaxHealth:
                maxHealth += modificationValue;
                currentHealth += modificationValue; // Adjust current health proportionally
                break;
            case EntityAttributes.HealthRegen:
                healthRegen += modificationValue;
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

            default:
                Debug.LogWarning($"Unknown attribute: {attributeName}");
                break;
        }
    }
}
