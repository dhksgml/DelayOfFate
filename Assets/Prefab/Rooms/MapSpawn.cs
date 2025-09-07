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

    [SerializeField] Camera minimapCam;

    // 맵을 가리는걸 설정하는 스크립트
    [SerializeField] Minimap_Blind blindScript;

    // 맵 저장용
    [SerializeField] GameObject mapSave;



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

            originBlind.CopyRoomBlindGet();

        }

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

                // 복제
                GameObject mapCorridor = Instantiate(corridorPrefabToUse, worldPos, Quaternion.identity, transform);

                // Minimap_Blind 복제 처리
                Minimap_Blind originBlind = corridorPrefabToUse.GetComponent<Minimap_Blind>();
                Minimap_Blind blind = mapCorridor.GetComponent<Minimap_Blind>();

                if (originBlind != null && blind != null)
                {
                    originBlind.copyRoomObj = mapCorridor;
                    originBlind.CopyRoomBlindGet();
                }
            }
        }

        // 3. 미니맵에 부활/판매/탈출 장소 복제
        foreach (Vector2 pos in roomGenerator.randomPlace)
        {
            Vector2 baseOff = new Vector2(baseOffset.x, baseOffset.y); 

            // 실제 맵 좌표 기준으로 어떤 장소인지 확인
            if (roomGenerator.placeManager.resurrection_pos == pos)
            {
                Instantiate(miniPlace_Resurrection, pos + baseOff, Quaternion.identity, transform);
            }
            else if (roomGenerator.placeManager.sale_pos == pos)
            {
                Instantiate(miniPlace_Sale, pos + baseOff, Quaternion.identity, transform);
            }
            else if (roomGenerator.placeManager.escape_pos == pos)
            {
                Instantiate(miniPlace_Escape, pos + baseOff, Quaternion.identity, transform);
            }
        }

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

        // 4. 카메라를 맵 중심으로 이동
        if (minimapCam != null)
        {
            Vector3 minPos = new Vector3(float.MaxValue, float.MaxValue, 0);
            Vector3 maxPos = new Vector3(float.MinValue, float.MinValue, 0);

            // 모든 복제된 자식 오브젝트 기준으로 min/max 계산
            foreach (Transform child in transform)
            {
                Vector3 pos = child.position; // 이미 spawn 시 baseOffset 적용
                minPos = Vector3.Min(minPos, pos);
                maxPos = Vector3.Max(maxPos, pos);
            }

            Vector3 center = (minPos + maxPos) / 2f;
            center.z = minimapCam.transform.position.z;
            minimapCam.transform.position = center;
        }

        Debug.Log($"총 {roomGenerator.roomObjects.Count}개의 방, 복도, 장소를 미니맵에 복제했습니다.");
    }
}
