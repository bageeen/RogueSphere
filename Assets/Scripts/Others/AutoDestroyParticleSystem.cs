using UnityEngine;

public class AutoDestroyParticleSystem : MonoBehaviour
{
    private ParticleSystem ps;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        if (ps == null)
        {
            Debug.LogError("No ParticleSystem found on " + gameObject.name);
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (ps != null && !ps.IsAlive())
        {
            Destroy(gameObject);
        }
    }
}
