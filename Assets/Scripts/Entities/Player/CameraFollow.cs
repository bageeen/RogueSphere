using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    [SerializeField] private float followSpeed = 2f;
    private Transform target;

    void Start()
    {
        target = GameObject.FindWithTag("Player").GetComponent<Transform>();
    }
    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 newPos = new Vector3(target.position.x, target.position.y, -10f);
        transform.position = Vector3.Slerp(transform.position, newPos, followSpeed*Time.deltaTime);
        
    }
}
