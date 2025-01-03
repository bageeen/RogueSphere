using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Roam : Action
{
    private BehaviourManager behaviourManager;

    private GameObject target;
    private Rigidbody2D rb;
    private Attributes entity;

    //--------Stats----------
    private float aggroRange;
    private float speed;
    private float rotationSpeed;
    //--------Stats----------

    //------------A*------------
    private Seeker seeker;
    private Path path;
    private bool reachedEndOfPath = false;
    private int currentWaypoint = 0;
    public float nextWaypointDistance = 3f;
    private Vector2 roamingTarget;
    private Coroutine roamingCoroutine;
    //--------------------------
    private Room room;
    private float minX;
    private float minY;
    private float maxX;
    private float maxY;

    void Start()
    {
        behaviourManager = GetComponent<BehaviourManager>();
        target = behaviourManager.GetTarget();
        rb = GetComponent<Rigidbody2D>();
        entity = GetComponent<Attributes>();
        seeker = GetComponent<Seeker>();
        aggroRange = behaviourManager.GetAggroRange();
        speed = entity.GetMoveSpeed();
        rotationSpeed = behaviourManager.GetRotationSpeed();
        InvokeRepeating("UpdatePath", 0f, 0.5f);
        roamingTarget = new Vector2(transform.position.x, transform.position.y);
        roamingCoroutine = StartCoroutine(GenerateWaypoints(3f, 10f));
        WaitForRoomInitialisation();
    }

    void FixedUpdate()
    {
        if (isExecuting)
        {
            if (path == null) { return; }
            if (currentWaypoint >= path.vectorPath.Count) { reachedEndOfPath = true; return; }
            else { reachedEndOfPath = false; }
            FollowPath(3);
        }
    }

    void Update()
    {
        if (isExecuting)
        {
            if (path != null && path.vectorPath.Count > currentWaypoint)
            {
                AimAtPosition(path.vectorPath[currentWaypoint]);
            }
        }
    }

    void UpdatePath()
    {
        if(isExecuting)
        {
            if (seeker.IsDone())
            {
                Vector2 targetPosition = roamingTarget;
                seeker.StartPath(rb.position, targetPosition, OnPathComplete);
            }
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
    private void WaitForRoomInitialisation()
    {
        
        StartCoroutine(InitializeRoomBounds());
    }
    private Room FindRoom()
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
    IEnumerator InitializeRoomBounds()
    {
        while (room == null)
        {
            room = FindRoom();
            yield return new WaitForSeconds(0.1f);
        }

        minX = room.bounds.min.x;
        minY = room.bounds.min.y;
        maxX = room.bounds.max.x;
        maxY = room.bounds.max.y;
    }

    void FollowPath(float speedDiv)
    {
        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = direction * (speed / speedDiv) * Time.fixedDeltaTime;

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
            if(isExecuting)
            {
                GenerateRandomWaypoint();  
            }
            yield return new WaitForSeconds(this.baseExecutionTime);
            this.baseExecutionTime = Random.Range(minDelay, maxDelay);
        }
    }
    void GenerateRandomWaypoint()
    {
        Vector2 randomDirection = (Random.insideUnitCircle) * 25;
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
}
