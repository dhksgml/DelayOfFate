using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using Unity.VisualScripting;
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
    public Boon_yeol_gwi entityObj; // ��ü
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

            Boon_yeol_gwi copyBoon = copyEnemy.GetComponentInChildren<Boon_yeol_gwi>();
            
            if (copyBoon == null) { Debug.LogWarning("�ڽľ���"); }
            copyBoon.entityObj = gameObject.GetComponentInChildren<Boon_yeol_gwi>();

            //����Ʈ�� �߰�����
            copyObjList.Add(copyEnemy);
            currentIndex++; //1���� ���������Ƿ� ++
        }
    }
    float listTime = 0;
    void Update()
    {
        // ����Ʈ ����
        if (!isDie) { listTime += Time.deltaTime; }

        if (listTime >= 0.5f)
        {
            listTime = 0;
            RemoveList();
        }
        // �п�ü ���� ����
        if (enemyHp <= 0 && type == Boon_yeol_gwi_Type.Copy)
        {
            entityObj.copyObjList.Remove(gameObject);
            entityObj.RemoveList();
        }

        //���� ü���� 0�����Ͻ�.
        if (enemyHp <= 0 && !isDie)
        {
            isDie = true;

            foreach (var boon_yeol_che in copyObjList)
            {
                Destroy(boon_yeol_che);
            }

            // ��ü ���ó��
            if (type == Boon_yeol_gwi_Type.Entity)
            {
                StartCoroutine(EnemyDie());
            }
            // �п�ü ��� ó��
            else if (type == Boon_yeol_gwi_Type.Copy)
            {
                // �׳� �Ҹ�ó��
                StartCoroutine(CopyDie());
            }
            
        }

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
            if (copyObjList.Count <= maxIndex)
            {
                //�θ��� ��ȯ
                GameObject copyEnemy = Instantiate(copyObj, transform.position, Quaternion.identity);

                Boon_yeol_gwi copyBoon = copyEnemy.GetComponentInChildren<Boon_yeol_gwi>();

                if (copyBoon == null) { Debug.LogWarning("�ڽľ���"); }
                copyBoon.entityObj = gameObject.GetComponentInChildren<Boon_yeol_gwi>();

                copyObjList.Add(copyEnemy);



                currentIndex++;

                if (copyObjList.Count <= maxIndex)
                {
                    GameObject copyEnemy2 = Instantiate(copyObj, transform.position, Quaternion.identity);

                    Boon_yeol_gwi copyBoon2 = copyEnemy2.GetComponentInChildren<Boon_yeol_gwi>();

                    if (copyBoon2 == null) { Debug.LogWarning("�ڽľ���"); }
                    copyBoon2.entityObj = gameObject.GetComponentInChildren<Boon_yeol_gwi>();

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
        if (type == Boon_yeol_gwi_Type.Copy)
        {
            // ��ü �߰�
            if (isEntityFind)
            {
                Vector3 target = (targetTrs - transform.position).normalized;
                transform.Translate(target * enemyMoveSpeed * Time.deltaTime);
            }

            // ������ ��ġ �Ҹ� �� ����
            else if (isItemEat)
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
                    enemyHp = 0f;
                }
                else
                {
                    enemyHp -= attack.damage;
                }

                EnemyHit(attack.damage);
                Invoke("EnemyHitRegen", enemyHitTime);
            }

            //item�� �������� ��
            ItemObject item = collision.gameObject.GetComponent<ItemObject>();

            if (collision.gameObject.CompareTag("Item") && item.itemData.Coin != 0 && !isItemEat)
            {
                //��ġ�� �÷��� ��
                enemyPrice += item.itemData.Coin;

                // �������� ��������
                Destroy(item.gameObject);

                isItemEat = true;
            }


            if (type == Boon_yeol_gwi_Type.Copy)
            {
                if (collision.gameObject.CompareTag("Enemy"))
                {
                    Boon_yeol_gwi entity = collision.GetComponent<Boon_yeol_gwi>();

                    //�浹�Ѱ� ��ü�϶�
                    if (entity == entityObj)
                    {
                        if (entity.type == Boon_yeol_gwi_Type.Entity && isItemEat && isItemEat)
                        {
                            //��ġ�� �÷���
                            entity.enemyPrice += enemyPrice;

                            entity.copyObjList.Remove(gameObject);

                            RemoveList();

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
    // �п�ü ���� ���ó��
    IEnumerator CopyDie()
    {
        Debug.Log("�׽�Ʈ");

        // ����� ����Ʈ 
        Instantiate(enemyDeathEffect, transform.position, Quaternion.identity);

        Color color = sp.color;

        //���� ���� ������ ���� ������ ������.
        Destroy(enemyTrace);
        Destroy(enemyAttack);
        Destroy(enemyColl);
        Destroy(rigid);

        // �̵��ӵ� 0���� �ؼ� �������� ���ϰ�
        enemyMoveSpeed = 0;


        //���� ���� 1.0���� 0.01�� ���ָ鼭 õõ�� �����ϰ� ����
        for (float i = 1.0f; i >= 0.0f; i -= 0.02f)
        {
            color.a = i;
            sp.color = color;
            //�����̸� ���� �ڷ�ƾ�� �������
            yield return new WaitForSeconds(0.01f);
        }
       
        Destroy(transform.parent.gameObject);
    }

    public void RemoveList()
    {
        //����Ʈ ����
        copyObjList.RemoveAll(obj => obj == null);

        // ��ü�� �п�ü�� �����ִ��� Ȯ���ϱ� ����
        if (copyObjList.Count <= 0 && type == Boon_yeol_gwi_Type.Entity)
        {
            StartCoroutine(EnemyDie());
        }
    }
}
