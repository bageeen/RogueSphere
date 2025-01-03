using System.Collections.Generic;
using UnityEngine;

public class SplatParticles : MonoBehaviour
{
    public ParticleSystem splatParticles;
    public GameObject splatPrefab;
    private List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();

    private float minSplatSize;
    private float maxSplatSize;

    // Dictionary to keep track of the current sorting order for each sorting layer
    private Dictionary<string, int> sortingOrders = new Dictionary<string, int>();

    private void OnParticleCollision(GameObject other)
    {
        ParticlePhysicsExtensions.GetCollisionEvents(splatParticles, other, collisionEvents);

        int count = collisionEvents.Count;
        for (int i = 0; i < count; i++)
        {
            GameObject splat = Instantiate(splatPrefab, collisionEvents[i].intersection, Quaternion.identity);

            Splat splatScript = splat.GetComponent<Splat>();
            splatScript.Initialize(Splat.SplatLocation.Foreground, splatParticles.startColor, minSplatSize,maxSplatSize);

            Transform parentTransform = other.transform.Find("SplatHolder");
            if (parentTransform != null)
            {
                splat.transform.SetParent(parentTransform, true);

                // Ensure the splat uses the same sorting layer and order as the enemy's Sprite Mask
                SpriteRenderer splatRenderer = splat.GetComponent<SpriteRenderer>();
                SpriteMask spriteMask = other.GetComponentInChildren<SpriteMask>();
                SpriteRenderer enemyRenderer = other.GetComponentInChildren<SpriteRenderer>();

                if (spriteMask != null && splatRenderer != null && enemyRenderer != null)
                {
                    string sortingLayerName = enemyRenderer.sortingLayerName;
                    int sortingLayerID = SortingLayer.NameToID(sortingLayerName);

                    // Check and update the sorting order for the given sorting layer
                    if (!sortingOrders.ContainsKey(sortingLayerName))
                    {
                        sortingOrders[sortingLayerName] = enemyRenderer.sortingOrder + 1;
                    }
                    else
                    {
                        sortingOrders[sortingLayerName]++;
                    }

                    splatRenderer.sortingLayerID = sortingLayerID;
                    splatRenderer.sortingOrder = sortingOrders[sortingLayerName];
                    splatRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;

                    
                }
            }
            else
            {
                Destroy(splat);
            }
        }
    }

    public void SetSplatSize(float min, float max)
    {
        minSplatSize = min;
        maxSplatSize = max;
    }
}