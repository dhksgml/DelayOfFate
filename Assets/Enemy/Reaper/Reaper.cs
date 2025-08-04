using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Reaper : Enemy
{
    PlayerController player; //플레이어

    //강해지는데 걸리는 시간
    [SerializeField] float powerUpTime;
    //이동속도 상승치
    [SerializeField] float speedUpValue;
    //크기 상승치
    [SerializeField] Vector3 scaleUpValue;
    //콜라이더 상승치
    [SerializeField] float colliderUpValue;
    //콜라이더
    [SerializeField] CircleCollider2D circleCollider;

    // 텍스트 바
    [SerializeField] GameObject summon_Text_Bar;
    [SerializeField] TextMeshProUGUI summon_Text;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        EnemyInt();
    }

    void Start()
    {
        // 저승사자 소환시 투명상태였다가 alpha값 회복
        StartCoroutine("ReaperAlpha");

        // 오브젝트를 찾아와서 저장해줌. 이 방식은 이름으로 찾는 것
        summon_Text_Bar = GameObject.Find("SummonReaper_Panel");
        summon_Text = GameObject.Find("Summon_Text").GetComponent<TextMeshProUGUI>();

        if (summon_Text == null || summon_Text_Bar == null)
        {
            Debug.Log("둘 중 하나가 존재하지 않음");
            return;
        }
        else
        {
            StartCoroutine(SummonText());
        }
    }

    void Update()
    {
        // 나중에 사망처리도 넣어줘야함
        EnemyMove();
    }

    //활성화시 실행
    void OnEnable()
    {
        StartCoroutine(ScaleAndSpeedUp());
        DeathJangseungStop();
    }

    //플레이어가 닿았을 시
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if(collision != null)
            {
                PlayerController player = collision.gameObject.GetComponent<PlayerController>();
                // 데미지 처리 부분
                player.DamagedHP(444444);
            }
        }

        //적 피격 부분
        //이부분은 아마 적 공동 코드로 사용할 것 같다.
        Attack_sc attack = collision.GetComponent<Attack_sc>();

        if (collision.gameObject.CompareTag("Attack") && !isEnemyHit && attack != null)
        {
            // 사신은 약점이 없으므로 주석처리
            // 타입이 일치하면 즉사
            //if (attack.attackType.ToString() == enemyWeakness.ToString())
            //{
            //    //이부분 없다 나와서 일단 주석 처리 해주었음.
            //    //attack.CheckWeakness();
            //    enemyHp = 0f;
            //}
            //else
            //{
            //    enemyHp -= attack.damage;
            //}

            // 데미지 처리
            enemyHp -= attack.damage;

            EnemyHit(attack.damage);
            Invoke("EnemyHitRegen", enemyHitTime);
        }
    }

    public override void EnemyMove()
    {
        //추적하는 타겟의 위치 - 자신의 위치를 구한 후 정규화를 해준다
        enemyTargetDir = (player.transform.position - transform.position).normalized;

        // 스프라이트 회전 처리
        EnemyTraceTurn2();

        rigid.MovePosition(transform.position + enemyTargetDir * enemyMoveSpeed * Time.deltaTime);
    }

    //40초마다 커지게 하기 위함
    IEnumerator ScaleAndSpeedUp()
    {
        while (true)
        {
            //지정한 시간만큼 딜레이를 줌
            yield return new WaitForSeconds(powerUpTime);

            // 크기 증가
            transform.localScale += scaleUpValue;

            // 이동속도 증가
            enemyMoveSpeed += speedUpValue;

            // 콜라이더 크기 증가
            circleCollider.radius += colliderUpValue;
        }
    }

    // 저승사자 툼명도 조절
    public IEnumerator ReaperAlpha()
    {
        Color color = sp.color;

        // 알파를 0으로 시작
        color.a = 0f;
        sp.color = color;

        // 알파를 0.0 → 1.0까지 증가시킴
        for (float i = 0.0f; i <= 1.0f; i += 0.01f)
        {
            color.a = i;
            sp.color = color;
            yield return new WaitForSeconds(0.05f);
        }
    }

    public void DeathJangseungStop()
    {
        Death_Jangseung[] death_Jangseungs = FindObjectsOfType<Death_Jangseung>();

        foreach (var js in death_Jangseungs)
        {
            js.isSpawnReaper = true;
        }
    }


    [SerializeField] float summonTextTime;

    public IEnumerator SummonText()
    {
        float currentTime = 0f;

        // 컴포넌트 얻기
        Image barImage = summon_Text_Bar.GetComponent<Image>();
        Color barColor = barImage.color;
        Color textColor = summon_Text.color;

        while (currentTime < summonTextTime)
        {
            float alpha = Mathf.Lerp(0f, 1f, currentTime / summonTextTime);

            barColor.a = alpha;
            textColor.a = alpha;

            barImage.color = barColor;
            summon_Text.color = textColor;

            currentTime += Time.deltaTime;
            yield return null;
        }

        // 마지막 값 고정
        barColor.a = 0.5f;
        textColor.a = 0.5f;
        barImage.color = barColor;
        summon_Text.color = textColor;

        // 3초 대기
        yield return new WaitForSeconds(4f);

        // 1 → 0 : 점점 사라지게
        currentTime = 0f;
        while (currentTime < summonTextTime)
        {
            float alpha = Mathf.Lerp(1f, 0f, currentTime / summonTextTime);

            barColor.a = alpha;
            textColor.a = alpha;

            barImage.color = barColor;
            summon_Text.color = textColor;

            currentTime += Time.deltaTime;
            yield return null;
        }

        // 완전 투명하게 마무리
        barColor.a = 0f;
        textColor.a = 0f;
        barImage.color = barColor;
        summon_Text.color = textColor; 
    }
}
