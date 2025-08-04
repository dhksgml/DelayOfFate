using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tal_hon_gwi : Enemy
{
    [Header("탈혼귀")]
    [SerializeField] Sprite[] randomImages;
    [SerializeField] Sprite[] talhongwiOriginSprite;
    [HideInInspector] public bool isSeek = false;
    [SerializeField] bool isDestroy = false;
    [SerializeField] int talhongwiDamage = 20;
    PlayerController player;

    void Awake()
    {
        // 이미지를 랜덤으로 가져와 줌
        int random = Random.Range(0, randomImages.Length);
        sp.sprite = randomImages[random];

        // 회전 값을 위한 랜덤
        int randomFlipX = Random.Range(0, 2);
        int randomFlipY = Random.Range(0, 2);

        // X회전
        if (randomFlipX == 0) { sp.flipX = true; }
        else { sp.flipX = false; }

        // Y회전
        if (randomFlipY == 0) { sp.flipY = true; }
        else { sp.flipY = false; }

        rigid = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    void Start()
    {
        EnemyInt();
    }


    void Update()
    {
        //적의 체력이 0이하일시.
        if (enemyHp <= 0 && !isDie)
        {
            isDie = true;

            sp.sprite = talhongwiOriginSprite[1];

            StartCoroutine(EnemyDie());
        }

        // 플레이어가 E키를 누르면 
        if (isSeek && !isDestroy)
        {
            isDestroy = true;

            if (player == null) { return; }

            // 본모습 등장
            sp.sprite = talhongwiOriginSprite[0];

            // 정신 데미지
            player.DamagedMP(talhongwiDamage);
            // 기력 데미지
            player.DamagedSP(player.maxSp * 0.3f);

            StartCoroutine(EnemySeek());
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {
            //적 피격 부분
            //이부분은 아마 적 공동 코드로 사용할 것 같다.
            Attack_sc attack = collision.GetComponent<Attack_sc>();

            if (collision.gameObject.CompareTag("Attack") && !isEnemyHit && attack != null)
            {
                // 타입이 일치하면 즉사
                if (attack.attackType.ToString() == enemyWeakness.ToString())
                {
                    //이부분 없다 나와서 일단 주석 처리 해주었음.
                    //attack.CheckWeakness();
                    enemyHp = 0f;
                }
                else
                {
                    enemyHp -= attack.damage;
                }

                EnemyHit(attack.damage);
                Invoke("EnemyHitRegen", enemyHitTime);
            }
        }
    }


    public override void EnemyMove()
    {

    }

    public IEnumerator EnemySeek()
    {
        // 사망시 이펙트 
        Instantiate(enemyDeathEffect, transform.position, Quaternion.identity);

        Color color = sp.color;

        //먼저 추적 범위와 공격 범위를 지워줌.
        Destroy(enemyTrace);
        Destroy(enemyAttack);
        Destroy(enemyColl);
        Destroy(rigid);

        // 이동속도 0으로 해서 움직이지 못하게
        enemyMoveSpeed = 0;


        //투명도 값을 1.0에서 0.01씩 뺴주면서 천천히 투명하게 해줌
        for (float i = 1.0f; i >= 0.0f; i -= 0.02f)
        {
            color.a = i;
            sp.color = color;
            //딜레이를 위해 코루틴을 사용해줌
            yield return new WaitForSeconds(0.01f);
        }
        if (SoundManager.Instance != null) SoundManager.Instance.PlaySFX(Resources.Load<AudioClip>("SFX/sfx_ghost_death"));
        
        // 시체의 부모 통째로 제거
        Destroy(transform.parent.gameObject);
    }
}
