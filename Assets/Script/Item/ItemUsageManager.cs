using UnityEngine;

public class ItemUsageManager : MonoBehaviour
{
    public GameObject at_Prefab; // 공격 프리팹
    public Transform spawnPoint; // 플레이어 생성 위치
    public GameObject Paper; // 부적 투사체 프리팹
    PlayerController playerController;
    private void Start()
    {
        playerController = GetComponent<PlayerController>();
    }

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
        GameEvents.CallUseItem(type);

        // 플레이어가 바라보는 방향 사용
        Vector2 direction = playerController.lastMoveDirection;
        if (direction == Vector2.zero)
            direction = Vector2.right; // 기본값: 오른쪽

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360f;

        float snappedAngle = Mathf.Round(angle / 45f) * 45f;
        Quaternion rotation = Quaternion.Euler(0, 0, snappedAngle);

        if (type == Attack_sc.AttackType.Sword || type == Attack_sc.AttackType.Bat || type == Attack_sc.AttackType.Bottle)
        {
            float spawnOffset = 1.5f;
            Vector3 spawnDir = rotation * Vector3.right;
            Vector3 spawnPos = spawnPoint.position + spawnDir.normalized * spawnOffset;

            GameObject go = Instantiate(at_Prefab, spawnPos, rotation);

            if (type == Attack_sc.AttackType.Sword)
                if (SoundManager.Instance != null) SoundManager.Instance.PlaySFX(Resources.Load<AudioClip>("SFX/sfx_Sword_1"));
            if (type == Attack_sc.AttackType.Bat)
                if (SoundManager.Instance != null) SoundManager.Instance.PlaySFX(Resources.Load<AudioClip>("SFX/sfx_Bat_1"));

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
            // 플레이어 방향 기반 투사체
            Vector3 paperDirection = direction.normalized;
            float offsetDistance = 0.5f;
            Vector3 spawnPos = spawnPoint.position + paperDirection * offsetDistance;

            float paperAngle = Mathf.Atan2(paperDirection.y, paperDirection.x) * Mathf.Rad2Deg;
            paperAngle += Random.Range(-5f, 5f);
            Quaternion paperRotation = Quaternion.Euler(0f, 0f, paperAngle);

            GameObject go = Instantiate(Paper, spawnPos, paperRotation);
            if (SoundManager.Instance != null) SoundManager.Instance.PlaySFX(Resources.Load<AudioClip>("SFX/sfx_throw"));

            if (direction.x < 0f)
            {
                Vector3 scale = go.transform.localScale;
                scale.y *= -1;
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
