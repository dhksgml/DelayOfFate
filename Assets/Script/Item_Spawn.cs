using UnityEngine;
using System.Collections.Generic;

public class Item_Spawn : MonoBehaviour
{
    public GameObject[] Item_Prefab; // 생성 가능한 아이템 프리팹들
    public int totalCoinPoint;       // 사용할 총 코인 점수
    private List<Transform> itemPoints = new List<Transform>();

    void Start()
    {

    }
    public void spoawn_item()
    {
        List<GameObject> spawnableItems = new List<GameObject>();
        int minCost = int.MaxValue;
        // 1. ItemPoint 위치 수집
        GameObject[] itemPointObjects = GameObject.FindGameObjectsWithTag("ItemPoint");
        foreach (GameObject obj in itemPointObjects)
        {
            if (obj.name.Contains("ItemPoint"))
                itemPoints.Add(obj.transform);
        }

        // 2. 유효한 아이템 프리팹만 필터링
        foreach (GameObject prefab in Item_Prefab)
        {
            if (prefab == null) continue;

            ItemObject item = prefab.GetComponentInChildren<ItemObject>();
            if (item != null)
            {
                spawnableItems.Add(prefab);
                int cost = item.itemData.ValPoint;
                if (cost < minCost)
                    minCost = cost;
            }
            else
            {
                Debug.LogWarning($"{prefab.name} 안에 ItemObject 컴포넌트가 없습니다.");
            }
        }

        // 3. 배치 로직
        int currentPoint = totalCoinPoint;

        while (currentPoint >= minCost && itemPoints.Count > 0)
        {
            // 아이템 랜덤 선택
            GameObject randomPrefab = spawnableItems[Random.Range(0, spawnableItems.Count)];
            ItemObject io = randomPrefab.GetComponentInChildren<ItemObject>(); // 자식에서 찾을 수도 있음
            if (io == null)
            {
                Debug.LogWarning($"{randomPrefab.name} 에 ItemObject가 없습니다.");
                continue;
            }

            int cost = io.itemData.ValPoint;

            if (cost <= currentPoint)
            {
                // 랜덤 위치 선택 및 제거
                int randomIndex = Random.Range(0, itemPoints.Count);
                Transform spawnPoint = itemPoints[randomIndex];
                itemPoints.RemoveAt(randomIndex);

                // 아이템 생성
                Instantiate(randomPrefab, spawnPoint.position, Quaternion.identity);
                currentPoint -= cost;

                Debug.Log($"스폰: {randomPrefab.name} at {spawnPoint.position} (cost: {cost}) / 남은 점수: {currentPoint}");
            }
        }
    }
}
