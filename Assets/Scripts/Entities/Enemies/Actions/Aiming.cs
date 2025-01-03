using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aiming : Action
{
    private GameObject target;
    private Rigidbody2D rb;
    private Transform lookDirection;
    private Attributes entity;
    private Rigidbody2D rbTarget;
    private Transform transformTarget;

    private bool playerInLineOfSight = false;
    private BehaviourManager behaviourManager;



    //*** Guns attributes ***//
    private GunController[] gunControllers;
    private float effectiveBulletForce;
    private float effectiveBulletMass;

    private float predictionTime = 1.0f;
    [SerializeField] private float shootAngleThreshold = 20f;
    private float rotationSpeed;
    private float lineOfSightRange;

    
    void Awake()
    {       
        lookDirection = transform.Find("Direction").transform;
        behaviourManager = GetComponent<BehaviourManager>();
        rotationSpeed = behaviourManager.GetRotationSpeed();
        entity = GetComponent<Attributes>();
    }

    void Start()
    {
        lineOfSightRange = GetComponent<BehaviourManager>().GetAggroRange();
        rb = GetComponent<Rigidbody2D>();
        
        
        UpdateGunControllers();

    }

    void FixedUpdate()
    {

        if (isExecuting)
        {
            playerInLineOfSight = CheckLineOfSight();
            Vector2 predictedPlayerPosition = PredictPlayerPosition();

            if (playerInLineOfSight)
            {
                AimAtPosition(predictedPlayerPosition);
            }

           
        }
    }




    private void UpdateGunControllers()
    {
        if (this.gunControllers == null)
        {
            this.gunControllers = GetComponentsInChildren<GunController>();
        }
        effectiveBulletForce = gunControllers[0].initialBulletForce * entity.GetBulletForce();
        effectiveBulletMass = gunControllers[0].initialBulletMass;
    }
    private Vector2 PredictPlayerPosition()
    {
        UpdateBehaviourAndTarget();
        predictionTime = CalculateTimeToReachDistance(effectiveBulletForce, effectiveBulletMass, Vector2.Distance(transform.position, target.transform.position));
        Vector2 playerVelocity = rbTarget.velocity;

        return rbTarget.position + playerVelocity * (predictionTime);
    }

    private void AimAtPosition(Vector2 targetPosition)
    {
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle - 90f));

        // Smoothly interpolate towards the target rotation
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime * 100);
    }
    public bool CheckLineOfSight()
    {
        UpdateBehaviourAndTarget();
        if (target == null) { return false; }
        float distanceToTarget = Vector2.Distance(transform.position, target.transform.position);
        if (distanceToTarget <= lineOfSightRange)
        {
            LayerMask layerMask = LayerMask.GetMask("Player", "SolidObjects"); // Example, adjust based on your setup
            RaycastHit2D hit = Physics2D.Linecast(transform.position, target.transform.position, layerMask);
            if (hit.collider != null)
            {
                if (hit.collider.transform == target.transform)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void UpdateBehaviourAndTarget()
    {
        if (behaviourManager == null) { behaviourManager = GetComponent<BehaviourManager>(); }
        target = behaviourManager.GetTarget();
        if(target != null)
        {
            rbTarget = target.GetComponent<Rigidbody2D>();
        }
    }
    
    public bool canShoot()
    {
        if (target == null)
        {
            return false;
        }
        playerInLineOfSight = CheckLineOfSight();
        Vector2 predictedPlayerPosition = PredictPlayerPosition();

        // Check if the gun is facing the player within the threshold angle
        Vector3 lookDirectionOfGuns = (new Vector2(lookDirection.position.x, lookDirection.position.y) - new Vector2(transform.position.x, transform.position.y));
        Vector3 directionOfPlayer = (predictedPlayerPosition - new Vector2(transform.position.x, transform.position.y));
        float gunPlayerAngle = Vector3.Angle(lookDirectionOfGuns, directionOfPlayer);

        return (gunPlayerAngle <= shootAngleThreshold && playerInLineOfSight);
    }
    float CalculateTimeToReachDistance(float force, float mass, float distance)
    {
        // Calculate the speed
        float speed = force / mass;

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
}
