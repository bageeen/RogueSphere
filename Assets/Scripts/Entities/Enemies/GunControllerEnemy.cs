using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunControllerEnemy : MonoBehaviour
{
    

    private Transform targetTransform;
    private Rigidbody2D rb;
    private Rigidbody2D rbTarget;
    private Transform entityTransform;
    private Attributes entity;

    private float effectiveBulletForce;
    private float effectiveBulletMass;


    private GunController[] gunControllers;

    private float nextFireTime = 0f;

    // Follow target
    [SerializeField] private float shootAngleThreshold = 20f;
    [SerializeField] private float predictionTime = 1.0f;

    [SerializeField] private float rotationSpeed;

    private float lineOfSightRange;
    [SerializeField] private Transform lookDirection;



    private bool playerInLineOfSight = false;


    private void FindGunControllers()
    {
        if (this.gunControllers == null)
        {
            this.gunControllers = GetComponentsInChildren<GunController>();
        }
    }


    void Start()
    {
        FindGunControllers();

        entity = GetComponentInParent<Attributes>();
        effectiveBulletForce = gunControllers[0].initialBulletForce * entity.GetBulletForce();
        effectiveBulletMass = gunControllers[0].initialBulletMass;

        entityTransform = GetComponentInParent<Transform>();
        rb = GetComponent<Rigidbody2D>();

        rbTarget = GetComponent <EnemyAI>().GetTarget().GetComponent<Rigidbody2D>();

        targetTransform = GetComponent<EnemyAI>().GetTarget().GetComponent<Transform>();
        lineOfSightRange = GetComponent<EnemyAI>().GetMaxDistanceAggro();
        
    }

    void FixedUpdate()
    {
        playerInLineOfSight = CheckLineOfSight();

        Vector2 predictedPlayerPosition = PredictPlayerPosition();

        if (playerInLineOfSight)
        {
            AimAtPosition(predictedPlayerPosition);
        }

        // Check if the gun is facing the player within the threshold angle
        Vector3 lookDirectionOfGuns = (new Vector2(lookDirection.position.x, lookDirection.position.y) - rb.position);
        Vector3 directionOfPlayer = (predictedPlayerPosition - rb.position);
        float gunPlayerAngle = Vector3.Angle(lookDirectionOfGuns, directionOfPlayer);


        if (gunPlayerAngle <= shootAngleThreshold && playerInLineOfSight && (Time.time >= nextFireTime))
        {
            nextFireTime = gunControllers[0].Shoot();
        }
    }



    float CalculateSpeed(float force, float mass)
    {
        // Calculate the initial velocity (speed)
        return force / mass;
    }

    float CalculateTimeToReachDistance(float force, float mass, float distance)
    {
        // Calculate the speed
        float speed = CalculateSpeed(force, mass);

        // Check if the speed is greater than zero
        if (speed <= 0)
        {
            Debug.LogError("Speed must be greater than zero.");
            return -1;
        }

        // Time is distance divided by speed
        float time = distance / speed;

        return time;
    }




    Vector2 PredictPlayerPosition()
    {
        predictionTime = CalculateTimeToReachDistance(effectiveBulletForce,effectiveBulletMass, Vector2.Distance(rb.position, rbTarget.position));
        Vector2 playerVelocity = rbTarget.velocity;

        return rbTarget.position + playerVelocity * (predictionTime);
    }

    void AimAtPosition(Vector2 targetPosition)
    {
        Vector2 lookDir = (targetPosition - rb.position).normalized;
        float targetAngle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;

        // Smoothly interpolate towards the target angle
        float currentAngle = rb.rotation;
        float smoothedAngle = Mathf.LerpAngle(currentAngle, targetAngle, rotationSpeed * Time.fixedDeltaTime);
        rb.MoveRotation(smoothedAngle);
    }
    

    bool CheckLineOfSight()
    {
        float distanceToTarget = Vector2.Distance(rb.position,rbTarget.position);
        if (distanceToTarget <= lineOfSightRange)
        {
            LayerMask layerMask = LayerMask.GetMask("Player", "SolidObjects"); // Example, adjust based on your setup


            RaycastHit2D hit = Physics2D.Linecast(entityTransform.position, targetTransform.position, layerMask);
            if(hit.collider != null)
            {
                if (hit.collider.transform == targetTransform){
                    return true;
                }
                
            }
        }
        return false;
    }
}
