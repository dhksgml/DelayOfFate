using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.GraphicsBuffer;

//���� ���� ��
public enum Yin_Yang_Type
{
    Yin,
    Yang
}

public class Yin_Yang : Enemy
{
    [Header("Yin_Yang")]
    //��ȸ�ϴ� ��, �̹������ �ҷ��� �ݶ��̴��� �ΰ� �־���� �Ѵ�.
    //���Ŀ� �ǰ� �ݶ��̴��� �����°� ��������
    [SerializeField] public Yin_Yang_Type type;
    [SerializeField] Yin_YangTrace yin_YangTrace;
    [SerializeField] GameObject summonReaper;
    public bool isFind;
    bool isSpawn = false;
    [SerializeField] GameObject yinObj;

    PlayerController player; //�÷��̾�

    // �ٷ� ��ü�Ǵ°� �����ֱ� ����
    float delay;
    float fusionTime = 5.0f;

    // static���� �� ������Ʈ�� �����ϴ� �浹 ����
    static bool hasFusion = false; 

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    void Start()
    {
        EnemyInt();

        if (type == Yin_Yang_Type.Yang)
        {
            // ó���� ������ ���� ����
            ChooseNewDirection();

            // �ֱ������� ���� ��ȯ
            StartCoroutine(ChangeDirectionRoutine());
        }
        Debug.Log(gameObject);
    }

    void Update()
    {
        //���� ü���� 0�����Ͻ�.
        if (enemyHp <= 0 && !isDie)
        {
            isDie = true;
            StartCoroutine(EnemyDie());
        }

        if (!isSpawn)
        {
            if (type == Yin_Yang_Type.Yang)
            {
                isSpawn = true;
                // �� 0,0,0�� ��ȯ
                GameObject test = Instantiate(yinObj, new Vector3(0, 0, 0), Quaternion.identity);
            }
        }

        delay += Time.deltaTime;

        EnemyMove();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null && !collision.gameObject.CompareTag("Enemy"))
        {
            Attack_sc attack = collision.GetComponent<Attack_sc>();

            if (collision.gameObject.CompareTag("Attack") && !isEnemyHit && attack != null)
            {
                Debug.Log(2);
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

                EnemyHit();

            }
        }
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Enemy"))
        {
            //Yin_Yang ��ũ��Ʈ�� ������ ��
            Yin_Yang yinYang = collision.gameObject.GetComponent<Yin_Yang>();

            if(yinYang != null)
            {
                if(delay >= fusionTime)
                {
                    if (!hasFusion)
                    {
                        SummonReaper();
                        hasFusion = true;
                    }
                    Destroy(transform.parent.gameObject);
                }
            }
        }

        //�浹�� ������ȯ
        ChooseNewDirection();
    }

    void OnDestroy()
    {
        ResetCollision();
    }

    public override void EnemyMove()
    {
        if(isFind)
        {
            moveDirection = (yin_YangTrace.target - transform.position).normalized;
            transform.Translate(moveDirection * enemyMoveSpeed * Time.deltaTime);
        }
        else if(type == Yin_Yang_Type.Yang)
        {
            // ���� �������� �̵�
            transform.Translate(moveDirection * enemyMoveSpeed * Time.deltaTime);
        }
    }


    //��� ��ȯ �޼���
    void SummonReaper()
    {
        Debug.Log("������ü");
        Instantiate(summonReaper, transform.position, Quaternion.identity);
    }

    //static ���� �ʱ�ȭ �޼���
    public static void ResetCollision()
    {
        hasFusion = false;
    }
}
