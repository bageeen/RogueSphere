using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoTurret : MonoBehaviour
{
    public float rotationSpeed = 5f;
    private GameObject target;

    void Update()
    {
        target = FindClosestEnemy();
        Debug.Log(target);
        if (target != null)
        {
            AimAtPosition(target.transform.position);
        }
    }

    public GameObject FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("E1");
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


    private void AimAtPosition(Vector2 targetPosition)
    {
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle - 90f));

        // Smoothly interpolate towards the target rotation
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime * 100);
    }
}
