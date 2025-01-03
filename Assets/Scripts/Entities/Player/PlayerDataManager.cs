using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance;

    public GameObject currentWeaponPrefab;
    public GameObject currentBodyPrefab;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetCurrentWeapon(GameObject weapon)
    {
        currentWeaponPrefab = weapon;
    }

    public void SetCurrentBody(GameObject body)
    {
        currentBodyPrefab = body;
    }
}
