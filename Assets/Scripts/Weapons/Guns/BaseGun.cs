using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseGun : GunShoot
{

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float initialBulletDurability;
    [SerializeField] private float initialBulletDamage;
    [SerializeField] private float bulletSize;
    [SerializeField] private float maxBounce;
    [SerializeField] private float bounceDamageReductionFactor;
    [SerializeField] private int penetration;
    [SerializeField] private float minSpeed;


    protected float bulletDurability;
    protected float bulletDamage;
    protected float bulletMass;
    protected float bulletLinearDrag;
    protected int effectivePenetration;
    protected float bulletForce;

    public float GetBulletSpeed() => bulletForce;

    protected override void Awake()
    {
        base.Awake();  
    }



    protected override void InitializeBulletProperties()
    {
        if (entity != null)
        {
            recoilValue = entity.GetRecoilValue() * initialRecoilValue;
            fireRate = entity.GetFireRate() * initialFireRate;
            bulletDurability = entity.GetBulletDurability() * initialBulletDurability;
            knockbackPower = entity.GetKnockbackPower() * initialKnockbackPower;
            bulletDamage = entity.GetBulletDamage() * initialBulletDamage;

            bulletMass = gunController.effectiveBulletMass;
            bulletForce = gunController.effectiveBulletForce;
            bulletLinearDrag = gunController.effectiveLinearDrag;

            effectivePenetration = entity.GetPenetration() + this.penetration;
        }
        
    }
    
    public override void UpdateAttributes()
    {
        recoilValue = entity.GetRecoilValue() * initialRecoilValue;
        fireRate = entity.GetFireRate() * initialFireRate;
        bulletDurability = entity.GetBulletDurability() * initialBulletDurability;
        knockbackPower = entity.GetKnockbackPower() * initialKnockbackPower;
        bulletDamage = entity.GetBulletDamage() * initialBulletDamage;

        bulletMass = gunController.effectiveBulletMass;
        bulletForce = gunController.effectiveBulletForce;
        bulletLinearDrag = gunController.effectiveLinearDrag;

        effectivePenetration = entity.GetPenetration() * this.penetration;

    }

    public override void Shoot()
    {
        
        if (canShoot()) {
            Recoil();
            CreateBullet();
        }

        base.Shoot();

    }

    private void CreateBullet()
    {
        UpdateAttributes();
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation, parentBullets.transform);
        ClassicBullet bulletScript = bullet.GetComponent<ClassicBullet>();
        bulletScript.scale = new Vector3(bulletSize, bulletSize, bulletSize); // Change size of the bullet
        bulletScript.mass = bulletMass;
        bulletScript.linearDrag = bulletLinearDrag;
        bulletScript.maxHealth = bulletDurability;
        bulletScript.damage = bulletDamage;
        bulletScript.SetMaxBounce(maxBounce, bounceDamageReductionFactor);
        bulletScript.knockbackPower = knockbackPower;
        bulletScript.penetration = effectivePenetration;
        bulletScript.minSpeed = minSpeed;
        bulletScript.SetColor(entity.GetRandomColor());
        bulletScript.SetSplatSize(minSplatSize, maxSplatSize);
        bullet.tag = bulletsTag;
        bullet.layer = LayerMask.NameToLayer(bulletsLayer);
        Rigidbody2D rbshot = bullet.GetComponent<Rigidbody2D>();
        gunAnimator.SetTrigger("Shoot");
        rbshot.AddForce(firePoint.up * bulletForce, ForceMode2D.Impulse);
    }
}
