using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    // Movement
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    public Vector2 moveInput;
    public float collisionOffset = 0.05f;
    public ContactFilter2D movementFilter;
    private List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();
    public InputActionReference move;
    public Camera cam;
    Vector2 movement;

    private KnockBack knockback;



    public void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        knockback = GetComponent<KnockBack>();
    }


    void Update(){

        
        
    }

    void FixedUpdate(){

        moveInput = move.action.ReadValue<Vector2>().normalized;

        bool success = MovePlayer(moveInput);

        if(!success){
            success = MovePlayer(new Vector2(moveInput.x, 0));

            if(!success){
                success = MovePlayer(new Vector2(0, moveInput.y));
            }
        }


    }

    


    bool MovePlayer(Vector2 direction){
        //if (!knockback.IsBeingKnockedBack)
        if (true)
        {
            int count = rb.Cast(direction, movementFilter, castCollisions, moveSpeed * Time.fixedDeltaTime + collisionOffset);

            if (count == 0)
            { // If no collisions
                Vector2 moveVector = direction * moveSpeed * Time.fixedDeltaTime;

                rb.AddForce(moveVector,ForceMode2D.Force);
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    


}
