using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseFollow : MonoBehaviour
{
    private Transform parentTransform;
    private Camera cam;

    Vector2 mousePos;
    public float turnSpeed = 0f; // Turn speed parameter in degrees per second

    void Awake()
    {
        parentTransform = transform;
        cam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
    }

    void FixedUpdate()
    {
        Debug.Log(turnSpeed);
        // Handle mouse position and calculate target angle
        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 lookDir = mousePos - (Vector2)parentTransform.position;
        float targetAngle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;

        // Get the current rotation as a Quaternion
        Quaternion currentRotation = transform.rotation;

        // Calculate the target rotation as a Quaternion
        Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);

        // Rotate towards the target rotation at a constant speed
        transform.rotation = Quaternion.RotateTowards(currentRotation, targetRotation, turnSpeed * Time.fixedDeltaTime);

        // Keep the position of the gun at the parent's position
        transform.position = parentTransform.position;
    }
    public void SetTurnSpeed(float turnSpeed)
    {
        this.turnSpeed = turnSpeed;
    }
}
