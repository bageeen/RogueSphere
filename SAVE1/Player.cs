using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{

    public int maxHealth = 100;
    public int currentHealth;

    public HealthBar healthBar;

    public InputActionReference takeDamage;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);

    }

    // Update is called once per frame
    void Update()
    {

        
    }

    void TakeDamage(InputAction.CallbackContext obj)
    {
        currentHealth -= 3;

        healthBar.SetHealth(currentHealth);
    }

    private void OnEnable()
    {
        takeDamage.action.started += TakeDamage;
    }
    private void OnDisable()
    {
        takeDamage.action.started -= TakeDamage;
    }
}
