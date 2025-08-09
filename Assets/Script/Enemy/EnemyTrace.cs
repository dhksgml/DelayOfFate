using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class EnemyTrace : MonoBehaviour
{
    [Header("Enemy Trace Range")]
    public float     enemyTraceRange; //���� �߰� ����

    [SerializeField]
    Collider2D       enemyTraceCollider; //���� �߰� ������ �������� �ݶ��̴�

    [Header("Reference")]
    public Vector3   targetPos; //�÷��̾��� ��ġ
    [SerializeField]
    Enemy            enemy;

    [System.Serializable]
    public class EnemyScript
    {
        [Header("�����")]
        public LandShark landShark;
        public LandSharkAttack landSharkAttack;

        [Header("�Ҹ��")]
        public Somyeon_gwi somyeon_Gwi;

        [Header("�п���")]
        public Boon_yeol_gwi boon_yeol_gwi;

        [Header("�����")]
        public Mumyeon_Gwi mumyeon_Gwi;

        [Header("�������")]
        public Death_Jangseung death_Jangseung;
        public Death_Jangseung_Attack death_Jangseung_Attack;

        [Header("�ξ�ô�")]
        public Duoksini duoksini;
        public Duoksini_Attack duoksini_Attack;

        [Header("�����")]
        public Eo_dook_jwi eo_dook_jwi;

        // ����� ����
        [Tooltip("���� �÷��̾��� Intensity�� �ٲ��� ���� ����� ��")]
        public float playerOrigin_Light_Intensity;
    }

    [SerializeField] EnemyScript enemyScript;

    void Awake()
    {
        //�ݶ��̴��� �����Ͻ� ũ�� ����
        if (enemyTraceCollider is CircleCollider2D circleColl) circleColl.radius = enemyTraceRange;
    }

    float boon_yeol_gwi_itemTime = 0;

    void Update()
    {
        //�����ϸ鼭. �ڽ��� �ƴ� �ٸ��ɷ� �и�����⿡ ���󰡰� ����
        transform.position = enemy.transform.position;

        // ���� ��� ����
        if (enemyScript.death_Jangseung_Attack != null && !enemyScript.death_Jangseung_Attack.isAttack)
        {
            jangseungtime += Time.deltaTime;
        }
        // �ξ�ô� ����
        else if (enemyScript.duoksini_Attack != null && !enemyScript.duoksini_Attack.isAttack)
        {
            duoksinitime += Time.deltaTime;
        }
        // �п���
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

    //����� ����
    [HideInInspector]
    public float landSharkAttackTime;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision != null)
        {
            //����� ����
            if (enemyScript.landShark != null)
            {
                if (collision.gameObject.CompareTag("Player"))
                {
                    //�ẹ���� ��ȯ
                    enemyScript.landShark.IsHide();
                }
            }

            //�Ҹ�� ����
            if (enemyScript.somyeon_Gwi != null)
            {
                //���� ������ �±׸� �߰��ϸ�
                if (collision.gameObject.CompareTag("Item"))
                {
                    //Ʈ���� Ȱ��ȭ ��
                    enemyScript.somyeon_Gwi.isFindItem = true;
                    //��ġ���� ��������
                    enemyScript.somyeon_Gwi.findItemVec = collision.gameObject.transform.position;
                }
            }

            //�п��� ����
            if (enemyScript.boon_yeol_gwi != null)
            {
                //item�� �������� ��
                ItemObject item = collision.gameObject.GetComponent<ItemObject>();

                //�������� ã�� ��ġ�� �Ծ���, �浹�� ������Ʈ�� ���̸�
                if (enemyScript.boon_yeol_gwi.isItemFind && enemyScript.boon_yeol_gwi.isItemEat && collision.gameObject.CompareTag("Enemy"))
                {
                    //������Ʈ�� ������ ��
                    Boon_yeol_gwi entity = collision.GetComponent<Boon_yeol_gwi>();

                    //���� ������
                    if (entity != null && enemyScript.boon_yeol_gwi.entityObj == entity)
                    {
                        //true�� ���� ��
                        enemyScript.boon_yeol_gwi.isEntityFind = true;
                        //��ġ�� ������
                        enemyScript.boon_yeol_gwi.targetTrs = entity.transform.position;
                    }
                }

                //ã���� �������̸�
                if (collision.gameObject.CompareTag("Item") && item.itemData.Coin != 0)
                {
                    enemyScript.boon_yeol_gwi.isItemFind = true;
                    enemyScript.boon_yeol_gwi.itemTrs = collision.gameObject.transform.position;
                }


            }
        }
    }

    //����� ����
    float landSharkOutTime;

    // ����� ����
    float mumyeon_Gwi_Stay_Time;

    // ���� ��� ����
    Vector3 jangseungTargetTrs;
    float jangseungtime = 0f;

    // �ξ�ô� ����
    Vector3 duoksiniTargetTrs;
    float duoksinitime = 0f;



    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision != null)
        {
            //�п��� ����
            if (enemyScript.boon_yeol_gwi != null)
            {
                //item�� �������� ��
                ItemObject item = collision.gameObject.GetComponent<ItemObject>();

                if (enemyScript.boon_yeol_gwi.isItemEat)
                {
                    if (collision.gameObject.CompareTag("Item") && item.itemData.Coin == 0)
                    {
                        enemyScript.boon_yeol_gwi.isItemFind = false;
                        return;
                    }
                }

                //ã���� �������̸�
                if (collision.gameObject.CompareTag("Item") && item.itemData.Coin != 0)
                {
                    enemyScript.boon_yeol_gwi.isItemFind = true;
                    enemyScript.boon_yeol_gwi.itemTrs = collision.gameObject.transform.position;
                }
            }

            //�÷��̾ ���� �ȿ� ������ �i�ư���
            if (collision.gameObject.CompareTag("Player"))
            {
                targetPos = collision.gameObject.transform.position;
                enemy.isTrace = true;
            }

            //����� ����
            if (enemyScript.landShark != null)
            {
                if (collision.gameObject.CompareTag("Player"))
                {
                    //���� �����ϋ�
                    if(enemyScript.landShark.isOut)
                    {
                        landSharkOutTime += Time.deltaTime;

                        //5�ʰ� ������
                        if(landSharkOutTime >= 5f)
                        {
                            //�ẹ���� ��ȯ
                            enemyScript.landShark.IsHide();
                            //�ʱ�ȭ
                            landSharkOutTime = 0f;
                        }
                    }    


                    PlayerController player = collision.GetComponent<PlayerController>();

                    //���� ������ �������� ���� && �����غ� ������
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

            // ����� ����
            if (enemyScript.mumyeon_Gwi != null)
            {
                if (collision.gameObject.CompareTag("Player"))
                {
                    mumyeon_Gwi_Stay_Time += Time.deltaTime;
                    // ����Ͱ� �����ϴ� �ð�
                    if (mumyeon_Gwi_Stay_Time >= 5f)
                    {
                        // ������ Ȱ��ȭ ����
                        enemyScript.mumyeon_Gwi.isTrace = true;
                    }
                }
            }
            // ���� ��� ����
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
            // �ξ�ô� ����
            if (enemyScript.duoksini != null)
            {
                if (collision.gameObject.CompareTag("Player"))
                {
                    Debug.Log("��ġ ������");
                    duoksiniTargetTrs = collision.transform.position;

                    if (duoksinitime >= enemyScript.duoksini.attackSeeTime && !enemyScript.duoksini.isAttackReady)
                    {
                        Debug.Log("�� ����");
                        enemyScript.duoksini.attackTargetTrs = duoksiniTargetTrs;

                        enemyScript.duoksini.isAttackReady = true;
                        enemyScript.duoksini_Attack.isTrsPass = true;

                        duoksinitime = 0f;
                    }

                }
            }
            // ����� ����
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
            //����� false�� ����
            if (collision.gameObject.CompareTag("Player"))
            {
                enemy.isTrace = false;

                //����� ����
                if (enemyScript.landShark != null)
                {
                    //����� ��ȯ
                    enemyScript.landShark.isHideOut();
                }

                // ����� ����
                if (enemyScript.mumyeon_Gwi != null)
                {
                    // ����� �ð� �ʱ�ȭ
                    mumyeon_Gwi_Stay_Time = 0f;
                }

            }

            // ����� ����
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
