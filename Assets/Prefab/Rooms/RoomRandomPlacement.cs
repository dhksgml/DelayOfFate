using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Tilemaps;
using Unity.VisualScripting;

public class RoomRandomPlacement : MonoBehaviour
{
    public Tilemap groundTilemap;
    public TileBase floorTiles;

    public int width;
    public int height;
    public int roomCount;
    public float spacing;//룸 거리 (이제 사실상 고정)

    [HideInInspector] public int[] Cost_list; //약값
    [HideInInspector] public int[] map_structure; //맵구조
    [HideInInspector] public int[] room_count; //방 곗수 (오차1)
    [HideInInspector] public int[] value_points; //바닥에 깔리는 그 가치
    [HideInInspector] public int[] value_error; //바닥에 깔리는 가치의 오차

    public GameObject[] allRoomPrefabs;
    public GameObject corridorPrefab;
    public float corridorThickness = 1f;
    private SpawnManager spawnManager;
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

    private void Awake() //배열 초기화
    {
        Cost_list = new int[] { 500, 1200, 2000, 2900, 4000 }; //약값
        map_structure = new int[] { 3, 4, 4, 5, 5 }; //맵구조
        room_count = new int[] { 8, 12, 16, 20, 24 }; //방 곗수 (오차1)
        value_points = new int[] { 350, 750, 1200, 1800, 2500 }; //바닥에 깔리는 그 가치
        value_error = new int[] { 50, 100, 200, 350, 550 }; //바닥에 깔리는 가치의 오차
    }

    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        placeManager = FindObjectOfType<PlaceManager>();
        spawnManager = GetComponent<SpawnManager>();
        Room_re_data(); // 현재 날짜에 맞게 값 재조정
        GenerateRooms();
        spawnManager.SpawnWave_ByPattern(GameManager.Instance.Day - 1); //요소들 스폰
        MovePlayerToRandomRoom(); // 추가
        FillTilemapWithFloorTiles();
    }
    void Room_re_data()
    {
        int Day = GameManager.Instance.Day - 1;
        int baseValue = map_structure[Day];
        width = baseValue;
        height = baseValue;

        if ((Day + 1) % 2 == 1)//홀수 날 일 경우 가로 세로 둘중 하나만 +1을 함
        {
            (width, height) = Random.Range(0, 2) == 0
                ? (baseValue + 1, baseValue)
                : (baseValue, baseValue + 1);
        }
        roomCount = room_count[Day];
        roomCount += Random.Range(-1, +2);
        spawnManager.totalValPoint = value_points[Day];
        int error = value_error[Day];
        spawnManager.totalValPoint += Random.Range(-error, +error+1);
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
    void FillTilemapWithFloorTiles()
    {
        if (floorTiles == null || groundTilemap == null)
        {
            Debug.LogWarning("타일맵 또는 타일 설정이 빠졌습니다.");
            return;
        }

        TileBase chosenTile = floorTiles;

        foreach (var kvp in roomObjects)
        {
            Vector3 roomWorldPos = kvp.Value.transform.position;
            Vector3Int originCell = groundTilemap.WorldToCell(roomWorldPos);

            for (int x = -12; x <= 12; x++)
            {
                for (int y = -12; y <= 12; y++)
                {
                    Vector3Int cell = originCell + new Vector3Int(x, y, 0);
                    groundTilemap.SetTile(cell, chosenTile);
                }
            }
        }

        // 복도에도 타일 깔기 (corridorPrefab에서 Instantiate된 복도들에 대해)
        foreach (Transform child in transform)
        {
            if (child.name.Contains("Corridor")) // 이름 필터 (prefab 이름 맞게 조정 가능)
            {
                Vector3 corridorWorldPos = child.position;
                Vector3Int originCell = groundTilemap.WorldToCell(corridorWorldPos);

                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        Vector3Int cell = originCell + new Vector3Int(x, y, 0);
                        groundTilemap.SetTile(cell, chosenTile);
                    }
                }
            }
        }
    }

}
