using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    // �����
    [SerializeField] LandShark landShark;
    [SerializeField] LandSharkAttack landSharkAttack;
    // �Ҹ��
    [SerializeField] Somyeon_gwi somyeon_Gwi;
    // �п���
    [SerializeField] Boon_yeol_gwi boon_yeol_gwi;
    // �����
    [SerializeField] Mumyeon_Gwi mumyeon_Gwi;
    // �������
    [SerializeField] Death_Jangseung death_Jangseung;
    [SerializeField] Death_Jangseung_Attack death_Jangseung_Attack;
    // �ξ�ô�
    [SerializeField] Duoksini duoksini;
    [SerializeField] Duoksini_Attack duoksini_Attack;

    void Awake()
    {
        //�ݶ��̴��� �����Ͻ� ũ�� ����
        if (enemyTraceCollider is CircleCollider2D circleColl) circleColl.radius = enemyTraceRange;
    }

    void Update()
    {
        //�����ϸ鼭. �ڽ��� �ƴ� �ٸ��ɷ� �и�����⿡ ���󰡰� ����
        transform.position = enemy.transform.position;

        // ���� ��� ����
        if (death_Jangseung_Attack != null && !death_Jangseung_Attack.isAttack)
        {
            jangseungtime += Time.deltaTime;
        }
        // �ξ�ô� ����
        else if (duoksini_Attack != null && !duoksini_Attack.isAttack)
        {
            duoksinitime += Time.deltaTime;
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
            if (landShark != null)
            {
                if (collision.gameObject.CompareTag("Player"))
                {
                    //�ẹ���� ��ȯ
                    landShark.IsHide();
                }
            }

            //�Ҹ�� ����
            if (somyeon_Gwi != null)
            {
                //���� ������ �±׸� �߰��ϸ�
                if (collision.gameObject.CompareTag("Item"))
                {
                    //Ʈ���� Ȱ��ȭ ��
                    somyeon_Gwi.isFindItem = true;
                    //��ġ���� ��������
                    somyeon_Gwi.findItemVec = collision.gameObject.transform.position;
                }
            }

            //�п��� ����
            if (boon_yeol_gwi != null)
            {
                //item�� �������� ��
                ItemObject item = collision.gameObject.GetComponent<ItemObject>();

                //�������� ã�� ��ġ�� �Ծ���, �浹�� ������Ʈ�� ���̸�
                if (boon_yeol_gwi.isItemFind && boon_yeol_gwi.isItemEat && collision.gameObject.CompareTag("Enemy"))
                {
                    //������Ʈ�� ������ ��
                    Boon_yeol_gwi entity = collision.GetComponent<Boon_yeol_gwi>();

                    //���� ������
                    if (entity != null)
                    {
                        //true�� ���� ��
                        boon_yeol_gwi.isEntityFind = true;
                        //��ġ�� ������
                        boon_yeol_gwi.targetTrs = entity.transform.position;
                    }
                }

                //ã���� �������̸�
                if (collision.gameObject.CompareTag("Item") && item.itemData.Coin != 0)
                {
                    boon_yeol_gwi.isItemFind = true;
                    boon_yeol_gwi.itemTrs = collision.gameObject.transform.position;
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
            if (boon_yeol_gwi != null)
            {
                //item�� �������� ��
                ItemObject item = collision.gameObject.GetComponent<ItemObject>();

                if (boon_yeol_gwi.isItemEat)
                {
                    if (collision.gameObject.CompareTag("Item") && item.itemData.Coin == 0)
                    {
                        boon_yeol_gwi.isItemFind = false;
                        return;
                    }
                }

                //ã���� �������̸�
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

            //�÷��̾ ���� �ȿ� ������ �i�ư���
            if (collision.gameObject.CompareTag("Player"))
            {
                targetPos = collision.gameObject.transform.position;
                enemy.isTrace = true;
            }

            //����� ����
            if (landShark != null)
            {
                if (collision.gameObject.CompareTag("Player"))
                {
                    //���� �����ϋ�
                    if(landShark.isOut)
                    {
                        landSharkOutTime += Time.deltaTime;

                        //5�ʰ� ������
                        if(landSharkOutTime >= 5f)
                        {
                            //�ẹ���� ��ȯ
                            landShark.IsHide();
                            //�ʱ�ȭ
                            landSharkOutTime = 0f;
                        }
                    }    


                    PlayerController player = collision.GetComponent<PlayerController>();

                    //���� ������ �������� ���� && �����غ� ������
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

            // ����� ����
            if (mumyeon_Gwi != null)
            {
                if (collision.gameObject.CompareTag("Player"))
                {
                    mumyeon_Gwi_Stay_Time += Time.deltaTime;
                    // ����Ͱ� �����ϴ� �ð�
                    if (mumyeon_Gwi_Stay_Time >= 5f)
                    {
                        // ������ Ȱ��ȭ ����
                        mumyeon_Gwi.isTrace = true;
                    }
                }
            }
            // ���� ��� ����
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
            // �ξ�ô� ����
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
            //����� false�� ����
            if (collision.gameObject.CompareTag("Player"))
            {
                enemy.isTrace = false;

                //����� ����
                if (landShark != null)
                {
                    //����� ��ȯ
                    landShark.isHideOut();
                }

                // ����� ����
                if (mumyeon_Gwi != null)
                {
                    // ����� �ð� �ʱ�ȭ
                    mumyeon_Gwi_Stay_Time = 0f;
                }

            }
        }
    }
}
