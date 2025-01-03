using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Seek : Action
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
    //--------------------------



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
    }


    void FixedUpdate()
    {
        if (isExecuting)
        {
            if (path == null) { return; }
            if (currentWaypoint >= path.vectorPath.Count){reachedEndOfPath = true; return;}
            else { reachedEndOfPath = false; }
            FollowPath(1);
        }
    }
    void Update()
    {
        if(isExecuting)
        {
            if (path != null && path.vectorPath.Count > currentWaypoint)
            {
                AimAtPosition(path.vectorPath[currentWaypoint]);
            }
        }
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

    void UpdatePath()
    {
        if(isExecuting)
        {
            UpdateGraphAroundAI();
            if (seeker.IsDone())
            {
                Vector2 targetPosition = target.transform.position;

                seeker.StartPath(transform.position, targetPosition, OnPathComplete);
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

    void UpdateGraphAroundAI()
    {
        // Define the bounds to update
        Bounds updateBounds = new Bounds(transform.position, Vector3.one * aggroRange * 2);

        // Update the graph in the specified bounds
        AstarPath.active.UpdateGraphs(updateBounds);
    }


}
