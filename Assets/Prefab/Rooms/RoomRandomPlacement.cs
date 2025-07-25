using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

public class RoomRandomPlacement : MonoBehaviour
{
    public int width;
    public int height;
    public int roomCount;
    public float spacing;

    public GameObject[] allRoomPrefabs;
    public GameObject corridorPrefab;
    public float corridorThickness = 1f;
    private SpawnManager Manager_Spawn;
    private PlayerController player;
    public GameObject Place_Resurrection; // 부활 장소
    public GameObject Place_Sale; // 판매 장소
    public GameObject Place_Escape; // 탈출 장소
    private PlaceManager placeManager;

    private List<Vector2Int> roomPositions = new();
    private Dictionary<Vector2Int, string> roomDirections = new();
    private Dictionary<Vector2Int, GameObject> roomObjects = new();

    // 사신 소환을 위함
    public List<Vector3> randomPlace;

    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        placeManager = FindObjectOfType<PlaceManager>();
        Manager_Spawn = GetComponent<SpawnManager>();
        GenerateRooms();
        Manager_Spawn.SpawnWave(Random.Range(0, Manager_Spawn.waveList.Length-1));
        MovePlayerToRandomRoom(); // 추가
    }

    void GenerateRooms()
    {
        while (true)
        {
            TryRandomRoomPositions();
            FilterLargestConnectedComponent();

            while (roomPositions.Count < roomCount)
            {
                TryExpandConnectedComponent();
            }

            if (roomPositions.Count == roomCount)
                break;
        }

        GenerateRoomData();
        PlaceRooms();
        ConnectCorridors();
    }
    void MovePlayerToRandomRoom()
    {
        if (roomObjects.Count < 4 || player == null)
        {
            Debug.LogWarning("Not enough rooms or player not assigned.");
            return;
        }

        // roomObjects.Values를 리스트로 가져와 섞는다
        var shuffledRooms = roomObjects.Values.OrderBy(x => Random.value).ToList();

        // 겹치지 않는 4개의 방을 선택
        Transform room1 = shuffledRooms[0].transform;
        Transform room2 = shuffledRooms[1].transform;
        Transform room3 = shuffledRooms[2].transform;
        Transform room4 = shuffledRooms[3].transform;

        // 플레이어 위치
        player.transform.position = room1.position;

        // 오브젝트 배치
       GameObject roomPlace1 = Instantiate(Place_Resurrection, room2.position, Quaternion.identity); placeManager.resurrection_pos = room2.position;
       GameObject roomPlace2 = Instantiate(Place_Sale, room3.position, Quaternion.identity); placeManager.sale_pos = room3.position;
       GameObject roomPlace3 = Instantiate(Place_Escape, room4.position, Quaternion.identity); placeManager.escape_pos = room4.position;

        // 위치 추출
        randomPlace.Add(roomPlace1.transform.position);
        randomPlace.Add(roomPlace2.transform.position);
        randomPlace.Add(roomPlace3.transform.position);
    }


    void TryRandomRoomPositions()
    {
        roomPositions.Clear();
        while (roomPositions.Count < roomCount)
        {
            int x = Random.Range(0, width);
            int y = Random.Range(0, height);
            Vector2Int pos = new(x, y);
            if (!roomPositions.Contains(pos))
                roomPositions.Add(pos);
        }
    }

    void FilterLargestConnectedComponent()
    {
        HashSet<Vector2Int> allRooms = new(roomPositions);
        List<List<Vector2Int>> components = new();

        while (allRooms.Count > 0)
        {
            Vector2Int seed = allRooms.First();
            List<Vector2Int> group = GetConnectedComponent(seed, allRooms);
            components.Add(group);
            foreach (var pos in group) allRooms.Remove(pos);
        }

        List<Vector2Int> largest = components.OrderByDescending(g => g.Count).First();
        roomPositions = largest;
    }

    List<Vector2Int> GetConnectedComponent(Vector2Int start, HashSet<Vector2Int> allRooms)
    {
        List<Vector2Int> connected = new();
        Stack<Vector2Int> stack = new();
        HashSet<Vector2Int> visited = new();

        stack.Push(start);
        visited.Add(start);

        while (stack.Count > 0)
        {
            Vector2Int current = stack.Pop();
            connected.Add(current);

            foreach (Vector2Int dir in new Vector2Int[] {
                new(0, 1), new(0, -1), new(-1, 0), new(1, 0)
            })
            {
                Vector2Int neighbor = current + dir;
                if (allRooms.Contains(neighbor) && !visited.Contains(neighbor))
                {
                    stack.Push(neighbor);
                    visited.Add(neighbor);
                }
            }
        }

        return connected;
    }

    void TryExpandConnectedComponent()
    {
        List<Vector2Int> candidates = new();

        foreach (Vector2Int pos in roomPositions)
        {
            foreach (Vector2Int dir in new Vector2Int[] {
                new(0, 1), new(0, -1), new(-1, 0), new(1, 0)
            })
            {
                Vector2Int neighbor = pos + dir;
                if (IsInBounds(neighbor) && !roomPositions.Contains(neighbor) && !candidates.Contains(neighbor))
                {
                    candidates.Add(neighbor);
                }
            }
        }

        if (candidates.Count > 0)
        {
            Vector2Int newPos = candidates[Random.Range(0, candidates.Count)];
            roomPositions.Add(newPos);
        }
    }

    bool IsInBounds(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;
    }

    void GenerateRoomData()
    {
        roomDirections.Clear();
        roomObjects.Clear();

        foreach (Vector2Int pos in roomPositions)
        {
            string directions = "";

            if (roomPositions.Contains(new Vector2Int(pos.x, pos.y + 1))) directions += "U";
            if (roomPositions.Contains(new Vector2Int(pos.x, pos.y - 1))) directions += "D";
            if (roomPositions.Contains(new Vector2Int(pos.x - 1, pos.y))) directions += "L";
            if (roomPositions.Contains(new Vector2Int(pos.x + 1, pos.y))) directions += "R";

            roomDirections[pos] = directions;
        }
    }

    void PlaceRooms()
    {
        foreach (Vector2Int pos in roomPositions)
        {
            string exits = roomDirections[pos];
            GameObject prefab = GetPrefabByExits(exits);
            if (prefab != null)
            {
                Vector3 worldPos = new(pos.x * spacing, pos.y * spacing, 0);
                GameObject room = Instantiate(prefab, worldPos, Quaternion.identity, transform);
                roomObjects[pos] = room;
            }
        }
    }

    void ConnectCorridors()
    {
        foreach (Vector2Int pos in roomPositions)
        {
            GameObject roomA = roomObjects[pos];
            string exits = roomDirections[pos];

            foreach (char dir in exits)
            {
                Vector2Int neighborPos = GetNeighbor(pos, dir);
                if (!roomPositions.Contains(neighborPos)) continue;

                if (pos.y > neighborPos.y || (pos.y == neighborPos.y && pos.x > neighborPos.x)) continue;

                GameObject roomB = roomObjects[neighborPos];
                ConnectRoomsWithDoubleCorridor(roomA, roomB, dir.ToString());
            }
        }
    }

    GameObject GetPrefabByExits(string exits)
    {
        char[] exitsChars = exits.ToCharArray();
        System.Array.Sort(exitsChars);
        string sortedExits = new(exitsChars);

        List<GameObject> candidates = new();

        foreach (GameObject prefab in allRoomPrefabs)
        {
            string[] split = prefab.name.Split('_');
            if (split.Length < 2) continue;

            string prefabExits = split[1].ToUpper();
            char[] prefabExitsChars = prefabExits.ToCharArray();
            System.Array.Sort(prefabExitsChars);
            string sortedPrefabExits = new(prefabExitsChars);

            if (sortedPrefabExits == sortedExits)
            {
                candidates.Add(prefab);
            }
        }

        if (candidates.Count == 0)
        {
            Debug.LogWarning("No prefab for exits: " + sortedExits);
            return null;
        }

        return candidates[Random.Range(0, candidates.Count)];
    }

    Vector2Int GetNeighbor(Vector2Int pos, char dir)
    {
        return dir switch
        {
            'U' => new Vector2Int(pos.x, pos.y + 1),
            'D' => new Vector2Int(pos.x, pos.y - 1),
            'L' => new Vector2Int(pos.x - 1, pos.y),
            'R' => new Vector2Int(pos.x + 1, pos.y),
            _ => pos,
        };
    }

    void ConnectRoomsWithDoubleCorridor(GameObject roomA, GameObject roomB, string direction)
    {
        string oppDirection = GetOppositeDirection(direction);
        string[] postfixes = { "1", "2" };

        foreach (string p in postfixes)
        {
            string exitNameA = $"Exit_{direction}_{p}";
            string exitNameB = $"Exit_{oppDirection}_{p}";

            Transform exitA = roomA.transform.Find(exitNameA);
            Transform exitB = roomB.transform.Find(exitNameB);

            if (exitA == null || exitB == null)
            {
                Debug.LogWarning($"Missing exits: {exitNameA} or {exitNameB}");
                continue;
            }

            Vector3 dirVec = (exitB.position - exitA.position).normalized;
            float length = Vector3.Distance(exitA.position, exitB.position);
            Vector3 mid = (exitA.position + exitB.position) / 2f;

            GameObject corridor = Instantiate(corridorPrefab, mid, Quaternion.identity, transform);
            corridor.transform.right = dirVec;
            corridor.transform.localScale = new Vector3(length, corridorThickness, corridorThickness);
        }
    }

    string GetOppositeDirection(string dir)
    {
        return dir switch
        {
            "L" => "R",
            "R" => "L",
            "U" => "D",
            "D" => "U",
            _ => dir,
        };
    }
}
