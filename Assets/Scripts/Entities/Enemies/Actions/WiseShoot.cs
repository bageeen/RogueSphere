using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WiseShoot : Action
{

    private Firing firing;
    private Aiming aiming;

    private bool canShoot = false;



    void Start()
    {
        
        firing = GetComponent<Firing>();
        aiming = GetComponent<Aiming>();
        StartCoroutine(UpdateFiringStateCoroutine());
    }

    void FixedUpdate()
    {

    }




    private IEnumerator UpdateFiringStateCoroutine()
    {
        while (true)
        {
            UpdateFiringState();
            yield return new WaitForSeconds(0.5f);
        }

    }
    private void UpdateFiringState()
    {
        UpdateExec();
        firing.SetState(canShoot && isExecuting);
    }


    public override void Execute(bool timed)
    {
        base.Execute(timed);
        aiming.Execute(false);
    }

    public override void Cancel()
    {
        this.isExecuting = false;
        aiming.Cancel();
    }


    void UpdateExec()
    {

        if (aiming.canShoot())
        {
            canShoot = true;
        }
        else
        {
            canShoot = false;
        }
    }
}
