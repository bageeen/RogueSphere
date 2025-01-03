using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCasterPaint : MonoBehaviour
{
    public ParticleSystem splatParticles;
    public GameObject splatPrefab;
    public Transform splatHolder;

    private void Update()
    {
        /*
        if (Input.GetMouseButtonDown(0))
        {
            CastRay();
        }
        */
    }


    public void CastRay(Vector3 worldPosition, Color color, float minSize, float maxSize)
    {
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);

        int layerToInclude = LayerMask.NameToLayer("Background"); // Replace "LayerToInclude" with the actual layer name
        int layerMask = 1 << layerToInclude; // This creates a mask that includes only the specified layer


        RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity, layerMask);

        if (hit.collider != null)
        {
            GameObject splat = Instantiate(splatPrefab, hit.point, Quaternion.identity) as GameObject;
            splat.transform.SetParent(splatHolder, true);
            Splat splatScript = splat.GetComponent<Splat>();

            splatParticles.transform.position = hit.point;
            splatParticles.startColor = color;
            splatParticles.GetComponent<SplatParticles>().SetSplatSize(minSize, maxSize);
            splatParticles.Play();

            if (hit.collider.gameObject.tag == "BG")
                splatScript.Initialize(Splat.SplatLocation.Background, color, minSize, maxSize);
            else
                splatScript.Initialize(Splat.SplatLocation.Foreground, color, minSize, maxSize);
            
        }
    }

}
