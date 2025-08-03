using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Somyeon_gwi : Enemy
{
    [Header("�Ҹ��")]
    PlayerController player;
    public GameObject[] randomItem; //������ ������
    [SerializeField] int throwItemCount; //�������� ����� Ƚ��
    [SerializeField] float eatDelay; //������ ������
    public bool isHit; //�ǰݽ�
    public bool isFindItem; //�������� ã���� ��
    public Vector3 findItemVec;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    void Start()
    {
        EnemyInt();
    }

    float eatTime;

    void Update()
    {
        //���� ü���� 0�����Ͻ�.
        if (enemyHp <= 0 && !isDie)
        {
            isDie = true;
            StartCoroutine(EnemyDie());

        }
        else
        {
            eatTime += Time.deltaTime;
            EnemyMove();
        }
    }

    public override void EnemyMove()
    {
        //�ǰݽ�
        if (isHit)
        {
            //�����ϴ� Ÿ���� ��ġ - �ڽ��� ��ġ�� ���� �� ����ȭ�� ���ش�
            enemyTargetDir = (player.transform.position - transform.position).normalized;

            //EnemyTraceTurn2();

            //���ϸ��̼�, ���� true �ٲپ���
            //anim.SetBool("isTrace", true);

            rigid.MovePosition(transform.position + enemyTargetDir * enemyRunSpeed * Time.deltaTime);
        }
        //�������� �߰��ϰ�, Ƚ���� ���������� �Դ� �ð��� �� �Ǹ�
        else if (isFindItem && throwItemCount > 0 && eatTime >= eatDelay)
        {
            enemyTargetDir = (findItemVec - transform.position).normalized;
            rigid.MovePosition(transform.position + enemyTargetDir * enemyMoveSpeed * Time.deltaTime);
        }

    }



    //�÷��̾ ����� ��
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            }

            //�ִ� 3������
            if (collision.gameObject.CompareTag("Item") && throwItemCount > 0 &&
                eatTime >= eatDelay)
            {
                //��ġ�� ������
                Vector3 itemPos = collision.transform.position;

                //���� �������� �Ҹ��Ų ��
                Destroy(collision.gameObject);

                //������ ��ġ�� �����ǰ� ����
                float randomPosX = itemPos.x + Random.Range(-4f, 4f);
                float randomPosY = itemPos.y + Random.Range(-4f, 4f);

                //�־�� �������� �������� �����ǰ� ���־���
                Instantiate(randomItem[Random.Range(0, randomItem.Length)],
                    new Vector3(randomPosX, randomPosY, itemPos.z), 
                    Quaternion.identity);

                //Ʈ���� �ʱ�ȭ ����
                isFindItem = false;
                //Ƚ���� ���ش�.
                throwItemCount--;
                eatTime = 0;
            }

            //�� �ǰ� �κ�
            //�̺κ��� �Ƹ� �� ���� �ڵ�� ����� �� ����.
            Attack_sc attack = collision.GetComponent<Attack_sc>();

            if (collision.gameObject.CompareTag("Attack") && !isEnemyHit && attack != null)
            {
                // Ÿ���� ��ġ�ϸ� ���
                if (attack.attackType.ToString() == enemyWeakness.ToString())
                {
                    //�̺κ� ���� ���ͼ� �ϴ� �ּ� ó�� ���־���.
                    //attack.CheckWeakness();
                    enemyHp = 0f;
                }
                else
                {
                    enemyHp -= attack.damage;
                }

                isHit = true;

                EnemyHit(attack.damage);
                Invoke("EnemyHitRegen", enemyHitTime);
            }
        }
    }
}
