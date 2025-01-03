using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [HideInInspector] public float maxHealth;
    [HideInInspector] public float knockbackPower;
    [HideInInspector] public float damage;
    [HideInInspector] public Vector3 scale;
    [HideInInspector] public float mass;
    [HideInInspector] public float linearDrag;
    public Color bulletColor;

    public RayCasterPaint paint;


    public int penetration;
    public float minSpeed;

    protected float minSplatSize;
    protected float maxSplatSize;


    protected float bounces = -1;
    protected float maxBounces;
    protected float bounceDamageReductionFactor;
    public Transform parentBullets;

    [HideInInspector] public Rigidbody2D rb;
    protected Animator anim;

    public bool isDead = false;
    protected float health;

    protected virtual void Awake()
    {
        anim = GetComponent<Animator>();
    }



    protected virtual void Start()
    {

        paint = GameObject.FindWithTag("MainCamera").GetComponent<RayCasterPaint>();


        Transform transform = GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();
        health = maxHealth;
        transform.localScale = scale;
        rb.mass = mass;
        rb.drag = linearDrag;

    }


    protected virtual void FixedUpdate()
    {
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
    }
    


    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
    }

    public void TakeDamage(float damage)
    {
        this.health -= damage;
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

    public virtual void SetSplatSize(float min, float max)
    {
        this.minSplatSize = min;
        this.maxSplatSize = max;
    }

    public virtual void CheckAlive()
    {
        if (this.health <= 0 && !isDead)
        {
            this.gameObject.layer = LayerMask.NameToLayer("DeadProjectiles");
            this.isDead = true;
            anim.SetTrigger("die");

            Destroy(gameObject,0.4f);
        }
    }

    public float GetHealth()
    {
        return health;
    }

    public float GetDamage()
    {
        return CalculateDamageReduction(maxBounces, bounces, bounceDamageReductionFactor);
    }
    protected float CalculateDamageReduction(float maxBounces, float bounces, float bounceDamageReductionFactor)
    {
        // Calculate the damage reduction based on the number of bounces
        float bounceReduction = 0;
        if (maxBounces > 0)
        {
            bounceReduction = (1 - (bounces / maxBounces)) * bounceDamageReductionFactor;
        }

        // Calculate the final damage
        return damage * (health / maxHealth) * (1 - bounceReduction);
    }
}
