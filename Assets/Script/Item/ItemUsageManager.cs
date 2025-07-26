using UnityEngine;

public class ItemUsageManager : MonoBehaviour
{
    public GameObject at_Prefab; // 공격 프리팹
    public Transform spawnPoint; // 플레이어 생성 위치
    public GameObject Paper; // 부적 투사체 프리팹
    public void UseItem(string itemName)
    {
        //Sword   // 환도
        //Bat    // 방망이
        //Paper   // 부적
        //Scroll  // 족자
       // Bottle   // 호리병
        switch (itemName)
        {
            case "환도":
                SpawnAttackEffect(Attack_sc.AttackType.Sword);
                break;
            case "방망이":
                SpawnAttackEffect(Attack_sc.AttackType.Bat);
                break;
            case "부적":
                SpawnAttackEffect(Attack_sc.AttackType.Paper);
                break;
            case "족자":
                SpawnAttackEffect(Attack_sc.AttackType.Scroll);
                break;
            case "호리병":
                SpawnAttackEffect(Attack_sc.AttackType.Bottle);
                break;
            default:
                Debug.Log("아이템 사용 불가능!");
                break;
        }
    }
    void SpawnAttackEffect(Attack_sc.AttackType type)
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mouseWorldPos - spawnPoint.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        if (angle < 0) angle += 360f;

        float snappedAngle = Mathf.Round(angle / 45f) * 45f;
        Quaternion rotation = Quaternion.Euler(0, 0, snappedAngle);

        if (type == Attack_sc.AttackType.Sword || type == Attack_sc.AttackType.Bat || type == Attack_sc.AttackType.Bottle)
        {
            float spawnOffset = 0.5f; // 몇 유닛 떨어뜨릴지
            Vector3 spawnDir = rotation * Vector3.right; // 회전 방향 기준 오른쪽
            Vector3 spawnPos = spawnPoint.position + spawnDir.normalized * spawnOffset;

            GameObject go = Instantiate(at_Prefab, spawnPos, rotation);
            if (type == Attack_sc.AttackType.Sword) if(SoundManager.Instance != null) SoundManager.Instance.PlaySFX(Resources.Load<AudioClip>("SFX/sfx_Sword_1"));
            if (type == Attack_sc.AttackType.Bat) if (SoundManager.Instance != null) SoundManager.Instance.PlaySFX(Resources.Load<AudioClip>("SFX/sfx_Bat_1"));
            // 방향에 따라 좌우 반전 (135도 ~ 225도 사이면 왼쪽 방향)
            if (snappedAngle >= 135f && snappedAngle <= 225f)
            {
                Vector3 scale = go.transform.localScale;
                scale.y *= -1;
                go.transform.localScale = scale;
            }

            Attack_sc attackEffect = go.GetComponent<Attack_sc>();
            attackEffect.attackType = type;
        }
        if (type == Attack_sc.AttackType.Paper)
        {
            // 마우스 방향 벡터 계산 (Z 값 제거)
            Vector3 mouseWorldPaperPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPaperPos.z = 0f;

            Vector3 paperDirection = (mouseWorldPaperPos - spawnPoint.position).normalized;

            // 생성 위치 계산 (방향으로 offset)
            float offsetDistance = 0.5f; // 살짝 떨어진 거리
            Vector3 spawnPos = spawnPoint.position + paperDirection * offsetDistance;

            // 회전 각도 계산
            float paperAngle = Mathf.Atan2(paperDirection.y, paperDirection.x) * Mathf.Rad2Deg;
            paperAngle += Random.Range(-5f, 5f); // 회전만 살짝 틀기
            Quaternion paperRotation = Quaternion.Euler(0f, 0f, paperAngle);
            // 생성
            GameObject go = Instantiate(Paper, spawnPos, paperRotation);
            if (SoundManager.Instance != null) SoundManager.Instance.PlaySFX(Resources.Load<AudioClip>("SFX/sfx_throw"));
            // 좌우 반전 (필요 시)
            if (direction.x < 0f)
            {
                Vector3 scale = go.transform.localScale;
                scale.y *= -1; // 또는 scale.x *= -1;
                go.transform.localScale = scale;
            }
        }

        if (type == Attack_sc.AttackType.Scroll)
        {
            Camera cam = Camera.main;

            GameObject[] mobs = GameObject.FindGameObjectsWithTag("Enemy");

            foreach (GameObject mob in mobs)
            {
                Vector3 viewportPos = cam.WorldToViewportPoint(mob.transform.position);

                // 화면 안에 있는지 확인 (0~1 범위)
                if (viewportPos.z > 0 && viewportPos.x >= 0 && viewportPos.x <= 1 && viewportPos.y >= 0 && viewportPos.y <= 1)
                {
                    GameObject go = Instantiate(at_Prefab, mob.transform.position, rotation);
                    Attack_sc attackEffect = go.GetComponent<Attack_sc>();
                    attackEffect.attackType = type;
                }
            }
        }
    }
}
