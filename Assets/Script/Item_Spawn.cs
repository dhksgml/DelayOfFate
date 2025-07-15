using UnityEngine;
using System.Collections.Generic;

public class Item_Spawn : MonoBehaviour
{
    public GameObject[] Item_Prefab; // ���� ������ ������ �����յ�
    public int totalCoinPoint;       // ����� �� ���� ����
    private List<Transform> itemPoints = new List<Transform>();

    void Start()
    {

    }
    public void spoawn_item()
    {
        List<GameObject> spawnableItems = new List<GameObject>();
        int minCost = int.MaxValue;
        // 1. ItemPoint ��ġ ����
        GameObject[] itemPointObjects = GameObject.FindGameObjectsWithTag("ItemPoint");
        foreach (GameObject obj in itemPointObjects)
        {
            if (obj.name.Contains("ItemPoint"))
                itemPoints.Add(obj.transform);
        }

        // 2. ��ȿ�� ������ �����ո� ���͸�
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
                Debug.LogWarning($"{prefab.name} �ȿ� ItemObject ������Ʈ�� �����ϴ�.");
            }
        }

        // 3. ��ġ ����
        int currentPoint = totalCoinPoint;

        while (currentPoint >= minCost && itemPoints.Count > 0)
        {
            // ������ ���� ����
            GameObject randomPrefab = spawnableItems[Random.Range(0, spawnableItems.Count)];
            ItemObject io = randomPrefab.GetComponentInChildren<ItemObject>(); // �ڽĿ��� ã�� ���� ����
            if (io == null)
            {
                Debug.LogWarning($"{randomPrefab.name} �� ItemObject�� �����ϴ�.");
                continue;
            }

            int cost = io.itemData.ValPoint;

            if (cost <= currentPoint)
            {
                // ���� ��ġ ���� �� ����
                int randomIndex = Random.Range(0, itemPoints.Count);
                Transform spawnPoint = itemPoints[randomIndex];
                itemPoints.RemoveAt(randomIndex);

                // ������ ����
                Instantiate(randomPrefab, spawnPoint.position, Quaternion.identity);
                currentPoint -= cost;

                Debug.Log($"����: {randomPrefab.name} at {spawnPoint.position} (cost: {cost}) / ���� ����: {currentPoint}");
            }
        }
    }
}
