using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;
    private Vector2 minBounds;
    private Vector2 maxBounds;
    private float halfHeight;
    private float halfWidth;
    public float smoothTime = 0.2f; // Time for the interpolation
    private Vector3 velocity = Vector3.zero; // Reference velocity for SmoothDamp

    void Start()
    {
        // UpdateCameraBounds(new Vector2(-75.5f, -37f), new Vector2(75.5f, 37)); // Initial bounds
        halfHeight = Camera.main.orthographicSize;
        halfWidth = halfHeight * Camera.main.aspect;
    }

    void LateUpdate()
    {
        halfHeight = Camera.main.orthographicSize;
        halfWidth = halfHeight * Camera.main.aspect;

        float clampedX = Mathf.Clamp(player.position.x, minBounds.x + halfWidth, maxBounds.x - halfWidth);
        float clampedY = Mathf.Clamp(player.position.y, minBounds.y + halfHeight, maxBounds.y - halfHeight);
        Vector3 targetPosition = new Vector3(clampedX, clampedY, transform.position.z);

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }

    public void UpdateCameraBounds(Vector2 newMinBounds, Vector2 newMaxBounds)
    {
        minBounds = newMinBounds;
        maxBounds = newMaxBounds;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector3(minBounds.x, minBounds.y, 0), new Vector3(minBounds.x, maxBounds.y, 0));
        Gizmos.DrawLine(new Vector3(minBounds.x, maxBounds.y, 0), new Vector3(maxBounds.x, maxBounds.y, 0));
        Gizmos.DrawLine(new Vector3(maxBounds.x, maxBounds.y, 0), new Vector3(maxBounds.x, minBounds.y, 0));
        Gizmos.DrawLine(new Vector3(maxBounds.x, minBounds.y, 0), new Vector3(minBounds.x, minBounds.y, 0));
    }

    public void SetTarget(GameObject target)
    {
        player = target.transform;
    }
}
