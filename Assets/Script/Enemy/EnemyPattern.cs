using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//���� Ʈ���� ����, �Ⱥ���, �÷��̾� �ֺ�, �÷��̾� ������, �״´ٷ� ����
public enum EnemyTrigger
{
    See,
    Not_See,
    Around_Player,
    Moving_Player,
    Die
}
//���� �ൿ ����, ����, �״´�, ��ģ�ٷ� ����
public enum EnemyAction
{
    Attack,
    Run,
    Die,
    Steal
}

public class EnemyPattern : MonoBehaviour
{
    //���� Ʈ����
    public EnemyTrigger enemyTrigger;
    //���� �ൿ
    public EnemyAction enemyAction;

    [Header("Reference")]
    [SerializeField]
    Enemy              enemy;

    //���� ������ �ʾƵ� �÷��̾ �i�ư��� ������
    public GameObject      player;

    void Awake()
    {
        //�÷��̾ ������ �ʾƵ� �i�ư��� ���͸� ������
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        //������ �ʾƵ� && �i�ư� ���� ��ħ
        if (enemyTrigger == EnemyTrigger.Not_See && enemyAction == EnemyAction.Steal && !enemy.isEnemyChase)
        {
            enemy.isEnemyChase = true;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {
            if(collision.gameObject.CompareTag("Player"))
            {
                if (enemyTrigger == EnemyTrigger.Not_See && enemyAction == EnemyAction.Steal)
                {
                    //enemy.isSteal = true;
                    Debug.Log("�÷��̾��� ��尡 �����Ͽ����ϴ�");
                    //���� �̰����� �浹�� �ݶ��̴����Լ� �÷��̾ �������ų�
                    //�̱������� ������ ���Ӹ޴������� ���� ���ҽ�Ű�� ���ָ� �� �� ����
                }
            }
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision != null)
        {
            //�÷��̾ ���� �ȿ� ������ �i�ư���
            if (collision.gameObject.CompareTag("Player"))
            {
                //���� && ������
                if (enemyTrigger == EnemyTrigger.See && enemyAction == EnemyAction.Run)
                {
                    enemy.isEnemyRun = true;
                }

                //���� && �����Ϸ���
                else if (enemyTrigger == EnemyTrigger.See && enemyAction == EnemyAction.Attack)
                {
                    enemy.isEnemyAttack = true;
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision != null)
        {
            //�÷��̾ ���� �ȿ� ������ �i�ư���
            if (collision.gameObject.CompareTag("Player"))
            {
                if (enemyTrigger == EnemyTrigger.See && enemyAction == EnemyAction.Run)
                {
                    Invoke("Delay", 1f);
                }
            }
        }
    }

    void Delay()
    {
        enemy.isEnemyRun = false;
    }

}
