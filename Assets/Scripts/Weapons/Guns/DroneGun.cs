using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneGun : GunShoot
{
    [SerializeField] private GameObject droneBulletPrefab;
    [SerializeField] private float initialDroneDurability;
    [SerializeField] private float initialDroneDamage;
    [SerializeField] private float droneBulletSize;
    [SerializeField] private float maxBounce;
    [SerializeField] private float bounceDamageReductionFactor;
    [SerializeField] private int penetration;
    [SerializeField] private float followSpeed;
    [SerializeField] private float droneTurnSpeed;
    [SerializeField] private int maxDrones = 5; // Maximum number of drones allowed

    protected float droneDurability;
    protected float droneDamage;
    protected float droneMass;
    protected int effectivePenetration;
    protected float droneForce;

    private List<GameObject> activeDrones = new List<GameObject>(); // List to track active drones

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        // Remove null entries from the list (i.e., destroyed drones)
        activeDrones.RemoveAll(drone => drone == null);
    }

    public override void Shoot()
    {
        if (canShoot() && activeDrones.Count < maxDrones)
        {
            Recoil();
            CreateDroneBullet();
        }

        base.Shoot();
    }

    private void CreateDroneBullet()
    {
        GameObject droneBullet = Instantiate(droneBulletPrefab, firePoint.position, firePoint.rotation, parentBullets.transform); // Reset rotation
        DroneBullet droneBulletScript = droneBullet.GetComponent<DroneBullet>();
        droneBulletScript.scale = new Vector3(droneBulletSize, droneBulletSize, droneBulletSize); // Change size of the bullet
        droneBullet.GetComponent<Rigidbody2D>().mass = gunController.effectiveBulletMass;
        droneBulletScript.mass = gunController.effectiveBulletMass;
        droneBulletScript.maxHealth = entity.GetBulletDurability() * initialDroneDurability;
        droneBulletScript.damage = entity.GetBulletDamage() * initialDroneDamage;
        droneBulletScript.turnSpeed = this.droneTurnSpeed;
        droneBulletScript.SetMaxBounce(maxBounce, bounceDamageReductionFactor);
        droneBulletScript.knockbackPower = entity.GetKnockbackPower() * initialKnockbackPower;
        droneBulletScript.penetration = entity.GetPenetration() + penetration;
        droneBulletScript.followSpeed = followSpeed;
        droneBulletScript.tag = bulletsTag;
        droneBulletScript.gameObject.layer = LayerMask.NameToLayer(bulletsLayer);

        Rigidbody2D rbshot = droneBullet.GetComponent<Rigidbody2D>();
        gunAnimator.SetTrigger("Shoot");
        rbshot.AddForce(firePoint.up * gunController.effectiveBulletForce, ForceMode2D.Impulse);

        // Add the new drone to the list
        activeDrones.Add(droneBullet);
    }
}
