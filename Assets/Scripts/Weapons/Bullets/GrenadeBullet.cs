using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeBullet : Bullet
{


    [SerializeField] private GameObject classicBulletPrefab; // Reference to ClassicBullet prefab
    public int numberOfBullets = 10; // Number of ClassicBullets to spawn on explosion
    public float explosionForce = 5f; // Force applied to each ClassicBullet
    public float angleSpread = 360f; // Angle spread for the exploded bullets

    // Properties for setting up the ClassicBullet (exploded bullets)
    public float explodedBulletSize = 1f;
    public float explodedBulletMass = 1f;
    public float explodedBulletDrag = 0.2f;
    public float explodedBulletDurability = 10f;
    public float explodedBulletDamage = 5f;
    public float explodedMaxBounce = 3f;
    public float explodedBounceDamageReductionFactor = 0.5f;
    public float explodedKnockbackPower = 2f;
    public int explodedEffectivePenetration = 1;
    public float explodedMinSpeed = 2f;

    private Vector2 preCollisionVelocity;


    protected override void FixedUpdate()
    {
        if (rb.velocity.magnitude < minSpeed && !isDead)
        {
            health = 0;
            CheckAlive();
        }

        // Store the current velocity as pre-collision velocity
        if (!isDead)
        {
            preCollisionVelocity = rb.velocity;
        }
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<Attributes>(out Attributes enemyComponent) && !isDead)
        {
            enemyComponent.TakeKnockback(gameObject, this.knockbackPower);
            enemyComponent.TakeDamage(this.GetDamage());
            this.penetration -= 1;
            if (this.penetration < 1)
            {
                this.health = 0;
            }
        }
        else if (collision.gameObject.CompareTag("SolidObjects") && !isDead)
        {
            this.bounces -= 1;
            if (bounces < 1)
            {
                this.health = 0;
            }
        }
        CheckAlive();
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (this.gameObject.tag == "PlayerProj")
        {
            CheckTriggers(collision);
        }
    }

    public void CheckTriggers(Collider2D collision)
    {
        Bullet bullet = collision.GetComponent<Bullet>();
        if (bullet != null && bullet.gameObject.tag != this.gameObject.tag && !isDead && !bullet.isDead)
        {
            float otherDamage = bullet.GetDamage();
            bullet.TakeDamage(this.GetDamage());
            this.TakeDamage(otherDamage);

            bullet.CheckAlive();
            CheckAlive();
        }
    }

    public override void CheckAlive()
    {
        if (this.health <= 0 && !isDead)
        {
            this.gameObject.layer = LayerMask.NameToLayer("DeadProjectiles");
            this.isDead = true;
            Explode();
            if (paint != null)
            {
                paint.CastRay(transform.position, bulletColor, minSplatSize, maxSplatSize);
            }
            Destroy(gameObject);
        }
    }


        public void Explode()
    {
        for (int i = 0; i < numberOfBullets; i++)
        {
            CreateBullet(i);
        }
    }

    private void CreateBullet(int index)
    {
        GameObject bullet = Instantiate(classicBulletPrefab, transform.position, Quaternion.identity, parentBullets);
        ClassicBullet bulletScript = bullet.GetComponent<ClassicBullet>();
        bulletScript.scale = new Vector3(explodedBulletSize, explodedBulletSize, explodedBulletSize); // Change size of the bullet
        bullet.GetComponent<Rigidbody2D>().mass = explodedBulletMass;
        bulletScript.mass = explodedBulletMass;
        bulletScript.linearDrag = explodedBulletDrag;
        bulletScript.maxHealth = explodedBulletDurability;
        bulletScript.damage = explodedBulletDamage;
        bulletScript.SetMaxBounce(explodedMaxBounce, explodedBounceDamageReductionFactor);
        bulletScript.knockbackPower = explodedKnockbackPower;
        bulletScript.penetration = explodedEffectivePenetration;
        bulletScript.minSpeed = explodedMinSpeed;
        bullet.tag = this.tag; // Use the same tag as the GrenadeBullet
        bullet.layer = this.gameObject.layer; // Use the same layer as the GrenadeBullet
        Rigidbody2D rbshot = bullet.GetComponent<Rigidbody2D>();

        // Calculate the direction for each bullet based on the angle spread
        Vector2 forwardDirection = preCollisionVelocity.normalized; // Use pre-collision velocity for direction
        float angleOffset = angleSpread / numberOfBullets;
        float angle = -angleSpread / 2 + angleOffset * index; // Spread bullets evenly within the angle spread
        Vector2 direction = Quaternion.Euler(0, 0, angle) * forwardDirection;

        // Apply an explosion force in the calculated direction
        rbshot.AddForce(direction * explosionForce, ForceMode2D.Impulse);
    }

    public float GetDamage()
    {
        return CalculateDamageReduction(maxBounces, bounces, bounceDamageReductionFactor);
    }

    public void SetMaxBounce(float b, float factor)
    {
        this.bounceDamageReductionFactor = Mathf.Clamp(factor, 0f, 1f);
        this.maxBounces = b;
        if (this.bounces == -1)
        {
            this.bounces = b;
        }
    }
}
