using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firing : Action
{

    private GunController[] gunControllers;
    private float nextFireTime = 0f;


    void Start()
    {
        UpdateGunControllers();
    }


    void FixedUpdate()
    {
        if (isExecuting)
        {
            if (Time.time >= nextFireTime)
            {
                nextFireTime = gunControllers[0].Shoot();
            }
        }
    }







    private void UpdateGunControllers()
    {
        if (this.gunControllers == null)
        {
            this.gunControllers = GetComponentsInChildren<GunController>();
        }
    }
}
