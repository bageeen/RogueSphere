using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;

public class GunShoot : MonoBehaviour
{
    // Public Fields
    [SerializeField] protected Transform firePoint;
    public Animator gunAnimator;

    // Serialized Fields
    [SerializeField] protected Attributes entity;
    [SerializeField] protected GunController gunController;


    [SerializeField] protected float initialKnockbackPower;
    [SerializeField] protected float initialRecoilValue;
    [SerializeField] protected float initialFireRate;

    // Private Fields
    protected Rigidbody2D rbEntity;
    protected string bulletsTag;
    protected string bulletsLayer;
    protected GameObject parentBullets;

    // Final attributes
    protected float recoilValue;
    protected float fireRate;
    protected float knockbackPower;

    // Ammo attributes
    [SerializeField] protected float ammoCost;
    protected AmmoManager ammoManager; 


    protected float minSplatSize;
    protected float maxSplatSize;

    // Public Methods
    
    public float GetFireRate() => fireRate;

    public void UpdatePrefabAttributes(Dictionary<string,string> data)
    {
        foreach (var kvp in data)
        {
            string tmpName = kvp.Key;
            string parameterName = char.ToLower(tmpName[0]) + tmpName.Substring(1,tmpName.Length-2);
            string parameterValue = kvp.Value;
            UpdateParameter(this, parameterName, parameterValue);
        }
    }

    private void UpdateParameter(Component component, string parameterName, string parameterValue)
    {
        Type type = component.GetType();
        FieldInfo field = type.GetField(parameterName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        PropertyInfo property = type.GetProperty(parameterName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        if (field != null)
        {
            // Update field value dynamically
            try
            {
                object convertedValue = Convert.ChangeType(parameterValue, field.FieldType);
                field.SetValue(component, convertedValue);
                Debug.Log($"Updated field {parameterName} with value {convertedValue}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to update field {parameterName}: {ex.Message}");
            }
        }
        else if (property != null)
        {
            // Update property value dynamically
            try
            {
                object convertedValue = Convert.ChangeType(parameterValue, property.PropertyType);
                property.SetValue(component, convertedValue);
                Debug.Log($"Updated property {parameterName} with value {convertedValue}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to update property {parameterName}: {ex.Message}");
            }
        }
        else
        {
            Debug.LogWarning($"Parameter not found: {parameterName}");
        }
    }

    // Protected Methods
    protected virtual void Awake()
    {
        InitializeGunController();
        InitializeAttributes();
        InitializeBulletProperties();
        this.ammoManager = GetFirstParentWithAmmoManager();
    }
    protected virtual void Start()
    {
        minSplatSize = gunController.GetMinSplatSize();
        maxSplatSize = gunController.GetMaxSplatSize();
        
    }

    protected virtual void FixedUpdate()
    {
        //UpdateAttributes();
    }

    public virtual void Shoot()
    {
            if (ammoCost > ammoManager.GetCurrentAmmo())
            {
                Debug.Log("OUT OF AMMO");
            }
            else
            {
                ammoManager.RemoveAmmo(ammoCost);
                ammoManager.UpdateAmmoBar();
            }
        
    }

    protected virtual bool canShoot()
    {
        return this.ammoCost < ammoManager.GetCurrentAmmo();
    }

    // Private Methods
    private void InitializeAttributes()
    {
        this.entity = GetFirstParentWithAttributes();
        if (this.entity != null)
        { 
            bulletsTag = entity.bulletTag;
            bulletsLayer = entity.bulletLayer;
            rbEntity = FindFirstRBInParents(gameObject);
            parentBullets = GameObject.FindWithTag("BulletsFired");
        }
    }

    public static Rigidbody2D FindFirstRBInParents(GameObject gameObject)
    {
        Transform currentTransform = gameObject.transform;
        while (currentTransform != null)
        {
            Rigidbody2D rb = currentTransform.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                return rb;
            }
            currentTransform = currentTransform.parent;
        }

        return null; 
    }


    private void InitializeGunController()
    {
       
        this.gunController = GetFirstParentWithGunHandler();
    }

    protected virtual void InitializeBulletProperties()
    {
        recoilValue = entity.GetRecoilValue() * initialRecoilValue;
        fireRate = entity.GetFireRate() * initialFireRate;
        knockbackPower = entity.GetKnockbackPower() * initialKnockbackPower;

    }

    public virtual void UpdateAttributes()
    {
        recoilValue = entity.GetRecoilValue() * initialRecoilValue;
        fireRate = entity.GetFireRate() * initialFireRate;
        knockbackPower = entity.GetKnockbackPower() * initialKnockbackPower;
    }

    protected void Recoil()
    {
        Vector2 lookDir = (new Vector2(firePoint.position.x, firePoint.position.y) - rbEntity.position).normalized;
        rbEntity.AddForce(-lookDir * recoilValue, ForceMode2D.Impulse);
    }

    private Attributes GetFirstParentWithAttributes()
    {
        Transform currentParent = transform.parent;

        while (currentParent != null)
        {
            Attributes attributes = currentParent.GetComponent<Attributes>();
            if (attributes != null)
            {
                return attributes;
            }

            currentParent = currentParent.parent;
        }

        return null;
    }

    private GunController GetFirstParentWithGunHandler()
    {
        Transform currentParent = transform.parent;

        while (currentParent != null)
        {
            GunController gunController = currentParent.GetComponent<GunController>();
            if (gunController != null)
            {
                return gunController;
            }

            currentParent = currentParent.parent;
        }

        return null;
    }

    private AmmoManager GetFirstParentWithAmmoManager()
    {
        Transform currentParent = transform.parent;

        while (currentParent != null)
        {
            AmmoManager ammoManager = currentParent.GetComponent<AmmoManager>();
            if (ammoManager != null)
            {
                return ammoManager;
            }
            currentParent = currentParent.parent;
        }

        return null; // No parent with AmmoManager found
    }

}
