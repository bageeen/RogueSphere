using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockBack : MonoBehaviour
{
    // Knockback

    public float knockbackTime = 0.2f;
    public float hitDirectionForce = 10f;
    public float constForce = 5f;
    public float inputForce = 7.5f;
    

    public AnimationCurve knockbackForceCurve;

    public bool IsBeingKnockedBack { get; private set; }

    private Rigidbody2D rb;

    private Coroutine knockbackCoroutine;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    public IEnumerator KnockbackAction(Vector2 hitDirection, Vector2 constantForceDirection, Vector2 inputDirection)
    {
        IsBeingKnockedBack = true;

        Vector2 _hitForce;
        Vector2 _constantForce;
        Vector2 _knockbackForce;
        Vector2 _combinedForce;
        float _time = 0f;

        
        _constantForce = constantForceDirection * constForce;

        float _elapsedTime = 0f;
        while (_elapsedTime < knockbackTime)
        {
            //iterate the timer
            _elapsedTime += Time.fixedDeltaTime;
            _time += Time.fixedDeltaTime;
            //update hitForce
            _hitForce = hitDirection * hitDirectionForce * knockbackForceCurve.Evaluate(_time);

            //combine _hitforce and _constantforce
            _knockbackForce = _hitForce + _constantForce;

            //combine knockBackForce with InputForce
            if ((inputDirection.x != 0) && (inputDirection.y != 0))
            {
                _combinedForce = _knockbackForce + inputDirection * inputForce;
            }
            else
            {
                _combinedForce = _knockbackForce;
            }

            //apply knockback
            rb.velocity = _combinedForce;

            yield return new WaitForFixedUpdate();
        }

        IsBeingKnockedBack = false;

    }
    public void CallKnockback(Vector2 hitDirection, Vector2 constantForceDirection, Vector2 inputDirection)
    {
        knockbackCoroutine = StartCoroutine(KnockbackAction(hitDirection, constantForceDirection, inputDirection));
    }
}
