using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Shooting : MonoBehaviour
{

    public InputMaster controls;
    public Transform firePoint;
    public GameObject bulletPrefab;
    public float bulletForce = 20f;
    public float fireRate;
    public float recoilValue;
    float nextFire;

    public Rigidbody2D rbPlayer;

    Vector2 mousePos;
    float currentTime;

    public Camera cam;


    InputMaster playerControls;

    // Knockback
    public InputActionReference shoot;
    public InputActionReference move;
    public KnockBack knockback;
    public Vector2 moveInput;


    private void Start()
    {
        knockback = GetComponent<KnockBack>();
    }

    private void Awake()
    {
        playerControls = new InputMaster();
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }
    private void OnDisable()
    {
        playerControls.Disable();
    }

    void FixedUpdate()
    {
        currentTime += Time.deltaTime;

        bool isShootKeyHeld = playerControls.Player.Shoot.ReadValue<float>() > 0.1f;

        if (isShootKeyHeld)
        {
            if (currentTime > fireRate)
            {
                Shoot();
                Recoil();
                currentTime = 0;

            }
        }

    }

    void Shoot()
    {

        if(Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Rigidbody2D rbshot = bullet.GetComponent<Rigidbody2D>();
            rbshot.AddForce(firePoint.up * bulletForce, ForceMode2D.Impulse);
        }
    }

    void Recoil()
    {

        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 lookDir = (mousePos - rbPlayer.position).normalized;

        moveInput = move.action.ReadValue<Vector2>();

        knockback.CallKnockback(-lookDir, new Vector2(0, 0), moveInput);
        
    }

}
