using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSpawn : MonoBehaviour
{
    [SerializeField] RoomRandomPlacement roomGenerator; // RoomRandomPlacement 참조
    [SerializeField] Vector3 baseOffset = new Vector3(0, -2000, 0);

    [SerializeField] GameObject verticalCorridorPrefab;    // 위/아래 연결
    [SerializeField] GameObject horizontalCorridorPrefab;  // 좌/우 연결

    // 장소
    [SerializeField] GameObject miniPlace_Resurrection;
    [SerializeField] GameObject miniPlace_Sale;
    [SerializeField] GameObject miniPlace_Escape;
    [SerializeField] GameObject miniPlace_Soul; // 혼 장소
    [SerializeField] GameObject miniPlace_Coin; // 냥 장소
    [SerializeField] GameObject miniPlace_Eye; // 눈 장소

    [SerializeField] Camera minimapCam;

    // 맵을 가리는걸 설정하는 스크립트
    [SerializeField] Minimap_Blind blindScript;

    // 맵 저장용
    [SerializeField] GameObject mapSave;

    // 카메라 최대 사이즈
    [SerializeField] int cameraMaxSize;
    // 카메라 기본 사이즈
    [SerializeField] int cameraBaseSize;

    [SerializeField] List<Minimap_Blind> blindList = new ();

    IEnumerator Start()
    {
        // 방이 생성될 때까지 대기
        yield return new WaitUntil(() => roomGenerator != null && roomGenerator.roomObjects.Count > 0);

        // 1. 방 복제
        foreach (var kvp in roomGenerator.roomObjects)
        {
            Vector2Int gridPos = kvp.Key;
            GameObject originalRoom = kvp.Value;
            Vector3 spawnPos = new Vector3(gridPos.x * roomGenerator.spacing, gridPos.y * roomGenerator.spacing, 0) + baseOffset;
            mapSave = Instantiate(originalRoom, spawnPos, Quaternion.identity, transform);


            // 컴포넌트 추출
            Minimap_Blind blind = mapSave.GetComponent<Minimap_Blind>();
            Minimap_Blind originBlind = originalRoom.GetComponent<Minimap_Blind>();

            originBlind.copyRoomObj = mapSave;
            originBlind.dontSeeRoom = mapSave;

            originBlind.CopyRoomBlindGet();

            blindList.Add(originBlind);

        }
        #region 구 코드
        // 2. 복도 생성
        //foreach (var kvp in roomGenerator.roomObjects)
        //{
        //    Vector2Int pos = kvp.Key;
        //    string exits = roomGenerator.roomDirections[pos];

        //    foreach (char dir in exits)
        //    {
        //        Vector2Int neighborPos = roomGenerator.GetNeighbor(pos, dir);

        //        // 이미 없는 방이면 무시
        //        if (!roomGenerator.roomObjects.ContainsKey(neighborPos)) continue;

        //        // 중복 생성 방지: 항상 작은 좌표 기준으로 생성
        //        if (pos.y > neighborPos.y || (pos.y == neighborPos.y && pos.x > neighborPos.x)) continue;

        //        Vector3 worldPos = (new Vector3(pos.x, pos.y, 0) + new Vector3(neighborPos.x, neighborPos.y, 0)) / 2f;
        //        worldPos = worldPos * roomGenerator.spacing + baseOffset;

        //        GameObject corridorPrefabToUse = (dir == 'U' || dir == 'D')
        //            ? verticalCorridorPrefab
        //            : horizontalCorridorPrefab;

        //        Instantiate(corridorPrefabToUse, worldPos, Quaternion.identity, transform);
        //    }
        //}

        //foreach (var kvp in roomGenerator.roomObjects)
        //{
        //    Vector2Int pos = kvp.Key;
        //    // 복도에 값을 전달해주기 위함
        //    GameObject originCor = roomGenerator.tossCor;
        //    Minimap_Blind corBlind = originCor.GetComponent<Minimap_Blind>();
        //    Debug.Log(" 복사할 복도 : " + originCor);

        //    string exits = roomGenerator.roomDirections[pos];


        //    foreach (char dir in exits)
        //    {
        //        Vector2Int neighborPos = roomGenerator.GetNeighbor(pos, dir);

        //        if (!roomGenerator.roomObjects.ContainsKey(neighborPos)) continue;
        //        if (pos.y > neighborPos.y || (pos.y == neighborPos.y && pos.x > neighborPos.x)) continue;

        //        Vector3 worldPos = (new Vector3(pos.x, pos.y, 0) + new Vector3(neighborPos.x, neighborPos.y, 0)) / 2f;
        //        worldPos = worldPos * roomGenerator.spacing + baseOffset;

        //        GameObject corridorPrefabToUse = (dir == 'U' || dir == 'D')
        //            ? verticalCorridorPrefab
        //            : horizontalCorridorPrefab;



        //        // 복제
        //        GameObject mapCorridor = Instantiate(corridorPrefabToUse, worldPos, Quaternion.identity, transform);

        //        corBlind.copyRoomObj = mapCorridor;

        //        corBlind.CopyRoomBlindGet();
        //        Debug.Log(" 복사된 복도 : " + corBlind.copyRoomObj);
        //    }
        //}
        #endregion

        foreach (var kvp in roomGenerator.roomObjects)
        {
            Vector2Int pos = kvp.Key;
            string exits = roomGenerator.roomDirections[pos];

            foreach (char dir in exits)
            {
                Vector2Int neighborPos = roomGenerator.GetNeighbor(pos, dir);
                if (!roomGenerator.roomObjects.ContainsKey(neighborPos)) continue;
                if (pos.y > neighborPos.y || (pos.y == neighborPos.y && pos.x > neighborPos.x)) continue;

                Vector3 worldPos = (new Vector3(pos.x, pos.y, 0) + new Vector3(neighborPos.x, neighborPos.y, 0)) / 2f;
                worldPos = worldPos * roomGenerator.spacing + baseOffset;

                GameObject corridorPrefabToUse = (dir == 'U' || dir == 'D')
                    ? verticalCorridorPrefab
                    : horizontalCorridorPrefab;

                // 미니맵용 복도 생성
                GameObject mapCorridor = Instantiate(corridorPrefabToUse, worldPos, Quaternion.identity, transform);

                // 원본 복도 찾기: 좌표 기준
                if (roomGenerator.corridorDict.TryGetValue((pos, neighborPos), out List<GameObject> originCors))
                {
                    foreach (var originCor in originCors)
                    {
                        Minimap_Blind corBlind = originCor.GetComponent<Minimap_Blind>();
                        corBlind.copyRoomObj = mapCorridor;
                        corBlind.dontSeeRoom = mapCorridor;
                        corBlind.CopyRoomBlindGet();
                        blindList.Add(corBlind);
                    }
                }
                else if (roomGenerator.corridorDict.TryGetValue((neighborPos, pos), out originCors))
                {
                    foreach (var originCor in originCors)
                    {
                        Minimap_Blind corBlind = originCor.GetComponent<Minimap_Blind>();
                        corBlind.copyRoomObj = mapCorridor;
                        corBlind.dontSeeRoom = mapCorridor;
                        corBlind.CopyRoomBlindGet();
                        blindList.Add(corBlind);    
                    }
                }
                else
                {
                    Debug.LogWarning($"원본 복도를 찾을 수 없음: {pos} - {neighborPos}");
                }
            }
        }

        // 3. 미니맵에 부활/판매/탈출 장소 복제
        foreach (GameObject originPlace in roomGenerator.randomPlaceObj)  // 오브젝트 기준
        {
            Vector2 pos = originPlace.transform.position;
            Vector2 baseOff = new Vector2(baseOffset.x, baseOffset.y);

            GameObject minimapPlace = null;

            // 실제 맵 좌표 기준으로 어떤 장소인지 확인
            if (roomGenerator.placeManager.resurrection_positions.Contains((Vector2)originPlace.transform.position))
            {
                minimapPlace = Instantiate(miniPlace_Resurrection, pos + baseOff, Quaternion.identity, transform);
            }
            else if (roomGenerator.placeManager.sale_positions.Contains((Vector2)originPlace.transform.position))
            {
                minimapPlace = Instantiate(miniPlace_Sale, pos + baseOff, Quaternion.identity, transform);
            }
            else if (roomGenerator.placeManager.escape_positions.Contains((Vector2)originPlace.transform.position))
            {
                minimapPlace = Instantiate(miniPlace_Escape, pos + baseOff, Quaternion.identity, transform);
            }
            else if (roomGenerator.placeManager.soul_positions.Contains((Vector2)originPlace.transform.position))
            {
                minimapPlace = Instantiate(miniPlace_Soul, pos + baseOff, Quaternion.identity, transform);
            }
            else if (roomGenerator.placeManager.coin_positions.Contains((Vector2)originPlace.transform.position))
            {
                minimapPlace = Instantiate(miniPlace_Coin, pos + baseOff, Quaternion.identity, transform);
            }
            else if (roomGenerator.placeManager.eye_positions.Contains((Vector2)originPlace.transform.position))
            {
                minimapPlace = Instantiate(miniPlace_Eye, pos + baseOff, Quaternion.identity, transform);
            }

            // Place_Player_Find 연결
            if (minimapPlace != null)
            {
                Place_Player_Find finder = originPlace.GetComponentInChildren <Place_Player_Find>();
                if (finder != null)
                {
                    Place_Player_Find toss = minimapPlace.GetComponentInChildren<Place_Player_Find>();
                    finder.minimapPlace = toss.minimapPlace;
                }
            }
        }


        #region 구 코드
        // 4. 카메라를 맵 중심으로 이동
        //if (minimapCam != null)
        //{
        //    Vector3 minPos = new Vector3(float.MaxValue, float.MaxValue, 0);
        //    Vector3 maxPos = new Vector3(float.MinValue, float.MinValue, 0);

        //    foreach (var room in roomGenerator.roomObjects.Values)
        //    {
        //        Vector3 pos = room.transform.position + baseOffset;
        //        minPos = Vector3.Min(minPos, pos);
        //        maxPos = Vector3.Max(maxPos, pos);
        //    }

        //    foreach (Transform child in transform)
        //    {
        //        if (child.name.Contains("Corridor"))
        //        {
        //            Vector3 pos = child.position;
        //            minPos = Vector3.Min(minPos, pos);
        //            maxPos = Vector3.Max(maxPos, pos);
        //        }
        //    }

        //    Vector3 center = (minPos + maxPos) / 2f;
        //    center.z = minimapCam.transform.position.z;
        //    minimapCam.transform.position = center;
        //}
        #endregion

        // 4. 카메라를 맵 중심으로 이동 및 사이즈 조정
        // 1. 미니맵 전용 맵만 선택
        List<Transform> minimapObjects = new List<Transform>();
        foreach (Transform child in transform)
        {
            // 예: 이름이나 레이어 등으로 미니맵 전용만 필터링
            if (child.name.Contains("Mini") || child.name.Contains("Corridor") || child.name.Contains("Place"))
                minimapObjects.Add(child);
        }

        if (minimapObjects.Count > 0)
        {
            Vector3 minPos = new Vector3(float.MaxValue, float.MaxValue, 0);
            Vector3 maxPos = new Vector3(float.MinValue, float.MinValue, 0);

            foreach (var obj in minimapObjects)
            {
                minPos = Vector3.Min(minPos, obj.position);
                maxPos = Vector3.Max(maxPos, obj.position);
            }

            Vector3 center = (minPos + maxPos) / 2f;
            center.z = minimapCam.transform.position.z;
            minimapCam.transform.position = center;

            // 카메라 사이즈 계산
            float mapWidth = maxPos.x - minPos.x;
            float mapHeight = maxPos.y - minPos.y;
            float aspect = minimapCam.aspect;

            float size = Mathf.Max(mapHeight / 4f, mapWidth / 4f / aspect);
            size = Mathf.Clamp(cameraBaseSize + size + 1f, 5f, cameraMaxSize);
            minimapCam.orthographicSize = size;
        }
        // 방 꺼주기
        foreach(var blind in blindList)
        {
            blind.RoomSetActiveFalse();
        }

        Debug.Log($"총 {roomGenerator.roomObjects.Count}개의 방, 복도, 장소를 미니맵에 복제했습니다.");
    }
}
