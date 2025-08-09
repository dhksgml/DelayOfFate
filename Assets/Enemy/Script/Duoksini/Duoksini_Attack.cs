using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEditor.Rendering;
using UnityEngine;

public class Duoksini_Attack : EnemyAttack
{
    [SerializeField] private LayerMask itemLayer;
    [SerializeField] Duoksini duoksini;
    [HideInInspector] PlayerController player;
    [SerializeField] GameObject attackRange;
    [HideInInspector] public bool isAttack = false;
    [HideInInspector] public bool isTrsPass = false;

    // ����� ���� ������ �����ְ� �� �� �ڿ� �������� ����
    [Tooltip("���� ������ ���� �� ���ݱ��� �ɸ��� �ð�")]
    [SerializeField] float duoksiniAttackDelay;
    float duoksiniAttackTime;

    void Awake()
    {
        //�÷��̾ ã�Ƽ� �������� ��
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    bool isAttackReady = false;
    private void Update()
    {
        // ���� �߿� �ð��� �ȿ���
        if (!isAttack) time += Time.deltaTime;
        if (isAttackReady) duoksiniAttackTime += Time.deltaTime;

        // ���� �غ� �� �Ǿ����� ����
        if (time >= enemyAttackSpeed && duoksini.isAttackReady && isTrsPass)
        {
            Debug.Log("���� ǥ��");
            isAttack = true;

            // ��ġ�� ���� ���� ������ �ǰ���
            transform.position = duoksini.attackTargetTrs;

            // ��Ÿ� ǥ��
            attackRange.SetActive(true);

            isTrsPass = false;
            isAttackReady = true;
        }

        if (duoksiniAttackTime >= duoksiniAttackDelay)
        {

            Debug.Log("����");
            // �ı�
            DestroyItem();

            // ��Ÿ� ��Ȱ��ȭ
            attackRange.SetActive(false);

            // �ݶ��̴��� Ȱ��ȭ ����
            enemyAttackCollider.enabled = true;

            // �ʱ�ȭ
            duoksiniAttackTime = 0;
            isAttack = false;
            duoksini.isAttackReady = false;
            isAttackReady = false;
            time = 0;


            // �ݶ��̴� ��Ȱ��ȭ
            Invoke("Dealy", 0.1f);
        }
    }


    // �÷��̾� ����
    void OnTriggerEnter2D(Collider2D collision)
    {
        // �÷��̾� ����
        if (collision.gameObject.CompareTag("Player") && !isAttack)
        {
            // ����
            player.DamagedHP(enemyDamage);
        }
    }


    void DestroyItem()
    {
        Vector2 center = enemyAttackCollider.bounds.center;
        float radius = ((CircleCollider2D)enemyAttackCollider).radius * enemyAttackCollider.transform.lossyScale.x;

        Collider2D[] itemColliders = Physics2D.OverlapCircleAll(center, radius, itemLayer);

        foreach (Collider2D collider in itemColliders)
        {
            ItemObject itemObject = collider.GetComponent<ItemObject>();

            if (itemObject != null)
            {
                Debug.Log("�ı�: " + itemObject.name);
                Destroy(itemObject.gameObject);
            }
            else
            {
                Debug.Log("ItemObject ������Ʈ ����: " + collider.name);
            }
        }
    }

    void Dealy()
    {
        enemyAttackCollider.enabled = false;
    }
}
