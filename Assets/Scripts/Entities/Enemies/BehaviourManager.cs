using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourManager : MonoBehaviour
{
    private GameObject target;

    [HideInInspector] public bool isAggro = false;
    [SerializeField] private string tagToAttack;
    private bool isAttacking = false;
    private bool isBusy = false;

    [SerializeField] private List<Action> actionsWhileNotAggro;
    [SerializeField] private List<Action> actionsWhileAggro;
    [SerializeField] private List<Action> actionsWhileAttacking;

    private Action lastAction;

    [SerializeField] private float aggroRange;
    [SerializeField] private float attackRange;
    [SerializeField] private float rotationSpeed;

    void Awake()
    {
        SetTarget(GameObject.FindGameObjectWithTag(tagToAttack));
    }
    void Start()
    {
        StartCoroutine(CheckAggroAndAttackCoroutine());
    }

    void FixedUpdate()
    {
        if (!isBusy) 
        {
            ChooseAction();
        }
    }

    public GameObject FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(tagToAttack);
        GameObject closestEnemy = null;
        float shortestDistance = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(currentPosition, enemy.transform.position);
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                closestEnemy = enemy;
            }
        }

        return closestEnemy;
    }


    private void ChooseAction()
    {
        int randomValue;
        if (!isAggro && !isAttacking)  // Not aggro
        {
            if (actionsWhileNotAggro.Count >= 1)
            {
                randomValue = Random.Range(0, actionsWhileNotAggro.Count);
                lastAction = actionsWhileNotAggro[randomValue];
                lastAction.Execute(true);
                StartCoroutine(MakeEntityBusy(actionsWhileNotAggro[randomValue].GetBaseExecutionTime()));
            }
        }
        else if (isAggro && !isAttacking) // Aggroed but not attacking
        {
            if (actionsWhileAggro.Count >= 1)
            {
                randomValue = Random.Range(0, actionsWhileAggro.Count);
                lastAction = actionsWhileAggro[randomValue];
                lastAction.Execute(true);
                StartCoroutine(MakeEntityBusy(actionsWhileAggro[randomValue].GetBaseExecutionTime()));
            }
        }
        else if (isAggro && isAttacking) // Attacking
        {
            if (actionsWhileAttacking.Count >= 1)
            {
                randomValue = Random.Range(0, actionsWhileAttacking.Count);
                lastAction = actionsWhileAttacking[randomValue];
                lastAction.Execute(true);
                StartCoroutine(MakeEntityBusy(actionsWhileAttacking[randomValue].GetBaseExecutionTime()));
            }
        }
        
    }


    private IEnumerator MakeEntityBusy(float time)
    {
        this.isBusy = true;
        yield return new WaitForSeconds(time);
        this.isBusy = false;
    }


    private IEnumerator CheckAggroAndAttackCoroutine()
    {
        while (true)
        {
            this.target = FindClosestEnemy();
            Debug.Log($"Enemy cibled by {gameObject.name}is {target}");
            if (target != null)
            { 
                CheckAggroAndAttack(); 
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void CheckAggroAndAttack()
    {
        bool tmpIsAggro = isAggro;
        bool tmpIsAttacking = isAttacking;

        float distanceToTarget = Vector2.Distance(transform.position, target.transform.position);

        this.isAggro = distanceToTarget < aggroRange;
        this.isAttacking = distanceToTarget < attackRange && CheckLineOfSight();
        if (lastAction != null)
        {
            if (lastAction.isStoppable && (tmpIsAggro != isAggro || tmpIsAttacking != isAttacking))
            {
                lastAction.Cancel();
                isBusy = false;
            }
        }
    }


    public bool CheckLineOfSight()
    {
        float distanceToTarget = Vector2.Distance(transform.position, target.transform.position);
        if (distanceToTarget <= attackRange)
        {
            LayerMask layerMask = LayerMask.GetMask("Player", "SolidObjects"); // Example, adjust based on your setup
            RaycastHit2D hit = Physics2D.Linecast(transform.position, target.transform.position, layerMask);
            if (hit.collider != null)
            {
                if (hit.collider.transform == target.transform)
                {
                    return true;
                }
            }
        }
        return false;
    }
    public float GetAggroRange() => this.aggroRange;
    public float GetRotationSpeed() => this.rotationSpeed;
    public void SetTarget(GameObject t) => this.target = t;
    public GameObject GetTarget() => this.target;
}
