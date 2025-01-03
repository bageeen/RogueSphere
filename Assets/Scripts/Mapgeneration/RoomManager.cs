using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;


public class RoomManager : MonoBehaviour
{
    [SerializeField] GameObject[] roomT1Prefab;
    [SerializeField] GameObject[] roomT2Prefab;
    [SerializeField] GameObject[] spawnRoomPrefab;

    private PlayerMovementControls player;

    public int spawnedRooms = 0;

    [SerializeField] private int maxRooms = 15;
    [SerializeField] private int minRooms = 10;
    [SerializeField] private int maxRoomTier = 1;

    private Transform transform;
    int roomWidth = 152;
    int roomHeight = 75;

    int gridSizeX = 11;
    int gridSizeY = 11;

    private List<GameObject> roomObjects = new List<GameObject>();

    private Queue<Vector2Int> roomQueue = new Queue<Vector2Int>();

    private int[,] roomGrid;

    private int roomCount;

    private bool generationComplete = false;

    private void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerMovementControls>();
        roomGrid = new int[gridSizeX, gridSizeY];
        roomQueue = new Queue<Vector2Int>();
        transform = GetComponent<Transform>();
        Vector2Int initialRoomIndex = new Vector2Int(gridSizeX/2, gridSizeY/2);
        StartRoomGenerationFromRoom(initialRoomIndex);
        }


    private void Update()
    {
        if (roomQueue.Count > 0 && roomCount < maxRooms && !generationComplete)
        {
            Vector2Int roomIndex = roomQueue.Dequeue();
            int gridX = roomIndex.x;
            int gridY = roomIndex.y;

            TryGenerateRooms(gridX, gridY);

        }
        else if (roomCount < minRooms)
        {
            Debug.Log("RoomCount was less than the minimum amount of rooms. Trying again.");
                RegenerateRooms();
        } 
        else if (!generationComplete)
        {
            Debug.Log($"Generation complete, {roomCount} rooms created");
            player.InitialRoom();
            AstarPath.active.Scan();
            //SpawnEnemies();
            generationComplete = true;

        }

        

    }


    private void StartRoomGenerationFromRoom(Vector2Int roomIndex)
    {
        roomQueue.Enqueue(roomIndex);
        int x = roomIndex.x;
        int y = roomIndex.y;
        roomGrid[x, y] = 1;
        roomCount++;
        var initialRoom = Instantiate(spawnRoomPrefab[Random.Range(0, spawnRoomPrefab.Length)], GetPositionFromGridIndex(roomIndex), Quaternion.identity, transform);
        initialRoom.name = $"Room-{roomCount}";
        initialRoom.GetComponent<Room>().RoomIndex = roomIndex;
        roomObjects.Add(initialRoom);

    }

    private void TryGenerateRooms(int gridX, int gridY)
    {
        if (maxRoomTier == 1)
        {
            TryGenerateRoomT1(new Vector2Int(gridX - 1, gridY));
            TryGenerateRoomT1(new Vector2Int(gridX + 1, gridY));
            TryGenerateRoomT1(new Vector2Int(gridX, gridY - 1));
            TryGenerateRoomT1(new Vector2Int(gridX, gridY + 1));
        }
        else if (maxRoomTier == 2)
        {
            if(Random.value < 0.1)
            {
                TryGenerateRoomT2(new Vector2Int(gridX - 2, gridY));
            }
            else
            {
                TryGenerateRoomT1(new Vector2Int(gridX - 1, gridY));
            }
            if (Random.value < 0.1)
            {
                TryGenerateRoomT2(new Vector2Int(gridX + 2, gridY));
            }
            else
            {
                TryGenerateRoomT1(new Vector2Int(gridX + 1, gridY));
            }
            if (Random.value < 0.1)
            {
                TryGenerateRoomT2(new Vector2Int(gridX, gridY - 2));
            }
            else
            {
                TryGenerateRoomT1(new Vector2Int(gridX, gridY - 1));
            }
            if (Random.value < 0.1)
            {
                TryGenerateRoomT2(new Vector2Int(gridX, gridY + 2));
            }
            else
            {
                TryGenerateRoomT1(new Vector2Int(gridX, gridY + 1));
            }
        }
    }

    private bool TryGenerateRoomT1(Vector2Int roomIndex)
    {
        int x = roomIndex.x;
        int y = roomIndex.y;

        if (x >= gridSizeX || y >= gridSizeY || x < 0 || y < 0)
        {
            return false;
        }
        if (roomGrid[x, y] != 0)
        {
            return false;
        }
            
        if (roomCount >= maxRooms)
        {
            return false;
        }

        if(Random.value < 0.5f && roomIndex != Vector2Int.zero)
        {
            return false;
        }

        if(CountAdjacentRooms(roomIndex) > 2)
        {
            return false;
        }


        roomQueue.Enqueue(roomIndex);
        roomGrid[x, y] = 1;
        roomCount++;

        var newRoom = Instantiate(roomT1Prefab[Random.Range(0, roomT1Prefab.Length)], GetPositionFromGridIndex(roomIndex), Quaternion.identity, transform);
        newRoom.GetComponent<Room>().RoomIndex = roomIndex;
        newRoom.name = $"Room-{roomCount}";
        roomObjects.Add(newRoom);

        OpenDoors(newRoom, x, y, 1);

        return true;
    }
    private bool TryGenerateRoomT2(Vector2Int roomIndex)
    {
        int x = roomIndex.x;
        int y = roomIndex.y;

        if (x + 1 >= gridSizeX || y + 1 >= gridSizeY || x - 1 < 0 || y - 1 < 0)
        {
            return false;
        }
        if (roomGrid[x-1, y+1] != 0 || roomGrid[x, y+1] != 0 || roomGrid[x+1, y+1] != 0 ||
            roomGrid[x-1, y  ] != 0 || roomGrid[x, y  ] != 0 || roomGrid[x+1, y  ] != 0 ||
            roomGrid[x-1, y-1] != 0 || roomGrid[x, y-1] != 0 || roomGrid[x+1, y-1] != 0)
        {
            return false;
        }

        if (roomCount >= maxRooms)
        {
            return false;
        }

        if (Random.value < 0.5f && roomIndex != Vector2Int.zero)
        {
            return false;
        }

        if (CountAdjacentRooms(roomIndex) > 2)
        {
            return false;
        }


        roomQueue.Enqueue(roomIndex);
        roomGrid[x-1, y+1] = 2; roomGrid[x, y+1] = 2; roomGrid[x+1, y+1] = 2;
        roomGrid[x-1, y] = 2; roomGrid[x, y] = 2; roomGrid[x+1, y] = 2;
        roomGrid[x-1, y-1] = 2; roomGrid[x, y-1] = 2; roomGrid[x+1, y-1] = 2;
        roomCount++;

        var newRoom = Instantiate(roomT2Prefab[Random.Range(0,roomT2Prefab.Length)], GetPositionFromGridIndex(roomIndex), Quaternion.identity, transform);
        newRoom.GetComponent<Room>().RoomIndex = roomIndex;
        newRoom.name = $"Room-{roomCount}";
        roomObjects.Add(newRoom);

        OpenDoors(newRoom, x, y, 2);

        return true;
    }

    //Clear all rooms and try again
    private void RegenerateRooms()
    {
        roomObjects.ForEach(Destroy);
        roomObjects.Clear();
        roomGrid = new int[gridSizeX, gridSizeY];
        roomQueue.Clear();
        roomCount = 0;
        generationComplete = false;

        Vector2Int initialRoomIndex = new Vector2Int(gridSizeX / 2, gridSizeY / 2);
        StartRoomGenerationFromRoom(initialRoomIndex);
    }

    void OpenDoors(GameObject room, int x, int y, int tier)
    {
        Room newRoomScript = room.GetComponent<Room>();

        //Neighbour
        Room leftRoomScript = GetRoomScriptAt(new Vector2Int(x - tier, y));
        Room rightRoomScript = GetRoomScriptAt(new Vector2Int(x + tier, y));
        Room topRoomScript = GetRoomScriptAt(new Vector2Int(x, y + tier));
        Room bottomRoomScript = GetRoomScriptAt(new Vector2Int(x, y - tier));

        //Determine which doors to open based on the direction

        if(x - tier + 1 > 0 && roomGrid[x-tier,y] == 1)
        {
            // Neighbouring room to the left
            newRoomScript.OpenDoor(Vector2Int.left);
            leftRoomScript.OpenDoor(Vector2Int.right);
        }
        if (x + tier - 1 < gridSizeX -1 && roomGrid[x+tier,y] == 1)
        {
            // Neighbouring room to the right
            newRoomScript.OpenDoor(Vector2Int.right);
            rightRoomScript.OpenDoor(Vector2Int.left);
        }
        if (y - tier + 1 > 0 && roomGrid[x,y-tier] == 1)
        {
            //Neighbouring room below
            newRoomScript.OpenDoor(Vector2Int.down);
            bottomRoomScript.OpenDoor(Vector2Int.up);
        }
        if (y + tier - 1 < gridSizeY - 1 && roomGrid[x,y+tier] == 1)
        {
            // Neighbouring room above
            newRoomScript.OpenDoor(Vector2Int.up);
            topRoomScript.OpenDoor(Vector2Int.down);
        }

    }

    Room GetRoomScriptAt(Vector2Int index)
    {
        GameObject roomObject = roomObjects.Find(r => r.GetComponent<Room>().RoomIndex == index);
        if (roomObject != null)
        {
            return roomObject.GetComponent<Room>();
        }
        return null;
    }

    private int CountAdjacentRooms(Vector2Int roomIndex)
    {
        int x = roomIndex.x;
        int y = roomIndex.y;

        int count = 0;

        if (x > 0 && roomGrid[x - 1, y] != 0)
        { count++; } // Left neighbour
        if (x < gridSizeX - 1 && roomGrid[x + 1, y] != 0)
        { count++; } // Right neighbour
        if (y > 0 && roomGrid[x, y - 1] != 0)
        { count++; } // Bottom neigbhour
        if (y < gridSizeY && roomGrid[x, y+1] != 0)
        { count++; } // Top neighbour

        return count;
    }


    private Vector3 GetPositionFromGridIndex(Vector2Int gridIndex)
    {
        int gridX = gridIndex.x;
        int gridY = gridIndex.y;
        return new Vector3(roomWidth * (gridX - gridSizeX / 2),
                              roomHeight * (gridY - gridSizeY / 2));
    }

    private void OnDrawGizmos()
    {
        Color gizmoColor = new Color(0, 1, 1, 0.05f);
        Gizmos.color = gizmoColor;

        for (int x = 0; x < gridSizeX; x++)
        {
            for(int y = 0; y < gridSizeY; y++)
            {
                Vector3 position = GetPositionFromGridIndex(new Vector2Int(x, y));
                Gizmos.DrawWireCube(position, new Vector3(roomWidth, roomHeight, 1));
            }
        }
    }


    public Bounds GetInitialBounds()
    {
        return roomObjects[0].GetComponent<Room>().GetRoomBounds();
    }

    public void SpawnEnemies()
    {
        foreach (GameObject room in roomObjects)
        {
            room.GetComponent<Room>().SpawnEnemies(1);
        }
    }
}
