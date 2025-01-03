using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementControls : MonoBehaviour
{
    public InputActionReference move;
    private float moveSpeed;
    public Vector2 moveInput;
    private Attributes player;
    private Rigidbody2D rb;

    [SerializeField] private RoomManager roomManager;

    private CameraController cameraController;

    // Collisions
    public float collisionOffset = 0.05f;
    public ContactFilter2D movementFilter;
    private List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();

    [SerializeField] private LayerMask collisionLayers;
    private Room currentRoom;

    public void Start()
    {
        
        cameraController = Camera.main.GetComponent<CameraController>();
        cameraController.SetTarget(gameObject);
        rb = GetComponent<Rigidbody2D>();
        player = GetComponent<Attributes>();
        movementFilter.layerMask = ~LayerMask.GetMask("Player Projectiles");
        movementFilter.useLayerMask = true;
        movementFilter.useTriggers = false;
        //InitialRoom();
    }


    void FixedUpdate()
    {
        moveSpeed = player.GetMoveSpeed();
        moveInput = move.action.ReadValue<Vector2>().normalized;
        
        bool success = MovePlayer(moveInput);

        if (!success)
        {
            success = MovePlayer(new Vector2(moveInput.x, 0));

            if (!success)
            {
                success = MovePlayer(new Vector2(0, moveInput.y));
            }
        }
    }


    bool MovePlayer(Vector2 direction)
    {
        // Cast to check for potential collisions
        int count = rb.Cast(direction, movementFilter, castCollisions, collisionOffset);

        // Filter out collisions that are not in the specified layers
        int validCollisionCount = 0;
        for (int i = 0; i < count; i++)
        {
            if (collisionLayers == (collisionLayers | (1 << castCollisions[i].collider.gameObject.layer)))
            {
                validCollisionCount++;
            }
        }

        if (validCollisionCount == 0)
        {
            // If no valid collisions
            Vector2 moveVector = direction * moveSpeed * Time.fixedDeltaTime;
            rb.AddForce(moveVector, ForceMode2D.Force);
            return true;
        }
        else
        {
            return false;
        }
    }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Room"))
        {
            currentRoom = other.GetComponent<Room>(); // Assuming room script is on parent
            
            // Update camera bounds when entering the first room
            UpdateCameraBoundsIfNeeded();
            
        }
    }

    void UpdateCameraBoundsIfNeeded()
    {
        if (currentRoom != null)
        {
            cameraController.UpdateCameraBounds(currentRoom.minBounds, currentRoom.maxBounds);
        }
    }

    public void InitialRoom()
    {
        Bounds initialBounds = roomManager.GetInitialBounds();

        Vector3 min = initialBounds.min;
        Vector3 max = initialBounds.max;
        Vector2 minBoundsTMP = new Vector2(min.x, min.y);
        Vector2 maxBoundsTMP = new Vector2(max.x, max.y);

        cameraController.UpdateCameraBounds(minBoundsTMP, maxBoundsTMP);
    }

}
