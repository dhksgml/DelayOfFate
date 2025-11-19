using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Tilemaps;
using Unity.VisualScripting;

public class RoomRandomPlacement : MonoBehaviour
{
    public Tilemap groundTilemap;
    public TileBase[] floorTiles;

    public int width;
    public int height;
    public int roomCount;
    public float spacing;//룸 거리 (이제 사실상 고정)

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
    public GameObject Place_Soul; // 혼 장소
    public GameObject Place_Coin; // 냥 장소
    public GameObject Place_Eye; // 눈 장소
    public PlaceManager placeManager;

    public List<Vector2Int> roomPositions = new();
    // 수정을 위해 비활성화
    public Dictionary<Vector2Int, string> roomDirections = new();
    //private Dictionary<Vector2Int, GameObject> roomObjects = new();

    public Dictionary<Vector2Int, GameObject> roomObjects = new Dictionary<Vector2Int, GameObject>();

    // 추가됨: 방 좌표 -> 사용된 프리팹 원본
    public Dictionary<Vector2Int, GameObject> roomPrefabsUsed = new Dictionary<Vector2Int, GameObject>();

    // 복도값을 주기 위함
    public List<GameObject> tossCors = new List<GameObject>();

    // 사신 소환을 위함
    public List<Vector3> randomPlace;

    public List<GameObject> randomPlaceObj;

    private void Awake() //배열 초기화
    {
        map_structure = new int[] { 3, 4, 4, 5, 5, 5, 5 }; //맵구조
        room_count = new int[] { 7, 10, 13, 16, 19, 21, 24 }; //방 곗수 (오차1)
        value_points = new int[] { 500, 700, 850, 1000, 1150, 1300, 1500 }; //바닥에 깔리는 그 가치
        value_error = new int[] { 50, 75, 100, 125, 150, 175, 200 }; //바닥에 깔리는 가치의 오차
    }

    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        placeManager = FindObjectOfType<PlaceManager>();
        spawnManager = GetComponent<SpawnManager>();
        
        Room_re_data(); // 현재 날짜에 맞게 값 재조정
        GenerateRooms();
        FillTilemapWithFloorTiles(); //타일맵 먼저 깔기
        MovePlayerToRandomRoom(); // 추가 플레이어 스폰 후
        spawnManager.SpawnWave_ByPattern(GameManager.Instance.Day - 1); //요소들 스폰 적, 아이템 스폰
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

        int dayIndex = GameManager.Instance.Day - 1;

        // 일차별 장소 개수 계산 (1,2일차: 1개 / 3,4일차: 2개 / 5,6일차: 3개 / 7일차: 4개)
        int placeCountPerType = (dayIndex / 2) + 1;

        // roomObjects.Values를 리스트로 가져와 섞는다
        var shuffledRooms = roomObjects.Values.OrderBy(x => Random.value).ToList();

        // 플레이어 위치 (첫 번째 방)
        player.transform.position = shuffledRooms[0].transform.position;

        // 모든 EnemyPoint 찾기 (플레이어 주변 정리)
        GameObject[] enemyPoints = GameObject.FindGameObjectsWithTag("EnemyPoint");

        foreach (GameObject point in enemyPoints)
        {
            float distance = Vector2.Distance(player.transform.position, point.transform.position);

            if (distance <= 32f)
            {
                point.tag = "Untagged";
                Destroy(point);
            }
        }

        int roomIndex = 1;

        // ========== 고정 장소 배치 (탈출, 판매) ==========
        // 탈출 장소 (고정)
        for (int i = 0; i < placeCountPerType && roomIndex < shuffledRooms.Count; i++)
        {
            GameObject obj = Instantiate(Place_Escape, shuffledRooms[roomIndex].transform.position, Quaternion.identity);
            placeManager.escape_positions.Add(shuffledRooms[roomIndex].transform.position);
            randomPlace.Add(obj.transform.position);
            randomPlaceObj.Add(obj);
            roomIndex++;
        }

        // 판매 장소 (고정)
        for (int i = 0; i < placeCountPerType && roomIndex < shuffledRooms.Count; i++)
        {
            GameObject obj = Instantiate(Place_Sale, shuffledRooms[roomIndex].transform.position, Quaternion.identity);
            placeManager.sale_positions.Add(shuffledRooms[roomIndex].transform.position);
            randomPlace.Add(obj.transform.position);
            randomPlaceObj.Add(obj);
            roomIndex++;
        }

        // ========== 랜덤 장소 배치 (부활, 혼, 냥, 눈) ==========
        // 랜덤 장소 풀 생성 (중복 가능)
        List<System.Action<int>> randomPlaceFunctions = new List<System.Action<int>>
        {
            // 부활 장소 임시제거
            /*(idx) => {
                GameObject obj = Instantiate(Place_Resurrection, shuffledRooms[idx].transform.position, Quaternion.identity);
                placeManager.resurrection_positions.Add(shuffledRooms[idx].transform.position);
                randomPlace.Add(obj.transform.position);
                randomPlaceObj.Add(obj);
            },*/
            // 혼 장소
            (idx) => {
                GameObject obj = Instantiate(Place_Soul, shuffledRooms[idx].transform.position, Quaternion.identity);
                placeManager.soul_positions.Add(shuffledRooms[idx].transform.position);
                randomPlace.Add(obj.transform.position);
                randomPlaceObj.Add(obj);
            },
            // 냥 장소
            (idx) => {
                GameObject obj = Instantiate(Place_Coin, shuffledRooms[idx].transform.position, Quaternion.identity);
                placeManager.coin_positions.Add(shuffledRooms[idx].transform.position);
                randomPlace.Add(obj.transform.position);
                randomPlaceObj.Add(obj);
            },
            // 눈 장소
            (idx) => {
                GameObject obj = Instantiate(Place_Eye, shuffledRooms[idx].transform.position, Quaternion.identity);
                placeManager.eye_positions.Add(shuffledRooms[idx].transform.position);
                randomPlace.Add(obj.transform.position);
                randomPlaceObj.Add(obj);
            }
        };

        // 랜덤 장소 배치 (placeCountPerType만큼, 중복 가능)
        for (int i = 0; i < placeCountPerType && roomIndex < shuffledRooms.Count; i++)
        {
            // 랜덤으로 장소 타입 선택 (중복 가능)
            int randomPlaceType = Random.Range(0, randomPlaceFunctions.Count);
            randomPlaceFunctions[randomPlaceType](roomIndex);
            roomIndex++;
        }
    }

    // 방 생성
    void TryRandomRoomPositions()
    {
        // 방 생성
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

    // 수정하느라 잠굼
    //void PlaceRooms()
    //{
    //    foreach (Vector2Int pos in roomPositions)
    //    {
    //        string exits = roomDirections[pos];
    //        GameObject prefab = GetPrefabByExits(exits);
    //        if (prefab != null)
    //        {
    //            Vector3 worldPos = new(pos.x * spacing, pos.y * spacing, 0);
    //            GameObject room = Instantiate(prefab, worldPos, Quaternion.identity, transform);
    //            roomObjects[pos] = room;
    //        }
    //    }
    //}

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

                // 추가: prefab 원본 기록
                roomPrefabsUsed[pos] = prefab;
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

    public Vector2Int GetNeighbor(Vector2Int pos, char dir)
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

    // 방을 연결
    //void ConnectRoomsWithDoubleCorridor(GameObject roomA, GameObject roomB, string direction)
    //{
    //    string oppDirection = GetOppositeDirection(direction);
    //    string[] postfixes = { "1", "2" };

    //    foreach (string p in postfixes)
    //    {
    //        string exitNameA = $"Exit_{direction}_{p}";
    //        string exitNameB = $"Exit_{oppDirection}_{p}";

    //        Transform exitA = roomA.transform.Find(exitNameA);
    //        Transform exitB = roomB.transform.Find(exitNameB);

    //        if (exitA == null || exitB == null)
    //        {
    //            Debug.LogWarning($"Missing exits: {exitNameA} or {exitNameB}");
    //            continue;
    //        }

    //        Vector3 dirVec = (exitB.position - exitA.position).normalized;
    //        float length = Vector3.Distance(exitA.position, exitB.position);
    //        Vector3 mid = (exitA.position + exitB.position) / 2f;

    //        GameObject corridor = Instantiate(corridorPrefab, mid, Quaternion.identity, transform);
    //        corridor.transform.right = dirVec;
    //        corridor.transform.localScale = new Vector3(length, corridorThickness, corridorThickness);
    //    }
    //}

    // RoomRandomPlacement에 추가
    public Dictionary<(Vector2Int, Vector2Int), List<GameObject>> corridorDict = new();

    // 방을 연결
    //void ConnectRoomsWithDoubleCorridor(GameObject roomA, GameObject roomB, string direction)
    //{
    //    string oppDirection = GetOppositeDirection(direction);
    //    string[] postfixes = { "1", "2" };

    //    foreach (string p in postfixes)
    //    {
    //        string exitNameA = $"Exit_{direction}_{p}";
    //        string exitNameB = $"Exit_{oppDirection}_{p}";

    //        Transform exitA = roomA.transform.Find(exitNameA);
    //        Transform exitB = roomB.transform.Find(exitNameB);

    //        if (exitA == null || exitB == null)
    //        {
    //            Debug.LogWarning($"Missing exits: {exitNameA} or {exitNameB}");
    //            continue;
    //        }

    //        Vector3 dirVec = (exitB.position - exitA.position).normalized;
    //        float length = Vector3.Distance(exitA.position, exitB.position);
    //        Vector3 mid = (exitA.position + exitB.position) / 2f;

    //        GameObject corridor = Instantiate(corridorPrefab, mid, Quaternion.identity, transform);
    //        corridor.transform.right = dirVec;
    //        corridor.transform.localScale = new Vector3(length, corridorThickness, corridorThickness);

    //        // 리스트에 추가
    //        tossCors.Add(corridor);
    //    }
    //}


    void ConnectRoomsWithDoubleCorridor(GameObject roomA, GameObject roomB, string direction)
    {
        string oppDirection = GetOppositeDirection(direction);
        string[] postfixes = { "1", "2" };

        foreach (string p in postfixes)
        {
            Transform exitA = roomA.transform.Find($"Exit_{direction}_{p}");
            Transform exitB = roomB.transform.Find($"Exit_{oppDirection}_{p}");
            if (exitA == null || exitB == null) continue;

            Vector3 dirVec = (exitB.position - exitA.position).normalized;
            float length = Vector3.Distance(exitA.position, exitB.position);
            Vector3 mid = (exitA.position + exitB.position) / 2f;

            GameObject corridor = Instantiate(corridorPrefab, mid, Quaternion.identity, transform);
            corridor.transform.right = dirVec;
            corridor.transform.localScale = new Vector3(length, corridorThickness, corridorThickness);

            tossCors.Add(corridor);

            // 좌표 기반으로 딕셔너리에 저장
            Vector2Int startPos = new Vector2Int(Mathf.RoundToInt(exitA.position.x / spacing), Mathf.RoundToInt(exitA.position.y / spacing));
            Vector2Int endPos = new Vector2Int(Mathf.RoundToInt(exitB.position.x / spacing), Mathf.RoundToInt(exitB.position.y / spacing));

            if (!corridorDict.ContainsKey((startPos, endPos)))
                corridorDict[(startPos, endPos)] = new List<GameObject>();

            corridorDict[(startPos, endPos)].Add(corridor);
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

        int[,] dayToIndex = new int[,]//각 일차별 맞는 타일들 2일차 마다 타일이 변경되야함 (원래는 장소를 선택하는 느낌을 주고 싶긴한데... 일단 이렇게)
        {
            {1, 0},
            {2, 0},
            {3, 1},
            {4, 1},
            {5, 2},
            {6, 2},
            {7, 3}
        };//(지금은 1만 있는데 2,3 도 있어야맞음)

        int day = GameManager.Instance.Day;
        int index = 0;

        for (int i = 0; i < dayToIndex.GetLength(0); i++)
        {
            if (dayToIndex[i, 0] == day)
            {
                index = dayToIndex[i, 1];
                break;
            }
        }

        TileBase chosenTile = floorTiles[index]; //타일 설정 완료


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

