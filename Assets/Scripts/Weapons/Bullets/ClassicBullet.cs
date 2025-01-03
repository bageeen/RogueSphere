using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassicBullet : Bullet
{
    [SerializeField] private SpriteRenderer spriteRenderer;



    protected override void Start()
    {
        base.Start();
        spriteRenderer = GetComponent<SpriteRenderer>();

    }

    protected override void FixedUpdate()
    {
        if (rb.velocity.magnitude < minSpeed && !isDead)
        {
            health = 0;
            CheckAlive();
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
            if (bounces < 0)
            {
                this.health = 0;
            }
        }
        CheckAlive();
    }

    public void SetColor(Color color)
    {
        this.bulletColor = color;

        float darkenColor = Random.Range(0.7f, 1.0f);
        spriteRenderer.color = new Color(color.r * darkenColor,color.g * darkenColor, color.b * darkenColor);
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (this.gameObject.tag == "PlayerProj")
        {
            CheckTriggers(collision);
        }
    }

    public override void CheckAlive()
    {
        if (this.health <= 0 && !isDead)
        {
            this.gameObject.layer = LayerMask.NameToLayer("DeadProjectiles");
            this.isDead = true;

            if (paint != null)
            {
                paint.CastRay(transform.position, bulletColor, minSplatSize, maxSplatSize);
            }


            Destroy(gameObject);
        }
    }

    public void CheckTriggers(Collider2D collision)
    {
        ClassicBullet bullet = collision.GetComponent<ClassicBullet>();
        if (bullet != null && bullet.gameObject.tag != this.gameObject.tag && !isDead && !bullet.isDead)
        {
            float otherDamage = bullet.GetDamage();
            bullet.TakeDamage(this.GetDamage());
            this.TakeDamage(otherDamage);

            bullet.CheckAlive();
            CheckAlive();
        }
    }

    

    
}
