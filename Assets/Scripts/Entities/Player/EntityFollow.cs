using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityFollow : MonoBehaviour
{



    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Rigidbody2D rbTarget;


    Vector2 mousePos;


    void Update()
    {

    }


    void FixedUpdate()
    {


        Vector2 lookDir = rbTarget.position - rb.position;

        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        rb.rotation = angle;
    }
}
