using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

//���� ���� Ÿ��
public enum EnemyAttackType
{
    HpAttack,
    StaminaAttack,
    MentalityAttack,
    None
}


public abstract class EnemyAttack : MonoBehaviour
{

    [Header("Enemy Attack Stat")]
    public int             enemyDamage; //���� ���ݷ�
    public float           enemyAttackSpeed; //���� ���� �ӵ�
    public float           enemyAttackRange; //���� ���� ����

    [Space(20f)]
    public EnemyAttackType enemyAttackType; //���� Ÿ�� ������ ����

    [Header("Reference")]
    public Collider2D      enemyAttackCollider; //�� ���� �ݶ��̴�
    
    public Enemy           enemy;

    public float time;
    

    public void rotationColl()
    {
        // Ÿ�� ����� ���� ������Ʈ�� ȸ�� ������ ���
        float angle = Mathf.Atan2(enemy.enemyTargetDir.y, enemy.enemyTargetDir.x) * Mathf.Rad2Deg;
        // z�� ȸ���� ����
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

}

