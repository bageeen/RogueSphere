using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Attributes : MonoBehaviour
{
   

    public float maxHealth = 100;
    [HideInInspector]  public float currentHealth;
    

    [SerializeField] public string bulletTag;
    [SerializeField] public string bulletLayer;
    [SerializeField] protected float healthRegen;

    public Rigidbody2D rb;

    [SerializeField] public float turnSpeed;
    [SerializeField] public float baseTurnSpeed = 100f;
    private AmmoManager ammoManager;


    // Attributes
    [SerializeField] protected float ramDamage;
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected float recoilValue;

   
    [SerializeField] protected float knockbackPower;
    [SerializeField] protected float fireRate;
    [SerializeField] protected float bulletForce;
    [SerializeField] protected float bulletDurability;
    [SerializeField] protected float bulletDamage;
    [SerializeField] protected float bulletSize;
    [SerializeField] protected float bulletMass;

    [SerializeField] protected int penetration = 1;

    [SerializeField] protected List<ColorEnum> colors;


    [SerializeField] SpriteRenderer body;

    protected Color effectiveColor;

    
    protected GunController[] gunControllers;


    protected bool isDeadBool = false;

    public float GetHealth() => currentHealth;
    public float GetRecoilValue() => recoilValue;
    public float GetFireRate() => fireRate;
    public float GetBulletForce() => bulletForce;
    public float GetKnockbackPower() => knockbackPower;
    public float GetBulletDurability() => bulletDurability;
    public float GetBulletDamage() => bulletDamage;
    public float GetRamDamage() => ramDamage;
    public float GetMoveSpeed() => moveSpeed;
    public float GetBulletSize() => bulletSize;
    public float GetBulletMass() => bulletMass;
    public int GetPenetration() => penetration;
    public Color GetColor() => effectiveColor;

    public Color GetRandomColor()
    {
        Color res = Color.white;
        if (colors.Count > 0)
        {
            res = colors[Random.Range(0, colors.Count)].GetColor();
        }
        return res;
    }

    public void AddColor(ColorEnum color)
    {
        colors.Add(color);
        effectiveColor = MergeColors(colors);
        body.color = effectiveColor;
    }

 
    public void UpdateGunControllersAttributes()
    {
        GunController[] gunControllers = GetComponentsInChildren<GunController>();
        foreach (var gunC in gunControllers)
        {
            gunC.UpdateAttributes();
        }
    }
    public void UpdateGunShootAttributes()
    {
        GunShoot[] gunShoots = GetComponentsInChildren<GunShoot>();
        foreach (var gunS in gunShoots)
        {
            gunS.UpdateAttributes();
        }
    }
    public virtual void UpdateGuns()
    {
        
        UpdateGunControllersAttributes();
        UpdateGunShootAttributes();
        this.ammoManager = GetComponentInChildren<AmmoManager>();
        this.turnSpeed = baseTurnSpeed * ammoManager.turnSpeedMult;
    }


    protected virtual void Start()
    {
        HealthBarManager.Instance.CreateHealthBar(gameObject);
        UpdateHealthBar();

        effectiveColor = MergeColors(colors);
        body.color = GetColor();
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();

    }

    

    protected void FixedUpdate()
    {
        UpdateHealthBar();
        
        isDead();
        if (currentHealth < maxHealth)
        {
            currentHealth += healthRegen / 50;
        }        
    }


    protected void UpdateHealthBar()
    {
        HealthBarManager.Instance.UpdateHealthBar(gameObject, currentHealth, maxHealth);
    }



    protected virtual void isDead()
    {
        if (currentHealth <= 0 && !isDeadBool)
        {
            HealthBarManager.Instance.RemoveHealthBar(gameObject);
            isDeadBool = true;
            gunControllers = GetComponentsInChildren<GunController>();

            foreach (GunController gun in gunControllers)
            {
                gun.enabled = false;
            }

            rb.isKinematic = false;
            ApplyDeathImpulse();
            StartCoroutine(DisableScriptsAfterDelay(0.5f)); // Optional delay for the impulse to be noticeable
        }
    }

    protected void ApplyDeathImpulse()
    {
        rb.constraints &= ~RigidbodyConstraints2D.FreezeRotation;
        float rotationalImpulse = 25f; // Adjust this value as needed
        float direction = Random.Range(0, 2) * 2 - 1; // Generates either -1 or 1
        rb.AddTorque(rotationalImpulse * direction , ForceMode2D.Impulse);
    }

    protected IEnumerator DisableScriptsAfterDelay(float delay)
    {
        

        yield return new WaitForSeconds(delay);

        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            if (script != this) // Optionally exclude this script if you need it to remain enabled
            {
                script.enabled = false;
            }
        }

        gameObject.layer = LayerMask.NameToLayer("SolidObjects");

        this.enabled = false;

    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        UpdateHealthBar();

    }

    public void TakeKnockback(GameObject source, float power)
    {
        rb.AddForce(-(source.GetComponent<Rigidbody2D>().position - rb.position).normalized * power, ForceMode2D.Impulse);
    }



    protected void ManageKnockback(Collision2D collision)
    {

        if (collision.gameObject.TryGetComponent<Attributes>(out Attributes other) && other.gameObject.tag != this.gameObject.tag)
        {
            float kbPower = 4 + other.rb.mass - this.rb.mass;
            if (kbPower < 0)
            {
                kbPower = 0;
            }
            rb.AddForce(-(other.rb.position - rb.position).normalized * kbPower, ForceMode2D.Impulse);
            this.TakeDamage(other.GetRamDamage());
        }
    }

    protected void OnCollisionEnter2D(Collision2D collision)
    {
        ManageKnockback(collision);
    }

    protected static Color MergeColors(List<ColorEnum> colors)
    {
        if (colors == null || colors.Count == 0)
        {
            return Color.white; // Default to white if no colors are provided
        }

        float r = 0f;
        float g = 0f;
        float b = 0f;
        float a = 0f;
        float totalWeight = 0f;

        for (int i = 0; i < colors.Count; i++)
        {
            Color color = colors[i].GetColor(); // Get color from end of list
            float weight = (i + 1); // Increasing weight for each color

            r += color.r * weight;
            g += color.g * weight;
            b += color.b * weight;
            a += color.a * weight;

            totalWeight += weight;
        }

        // Average the RGB values with weights
        r /= totalWeight;
        g /= totalWeight;
        b /= totalWeight;
        a /= totalWeight;

        // Create the final color
        Color finalColor = new Color(r, g, b, a);

        // Adjust the final color's brightness and saturation if needed
        finalColor = AdjustBrightnessAndSaturation(finalColor);

        return finalColor;
    }


    protected static Color AdjustBrightnessAndSaturation(Color color)
    {
        // Convert color to HSV
        Color.RGBToHSV(color, out float h, out float s, out float v);

        // Adjust the saturation and brightness
        s = Mathf.Clamp(s, 0.5f, 1f); // Ensure saturation is within a certain range
        v = Mathf.Clamp(v, 0.5f, 1f); // Ensure brightness is within a certain range

        // Convert back to RGB
        color = Color.HSVToRGB(h, s, v);
        color.a = 1f; // Ensure alpha is fully opaque

        return color;
    }

}

public enum EntityAttributes
{
    MaxHealth,
    HealthRegen,
    RamDamage,
    MoveSpeed,
    RecoilValue,
    KnockbackPower,
    FireRate,
    BulletForce,
    BulletDurability,
    BulletDamage,
    BulletSize,
    BulletMass,
    Penetration,
    CamSize,
    TurnSpeed
}


