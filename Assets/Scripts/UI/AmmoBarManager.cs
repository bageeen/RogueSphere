using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class AmmoBarManager : MonoBehaviour
{
    public static AmmoBarManager Instance;
    public GameObject ammoBarPrefab;
    public Canvas ammoBarCanvas;

    private Dictionary<AmmoManager, GameObject> ammoBars = new Dictionary<AmmoManager, GameObject>();
    private Dictionary<AmmoManager, Coroutine> fadeOutCoroutines = new Dictionary<AmmoManager, Coroutine>();

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

    public void CreateAmmoBar(AmmoManager ammoManager, Color color)
    {
        GameObject ammoBarInstance = Instantiate(ammoBarPrefab, ammoBarCanvas.transform);
        ammoBars.Add(ammoManager, ammoBarInstance);
        AmmoBar ammoBarScript = ammoBarInstance.GetComponent<AmmoBar>();
        ammoBarScript.SetColor(color);

        // Ensure the ammo bar RectTransform is reset properly
        RectTransform ammoBarRect = ammoBarInstance.GetComponent<RectTransform>();
        ammoBarRect.anchorMin = new Vector2(0.5f, 0.5f);
        ammoBarRect.anchorMax = new Vector2(0.5f, 0.5f);
        ammoBarRect.pivot = new Vector2(0.5f, 0.5f);
        ammoBarRect.localPosition = Vector3.zero;
    }

    public void UpdateAmmoBar(AmmoManager ammoManager, float ammo, float maxAmmo)
    {
        if (ammoBars.ContainsKey(ammoManager))
        {
            GameObject ammoBarInstance = ammoBars[ammoManager];

            AmmoBar ammoBarScript = ammoBarInstance.GetComponent<AmmoBar>();
            ammoBarScript.SetMaxAmmo(maxAmmo);
            ammoBarScript.SetAmmo(ammo);

            // Get the CanvasGroup component for fade-out effect
            CanvasGroup canvasGroup = ammoBarInstance.GetComponent<CanvasGroup>();

            if (ammo >= maxAmmo)
            {
                if (!fadeOutCoroutines.ContainsKey(ammoManager))
                {
                    fadeOutCoroutines[ammoManager] = StartCoroutine(FadeOutAmmoBar(canvasGroup, ammoManager));
                }
            }
            else
            {
                if (fadeOutCoroutines.ContainsKey(ammoManager))
                {
                    StopCoroutine(fadeOutCoroutines[ammoManager]);
                    fadeOutCoroutines.Remove(ammoManager);
                }
                canvasGroup.alpha = 1f;
            }

            Vector3 entityScreenPosition = Camera.main.WorldToScreenPoint(ammoManager.entity.transform.position);
            Vector3 ammoBarPosition = new Vector3(entityScreenPosition.x, entityScreenPosition.y - 50, entityScreenPosition.z);

            // Offset the position if there are multiple ammo bars
            int index = 0;
            foreach (var key in ammoBars.Keys)
            {
                if (key == ammoManager)
                    break;
                index++;
            }

            ammoBarPosition.y -= index * 10; // Offset by 20 units per ammo bar

            RectTransform ammoBarRect = ammoBarInstance.GetComponent<RectTransform>();
            ammoBarRect.position = ammoBarPosition;

            // Ensure the rotation is reset
            ammoBarRect.rotation = Quaternion.identity;
        }
    }

    private IEnumerator FadeOutAmmoBar(CanvasGroup canvasGroup, AmmoManager ammoManager)
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

        fadeOutCoroutines.Remove(ammoManager);
    }

    public void RemoveAmmoBar(AmmoManager ammoManager)
    {
        if (ammoBars.ContainsKey(ammoManager))
        {
            if (fadeOutCoroutines.ContainsKey(ammoManager))
            {
                StopCoroutine(fadeOutCoroutines[ammoManager]);
                fadeOutCoroutines.Remove(ammoManager);
            }
            Destroy(ammoBars[ammoManager]);
            ammoBars.Remove(ammoManager);
        }
    }
}
