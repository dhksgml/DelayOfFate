using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

//적의 공격 타입
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
    public int             enemyDamage; //적의 공격력
    public float           enemyAttackSpeed; //적의 공격 속도
    public float           enemyAttackRange; //적의 공격 범위

    [Space(20f)]
    public EnemyAttackType enemyAttackType; //공격 타입 열거형 선언

    [Header("Reference")]
    public Collider2D      enemyAttackCollider; //적 공격 콜라이더
    
    public Enemy           enemy;

    public float time;
    

    public void rotationColl()
    {
        // 타겟 방향과 현재 오브젝트의 회전 각도를 계산
        float angle = Mathf.Atan2(enemy.enemyTargetDir.y, enemy.enemyTargetDir.x) * Mathf.Rad2Deg;
        // z축 회전만 적용
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

}

