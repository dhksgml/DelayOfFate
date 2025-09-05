using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSpawn : MonoBehaviour
{
    public RoomRandomPlacement roomGenerator; // RoomRandomPlacement 참조
    public Vector3 baseOffset = new Vector3(0, -2000, 0);

    public GameObject verticalCorridorPrefab;    // 위/아래 연결
    public GameObject horizontalCorridorPrefab;  // 좌/우 연결

    [SerializeField] Camera minimapCam;

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
            Instantiate(originalRoom, spawnPos, Quaternion.identity, transform);
        }

        // 2. 복도 생성
        foreach (var kvp in roomGenerator.roomObjects)
        {
            Vector2Int pos = kvp.Key;
            string exits = roomGenerator.roomDirections[pos];

            foreach (char dir in exits)
            {
                Vector2Int neighborPos = roomGenerator.GetNeighbor(pos, dir);

                // 이미 없는 방이면 무시
                if (!roomGenerator.roomObjects.ContainsKey(neighborPos)) continue;

                // 중복 생성 방지: 항상 작은 좌표 기준으로 생성
                if (pos.y > neighborPos.y || (pos.y == neighborPos.y && pos.x > neighborPos.x)) continue;

                Vector3 worldPos = (new Vector3(pos.x, pos.y, 0) + new Vector3(neighborPos.x, neighborPos.y, 0)) / 2f;
                worldPos = worldPos * roomGenerator.spacing + baseOffset;

                GameObject corridorPrefabToUse = (dir == 'U' || dir == 'D')
                    ? verticalCorridorPrefab
                    : horizontalCorridorPrefab;

                Instantiate(corridorPrefabToUse, worldPos, Quaternion.identity, transform);
            }
        }

        // 3. 카메라를 맵 중심으로 이동
        if (minimapCam != null)
        {
            Vector3 minPos = new Vector3(float.MaxValue, float.MaxValue, 0);
            Vector3 maxPos = new Vector3(float.MinValue, float.MinValue, 0);

            // 방 위치 포함
            foreach (var room in roomGenerator.roomObjects.Values)
            {
                Vector3 pos = room.transform.position + baseOffset;
                minPos = Vector3.Min(minPos, pos);
                maxPos = Vector3.Max(maxPos, pos);
            }

            // 복도 위치 포함
            foreach (Transform child in transform)
            {
                if (child.name.Contains("Corridor"))
                {
                    Vector3 pos = child.position;
                    minPos = Vector3.Min(minPos, pos);
                    maxPos = Vector3.Max(maxPos, pos);
                }
            }

            Vector3 center = (minPos + maxPos) / 2f;
            center.z = minimapCam.transform.position.z; // 기존 Z 유지
            minimapCam.transform.position = center;
        }


        Debug.Log($"총 {roomGenerator.roomObjects.Count}개의 방과 복도를 미니맵에 복제했습니다.");
    }
}
