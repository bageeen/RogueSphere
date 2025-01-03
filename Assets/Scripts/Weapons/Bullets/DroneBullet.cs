using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneBullet : Bullet
{
    [HideInInspector] public float followSpeed = 5f; // Speed at which the drone bullet follows the mouse cursor
    [HideInInspector] public float turnSpeed = 5f; // Speed at which the drone bullet turns towards the mouse cursor
    [HideInInspector] public float acceleration = 10f; // Acceleration for the bullet
    [HideInInspector] public float separationDistance = 2f; // Minimum distance to keep from other bullets
    [HideInInspector] public float separationForce = 5f; // Force applied for separation

    private Vector3 targetPosition;
    private Rigidbody2D rb;
    private bool isStopped = false; // Flag to track if the drone is stopped

    private static List<DroneBullet> allDrones = new List<DroneBullet>(); // List to track all drone bullets

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
        rb.interpolation = RigidbodyInterpolation2D.Interpolate; // Enable interpolation
    }

    protected void OnEnable()
    {
        allDrones.Add(this); // Add this drone to the list
    }

    protected void OnDisable()
    {
        allDrones.Remove(this); // Remove this drone from the list
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (!isStopped)
        {
            // Get the mouse position in world coordinates
            targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetPosition.z = 0; // Ensure the target position is on the same plane as the bullet

            // Calculate the direction to the mouse position
            Vector2 direction = (targetPosition - transform.position).normalized;

            // Rotate the bullet smoothly to face the mouse position
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            float angle = Mathf.LerpAngle(transform.eulerAngles.z, targetAngle, Time.fixedDeltaTime * turnSpeed);
            rb.MoveRotation(angle); // Update the Rigidbody2D's rotation

            // Apply separation force to avoid stacking
            ApplySeparationForce();

            // Move the bullet in the direction it is pointing
            Vector2 moveDirection = transform.up; // transform.up points in the direction the bullet is facing
            Vector2 desiredVelocity = moveDirection * followSpeed;
            Vector2 force = (desiredVelocity - rb.velocity) * acceleration;
            rb.AddForce(force);
        }
    }

    private void ApplySeparationForce()
    {
        Vector2 separation = Vector2.zero;
        foreach (var drone in allDrones)
        {
            if (drone != this)
            {
                float distance = Vector2.Distance(transform.position, drone.transform.position);
                if (distance < separationDistance)
                {
                    Vector2 diff = (Vector2)(transform.position - drone.transform.position);
                    separation += diff.normalized / distance; // Apply separation force inversely proportional to distance
                }
            }
        }
        separation *= separationForce;
        rb.AddForce(separation); // Add separation force
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isDead)
        {
            if (collision.gameObject.TryGetComponent<Attributes>(out Attributes enemyComponent))
            {
                Debug.Log("Collision");
                enemyComponent.TakeKnockback(gameObject, this.knockbackPower);
                enemyComponent.TakeDamage(this.GetDamage());
                this.penetration -= 1;
                if (this.penetration < 1)
                {
                    this.health = 0;
                }
            }

            // Apply knockback and stop following cursor
            ManageKnockback(collision);
            StartCoroutine(StopFollowingCursor());

            if (collision.gameObject.CompareTag("SolidObjects"))
            {
                this.bounces -= 1;
                if (bounces < 1)
                {
                    this.health = 0;
                }
            }

            CheckAlive();
        }
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

    private void ManageKnockback(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<Attributes>(out Attributes other) && other.gameObject.tag != this.gameObject.tag)
        {
            rb.AddForce(-(other.rb.position - rb.position).normalized * 0.2f, ForceMode2D.Impulse);
        }
        else if (collision.gameObject.CompareTag("SolidObjects"))
        {
            rb.AddForce(-(collision.contacts[0].point - (Vector2)transform.position).normalized * 0.2f, ForceMode2D.Impulse);
        }
    }

    private IEnumerator StopFollowingCursor()
    {
        isStopped = true;
        yield return new WaitForSeconds(0.5f);
        isStopped = false;
    }
}
