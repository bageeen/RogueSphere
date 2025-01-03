using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserGun : GunShoot
{
    // Public Fields
    public LineRenderer laserLine; // Assign this in the Inspector
    public float laserDuration; // Total duration of the laser firing
    public float laserDamage; // Total damage over the laser's duration
    public float maxRange; // Maximum range of the laser
    public float fadeDuration; // Duration for the laser to fade out


    public GameObject startVFX;
    public GameObject endVFX; // Assign this in the Inspector

    [SerializeField] private GameObject lineObject;

    private List<ParticleSystem> particles;

    public float laserWidth = 0.15f;
    

    // Private Fields
    private float effectiveLaserDamage;
    private bool isFiringLaser = false;
    private Coroutine firingCoroutine;
    private float damagePerFrame; // Damage to apply each frame
    private int totalFrames; // Total number of frames the laser will be active
    private int parentLayer;
    private int targetLayerMask;
    private int framesApplied = 0; // Number of frames damage has been applied
    private int compteurTemp = 0;

    private GameObject parentLaser;
    // Animations

    public float textureScrollSpeed = 5.0f;

    // Protected Methods
    protected override void Awake()
    {
        base.Awake();
        InitializeLaserLine();
        InitializeLayerMasks();
        UpdateAttributes();
        parentLaser = GameObject.Find("BulletsFired");
    }

    void Start()
    {
        FillLists();
        for (int i = 0; i < particles.Count; i++)
        {
            particles[i].Stop();
        }
        laserLine.enabled = true;
        laserLine.transform.parent = parentLaser.transform;

    }

    protected override void FixedUpdate()
    {
        
    }

    void OnDestroy()
    {
        
        Destroy(this.lineObject);
        Debug.Log($"{this.lineObject} is destroyed");
    }
    void Update()
    {
        base.FixedUpdate();
        if (isFiringLaser)
        {
            FireLaser();
        }
        else
        {
            laserLine.enabled = false;
        }
        //AnimateLaser();
    }

    void FillLists()
    {
        particles = new List<ParticleSystem>();
        for (int i = 0; i < startVFX.transform.childCount; i++)
        {
            var ps = startVFX.transform.GetChild(i).GetComponent<ParticleSystem>();
            if (ps != null)
            {
                Debug.Log($"{ps}");
                particles.Add(ps);
            }
        }
        for (int i = 0; i < endVFX.transform.childCount; i++)
        {
            var ps = endVFX.transform.GetChild(i).GetComponent<ParticleSystem>();
            if (ps != null)
            {
                Debug.Log($"{ps}");
                particles.Add(ps);
            }
        }
    }


    void AnimateLaser()
    {
        // Adjust pulsateAmount to be 10% of baseWidth
        float pulsateAmount = laserWidth * 0.1f; // 10% of the baseWidth
        float pulsateSpeed = 10.0f; // Speed of the pulsating effect, adjust as needed

        // Pulsate Width
        float pulsate = Mathf.Sin(Time.time * pulsateSpeed) * pulsateAmount;
        float currentWidth = laserWidth - pulsate;
        laserLine.startWidth = currentWidth;
        laserLine.endWidth = currentWidth;

        // Scroll Texture
        float textureScrollSpeed = 5.0f; // Adjust texture scroll speed as needed
        float textureOffset = Time.time * textureScrollSpeed;
        //laserLine.material.mainTextureOffset = new Vector2(-textureOffset, 0);
    }


    public override void UpdateAttributes()
    {
        base.UpdateAttributes();
        effectiveLaserDamage = entity.GetBulletDamage() * laserDamage;
    }


    public override void Shoot()
    {
        if (!canShoot())
        {
            return;
        }
        if (!isFiringLaser)
        {
            StartFiringLaser();
        }
        else
        {
            ResetLaserDuration();
        }

        base.Shoot();
    }

    // Private Methods
    private void InitializeLaserLine()
    {
        if (laserLine == null)
        {
            Debug.LogError("LaserGun requires a LineRenderer component.");
        }
        laserLine.enabled = false;
        laserLine.positionCount = 2;
        laserLine.startWidth = laserWidth;
        laserLine.endWidth = laserWidth;
        //laserLine.material = new Material(Shader.Find("Sprites/Default"));
        //laserLine.startColor = Color.red;
        //laserLine.endColor = Color.red;
    }

    private void InitializeLayerMasks()
    {
        // Define all the layers
        int ennemies1Layer = LayerMask.NameToLayer("Ennemies1");
        int ennemies1ProjectilesLayer = LayerMask.NameToLayer("Ennemies1Projectiles");
        int playerLayer = LayerMask.NameToLayer("Player");
        int playerProjectilesLayer = LayerMask.NameToLayer("Player Projectiles");
        int solidObjects = LayerMask.NameToLayer("SolidObjects");

        // Create the base layer mask including all relevant layers
        int baseLayerMask = (1 << solidObjects) | (1 << ennemies1Layer) | (1 << ennemies1ProjectilesLayer) | (1 << playerLayer) | (1 << playerProjectilesLayer);

        // Get the parent layer and bullet layer of the entity
        var (parentLayer, bulletLayer) = GetParentLayerWithAttributes();

        // Remove the parent entity's layer and bullet layer from the base layer mask
        targetLayerMask = baseLayerMask & ~(1 << parentLayer) & ~(1 << bulletLayer);
    }

    private void StartFiringLaser()
    {
        for (int i = 0; i < particles.Count; i++)
        {
            particles[i].Play();
        }

        isFiringLaser = true;
        framesApplied = 0;
        totalFrames = Mathf.CeilToInt(laserDuration / Time.deltaTime);
        damagePerFrame = effectiveLaserDamage / totalFrames;
        if (firingCoroutine != null)
        {
            StopCoroutine(firingCoroutine);
        }
        firingCoroutine = StartCoroutine(LaserFiringCoroutine());
    }

    private IEnumerator LaserFiringCoroutine()
    {
        laserLine.enabled = true;
        yield return new WaitForSeconds(laserDuration);
        yield return StartCoroutine(FadeLaserOut());
        StopFiringLaser();
    }

    private void ResetLaserDuration()
    {
        if (firingCoroutine != null)
        {
            StopCoroutine(firingCoroutine);
        }
        framesApplied = 0;
        firingCoroutine = StartCoroutine(LaserFiringCoroutine());
    }

    private void StopFiringLaser()
    {
        for (int i = 0; i < particles.Count; i++)
        {
            particles[i].Stop();
        }
        isFiringLaser = false;
        laserLine.enabled = false;
        laserLine.SetPosition(0, Vector3.zero); // Reset laser position
        laserLine.SetPosition(1, Vector3.zero); // Reset laser position
        laserLine.startColor = new Color(laserLine.startColor.r, laserLine.startColor.g, laserLine.startColor.b, 1f);
        laserLine.endColor = new Color(laserLine.endColor.r, laserLine.endColor.g, laserLine.endColor.b, 1f);
    }

    private IEnumerator FadeLaserOut()
    {


        Color startColor = laserLine.startColor;
        Color endColor = laserLine.endColor;

        

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            laserLine.startColor = new Color(startColor.r, startColor.g, startColor.b, alpha);
            laserLine.endColor = new Color(endColor.r, endColor.g, endColor.b, alpha);
            yield return null;
        }

        laserLine.enabled = false;
        compteurTemp = 0;
    }

    private void FireLaser()
    {
        if (framesApplied >= totalFrames)
        {
            return; // Stop applying damage after the intended number of frames
        }

        laserLine.enabled = true;

        startVFX.transform.position = (Vector2)firePoint.position;

        
        //laserLine.startColor = Color.red;
        //laserLine.endColor = Color.red;
        laserLine.SetPosition(0, firePoint.position);

        float remainingDamage = damagePerFrame;
        Vector2 currentPosition = firePoint.position;
        Vector2 direction = firePoint.up;
        float remainingRange = maxRange;

        while (remainingDamage > 0 && remainingRange > 0)
        {
            Recoil();
            RaycastHit2D[] hits = Physics2D.RaycastAll(currentPosition, direction, remainingRange, targetLayerMask);
            if (hits.Length == 0)
            {
                // No more hits within the range
                break;
            }

            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider != null && !hit.collider.isTrigger) // Check if the collider is not a trigger
                {
                    float distance = Vector2.Distance(currentPosition, hit.point);
                    if (distance > remainingRange)
                    {
                        // Hit is beyond the remaining range
                        break;
                    }

                    laserLine.SetPosition(1, hit.point);



                    // Deal damage to the object hit
                    var damageable = hit.collider.GetComponent<Attributes>();
                    if (damageable != null)
                    {
                        float objectHealth = damageable.GetHealth();
                        float damageToDeal = Mathf.Min(remainingDamage, objectHealth);
                        damageable.TakeKnockback(entity.gameObject, knockbackPower);
                        damageable.TakeDamage(damageToDeal);
                        remainingDamage -= damageToDeal;

                        if (remainingDamage <= 0)
                        {
                            // No remaining damage to pass through
                            break;
                        }
                    }
                    else
                    {
                        var damageableBullet = hit.collider.GetComponent<Bullet>();
                        if (damageableBullet != null)
                        {
                            Debug.Log("bullet touched");
                            float bulletHealth = damageableBullet.GetHealth();
                            float damageToDeal = Mathf.Min(remainingDamage, bulletHealth);
                            damageableBullet.TakeDamage(damageToDeal);
                            damageableBullet.CheckAlive();
                            remainingDamage -= damageToDeal;

                            if (remainingDamage <= 0)
                            {
                                // No remaining damage to pass through
                                break;
                            }
                        }
                    }

                    // Check if hit object is a solid obstacle and stop laser
                    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("SolidObjects"))
                    {
                        remainingDamage = 0;
                        remainingRange = 0;
                        break;
                    }

                    // Update the remaining range and position
                    remainingRange -= distance;
                    currentPosition = hit.point;
                }
            }

            if (remainingDamage > 0)
            {
                // Set the final position of the laser line if there are no more hits
                laserLine.SetPosition(1, currentPosition + direction * remainingRange);
                break;
            }
        }

        if (remainingDamage > 0)
        {
            // Set the final position of the laser line if there are no hits
            laserLine.SetPosition(1, currentPosition + direction * remainingRange);
        }

        endVFX.transform.position = laserLine.GetPosition(1);
     
        framesApplied++;
    }

    private (int parentLayer, int bulletLayer) GetParentLayerWithAttributes()
    {
        Transform currentTransform = transform;
        while (currentTransform != null)
        {
            Attributes attributes = currentTransform.GetComponent<Attributes>();
            if (attributes != null)
            {
                return (currentTransform.gameObject.layer, LayerMask.NameToLayer(attributes.bulletLayer));
            }
            currentTransform = currentTransform.parent;
        }
        return (gameObject.layer, LayerMask.NameToLayer("Default")); // Fallback to the gun's own layer and default bullet layer if no Attributes component is found
    }
}
