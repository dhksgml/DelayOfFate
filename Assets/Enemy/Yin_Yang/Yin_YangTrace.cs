using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Yin_YangTrace : EnemyAttack
{
    [Header("Yin_YangTrace")]
    [SerializeField] Yin_Yang yinyang;
    [SerializeField] GameObject follow;
    [SerializeField] float attackDelay;
    public Vector3 target;

    void Update()
    {
        transform.position = follow.transform.position;
        attackTime += Time.deltaTime;
    }

    float attackTime;

    void OnTriggerStay2D(Collider2D collision)
    {
        //�� �±��̸�
        if (collision.gameObject.CompareTag("Enemy"))
        {
            //Yin_Yang ��ũ��Ʈ�� ������ ��
            Yin_Yang yinYang = collision.GetComponent<Yin_Yang>();

            if (yinyang.type == Yin_Yang_Type.Yin && yinYang.type == Yin_Yang_Type.Yang)
            {
                //Ÿ���� ��ǥ�� �������� bool���� true�� ���ش�.
                target = collision.transform.position;
                yinyang.isFind = true;
            }

            else if (yinyang.type == Yin_Yang_Type.Yang && yinYang.type == Yin_Yang_Type.Yin)
            {
                //Ÿ���� ��ǥ�� �������� bool���� true�� ���ش�.
                target = collision.transform.position;
                yinyang.isFind = true;
            }
        }
        //���� �ð��� 1�ʷ� ���־���. �ʿ�� ����
        if (collision.gameObject.CompareTag("Player") && attackTime >= attackDelay)
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();

            switch(yinyang.type)
            {
                case Yin_Yang_Type.Yin:
                    player.DamagedHP(enemyDamage); //��������ŭ ����. �ٵ� -3���� �س���
                    attackTime = 0;
                    break;

                case Yin_Yang_Type.Yang:
                    player.currentMp += enemyDamage; //���� ȸ��

                    // ������ �ִ�ġ�� �Ѿ�� �ִ밪�� ����������
                    if (player.currentMp >= player.maxMp)
                    {
                        player.currentMp = player.maxMp;
                    }

                    attackTime = 0;
                    break;
            }
        }
    }
}
