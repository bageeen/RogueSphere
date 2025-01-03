using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class HealthBarManager : MonoBehaviour
{
    public static HealthBarManager Instance;
    public GameObject healthBarPrefab; // Assign the health bar prefab in the Inspector
    public Canvas healthBarCanvas; // Assign the main canvas in the Inspector


    private Dictionary<GameObject, GameObject> healthBars = new Dictionary<GameObject, GameObject>();
    private Dictionary<GameObject, Coroutine> fadeOutCoroutines = new Dictionary<GameObject, Coroutine>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void CreateHealthBar(GameObject entity)
    {
        GameObject healthBarInstance = Instantiate(healthBarPrefab, healthBarCanvas.transform);
        healthBars.Add(entity, healthBarInstance);

        // Ensure the health bar RectTransform is reset properly
        RectTransform healthBarRect = healthBarInstance.GetComponent<RectTransform>();
        healthBarRect.anchorMin = new Vector2(0.5f, 0.5f);
        healthBarRect.anchorMax = new Vector2(0.5f, 0.5f);
        healthBarRect.pivot = new Vector2(0.5f, 0.5f);
        healthBarRect.localPosition = Vector3.zero;
    }

    public void UpdateHealthBar(GameObject entity, float health, float maxHealth)
    {
        if (healthBars.ContainsKey(entity))
        {
            GameObject healthBarInstance = healthBars[entity];

            HealthBar healthBarScript = healthBarInstance.GetComponent<HealthBar>();
            healthBarScript.SetMaxHealth(maxHealth);
            healthBarScript.SetHealth(health);

            // Get the CanvasGroup component for fade-out effect
            CanvasGroup canvasGroup = healthBarInstance.GetComponent<CanvasGroup>();

            if (health >= maxHealth)
            {
                if (!fadeOutCoroutines.ContainsKey(entity))
                {
                    fadeOutCoroutines[entity] = StartCoroutine(FadeOutHealthBar(canvasGroup, entity));
                }
            }
            else
            {
                if (fadeOutCoroutines.ContainsKey(entity))
                {
                    StopCoroutine(fadeOutCoroutines[entity]);
                    fadeOutCoroutines.Remove(entity);
                }
                canvasGroup.alpha = 1f;
            }

            Vector3 entityScreenPosition = Camera.main.WorldToScreenPoint(entity.transform.position);
            Vector3 healthBarPosition = new Vector3(entityScreenPosition.x, entityScreenPosition.y - 40, entityScreenPosition.z);
            RectTransform healthBarRect = healthBarInstance.GetComponent<RectTransform>();
            healthBarRect.position = healthBarPosition;

            // Ensure the rotation is reset
            healthBarRect.rotation = Quaternion.identity;
        }
    }

    private IEnumerator FadeOutHealthBar(CanvasGroup canvasGroup, GameObject entity)
    {
        float duration = 0.5f; // Duration of the fade-out in seconds
        float startAlpha = canvasGroup.alpha;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, 0f, elapsed / duration);
            canvasGroup.alpha = alpha;
            yield return null;
        }

        // Ensure the final alpha value is set to 0
        canvasGroup.alpha = 0f;

        fadeOutCoroutines.Remove(entity);
    }

    public void RemoveHealthBar(GameObject entity)
    {
        if (healthBars.ContainsKey(entity))
        {
            if (fadeOutCoroutines.ContainsKey(entity))
            {
                StopCoroutine(fadeOutCoroutines[entity]);
                fadeOutCoroutines.Remove(entity);
            }
            Destroy(healthBars[entity]);
            healthBars.Remove(entity);
        }
    }
}
