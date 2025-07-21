using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using Color = UnityEngine.Color;

public class WomanGhost : Enemy
{

    //������� ��ġ
    Vector3 invisibleTrans;
    //������� ���߱� ����
    [SerializeField] bool isStop;
    //�÷��̾ �����ִ��� Ȯ��
    [SerializeField] bool isPlayerSee;
    bool isinvisible;
    bool isAction = false;
    public bool isAttack;
    bool isWomanTrace = false;
    

    [SerializeField] float seeTime;
    [SerializeField] float dontSeeTime;

    PlayerController player; //�÷��̾�
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        EnemyInt();
    }

    void Start()
    {
        // ó���� ������ ���� ����
        ChooseNewDirection();

        // �ֱ������� ���� ��ȯ
        StartCoroutine(ChangeDirectionRoutine());
    }
    void Update()
    {
        if (isTrace) { isWomanTrace = true; }
        
        if (!isPlayerSee && isinvisible) { dontSeeTime += Time.deltaTime; }
       
        //�Ⱥ��� 5�ʰ� ������ �ٽ� ������
        if (dontSeeTime >= 5f && !isPlayerSee && !isAttack && !isAction)
        {
            StartCoroutine(PlayerDontSee());
            dontSeeTime = 0f;
        }

        //���� ü���� 0�����Ͻ�.
        if (enemyHp <= 0 && !isDie)
        {
            isDie = true;
            StartCoroutine(EnemyDie());
        }

        EnemyMove();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {
            Attack_sc attack = collision.GetComponent<Attack_sc>();

            if (collision.gameObject.CompareTag("Attack") && !isEnemyHit && attack != null)
            {
                Debug.Log(1);
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

            //�÷��̾��� �þ߰� ������
            if (collision.gameObject.CompareTag("Sight"))
            {
                isPlayerSee = true;
            }
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        //����  �÷��̾ �� ���͸� ���Ѻ��� �ִٸ�
        if (collision.gameObject.CompareTag("Sight") && isPlayerSee)
        {
            seeTime += Time.deltaTime;

            if (seeTime >= 3f && isPlayerSee && !isStop && !isAttack && !isAction)
            {
                StartCoroutine(PlayerSee());
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Sight"))
        {
            isPlayerSee = false;
        }
    }

    public override void EnemyMove()
    {
        //�����ϴ� Ÿ���� ��ġ - �ڽ��� ��ġ�� ���� �� ����ȭ�� ���ش�
        enemyTargetDir = (player.transform.position - transform.position).normalized;

        //�÷��̾ ���Ѻ���
        if (isPlayerSee && !isDie && !isEnemyHit && !isStop)
        {
            EnemyTraceTurn();

            anim.SetBool("isTrace", false);

            // �ѹ� ���������� ������ �i�ƿ�
            isWomanTrace = false;

            isTrace = false;

            //�ݴ� �������� ��������
            rigid.MovePosition(transform.position + -enemyTargetDir * enemyRunSpeed * Time.deltaTime);
        }

        //�������϶�
        else if (isWomanTrace && !isDie && !isEnemyHit && !isStop && !isPlayerSee)
        {
            EnemyTraceTurn2();
   
            //���ϸ��̼�, ���� true �ٲپ���
            anim.SetBool("isTrace", true);

            // ��ǥ�� ���� �̵�
            transform.Translate(enemyTargetDir * enemyMoveSpeed * 3 * Time.deltaTime);

        }

        //�������� �ƴϸ�
        else if (!isTrace && !isDie && !isEnemyHit && !isStop && !isWomanTrace)
        {
            //��������Ʈ ������ �̰� �������
            EnemyNormalTurn2();

            //���ϸ��̼�, ���� false�� �ٲپ���
            anim.SetBool("isTrace", false);

            // ���� �������� �̵�
            transform.Translate(moveDirection * enemyMoveSpeed * Time.deltaTime);
        }
    }

    IEnumerator PlayerSee()
    {
        isStop = true;
        Color color = sp.color;

        //���� ��ġ���� �������� ��
        invisibleTrans = transform.position;

        for (float i = 1.0f; i >= 0.0f; i -= 0.01f)
        {
            color.a = i;
            sp.color = color;
            //�����̸� ���� �ڷ�ƾ�� �������
            yield return new WaitForSeconds(0.01f);
        }

        seeTime = 0f;
        isinvisible = true;

    }

    //5��?7�� �ڿ��� �ڷ�ƾ �������� Time.delta�� �����ִ°ɷ� 
    IEnumerator PlayerDontSee()
    {

        Color color = sp.color;

        for (float i = 0.0f; i <= 1.0f; i += 0.01f)
        {
            color.a = i;
            sp.color = color;
            //�����̸� ���� �ڷ�ƾ�� �������
            yield return new WaitForSeconds(0.01f);
        }

        isinvisible = false;
        isStop = false;

    }

    public IEnumerator PlayerPossession()
    {
        anim.SetBool("isAttack", true);
        Color color = sp.color;
        //���� -3 ~ -5, -8 ~ -12, -10 ~ -11�� ���־���
        int randomHpDamage = Random.Range(3, 6);
        int randomMpDamage = Random.Range(8, 13);
        int randomSpDamage = Random.Range(10, 12);

        //�÷��̾ �̵� ���ϰ� ����. �̺κ��� ��ũ��Ʈ �������°ɷ�
        player.isFreeze = true;
        isStop = true;
        isinvisible = true;
        isAttack = true;

        for (float i = 1.0f; i >= 0.0f; i -= 0.01f)
        {
            color.a = i;
            sp.color = color;
            //�����̸� ���� �ڷ�ƾ�� �������
            yield return new WaitForSeconds(0.01f);
        }

        while (player.isFreeze)
        {

            //ä��, ���ŷ�, ����� 1�ʸ��� ���ҵ�
            player.DamagedHP(randomHpDamage);
            player.DamagedMP(randomMpDamage);
            player.DamagedSP(randomSpDamage);

            yield return new WaitForSeconds(1f);
        }

        for (float i = 0.0f; i <= 1.0f; i += 0.01f)
        {
            color.a = i;
            sp.color = color;
            //�����̸� ���� �ڷ�ƾ�� �������
            yield return new WaitForSeconds(0.01f);
        }

        player.isFreeze = false;
        isAttack = false;
        isinvisible = false;
        isStop = false;
        //10���� ��ä��� �ٽ� ��Ÿ���� ����
        anim.SetBool("isAttack", false);
    }
}
