using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] GameObject topDoor;
    [SerializeField] GameObject bottomDoor;
    [SerializeField] GameObject leftDoor;
    [SerializeField] GameObject rightDoor;

    [SerializeField] GameObject topWall;
    [SerializeField] GameObject bottomWall;
    [SerializeField] GameObject leftWall;
    [SerializeField] GameObject rightWall;


    public Vector2 minBounds;
    public Vector2 maxBounds;

    public Vector2Int RoomIndex { get; set; }

    private Transform floor;
    public Bounds bounds;

    private int currentSortingLayerIndex = 0;

    private bool hasEnemiesSpawned = false;

    private RoomManager roomManager;

    public void Start()
    {
        bounds = GetRoomBounds();
        CombineRoomMeshes();
        UpdateBounds();
        roomManager = GetComponentInParent<RoomManager>();
    }

    public Bounds GetRoomBounds()
    {
        floor = transform.GetChild(0);
        return floor.GetComponent<SpriteRenderer>().bounds;
    }

    public void OpenDoor(Vector2Int direction)
    {
        if (direction == Vector2Int.up)
        {
            topWall.SetActive(false);
            topDoor.SetActive(true);
        }
        if (direction == Vector2Int.down)
        {
            bottomWall.SetActive(false);
            bottomDoor.SetActive(true);
        }
        if (direction == Vector2Int.left)
        {
            leftWall.SetActive(false);
            leftDoor.SetActive(true);
        }
        if (direction == Vector2Int.right)
        {
            rightWall.SetActive(false);
            rightDoor.SetActive(true);
        }
    }

    void CombineRoomMeshes()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        for (int i = 0; i < meshFilters.Length; i++)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);
        }

        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = new Mesh();
        meshFilter.mesh.CombineMeshes(combine);

        gameObject.AddComponent<MeshRenderer>();
        gameObject.SetActive(true);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(bounds.center, bounds.size);
    }

    void UpdateBounds()
    {
        Vector3 min = bounds.min;
        Vector3 max = bounds.max;
        minBounds = new Vector2(min.x, min.y);
        maxBounds = new Vector2(max.x, max.y);
    }

    public void SpawnEnemies(int level)
    {
        
        Spawner[] spawners = GetComponentsInChildren<Spawner>();

        foreach (Spawner spawner in spawners)
        {
            GameObject enemy = spawner.SpawnRandomEnemy(level);

        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasEnemiesSpawned && other.CompareTag("Player") && RoomIndex != new Vector2Int(5,5))
        {
            hasEnemiesSpawned = true;
            Debug.Log($"Spawned enemies level is  {roomManager.spawnedRooms}");
            SpawnEnemies(roomManager.spawnedRooms);
            roomManager.spawnedRooms += 1;
        }
    }


}
