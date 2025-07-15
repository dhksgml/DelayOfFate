using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WomanGhostAttack : EnemyAttack
{
    [SerializeField] WomanGhost womanGhost;
    [HideInInspector] PlayerController player;


     void Awake()
    {
        //�÷��̾ ã�Ƽ� �������� ��
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();   
    }


    void Update()
    {
        //�Ÿ��� ����ϰ�
        float distance = Vector3.Distance(transform.position, player.transform.position);

        transform.position = enemy.transform.position;
        if(!womanGhost.isAttack) time += Time.deltaTime;

        rotationColl();

        //���� �ð��� �ư�, ��ǥ���� �Ÿ��� 1f ���� �۰ų� ������ ����
        if (time >= enemyAttackSpeed && !womanGhost.isAttack 
            && distance <= 1f)
        {
            //���ϸ��̼� Ȱ��ȭ
            womanGhost.anim.SetBool("isAttack", true);

            //���� �ݶ��̴� Ȱ��ȭ ����
            enemyAttackCollider.enabled = true;
            enemy.isEnemyAttack = false;
            time = 0;
            Invoke("Delay", 0.5f);
        }
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player") && !womanGhost.isAttack)
        {
            //ó��ͽ� ��ũ��Ʈ�� �ۼ��صξ��� �ڷ�ƾ ����
            StartCoroutine(womanGhost.PlayerPossession());
        }
    }

    public void Delay()
    {
        enemyAttackCollider.enabled = false;
        womanGhost.anim.SetBool("isAttack", false);
    }
}
