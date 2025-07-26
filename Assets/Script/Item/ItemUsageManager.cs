using UnityEngine;

public class ItemUsageManager : MonoBehaviour
{
    public GameObject at_Prefab; // ���� ������
    public Transform spawnPoint; // �÷��̾� ���� ��ġ
    public GameObject Paper; // ���� ����ü ������
    public void UseItem(string itemName)
    {
        //Sword   // ȯ��
        //Bat    // �����
        //Paper   // ����
        //Scroll  // ����
       // Bottle   // ȣ����
        switch (itemName)
        {
            case "ȯ��":
                SpawnAttackEffect(Attack_sc.AttackType.Sword);
                break;
            case "�����":
                SpawnAttackEffect(Attack_sc.AttackType.Bat);
                break;
            case "����":
                SpawnAttackEffect(Attack_sc.AttackType.Paper);
                break;
            case "����":
                SpawnAttackEffect(Attack_sc.AttackType.Scroll);
                break;
            case "ȣ����":
                SpawnAttackEffect(Attack_sc.AttackType.Bottle);
                break;
            default:
                Debug.Log("������ ��� �Ұ���!");
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
            float spawnOffset = 0.5f; // �� ���� ����߸���
            Vector3 spawnDir = rotation * Vector3.right; // ȸ�� ���� ���� ������
            Vector3 spawnPos = spawnPoint.position + spawnDir.normalized * spawnOffset;

            GameObject go = Instantiate(at_Prefab, spawnPos, rotation);
            if (type == Attack_sc.AttackType.Sword) if(SoundManager.Instance != null) SoundManager.Instance.PlaySFX(Resources.Load<AudioClip>("SFX/sfx_Sword_1"));
            if (type == Attack_sc.AttackType.Bat) if (SoundManager.Instance != null) SoundManager.Instance.PlaySFX(Resources.Load<AudioClip>("SFX/sfx_Bat_1"));
            // ���⿡ ���� �¿� ���� (135�� ~ 225�� ���̸� ���� ����)
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
            // ���콺 ���� ���� ��� (Z �� ����)
            Vector3 mouseWorldPaperPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPaperPos.z = 0f;

            Vector3 paperDirection = (mouseWorldPaperPos - spawnPoint.position).normalized;

            // ���� ��ġ ��� (�������� offset)
            float offsetDistance = 0.5f; // ��¦ ������ �Ÿ�
            Vector3 spawnPos = spawnPoint.position + paperDirection * offsetDistance;

            // ȸ�� ���� ���
            float paperAngle = Mathf.Atan2(paperDirection.y, paperDirection.x) * Mathf.Rad2Deg;
            paperAngle += Random.Range(-5f, 5f); // ȸ���� ��¦ Ʋ��
            Quaternion paperRotation = Quaternion.Euler(0f, 0f, paperAngle);
            // ����
            GameObject go = Instantiate(Paper, spawnPos, paperRotation);
            if (SoundManager.Instance != null) SoundManager.Instance.PlaySFX(Resources.Load<AudioClip>("SFX/sfx_throw"));
            // �¿� ���� (�ʿ� ��)
            if (direction.x < 0f)
            {
                Vector3 scale = go.transform.localScale;
                scale.y *= -1; // �Ǵ� scale.x *= -1;
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

                // ȭ�� �ȿ� �ִ��� Ȯ�� (0~1 ����)
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
