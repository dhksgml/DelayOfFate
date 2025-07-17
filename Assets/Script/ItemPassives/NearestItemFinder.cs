using UnityEngine;

public class NearestItemFinder : MonoBehaviour
{
    public LayerMask itemLayer;          // 인스펙터에서 "Item" 레이어 선택
    public float searchRadius = 20f;     // 탐색 반경
    public Transform nearestItem;

    public void FindNearestItem()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, searchRadius, itemLayer);

        float closestDistance = Mathf.Infinity;
        Transform closest = null;

        foreach (Collider2D hit in hits)
        {
            float distance = Vector3.Distance(transform.position, hit.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = hit.transform;
            }
        }

        nearestItem = closest;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, searchRadius);
    }
}
