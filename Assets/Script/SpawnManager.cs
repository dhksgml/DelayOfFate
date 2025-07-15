using UnityEngine;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject[] enemyPrefabs; // �� ������
    public GameObject itemPrefab;     // ���� ������ ������
    public Item[] item_date;          // ScriptableObject ��� ������ ������ �迭

    public int totalD_Point;          // ���� ���� ����
    public int totalValPoint;         // ���� ���� ����

    private List<Transform> enemySpawnPoints = new List<Transform>();
    private List<Transform> itemSpawnPoints = new List<Transform>();

    public void SpawnAll()
    {
        enemySpawnPoints.Clear();
        itemSpawnPoints.Clear();

        // 1. ���� ��ġ ����
        GameObject[] enemyPoints = GameObject.FindGameObjectsWithTag("EnemyPoint");
        foreach (GameObject obj in enemyPoints)
        {
            if (obj.name.Contains("EnemyPoint"))
            {
                enemySpawnPoints.Add(obj.transform);

                Create_Disable disabler = obj.GetComponent<Create_Disable>();
                if (disabler != null)
                    disabler.Disable();
                else
                    Debug.LogWarning($"{obj.name}�� Create_Disable ������Ʈ�� �����ϴ�.");
            }
        }

        GameObject[] itemPoints = GameObject.FindGameObjectsWithTag("ItemPoint");
        foreach (GameObject obj in itemPoints)
        {
            if (obj.name.Contains("ItemPoint"))
            {
                itemSpawnPoints.Add(obj.transform);

                Create_Disable disabler = obj.GetComponent<Create_Disable>();
                if (disabler != null)
                    disabler.Disable();
                else
                    Debug.LogWarning($"{obj.name}�� Create_Disable ������Ʈ�� �����ϴ�.");
            }
        }

        // 2. ���� ������ �ּ� ���� ���
        int minEnemyDanger = int.MaxValue;
        int minEnemyCoin = int.MaxValue;
        int minItemCoin = int.MaxValue;

        List<GameObject> validEnemies = new List<GameObject>();
        List<Item> validItems = new List<Item>();

        foreach (GameObject prefab in enemyPrefabs)
        {
            if (prefab == null) continue;

            Enemy enemy = prefab.GetComponentInChildren<Enemy>();
            if (enemy != null && enemy.enemyData != null)
            {
                validEnemies.Add(prefab);

                if (enemy.enemyData.D_Point < minEnemyDanger)
                    minEnemyDanger = enemy.enemyData.D_Point;

                if (enemy.enemyData.Coin < minEnemyCoin)
                    minEnemyCoin = enemy.enemyData.Coin;
            }
        }


        foreach (Item item in item_date)
        {
            if (item != null)
            {
                validItems.Add(item);
                if (item.ValPoint < minItemCoin)
                    minItemCoin = item.ValPoint;
            }
        }

        // 3. �� ����
        int dangerRemain = totalD_Point;
        int coinRemain = totalValPoint;

        while (dangerRemain >= minEnemyDanger && coinRemain >= minEnemyCoin && enemySpawnPoints.Count > 0)
        {
            List<GameObject> spawnables = validEnemies.FindAll(e =>
            {
                Enemy enemy = e.GetComponentInChildren<Enemy>();
                return enemy != null &&
                       enemy.enemyData != null &&
                       enemy.enemyData.D_Point <= dangerRemain &&
                       enemy.enemyData.Coin <= coinRemain;
            });

            if (spawnables.Count == 0) break;

            GameObject randomEnemy = spawnables[Random.Range(0, spawnables.Count)];
            Enemy enemyComponent = randomEnemy.GetComponentInChildren<Enemy>();
            Enemy_data edata = enemyComponent.enemyData;

            int index = Random.Range(0, enemySpawnPoints.Count);
            Transform spawnPoint = enemySpawnPoints[index];
            enemySpawnPoints.RemoveAt(index);

            Instantiate(randomEnemy, spawnPoint.position, Quaternion.identity);
            dangerRemain -= edata.D_Point;
            coinRemain -= edata.Coin;
        }


        // 4. ������ ����
        while (coinRemain >= minItemCoin && itemSpawnPoints.Count > 0)
        {
            List<Item> spawnables = validItems.FindAll(i => i.ValPoint <= coinRemain);
            if (spawnables.Count == 0) break;

            Item randomItemData = spawnables[Random.Range(0, spawnables.Count)];

            int index = Random.Range(0, itemSpawnPoints.Count);
            Transform spawnPoint = itemSpawnPoints[index];
            itemSpawnPoints.RemoveAt(index);

            GameObject itemObj = Instantiate(itemPrefab, spawnPoint.position, Quaternion.identity);
            ItemObject itemObjComp = itemObj.GetComponentInChildren<ItemObject>();
            if (itemObjComp != null)
            {
                itemObjComp.itemData = randomItemData;
            }
            else
            {
                Debug.LogWarning($"{itemObj.name}�� ItemObject ������Ʈ�� �����ϴ�.");
            }

            coinRemain -= randomItemData.ValPoint;
        }
    }
}
