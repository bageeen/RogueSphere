using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class XPBar : MonoBehaviour
{

    public Slider slider;
    public Gradient gradient;
    public Image fill;

    private float currentXP;
    private float requiredXP;

    private PlayerWeaponManager weaponManager;
    private Inventory inventory;

    public void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        weaponManager = player.GetComponent<PlayerWeaponManager>();
        inventory = player.GetComponent<Inventory>();
    }

    public void Initialize(float startXP, float startRequiredXP)
    {
        currentXP = startXP;
        requiredXP = startRequiredXP;
        UpdateXPBar();
    }

    public void AddXP(float amount)
    {
        currentXP += amount;
        if (currentXP >= requiredXP)
        {
            LevelUp();
        }
        UpdateXPBar();
    }

    private void UpdateXPBar()
    {
        slider.maxValue = requiredXP;
        slider.value = currentXP;

    }

    private void LevelUp()
    {
        StartCoroutine(PauseAndShowEvolutionChoices());
        currentXP -= requiredXP;
        requiredXP *= 1.1f; // Increase required XP for the next level
        
    }

    private IEnumerator PauseAndShowEvolutionChoices()
    {
        // Wait for one frame
        yield return null;

        // Now pause the game
        inventory.PauseGame(true);

        // Show evolution choices
        weaponManager.ShowEvolutionChoices();
    }
}
