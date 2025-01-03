using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoManager : MonoBehaviour
{
    public float maxAmmo = 100;
    public float currentAmmo;
    public float ammoPerSecond;
    public float turnSpeedMult = 1f;
    [HideInInspector] public Attributes entity;
    [SerializeField] private Color color;

    public float GetMaxAmmo()
    {
        return maxAmmo;
    }

    public float GetCurrentAmmo()
    {
        return currentAmmo;
    }
    public void RemoveAmmo(float amount)
    {
        this.currentAmmo -= amount;
    }

    void Start()
    {
        currentAmmo = maxAmmo;
        entity = GetComponentInParent<Attributes>();
        GunControllerPlayer gunC = GetFirstGunControllerPlayer();
        if (gunC != null)
        {
            this.color = gunC.GetAmmoColor();
        }
        AmmoBarManager.Instance.CreateAmmoBar(this, color); // Pass the GunController instance

    }
    void FixedUpdate()
    {
        UpdateAmmoBar();
    }

    public void SetColor(Color color)
    {
        this.color = color;
    }
    public void UpdateAmmoBar()
    {
        if (currentAmmo < maxAmmo)
        {
            this.currentAmmo += (ammoPerSecond / (1 / Time.fixedDeltaTime));
        }
        AmmoBarManager.Instance.UpdateAmmoBar(this, currentAmmo, maxAmmo); // Pass the GunController instance
    }


    private GunControllerPlayer GetFirstGunControllerPlayer()
    {
        Transform currentParent = transform.parent;

        while (currentParent != null)
        {
            GunControllerPlayer gunC = currentParent.GetComponent<GunControllerPlayer>();
            if (gunC != null)
            {
                return gunC;
            }

            currentParent = currentParent.parent;
        }

        return null;
    }
}
