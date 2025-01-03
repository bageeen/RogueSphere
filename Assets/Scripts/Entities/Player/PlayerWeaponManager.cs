using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PlayerWeaponManager : MonoBehaviour
{
    public WeaponNode initialWeapon;
    private WeaponNode currentWeaponNode;
    private GameObject currentWeapon;

    [SerializeField] private AmmoBarManager ammoBarManager;

    public GameObject evolutionChoicePanel;
    public Button[] evolutionButtons; // Ensure this array has exactly 3 buttons

    private Inventory inventory;

    private GameObject player;
    private Transform playerBody;

    public Camera screenshotCamera; // Reference to the dedicated screenshot camera

    // List of attribute evolutions
    public List<AttributeNode> attributeNodes;
    public List<EquipementNode> equipementNodes;

    // Placeholder sprite for attribute evolutions
    [SerializeField] private Sprite[] placeholderSprites;

    // Dictionary to keep track of which evolution (weapon, attribute, or equipement) is associated with each button
    private Dictionary<int, ScriptableObject> buttonToEvolutionMap = new Dictionary<int, ScriptableObject>();

    [SerializeField] private bool mergeEvolutions = true;
    // Probabilities for choosing between WeaponNode, AttributeNode, and EquipementNode
    [Range(0, 1)] public float weaponNodeProbability = 0.1f;
    [Range(0, 1)] public float attributeNodeProbability = 0.8f;
    [Range(0, 1)] public float equipementNodeProbability = 0.1f;

    // HUD for equipped items
    [SerializeField] private GameObject equipementHUD;
    [SerializeField] private Image equipementIconPrefab;

    // Dictionary to keep track of equipped items and their types
    private Dictionary<string, GameObject> equippedItemsByType = new Dictionary<string, GameObject>();
    private Dictionary<string, EquipementNode> equippedEquipementNodes = new Dictionary<string, EquipementNode>();
    private Dictionary<string, Image> equippedHUDIcon = new Dictionary<string, Image>();

    void Start()
    {
        // Get reference to Inventory script
        inventory = FindObjectOfType<Inventory>();

        // Find the player GameObject
        player = GameObject.FindWithTag("Player"); // Ensure your player GameObject has the "Player" tag
        if (player == null)
        {
            Debug.LogError("Player GameObject not found.");
            return;
        }

        // Find the player's body
        playerBody = player.transform.Find("Body"); // Adjust this if your body object has a different name
        if (playerBody == null)
        {
            Debug.LogError("Player's body not found.");
        }

        EquipWeapon(initialWeapon);
    }

    public void EquipWeapon(WeaponNode weaponNode)
    {
        if (currentWeapon != null)
        {
            AmmoManager[] ammoManagers = GetComponentsInChildren<AmmoManager>();
            foreach (AmmoManager ammoManager in ammoManagers)
            {
                ammoBarManager.RemoveAmmoBar(ammoManager);
            }
            Destroy(currentWeapon);
        }

        currentWeapon = Instantiate(weaponNode.weaponPrefab, playerBody.transform);
        currentWeaponNode = weaponNode;
        // Optionally, you can update current weapon in PlayerDataManager
        StartCoroutine(DelayedUpdateGuns());
    }

    private IEnumerator DelayedUpdateGuns()
    {
        yield return new WaitForSeconds(0.2f);

        // Call UpdateGuns after the delay
        GetComponent<AttributesPlayer>().UpdateGuns();

    }


    public void ShowEvolutionChoices()
    {
        StartCoroutine(ShowEvolutionChoicesCoroutine());
    }

    private IEnumerator ShowEvolutionChoicesCoroutine()
    {
        // Close the inventory if it's open
        if (inventory.inventoryPanel.activeSelf)
        {
            inventory.inventoryPanel.SetActive(false);
        }

        evolutionChoicePanel.SetActive(true);

        // Clear the button to evolution map
        buttonToEvolutionMap.Clear();

        // Create separate lists for available evolutions
        List<WeaponNode> availableWeaponEvolutions = new List<WeaponNode>(currentWeaponNode.evolutions);
        List<AttributeNode> availableAttributeEvolutions = new List<AttributeNode>(attributeNodes);
        List<EquipementNode> availableEquipementEvolutions = GetAvailableEquipementNodes();

        List<EvolutionNode> mergedEvolutions = new List<EvolutionNode>(availableWeaponEvolutions);
        mergedEvolutions.AddRange(availableAttributeEvolutions);
        mergedEvolutions.AddRange(availableEquipementEvolutions);

        Debug.Log($"At the start of the script, there is {availableEquipementEvolutions.Count} evolutions");
        for (int i = 0; i < evolutionButtons.Length; i++)
        {
            if (availableWeaponEvolutions.Count == 0 && availableAttributeEvolutions.Count == 0 && availableEquipementEvolutions.Count == 0)
            {
                // No more evolutions available
                evolutionButtons[i].gameObject.SetActive(false);
                continue;
            }

            ScriptableObject evolution = null;

            if (mergeEvolutions)
            {
                
                evolution = SelectWeightedEvolution(mergedEvolutions);
                mergedEvolutions.Remove((EvolutionNode)evolution);
            }
            else
            {
                evolution = SelectEvolution(ref availableWeaponEvolutions, ref availableAttributeEvolutions, ref availableEquipementEvolutions);
            }


            Button button = evolutionButtons[i];
            TextMeshProUGUI buttonText = evolutionButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            Image buttonImage = evolutionButtons[i].transform.Find("Image").GetComponent<Image>();

            // Reset the color to white before setting the new color
            buttonImage.color = Color.white;

            if (evolution is WeaponNode weaponEvolution)
            {
                buttonText.text = weaponEvolution.description;
                Sprite screenshotSprite = CaptureScreenshotWithPlayerBody(weaponEvolution);
                buttonImage.sprite = screenshotSprite;
                button.GetComponent<Image>().color = weaponEvolution.rarityLevel.GetColor();
            }
            else if (evolution is AttributeNode attributeEvolution)
            {
                buttonText.text = attributeEvolution.description;
                buttonImage.sprite = placeholderSprites[Random.Range(0, placeholderSprites.Length)]; // Use the placeholder sprite
                buttonImage.color = attributeEvolution.attributeColor.GetColor(); // Set the color of the sprite
                button.GetComponent<Image>().color = attributeEvolution.rarityLevel.GetColor();
            }
            else if (evolution is EquipementNode equipementEvolution)
            {
                buttonText.text = equipementEvolution.description;
                Sprite screenshotSprite = CaptureScreenshotWithPlayerBody(equipementEvolution);
                buttonImage.sprite = screenshotSprite;
                button.GetComponent<Image>().color = equipementEvolution.rarityLevel.GetColor();
            }

            int index = i;  // Capture the index for the onClick listener
            buttonToEvolutionMap[index] = evolution;

            evolutionButtons[i].onClick.RemoveAllListeners();
            evolutionButtons[i].onClick.AddListener(() => ChooseEvolution(index));
            evolutionButtons[i].gameObject.SetActive(true);

            // Wait for a frame before continuing to the next button
            yield return new WaitForEndOfFrame();
        }
    }

    private List<EquipementNode> GetAvailableEquipementNodes()
    {
        List<EquipementNode> availableEquipementEvolutions = new List<EquipementNode>();

        foreach (EquipementNode equipement in equipementNodes)
        {
            EquipementNode currentEquipement = equipement;

            // Check if an item of this type is already equipped
            if (equippedEquipementNodes.ContainsKey(currentEquipement.type))
            {
                if (equippedEquipementNodes[currentEquipement.type].nextTier != null)
                {
                    availableEquipementEvolutions.Add(equippedEquipementNodes[currentEquipement.type].nextTier);
                }
            }
            else if (!equippedEquipementNodes.ContainsKey(equipement.type))
            {
                availableEquipementEvolutions.Add(currentEquipement);
            }
        }
        return availableEquipementEvolutions;
    }

    private ScriptableObject SelectEvolution(ref List<WeaponNode> availableWeaponEvolutions, ref List<AttributeNode> availableAttributeEvolutions, ref List<EquipementNode> availableEquipementEvolutions)
    {
        float randomValue = Random.Range(0f, 1f);
        ScriptableObject selectedEvolution;

        if (randomValue <= weaponNodeProbability && availableWeaponEvolutions.Count > 0)
        {
            selectedEvolution = SelectWeightedEvolution(availableWeaponEvolutions);
            availableWeaponEvolutions.Remove((WeaponNode)selectedEvolution);
        }
        else if (randomValue <= weaponNodeProbability + equipementNodeProbability && availableEquipementEvolutions.Count > 0)
        {
            selectedEvolution = SelectWeightedEvolution(availableEquipementEvolutions);
            availableEquipementEvolutions.Remove((EquipementNode)selectedEvolution);
        }
        else if (availableAttributeEvolutions.Count > 0)
        {
            selectedEvolution = SelectWeightedEvolution(availableAttributeEvolutions);
            availableAttributeEvolutions.Remove((AttributeNode)selectedEvolution);
        }
        else
        {
            selectedEvolution = null; // No evolutions left to choose from
        }

        return selectedEvolution;
    }

    private T SelectWeightedEvolution<T>(List<T> evolutions) where T : ScriptableObject
    {
        List<float> weights = new List<float>();
        float totalWeight = 0;

        foreach (var evolution in evolutions)
        {
            float weight = 1.0f;
            if (evolution is WeaponNode weaponNode)
            {
                weight = weaponNode.weight;
            }
            else if (evolution is AttributeNode attributeNode)
            {
                weight = attributeNode.weight;
            }
            else if (evolution is EquipementNode equipementNode)
            {
                weight = equipementNode.weight;
            }
            weights.Add(weight);
            totalWeight += weight;
        }

        float randomValue = Random.Range(0, totalWeight);
        float cumulativeWeight = 0;

        for (int i = 0; i < evolutions.Count; i++)
        {
            cumulativeWeight += weights[i];
            if (randomValue <= cumulativeWeight)
            {
                return evolutions[i];
            }
        }

        return evolutions[evolutions.Count - 1]; // Fallback in case of rounding errors
    }

    private Sprite CaptureScreenshotWithPlayerBody(EvolutionNode node)
    {
        if (node is WeaponNode)
        {
            currentWeapon.SetActive(false);
        }
        else if (node is EquipementNode equipNode)
        {
            if (equippedItemsByType.ContainsKey(equipNode.type))
            {
                equippedItemsByType[equipNode.type].SetActive(false);
            }
        }

        WeaponScreenshot screenshot = gameObject.AddComponent<WeaponScreenshot>();
        screenshot.prefab = node.GetPrefab();
        screenshot.screenshotCamera = screenshotCamera; // Use the dedicated screenshot camera
        Sprite screenshotSprite = screenshot.CaptureScreenshot(playerBody.gameObject);
        DestroyImmediate(screenshot); // Clean up the component after capturing the screenshot

        if (node is WeaponNode)
        {
            currentWeapon.SetActive(true);
        }
        else if (node is EquipementNode equipNode)
        {
            if (equippedItemsByType.ContainsKey(equipNode.type))
            {
                equippedItemsByType[equipNode.type].SetActive(true);
            }
        }

        return screenshotSprite;
    }

    public void ChooseEvolution(int index)
    {
        if (buttonToEvolutionMap.ContainsKey(index))
        {
            ScriptableObject chosenEvolution = buttonToEvolutionMap[index];
            if (chosenEvolution is WeaponNode weaponEvolution)
            {
                EquipWeapon(weaponEvolution);
            }
            else if (chosenEvolution is AttributeNode attributeEvolution)
            {
                ApplyAttributeEvolution(attributeEvolution);
            }
            else if (chosenEvolution is EquipementNode equipementEvolution)
            {
                EquipEquipement(equipementEvolution);
            }
            evolutionChoicePanel.SetActive(false);
        }
        else
        {
            Debug.LogError("Invalid evolution index");
        }
        inventory.PauseGame(false); // Unpause the game if needed
    }

    private void ApplyAttributeEvolution(AttributeNode attributeEvolution)
    {
        AttributesPlayer playerAttributes = player.GetComponent<AttributesPlayer>();
        if (playerAttributes != null)
        {
            playerAttributes.ApplyAttributeNode(attributeEvolution);
        }
    }

    private void EquipEquipement(EquipementNode equipementNode)
    {
        AttributesPlayer playerAttributes = player.GetComponent<AttributesPlayer>();

        // Unequip the current tier if it exists
        if (equippedItemsByType.ContainsKey(equipementNode.type))
        {
            // Remove attribute modifications of the current tier
            if (playerAttributes != null)
            {
                foreach (var modification in equippedEquipementNodes[equipementNode.type].attributeModifications)
                {
                    playerAttributes.RemoveAttributeModification(modification.attributeName, modification.Value);
                }
            }
            // Destroy the current equipment game object
            Destroy(equippedItemsByType[equipementNode.type]);
            Destroy(equippedHUDIcon[equipementNode.type].gameObject);
        }

        // Equip the new tier and apply its attribute modifications
        GameObject equipementInstance = Instantiate(equipementNode.equipementPrefab, player.transform.Find("Body"));
        if (playerAttributes != null)
        {
            foreach (var modification in equipementNode.attributeModifications)
            {
                playerAttributes.ApplyAttributeModification(modification.attributeName, modification.Value);
            }
        }
        AddEquipementToHUD(equipementNode);

        // Update the equippedItemsByType dictionary with the new tier
        equippedItemsByType[equipementNode.type] = equipementInstance;
        equippedEquipementNodes[equipementNode.type] = equipementNode;
    }

    private void AddEquipementToHUD(EquipementNode equipementNode)
    {
        Image iconInstance = Instantiate(equipementIconPrefab, equipementHUD.transform);
        equippedHUDIcon[equipementNode.type] = iconInstance;
        iconInstance.sprite = equipementNode.equipementIcon;
    }

    // Method to shuffle a list
    private List<T> ShuffleList<T>(List<T> list)
    {
        System.Random rng = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
        return list;
    }
}
