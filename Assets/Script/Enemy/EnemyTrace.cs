using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

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

    [System.Serializable]
    public class EnemyScript
    {
        [Header("땅상어")]
        public LandShark landShark;
        public LandSharkAttack landSharkAttack;

        [Header("소면귀")]
        public Somyeon_gwi somyeon_Gwi;

        [Header("분열귀")]
        public Boon_yeol_gwi boon_yeol_gwi;

        [Header("무면귀")]
        public Mumyeon_Gwi mumyeon_Gwi;

        [Header("죽음장승")]
        public Death_Jangseung death_Jangseung;
        public Death_Jangseung_Attack death_Jangseung_Attack;

        [Header("두억시니")]
        public Duoksini duoksini;
        public Duoksini_Attack duoksini_Attack;

        [Header("어둑쥐")]
        public Eo_dook_jwi eo_dook_jwi;

        // 어둑쥐 전용
        [Tooltip("추후 플레이어의 Intensity가 바뀔떄 같이 해줘야 함")]
        public float playerOrigin_Light_Intensity;
    }

    [SerializeField] EnemyScript enemyScript;

    void Awake()
    {
        //콜라이더가 원형일시 크기 조절
        if (enemyTraceCollider is CircleCollider2D circleColl) circleColl.radius = enemyTraceRange;
    }

    float boon_yeol_gwi_itemTime = 0;

    void Update()
    {
        //수정하면서. 자식이 아닌 다른걸로 분리해줬기에 따라가게 해줌
        transform.position = enemy.transform.position;

        // 죽음 장승 전용
        if (enemyScript.death_Jangseung_Attack != null && !enemyScript.death_Jangseung_Attack.isAttack)
        {
            jangseungtime += Time.deltaTime;
        }
        // 두억시니 전용
        else if (enemyScript.duoksini_Attack != null && !enemyScript.duoksini_Attack.isAttack)
        {
            duoksinitime += Time.deltaTime;
        }
        // 분열귀
        else if (enemyScript.boon_yeol_gwi != null)
        {
            boon_yeol_gwi_itemTime += Time.deltaTime;

            if (boon_yeol_gwi_itemTime >= 1f)
            {
                boon_yeol_gwi_itemTime = 0;
                enemyScript.boon_yeol_gwi.isItemFind = false;
            }
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
            if (enemyScript.landShark != null)
            {
                if (collision.gameObject.CompareTag("Player"))
                {
                    //잠복으로 변환
                    enemyScript.landShark.IsHide();
                }
            }

            //소면귀 전용
            if (enemyScript.somyeon_Gwi != null)
            {
                //만약 아이템 태그를 발견하면
                if (collision.gameObject.CompareTag("Item"))
                {
                    //트리거 활성화 후
                    enemyScript.somyeon_Gwi.isFindItem = true;
                    //위치값을 전달해줌
                    enemyScript.somyeon_Gwi.findItemVec = collision.gameObject.transform.position;
                }
            }

            //분열귀 전용
            if (enemyScript.boon_yeol_gwi != null)
            {
                //item을 가져와준 후
                ItemObject item = collision.gameObject.GetComponent<ItemObject>();

                //아이템을 찾고 가치를 먹었고, 충돌한 오브젝트가 적이면
                if (enemyScript.boon_yeol_gwi.isItemFind && enemyScript.boon_yeol_gwi.isItemEat && collision.gameObject.CompareTag("Enemy"))
                {
                    //컴포넌트를 가져온 후
                    Boon_yeol_gwi entity = collision.GetComponent<Boon_yeol_gwi>();

                    //값이 맞으면
                    if (entity != null && enemyScript.boon_yeol_gwi.entityObj == entity)
                    {
                        //true로 해준 후
                        enemyScript.boon_yeol_gwi.isEntityFind = true;
                        //위치를 보내줌
                        enemyScript.boon_yeol_gwi.targetTrs = entity.transform.position;
                    }
                }

                //찾은게 아이템이면
                if (collision.gameObject.CompareTag("Item") && item.itemData.Coin != 0)
                {
                    enemyScript.boon_yeol_gwi.isItemFind = true;
                    enemyScript.boon_yeol_gwi.itemTrs = collision.gameObject.transform.position;
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
            if (enemyScript.boon_yeol_gwi != null)
            {
                //item을 가져와준 후
                ItemObject item = collision.gameObject.GetComponent<ItemObject>();

                if (enemyScript.boon_yeol_gwi.isItemEat)
                {
                    if (collision.gameObject.CompareTag("Item") && item.itemData.Coin == 0)
                    {
                        enemyScript.boon_yeol_gwi.isItemFind = false;
                        return;
                    }
                }

                //찾은게 아이템이면
                if (collision.gameObject.CompareTag("Item") && item.itemData.Coin != 0)
                {
                    enemyScript.boon_yeol_gwi.isItemFind = true;
                    enemyScript.boon_yeol_gwi.itemTrs = collision.gameObject.transform.position;
                }
            }

            //플레이어가 범위 안에 들어오면 쫒아가줌
            if (collision.gameObject.CompareTag("Player"))
            {
                targetPos = collision.gameObject.transform.position;
                enemy.isTrace = true;
            }

            //땅상어 전용
            if (enemyScript.landShark != null)
            {
                if (collision.gameObject.CompareTag("Player"))
                {
                    //돌출 상태일떄
                    if(enemyScript.landShark.isOut)
                    {
                        landSharkOutTime += Time.deltaTime;

                        //5초가 지나면
                        if(landSharkOutTime >= 5f)
                        {
                            //잠복으로 변환
                            enemyScript.landShark.IsHide();
                            //초기화
                            landSharkOutTime = 0f;
                        }
                    }    


                    PlayerController player = collision.GetComponent<PlayerController>();

                    //범위 내에서 움직임을 감지 && 공격준비가 됐으면
                    if (player.isMoving && enemyScript.landShark.isAttackReady && enemyScript.landShark.isIn)
                    {
                        landSharkAttackTime += Time.deltaTime;

                        if(landSharkAttackTime >= 0.5f)
                        {
                            enemyScript.landSharkAttack.JumpAttack();
                        }
                    }
                }

            }

            // 무면귀 전용
            if (enemyScript.mumyeon_Gwi != null)
            {
                if (collision.gameObject.CompareTag("Player"))
                {
                    mumyeon_Gwi_Stay_Time += Time.deltaTime;
                    // 무면귀가 추적하는 시간
                    if (mumyeon_Gwi_Stay_Time >= 5f)
                    {
                        // 추적을 활성화 해줌
                        enemyScript.mumyeon_Gwi.isTrace = true;
                    }
                }
            }
            // 죽음 장승 전용
            if (enemyScript.death_Jangseung != null)
            {
                if (collision.gameObject.CompareTag("Player"))
                {
                    jangseungTargetTrs = collision.transform.position;

                    if (jangseungtime >= enemyScript.death_Jangseung.attackSeeTime && !enemyScript.death_Jangseung.isAttackReady)
                    {
                        enemyScript.death_Jangseung.attackTargetTrs = jangseungTargetTrs;

                        enemyScript.death_Jangseung.isAttackReady = true;

                        jangseungtime = 0f;
                    }

                }
            }
            // 두억시니 전용
            if (enemyScript.duoksini != null)
            {
                if (collision.gameObject.CompareTag("Player"))
                {
                    Debug.Log("위치 추적중");
                    duoksiniTargetTrs = collision.transform.position;

                    if (duoksinitime >= enemyScript.duoksini.attackSeeTime && !enemyScript.duoksini.isAttackReady)
                    {
                        Debug.Log("값 전달");
                        enemyScript.duoksini.attackTargetTrs = duoksiniTargetTrs;

                        enemyScript.duoksini.isAttackReady = true;
                        enemyScript.duoksini_Attack.isTrsPass = true;

                        duoksinitime = 0f;
                    }

                }
            }
            // 어둑쥐 전용
            if (enemyScript.eo_dook_jwi != null)
            {
                if (collision.gameObject.CompareTag("Light"))
                {
                    Light2D light = collision.gameObject.GetComponent<Light2D>();
                    light.intensity = 0.1f;
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
                if (enemyScript.landShark != null)
                {
                    //돌출로 변환
                    enemyScript.landShark.isHideOut();
                }

                // 무면귀 전용
                if (enemyScript.mumyeon_Gwi != null)
                {
                    // 벗어나면 시간 초기화
                    mumyeon_Gwi_Stay_Time = 0f;
                }

            }

            // 어둑쥐 전용
            if (enemyScript.eo_dook_jwi != null)
            {
                if (collision.gameObject.CompareTag("Light"))
                {
                    Light2D light = collision.gameObject.GetComponent<Light2D>();
                    light.intensity = enemyScript.playerOrigin_Light_Intensity;
                }
            }
        }
    }
}
