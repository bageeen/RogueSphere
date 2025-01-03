using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunControllerPlayer : MonoBehaviour
{



    private GunController leftGunController;
    private GunController rightGunController;
    private InputMaster playerControls;
    

    private float nextFireTimeLeft = 0f;
    private float nextFireTimeRight = 0f;

    [SerializeField] private string mouseButton = "Left";
    [SerializeField] private Color ammoColor;

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

    public Color GetAmmoColor()
    { 
        return ammoColor; 
    }

    

    void FixedUpdate()
    {

        bool isShootKeyHeldLeft = false;
        bool isShootKeyHeldRight = false;


        
        isShootKeyHeldLeft = playerControls.Player.ShootLeft.ReadValue<float>() > 0.1f;
        isShootKeyHeldRight = playerControls.Player.ShootRight.ReadValue<float>() > 0.1f;
        

        if (isShootKeyHeldLeft && Time.time >= nextFireTimeLeft)
        {
            FindGunControllers();
            nextFireTimeLeft = leftGunController.Shoot();
        }
        if (isShootKeyHeldRight && Time.time >= nextFireTimeRight)
        {
            FindGunControllers();
            nextFireTimeRight = rightGunController.Shoot();
        }
    }

    private void FindGunControllers()
    {
        GunController[] guns = GetComponentsInChildren<GunController>();
        if (this.leftGunController == null || this.rightGunController == null)
        {
            this.leftGunController = guns[0];
            if(guns.Length >= 2)
            {
                this.rightGunController = guns[1];
            }
        }
    }

    private Color HexToColor(string hex)
    {
        hex = hex.Replace("#", "");
        if (hex.Length == 6)
        {
            byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

            return new Color32(r, g, b, 255);
        }
        else
        {
            Debug.LogError("Hex color is not in the correct format");
            return Color.black;
        }
    }
}
