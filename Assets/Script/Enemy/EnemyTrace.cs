using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTrace : MonoBehaviour
{
    [Header("Enemy Trace Range")]
    public float     enemyTraceRange; //적의 추격 범위

    [SerializeField]
    Collider2D       enemyTraceCollider; //적의 추격 범위를 설정해줄 콜라이더

    [Header("Reference")]
    public Vector3   targetPos; //플레이어의 위치
    [SerializeField]
    Enemy            enemy;
    // 땅상어
    [SerializeField] LandShark landShark;
    [SerializeField] LandSharkAttack landSharkAttack;
    // 소면귀
    [SerializeField] Somyeon_gwi somyeon_Gwi;
    // 분열귀
    [SerializeField] Boon_yeol_gwi boon_yeol_gwi;
    // 무면귀
    [SerializeField] Mumyeon_Gwi mumyeon_Gwi;
    // 죽음장승
    [SerializeField] Death_Jangseung death_Jangseung;
    [SerializeField] Death_Jangseung_Attack death_Jangseung_Attack;
    // 두억시니
    [SerializeField] Duoksini duoksini;
    [SerializeField] Duoksini_Attack duoksini_Attack;

    void Awake()
    {
        //콜라이더가 원형일시 크기 조절
        if (enemyTraceCollider is CircleCollider2D circleColl) circleColl.radius = enemyTraceRange;
    }

    void Update()
    {
        //수정하면서. 자식이 아닌 다른걸로 분리해줬기에 따라가게 해줌
        transform.position = enemy.transform.position;

        // 죽음 장승 전용
        if (death_Jangseung_Attack != null && !death_Jangseung_Attack.isAttack)
        {
            jangseungtime += Time.deltaTime;
        }
        // 두억시니 전용
        else if (duoksini_Attack != null && !duoksini_Attack.isAttack)
        {
            duoksinitime += Time.deltaTime;
        }
    }

    //땅상어 전용
    [HideInInspector]
    public float landSharkAttackTime;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision != null)
        {
            //땅상어 전용
            if (landShark != null)
            {
                if (collision.gameObject.CompareTag("Player"))
                {
                    //잠복으로 변환
                    landShark.IsHide();
                }
            }

            //소면귀 전용
            if (somyeon_Gwi != null)
            {
                //만약 아이템 태그를 발견하면
                if (collision.gameObject.CompareTag("Item"))
                {
                    //트리거 활성화 후
                    somyeon_Gwi.isFindItem = true;
                    //위치값을 전달해줌
                    somyeon_Gwi.findItemVec = collision.gameObject.transform.position;
                }
            }

            //분열귀 전용
            if (boon_yeol_gwi != null)
            {
                //item을 가져와준 후
                ItemObject item = collision.gameObject.GetComponent<ItemObject>();

                //아이템을 찾고 가치를 먹었고, 충돌한 오브젝트가 적이면
                if (boon_yeol_gwi.isItemFind && boon_yeol_gwi.isItemEat && collision.gameObject.CompareTag("Enemy"))
                {
                    //컴포넌트를 가져온 후
                    Boon_yeol_gwi entity = collision.GetComponent<Boon_yeol_gwi>();

                    //값이 맞으면
                    if (entity != null)
                    {
                        //true로 해준 후
                        boon_yeol_gwi.isEntityFind = true;
                        //위치를 보내줌
                        boon_yeol_gwi.targetTrs = entity.transform.position;
                    }
                }

                //찾은게 아이템이면
                if (collision.gameObject.CompareTag("Item") && item.itemData.Coin != 0)
                {
                    boon_yeol_gwi.isItemFind = true;
                    boon_yeol_gwi.itemTrs = collision.gameObject.transform.position;
                }


            }
        }
    }

    //땅상어 전용
    float landSharkOutTime;

    // 무면귀 전용
    float mumyeon_Gwi_Stay_Time;

    // 죽음 장승 전용
    Vector3 jangseungTargetTrs;
    float jangseungtime = 0f;

    // 두억시니 전용
    Vector3 duoksiniTargetTrs;
    float duoksinitime = 0f;

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision != null)
        {
            //분열귀 전용
            if (boon_yeol_gwi != null)
            {
                //item을 가져와준 후
                ItemObject item = collision.gameObject.GetComponent<ItemObject>();

                if (boon_yeol_gwi.isItemEat)
                {
                    if (collision.gameObject.CompareTag("Item") && item.itemData.Coin == 0)
                    {
                        boon_yeol_gwi.isItemFind = false;
                        return;
                    }
                }

                //찾은게 아이템이면
                if (collision.gameObject.CompareTag("Item") && item.itemData.Coin != 0)
                {
                    boon_yeol_gwi.isItemFind = true;
                    boon_yeol_gwi.itemTrs = collision.gameObject.transform.position;
                }
                
                else if (collision.gameObject.CompareTag("Item") && item.itemData.Coin == 0)
                {
                    boon_yeol_gwi.isItemFind = false;
                }


            }

            //플레이어가 범위 안에 들어오면 쫒아가줌
            if (collision.gameObject.CompareTag("Player"))
            {
                targetPos = collision.gameObject.transform.position;
                enemy.isTrace = true;
            }

            //땅상어 전용
            if (landShark != null)
            {
                if (collision.gameObject.CompareTag("Player"))
                {
                    //돌출 상태일떄
                    if(landShark.isOut)
                    {
                        landSharkOutTime += Time.deltaTime;

                        //5초가 지나면
                        if(landSharkOutTime >= 5f)
                        {
                            //잠복으로 변환
                            landShark.IsHide();
                            //초기화
                            landSharkOutTime = 0f;
                        }
                    }    


                    PlayerController player = collision.GetComponent<PlayerController>();

                    //범위 내에서 움직임을 감지 && 공격준비가 됐으면
                    if (player.isMoving && landShark.isAttackReady && landShark.isIn)
                    {
                        landSharkAttackTime += Time.deltaTime;

                        if(landSharkAttackTime >= 0.5f)
                        {
                            landSharkAttack.JumpAttack();
                        }
                    }
                }

            }

            // 무면귀 전용
            if (mumyeon_Gwi != null)
            {
                if (collision.gameObject.CompareTag("Player"))
                {
                    mumyeon_Gwi_Stay_Time += Time.deltaTime;
                    // 무면귀가 추적하는 시간
                    if (mumyeon_Gwi_Stay_Time >= 5f)
                    {
                        // 추적을 활성화 해줌
                        mumyeon_Gwi.isTrace = true;
                    }
                }
            }
            // 죽음 장승 전용
            if (death_Jangseung != null)
            {
                if (collision.gameObject.CompareTag("Player"))
                {
                    duoksiniTargetTrs = collision.transform.position;

                    if (jangseungtime >= death_Jangseung.attackSeeTime && !death_Jangseung.isAttackReady)
                    {
                        death_Jangseung.attackTargetTrs = jangseungTargetTrs;

                        death_Jangseung.isAttackReady = true;

                        jangseungtime = 0f;
                    }

                }
            }
            // 두억시니 전용
            if (duoksini != null)
            {
                if (collision.gameObject.CompareTag("Player"))
                {
                    duoksiniTargetTrs = collision.transform.position;

                    if (duoksinitime >= duoksini.attackSeeTime && !duoksini.isAttackReady)
                    {
                        duoksini.isAttack = true;   

                        duoksini.attackTargetTrs = duoksiniTargetTrs;

                        duoksini.isAttackReady = true;

                        duoksinitime = 0f;
                    }

                }
            }

        }

    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision != null)
        {
            //벗어나면 false로 변경
            if (collision.gameObject.CompareTag("Player"))
            {
                enemy.isTrace = false;

                //땅상어 전용
                if (landShark != null)
                {
                    //돌출로 변환
                    landShark.isHideOut();
                }

                // 무면귀 전용
                if (mumyeon_Gwi != null)
                {
                    // 벗어나면 시간 초기화
                    mumyeon_Gwi_Stay_Time = 0f;
                }

            }
        }
    }
}
