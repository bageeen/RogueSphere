using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action : MonoBehaviour
{

    [SerializeField] protected bool isExecuting = false;
    [SerializeField] protected float baseExecutionTime = 1.0f;
    [SerializeField] public bool isStoppable = true;

    // -------------------- Executing methods ------------------------------
    public virtual void Execute(bool timed)
    {
        if (timed)
        {
            this.Execute(baseExecutionTime);
        }
        else
        {
            this.isExecuting = true;
        }
        
    }

    public virtual void Cancel()
    {
        this.isExecuting = false;
    }

    public virtual void Execute(float time)
    {
        StartCoroutine(ExecuteCoroutine(time));
    }

    private IEnumerator ExecuteCoroutine(float time)
    {
        yield return null;
        Execute(false);
        yield return new WaitForSeconds(time);
        Cancel();
        yield return null;
    }

    // ----------------------------------------------------------------------




    public virtual float GetBaseExecutionTime()
    {
        return baseExecutionTime;
    }

    public virtual bool GetState()
    {
        return isExecuting;
    }
    public virtual void SetState(bool b)
    {
        isExecuting = b;
    }
}
