using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitGun : GunShoot
{
    // GrenadeBullet attributes
    [SerializeField] private GameObject grenadeBulletPrefab;
    [SerializeField] private float initialBulletDurability;
    [SerializeField] private float initialBulletDamage;
    [SerializeField] private float bulletSize;
    [SerializeField] private float maxBounce;
    [SerializeField] private float bounceDamageReductionFactor;
    [SerializeField] private int penetration;
    [SerializeField] private float minSpeed;

    [SerializeField] private int numberOfBullets; // Number of ClassicBullets to spawn on explosion
    [SerializeField] private float explosionForce; // Force applied to each ClassicBullet
    [SerializeField] private float angleSpread; // Angle spread for the exploded bullets

    // ClassicBullet attributes
    [SerializeField] private float explodedBulletSize;
    [SerializeField] private float explodedBulletMass;
    [SerializeField] private float explodedBulletDrag;
    [SerializeField] private float explodedBulletDurability;
    [SerializeField] private float explodedBulletDamage;
    [SerializeField] private float explodedMaxBounce;
    [SerializeField] private float explodedBounceDamageReductionFactor;
    [SerializeField] private float explodedKnockbackPower;
    [SerializeField] private int explodedEffectivePenetration;
    [SerializeField] private float explodedMinSpeed;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void UpdateAttributes()
    {
        base.UpdateAttributes();
        //complete that
    }

    public override void Shoot()
    {
        if (canShoot())
        {
            Recoil();
            CreateGrenadeBullet();
        }

        base.Shoot();
    }

    private void CreateGrenadeBullet()
    {
        GameObject grenadeBullet = Instantiate(grenadeBulletPrefab, firePoint.position, firePoint.rotation, parentBullets.transform);
        GrenadeBullet grenadeBulletScript = grenadeBullet.GetComponent<GrenadeBullet>();
        grenadeBulletScript.scale = new Vector3(bulletSize, bulletSize, bulletSize); // Change size of the bullet
        grenadeBullet.GetComponent<Rigidbody2D>().mass = gunController.effectiveBulletMass;
        grenadeBulletScript.mass = gunController.effectiveBulletMass;
        grenadeBulletScript.linearDrag = gunController.effectiveLinearDrag;
        grenadeBulletScript.maxHealth = entity.GetBulletDurability() * initialBulletDurability;
        grenadeBulletScript.damage = entity.GetBulletDamage() * initialBulletDamage;
        grenadeBulletScript.SetMaxBounce(maxBounce, bounceDamageReductionFactor);
        grenadeBulletScript.knockbackPower = entity.GetKnockbackPower() * initialKnockbackPower;
        grenadeBulletScript.penetration = entity.GetPenetration() + penetration;
        grenadeBulletScript.minSpeed = minSpeed;
        grenadeBulletScript.numberOfBullets = numberOfBullets;
        grenadeBulletScript.explosionForce = explosionForce;
        grenadeBulletScript.angleSpread = angleSpread;
        grenadeBulletScript.explodedBulletSize = explodedBulletSize;
        grenadeBulletScript.explodedBulletMass = explodedBulletMass;
        grenadeBulletScript.explodedBulletDrag = explodedBulletDrag;
        grenadeBulletScript.explodedBulletDurability = explodedBulletDurability;
        grenadeBulletScript.explodedBulletDamage = explodedBulletDamage;
        grenadeBulletScript.explodedMaxBounce = explodedMaxBounce;
        grenadeBulletScript.explodedBounceDamageReductionFactor = explodedBounceDamageReductionFactor;
        grenadeBulletScript.explodedKnockbackPower = explodedKnockbackPower;
        grenadeBulletScript.explodedEffectivePenetration = explodedEffectivePenetration;
        grenadeBulletScript.explodedMinSpeed = explodedMinSpeed;
        grenadeBulletScript.tag = bulletsTag;
        grenadeBulletScript.gameObject.layer = LayerMask.NameToLayer(bulletsLayer);

        Rigidbody2D rbshot = grenadeBullet.GetComponent<Rigidbody2D>();
        gunAnimator.SetTrigger("Shoot");
        rbshot.AddForce(firePoint.up * gunController.effectiveBulletForce, ForceMode2D.Impulse);
    }
}
