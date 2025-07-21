using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Boon_yeol_gwi_Type
{
    Entity,
    Copy
}

public class Boon_yeol_gwi : Enemy
{
    [Header("�п���")]
    [SerializeField] Boon_yeol_gwi_Type type;
    [SerializeField] GameObject copyObj; //�п�ü ������Ʈ
    [SerializeField] Vector3 entityTrs; //��ü ��ġ
    [SerializeField] int explosionSelfvalue; //��ġ�� �̰� �̻��̸� ����

    [SerializeField] public bool isItemFind = false; //�п�ü�� �������� ã������
    [SerializeField] public bool isItemEat = false; //�п�ü�� �������� �Ծ�����
    [SerializeField] public bool isEntityFind = false; //�п�ü�� ��ü�� ã������
    [SerializeField] bool isSpawn = false; //�����ϴ� Ʈ����

    [SerializeField] int currentIndex = 0; //���� �ε���
    int maxIndex = 2; //�ִ� 3������ ����

    public List<GameObject> copyObjList; //������ ���� ������ ����Ʈ

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        EnemyInt();

        //�п�ü�ϋ�
        if (type == Boon_yeol_gwi_Type.Copy)
        {
            //�����ϸ� ��ġ�� ��������
            entityTrs = transform.position;
            // ó���� ������ ���� ����
            ChooseNewDirection();

            // �ֱ������� ���� ��ȯ
            StartCoroutine(ChangeDirectionRoutine());
        }
        //��ä�ϋ�
        else if (type == Boon_yeol_gwi_Type.Entity)
        {
            GameObject copyEnemy = Instantiate(copyObj, transform.position, Quaternion.identity);
            //����Ʈ�� �߰�����
            copyObjList.Add(copyEnemy);
            currentIndex++; //1���� ���������Ƿ� ++
        }
    }

    void Update()
    {
        //��ü�϶� ���� ������ �Ǹ�
        if (enemyPrice >= explosionSelfvalue && type == Boon_yeol_gwi_Type.Entity)
        {
            //�ϴ� ���� ó������
            Destroy(transform.parent.gameObject);

            //�п�ü�� ����ó������
            foreach (GameObject obj in copyObjList)
            {
                Destroy(obj);
            }
        }

        //���� �غ� �Ǹ�
        else if (type == Boon_yeol_gwi_Type.Entity && isSpawn)
        {
            if (currentIndex <= maxIndex)
            {
                //�θ��� ��ȯ
                GameObject copyEnemy = Instantiate(copyObj, transform.position, Quaternion.identity);
                copyObjList.Add(copyEnemy);

                currentIndex++;

                if (currentIndex <= maxIndex)
                {
                    GameObject copyEnemy2 = Instantiate(copyObj, transform.position, Quaternion.identity);
                    copyObjList.Add(copyEnemy2);
                    currentIndex++;
                }

                isSpawn = false;
            }
        }

        EnemyMove();
    }

    [HideInInspector] public Vector3 itemTrs; //������ ��ġ
    [HideInInspector] public Vector3 targetTrs; //Ÿ�� ��ġ

    public override void EnemyMove()
    {
        // ��ü �߰�
        if (isEntityFind)
        {
            Vector3 target = (targetTrs - transform.position).normalized;
            transform.Translate(target * enemyMoveSpeed * Time.deltaTime);
        }

        // ������ ��ġ �Ҹ� �� ����
        else if (isItemFind && isItemEat)
        {
            Vector3 entity = (entityTrs - transform.position).normalized;
            transform.Translate(entity * enemyMoveSpeed * Time.deltaTime);
        }

        // ������ �߰�
        else if (isItemFind)
        {
            Vector3 itemPos = (itemTrs - transform.position).normalized;
            transform.Translate(itemPos * enemyMoveSpeed * Time.deltaTime);
        }
        // �� ��
        else if (!isItemEat && !isItemFind && !isEntityFind)
        {
            transform.Translate(moveDirection * enemyMoveSpeed * Time.deltaTime);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {
            //�ǰ� ȿ��
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

                EnemyHit();
                Invoke("EnemyHitRegen", enemyHitTime);
            }

            //item�� �������� ��
            ItemObject item = collision.gameObject.GetComponent<ItemObject>();

            if (collision.gameObject.CompareTag("Item") && item.itemData.Coin != 0 && !isItemEat)
            {
                //��ġ�� �÷��� ��
                enemyPrice += item.itemData.Coin;

                //��ġ�� ��������
                item.itemData.Coin = 0;

                isItemEat = true;
            }


            if (type == Boon_yeol_gwi_Type.Copy)
            {
                if (collision.gameObject.CompareTag("Enemy"))
                {
                    Boon_yeol_gwi entity = collision.GetComponent<Boon_yeol_gwi>();

                    if (entity != null)
                    {
                        //�浹�Ѱ� ��ü�϶�
                        if (entity.type == Boon_yeol_gwi_Type.Entity &&
                            isItemEat && isItemEat)
                        {
                            //��ġ�� �÷���
                            entity.enemyPrice += enemyPrice;

                            //�ڱ� �ڽ��� ����Ʈ���� ����
                            if (entity.copyObjList.Contains(gameObject))
                            {
                                entity.copyObjList.Remove(gameObject);
                            }
                            //����Ʈ ����
                            for (int i = entity.copyObjList.Count - 1; i >= 0; i--)
                            {
                                if (entity.copyObjList[i] == null)
                                {
                                    entity.copyObjList.RemoveAt(i);
                                }
                            }

                            //�׸��� ������ �����
                            Destroy(transform.parent.gameObject);

                            //���� �ε����� ����
                            entity.currentIndex--;

                            //�׸��� �����ϴ� Ʈ���� Ȱ��ȭ
                            entity.isSpawn = true;
                        }
                    }
                }
            }
        }
    }
}
