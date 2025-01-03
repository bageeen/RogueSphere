using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyAI : MonoBehaviour
{
    public GameObject targetObject;
    private Transform target;
    private Attributes attributes;
    private float speed;
    public float nextWaypointDistance = 3f;
    [SerializeField] private float minDistanceFollow;
    [SerializeField] private float maxDistanceAggro;

    private bool isAggro = false;
    [SerializeField] private float roamingRadius = 10f;

    [SerializeField] public float rotationSpeed;

    Path path;
    int currentWaypoint = 0;
    bool reachedEndOfPath = false;

    Seeker seeker;
    Rigidbody2D rb;

    private Room room;

    Vector2 roamingTarget;

    private float minX;
    private float minY;
    private float maxX;
    private float maxY;

    private Coroutine roamingCoroutine;

    private bool previousAggroState;

    float randomWaypointRepeatDelay;

    public float GetMaxDistanceAggro()
    {
        return maxDistanceAggro;
    }

    public Rigidbody2D GetRB()
    {
        return rb;
    }

    // Start is called before the first frame update
    void Start()
    {
        SetTargetToPlayer();
        attributes = GetComponent<Attributes>();
        speed = attributes.GetMoveSpeed();
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        target = targetObject.transform;
        roamingTarget = (Vector2)transform.position;
        InvokeRepeating("UpdatePath", 0f, 0.5f);
        roamingCoroutine = StartCoroutine(GenerateWaypoints(4f, 6f));
        WaitForRoomInitialisation();
    }

    private void SetTargetToPlayer()
    {
        this.targetObject = GameObject.FindWithTag("Player");
    }

    private void WaitForRoomInitialisation()
    {
        room = findRoom();
        StartCoroutine(InitializeRoomBounds());
    }

    IEnumerator InitializeRoomBounds()
    {
        yield return new WaitUntil(() => room != null && room.bounds != null && room.bounds.size != Vector3.zero);
        minX = room.bounds.min.x;
        minY = room.bounds.min.y;
        maxX = room.bounds.max.x;
        maxY = room.bounds.max.y;
    }

    private Room findRoom()
    {
        Transform currentTransform = GetComponent<Transform>();

        while (currentTransform != null)
        {
            if (currentTransform.parent != null && currentTransform.parent.CompareTag("Room"))
            {
                return currentTransform.parent.GetComponent<Room>();
            }
            currentTransform = currentTransform.parent;
        }
        return null;
    }

    public GameObject GetTarget()
    {
        return targetObject;
    }

    void UpdatePath()
    {
        UpdateGraphAroundAI();
        if (seeker.IsDone())
        {
            Vector2 targetPosition = isAggro ? target.position : roamingTarget;
            seeker.StartPath(rb.position, targetPosition, OnPathComplete);
        }
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = (path.vectorPath.Count > 1 && Vector2.Distance(transform.position, path.vectorPath[0]) < 1) ? 1 : 0;
        }
    }

    void FixedUpdate()
    {
        speed = attributes.GetMoveSpeed();

        if (path == null)
        {
            return;
        }

        if (currentWaypoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            return;
        }
        else
        {
            reachedEndOfPath = false;
        }

        isAggro = (Vector2.Distance(rb.position, target.position) < maxDistanceAggro);

        if (isAggro != previousAggroState)
        {
            if (roamingCoroutine != null)
            {
                StopCoroutine(roamingCoroutine);
            }
            if (isAggro)
            {
                CancelInvoke("UpdatePath");
                InvokeRepeating("UpdatePath", 0f, 0.5f);

                // Start closer waypoint generation
                //roamingCoroutine = StartCoroutine(GenerateWaypoints(0.5f, 1.5f));
            }
            else
            {
                CancelInvoke("UpdatePath");
                InvokeRepeating("UpdatePath", 0f, 0.5f);

                // Start farther waypoint generation
                roamingCoroutine = StartCoroutine(GenerateWaypoints(4f, 6f));
            }
            previousAggroState = isAggro;
        }

        if (isAggro && (Vector2.Distance(rb.position, target.position) > minDistanceFollow))
        {
            // Follow the player
            FollowPath(1);
        }
        else if (isAggro && (Vector2.Distance(rb.position, target.position) <= minDistanceFollow))
        {
            // Little unpredictable moves
            MoveWhileFiring();
        }
        else
        {
            // Roam randomly while out of range
            
            FollowPath(3);
        }
    }

    void Update()
    {
        if (!isAggro)
        {
            if (path != null && path.vectorPath.Count > currentWaypoint) { 
                AimAtPosition(path.vectorPath[currentWaypoint]);
            }
        }
        else if (isAggro && !CheckLineOfSight())
        {
            AimAtPosition(path.vectorPath[currentWaypoint]);
        }
    }

    void FollowPath(float speedDiv)
    {
        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = direction * (speed/speedDiv) * Time.fixedDeltaTime;

        rb.AddForce(force);

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }
    }

    void MoveWhileFiring()
    {
        Vector2 direction = ((Vector2)roamingTarget - rb.position).normalized;
        Vector2 force = direction * speed * Time.fixedDeltaTime;
        rb.AddForce(force);

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }
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

    IEnumerator GenerateWaypoints(float minDelay, float maxDelay)
    {
        while (true)
        {
            GenerateRandomWaypoint();
            yield return new WaitForSeconds(Random.Range(minDelay, maxDelay));
        }
    }

    void GenerateRandomWaypoint()
    {
        Vector2 randomDirection = Random.insideUnitCircle * roamingRadius;
        Vector2 randomWaypoint = (Vector2)transform.position + randomDirection;

        roamingTarget = ClampToBounds(randomWaypoint);

        if (seeker.IsDone())
        {
            seeker.StartPath(rb.position, roamingTarget, OnPathComplete);
        }
    }

    Vector2 ClampToBounds(Vector2 target)
    {
        target.x = Mathf.Clamp(target.x, minX, maxX);
        target.y = Mathf.Clamp(target.y, minY, maxY);
        return target;
    }


    bool CheckLineOfSight()
    {
        Rigidbody2D rbTarget = target.GetComponent<Rigidbody2D>();
        float distanceToTarget = Vector2.Distance(rb.position, rbTarget.position);
        if (distanceToTarget <= maxDistanceAggro)
        {
            LayerMask layerMask = LayerMask.GetMask("Player", "SolidObjects"); // Example, adjust based on your setup


            RaycastHit2D hit = Physics2D.Linecast(transform.position, target.position, layerMask);
            if (hit.collider != null)
            {
                if (hit.collider.transform == target)
                {
                    return true;
                }

            }
        }
        return false;
    }

    void UpdateGraphAroundAI()
    {
        // Define the bounds to update
        Bounds updateBounds = new Bounds(transform.position, Vector3.one * maxDistanceAggro * 2);

        // Update the graph in the specified bounds
        AstarPath.active.UpdateGraphs(updateBounds);
    }
}
